using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Windows;
using static projekt.Orders;
using static projekt.Tickets;

namespace projekt
{
    public partial class MainWindow : Window
    {
        static SpeechRecognitionEngine sre;
        static SpeechSynthesizer ss;
        static CultureInfo ci = new CultureInfo("pl-PL");

        private List<Ticket> tickets;
        private List<Ticket> ticketsFiltered;
        private string? zone;
        private string? duration;
        private int? quantity;
        private bool? reduced;
        private Ticket? chosenTicket;

        public MainWindow()
        {
            ss = new SpeechSynthesizer();
            sre = new SpeechRecognitionEngine();
            InitializeComponent();
            ss.SetOutputToDefaultAudioDevice();
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += SpeechRecognized;
        }

        private class GrammarInfo
        {
            public Grammar Grammar { get; set; }
            public GrammarBuilder Builder { get; set; }
        }

        private static GrammarInfo CreateGrammar(string[] choices, string key)
        {
            Choices c = new Choices(choices);
            GrammarBuilder gb = new GrammarBuilder(new SemanticResultKey(key, c)) { Culture = ci };
            Grammar g = new Grammar(gb);
            return new GrammarInfo { Grammar = g, Builder = gb };
        }

        private async void startListening()
        {
            tickets = await GetTicketsAsync();
            ticketsFiltered.AddRange(tickets);
            stackPanel.DataContext = new { Tickets = tickets };
            string[] availableZones = tickets.Select(t => t.Zone).Distinct().ToArray();
            string[] availableDurations = tickets.Select(t => t.Duration.ToString()).Distinct().ToArray();

            GrammarInfo zoneInfo = CreateGrammar(availableZones, "strefa");
            GrammarInfo durationInfo = CreateGrammar(availableDurations, "długość");
            GrammarInfo reducedInfo = CreateGrammar(new string[] { "normalny", "ulgowy" }, "rodzaj");
            GrammarInfo quantityInfo = CreateGrammar(new string[] { "1", "2", "3", "4", "5", "6", "7", "8"}, "ilość");

            // Create the combo grammar by combining the individual grammars
            // 0, 1 makes a phrase optional
            GrammarBuilder ticketComboBuilder = new GrammarBuilder() { Culture = ci }; ;
            ticketComboBuilder.Append("strefa", 0, 1);
            ticketComboBuilder.Append(zoneInfo.Builder);
            ticketComboBuilder.Append(durationInfo.Builder);
            ticketComboBuilder.Append("minut", 0, 1);
            ticketComboBuilder.Append("bilet", 0, 1);
            ticketComboBuilder.Append(reducedInfo.Builder);
            ticketComboBuilder.Append("ilość", 0, 1);
            ticketComboBuilder.Append(quantityInfo.Builder);
            Grammar ticketComboGrammar = new Grammar(ticketComboBuilder);

            // Load the grammar into the speech recognizer
            sre.LoadGrammarAsync(zoneInfo.Grammar);
            sre.LoadGrammarAsync(durationInfo.Grammar);
            sre.LoadGrammarAsync(reducedInfo.Grammar);
            sre.LoadGrammarAsync(quantityInfo.Grammar);
            sre.LoadGrammarAsync(ticketComboGrammar);
            string response = $"Podaj strefę biletu. ({String.Join(", ", availableZones)})";
            previewText.Text = response;
            ss.SpeakAsync(response);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void reset()
        {
            zone = null;
            duration = null;
            quantity = null;
            reduced = null;
            chosenTicket = null;

            zoneText.Text = "-";
            durationText.Text = "-";
            quantityText.Text = "-";
            reducedText.Text = "-";
            ticketChoiceIdText.Text = "-";
            previewText.Text = "Witaj w Ticketoo";
            tickets = new List<Ticket>();
            ticketsFiltered = new List<Ticket>();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            sre.RecognizeAsyncStop();
            reset();
            ss.SpeakAsync("Witaj w Tiketu.");
            startListening();
            btnStartStop.Content = "RESET";
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string response = "Proszę powtórzyć";

            if (e.Result.Confidence < 0.6)
            {
                ss.SpeakAsync(response);
                previewText.Text = "Proszę powtórzyć";
                return;
            }

            hearedText.Text = e.Result.Text;
            SemanticValue semantics = e.Result.Semantics;
            assignValues(semantics);

            // If all options were specified, place the order
            if (zone != null && duration != null && quantity != null && reduced != null)
            {
                if (ticketsFiltered.Count == 0)
                {
                    ss.SpeakAsync("Przepraszam, nie ma biletów spełniających podane kryteria. Proszę spróbować ponownie.");
                    reset();
                }
                else if (ticketsFiltered.Count == 1)
                {
                    chosenTicket = ticketsFiltered[0];
                    completeOrder();
                }
                else
                {
                    throw AssertionFailedException("Tickets are not unique");
                }
                return;
            }
            // Otherwise, provide an appropriate response and repeat the question
            else
            {
                if (zone == null)
                {
                    string availableZones = String.Join(", ", ticketsFiltered.Select(t => t.Zone).Distinct().ToArray());
                    response = $"Podaj strefę. {availableZones}";
                }
                else if (duration == null)
                {
                    string availableDurations= String.Join(", ", ticketsFiltered.Select(t => t.Duration).Distinct().ToArray());
                    response = $"Podaj długość biletu. {availableDurations}";
                }
                else if (reduced == null) {
                    response = $"Podaj rodzaj biletu. normalny lub ulgowy"; 
                }
                else if (quantity == null)
                {
                    response = "Podaj ilość biletu. Minimum 1, maksimum 8";
                }
            }

            previewText.Text = response;
            ss.SpeakAsync(response);
        }

        private Exception AssertionFailedException(string v)
        {
            throw new NotImplementedException();
        }

        private void completeOrder()
        {
            string isReducedText = (bool)reduced ? "ulgowy" : "normalny";
            int price = (bool)reduced ? chosenTicket.ReducedPrice : chosenTicket.Price;
            float totalPrice = (float)((float)price * quantity / 100);

            ticketChoiceIdText.Text = chosenTicket.Id.ToString();
            totalPriceText.Text = totalPrice.ToString();

            string response = (
                $"Wybrany bilet to: strefa {zone}, długość {duration}, ilość {quantity}, bilet {isReducedText}. " +
                $"Cena całkowita to {totalPrice}."
            );

            ss.SpeakAsync(response);
            Order order = new Order { Id = (int)chosenTicket.Id, Quantity = (int)quantity, Reduced = (bool)reduced };
            Orders.OrderTicketAsync(order, onOrderComplete);
        }

        private void onOrderComplete(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                ss.SpeakAsync("Bilet został kupiony pomyślnie.");
            }
            else
            {
                ss.SpeakAsync("Coś poszło nie tak. Bilet nie został zakupiony. Proszę spróbować później.");
            }
            sre.RecognizeAsyncStop();
        }

        private void assignValues(SemanticValue semantics)
        {
            foreach (KeyValuePair<String, SemanticValue> child in semantics)
            {
                string key = child.Key;
                string strValue = child.Value.Value.ToString();

                switch (key)
                {
                    case "strefa":
                        zone = strValue;
                        zoneText.Text = zone;
                        ticketsFiltered = ticketsFiltered.Where(t => t.Zone == zone).ToList();
                        break;
                    case "długość":
                        duration = strValue;
                        durationText.Text = strValue;
                        ticketsFiltered = ticketsFiltered.Where(t => t.Duration == duration).ToList();
                        break;
                    case "ilość":
                        quantity = int.Parse(strValue);
                        quantityText.Text = strValue;
                        break;
                    case "rodzaj":
                        reduced = strValue == "ulgowy" ? true : false;
                        reducedText.Text = strValue;
                        break;
                }
            }
            stackPanel.DataContext = new { Tickets = ticketsFiltered };
            if (ticketsFiltered.Count == 0)
            {
                ss.SpeakAsync("Przepraszam, nie ma biletów spełniających podane kryteria. Proszę spróbować ponownie.");
                reset();
            }
        }
    }
}
