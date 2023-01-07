using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace projekt
{
    internal class Tickets
    {
        const string API_ENDPOINT_TICKETS = "http://localhost:8000/tickets/";

        public class Ticket
        {
            public int Id { get; set; }
            public string Duration { get; set; }
            public int Price { get; set; }
            [JsonProperty("reduced_price")]
            public int ReducedPrice { get; set; }
            public string Zone { get; set; }
            public string ZoneFormatted
            {
                get
                {
                    return Zone.ToUpper();
                }
            }
            public string PriceFormatted
            {
                get
                {
                    return ((float)Price / 100).ToString("0.00", CultureInfo.CreateSpecificCulture("pl-PL")) + " PLN";
                }
            }
            public string DurationFormatted
            {
                get
                {
                    if (int.TryParse(Duration, out int minutes))
                    {
                        return $"{minutes} min";
                    }
                    else
                    {
                        return Duration.ToUpper();
                    }
                }
            }
        }

        public static async Task<List<Ticket>> GetTicketsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Send the GET request
                HttpResponseMessage response = await client.GetAsync(API_ENDPOINT_TICKETS);

                // If the response is successful, deserialize the response content into a list of tickets
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<Ticket> tickets = JsonConvert.DeserializeObject<List<Ticket>>(json);
                    if (tickets != null)
                    {
                        return tickets;
                    }
                    return new List<Ticket>();
                }
                else
                {
                    // If the response is not successful, throw an exception
                    throw new Exception("Failed to fetch tickets from API endpoint");
                }
            }
        }
    }
}
