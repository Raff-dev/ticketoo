import React, { useState, useEffect } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import SpeechRecognition, { useSpeechRecognition } from "react-speech-recognition";

// Define the styles for the ticket card
const useStyles = makeStyles((theme) => ({
    rootCard: {
        minWidth: 275,
    },
    topCard: {
        backgroundColor: "#f5f5f5",
    },
    botCard: {},
    title: {
        fontSize: 14,
        fontFamily: "monospace",
        display: "flex",
        justifyContent: "center",
        padding: theme.spacing(1),
    },
    value: {
        fontSize: 18,
        fontWeight: "bold",
        display: "flex",
        justifyContent: "center",
        padding: theme.spacing(1),
    },
    duration: {
        fontSize: 50,
        fontWeight: "bold",
        display: "flex",
        justifyContent: "center",
        margin: theme.spacing(3),
    },
}));

const valueToCurrency = (value) => {
    return (value / 100).toLocaleString("en-US", {
        style: "currency",
        currency: "PLN",
    });
};

// Create a component to display a ticket card
const TicketCard = ({ ticket }) => {
    const classes = useStyles();

    return (
        <Card className={classes.rootCard}>
            <Card className={classes.topCard}>
                <Typography className={classes.title} color="textSecondary">
                    Zone {ticket.zone}
                </Typography>
                <Typography variant="h5" component="h2">
                    <p className={classes.duration}>{ticket.duration}</p>
                    <p className={classes.value}>minutes</p>
                </Typography>
            </Card>
            <Typography className={classes.title} color="textSecondary">
                Price
            </Typography>
            <Typography className={classes.value} variant="h5" component="h2">
                {valueToCurrency(ticket.price)}
            </Typography>
            <Typography className={classes.title} color="textSecondary">
                Reduced price
            </Typography>
            <Typography className={classes.value} variant="h5" component="h2">
                {valueToCurrency(ticket.reduced_price)}
            </Typography>
        </Card>
    );
};

const API_ENDPOINT = "http://localhost:8000/tickets/tickets/";

const TicketSpeechRecognition = ({ tickets }) => {
    const [zone, setZone] = useState(null);
    const [duration, setDuration] = useState(null);
    const [reduced, setReduced] = useState(null);
    const [message, setMessage] = useState("");
    const [active, setActive] = useState(false);

    const speak = (text) => {
        console.log("speak", text);
        setMessage(text);
        const utterance = new SpeechSynthesisUtterance(text);
        window.speechSynthesis.cancel();
        window.speechSynthesis.volume = 1;
        window.speechSynthesis.speak(utterance);
        SpeechRecognition.startListening({ continuous: true });
        resetTranscript();
    };

    const getNextResponse = () => {
        // get question based on missing data
        if (zone === null) {
            return "Choose the zone of the ticket";
        }
        if (duration === null) {
            return "Choose the duration of the ticket";
        }
        if (reduced === null) {
            return "Would you like to buy a reduced price or normal ticket?";
        }

        tickets.forEach((ticket) => {
            if (zone === ticket.zone.toString() && duration === ticket.duration.toString()) {
                console.log("found ticket", ticket);
                let price = reduced ? ticket.reduced_price : ticket.price;
                let message =
                    `You have chosen a ticket for zone ${zone} for ${duration} minutes. ` +
                    `The price is ${price}. Do you confirm the order?`;
                console.log("message", message);
                return message;
            }
        });

        resetData();
        return "Sorry, we don't have such a ticket";
    };

    useEffect(() => {
        if (active) {
            SpeechRecognition.startListening({ continuous: true });

            return;
        }
        SpeechRecognition.startListening({ continuous: true });
        //  cancel listening when component unmounts
        let response = getNextResponse();
        speak(response);
        return () => SpeechRecognition.stopListening();
    }, []);

    const resetData = () => {
        setZone(null);
        setDuration(null);
        setReduced(null);
        setActive(false);
    };

    const commands = [
        {
            command: "zone *",
            callback: (zone) => {
                let exists = tickets.some((ticket) => ticket.zone.toString() === zone);
                if (!exists) {
                    let nextResponse = getNextResponse();
                    speak("Sorry, we don't have such a ticket. Please repeat. " + nextResponse);
                    return;
                }

                setZone(zone);
                speak(`You chose zone ${zone}.`);
            },
        },
        {
            command: ":duration minutes",
            callback: (duration) => {
                if (isNaN(duration)) {
                    let nextResponse = getNextResponse();
                    speak("Sorry, I didn't understand. Please repeat. " + nextResponse);
                    return;
                }

                let exists = tickets.some((ticket) => ticket.duration.toString() === duration);
                if (!exists) {
                    let nextResponse = getNextResponse();
                    speak("Sorry, we don't have such a ticket. Please repeat. " + nextResponse);
                    return;
                }

                setDuration(duration);
                speak(`You chose ${duration} minutes.`);
            },
        },
        {
            command: ["reduced (price)", "normal"],
            callback: (text) => {
                let valid = text.includes("reduced") || text.includes("normal");
                if (!valid) {
                    speak("Sorry, I didn't understand. Please repeat.");
                    speak(getNextResponse());
                    return;
                }

                let reduced = text.includes("reduced");
                setReduced(reduced);
                let message = reduced ? "reduced price" : "normal price";
                speak(`You chose ${message} ticket.`);
            },
            isFuzzyMatch: true,
            fuzzyMatchingThreshold: 0.3,
        },
        {
            command: ["Hello", "Hi", "Hey", "Start", "Begin", "Start over", "Restart"],
            callback: ({ command, resetTranscript }) => {
                if (active) {
                    return;
                }
                speak("Hello, welcome to Ticketoo.");
                setActive(true);
            },
            matchInterim: true,
        },
        {
            command: ["yes", "no", "confirm", "cancel"],
            callback: ({ command, resetTranscript }) => {
                if (!active || zone === null || duration === null || reduced === null) {
                    return;
                }
                if (command.includes("yes") || command.includes("confirm")) {
                    speak("Thank you for your order.");

                    let data = { zone: zone, duration: duration, reduced: reduced };
                    let options = {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(data),
                    };
                    fetch(API_ENDPOINT, options)
                        .then(() => resetData())
                        .catch((error) => {
                            console.log(error);
                            speak("Sorry, something went wrong. Would you like to try again?");
                        });
                } else if (command.includes("no") || command.includes("cancel")) {
                    speak("Ok, canceling.");
                    resetData();
                }
            },
            matchInterim: true,
        },
        {
            command: ["clear", "reset", "start over", "restart", "exit"],
            callback: ({ resetTranscript }) => {
                resetTranscript();
                speak("Clearing the data.");
                resetData();
            },
        },
    ];

    const { transcript, browserSupportsSpeechRecognition, resetTranscript } = useSpeechRecognition({
        commands,
    });

    if (!browserSupportsSpeechRecognition) {
        console.log("Your browser does not support speech recognition software! Try Chrome desktop.");
        return null;
    }

    return (
        <div>
            <p>Response: {message}</p>
            <p>active: {"" + active}</p>
            <p>zone: {zone}</p>
            <p>duration: {duration}</p>
            <p>reduced: {"" + reduced}</p>
            <p>transcript: {transcript}</p>

            <button onClick={resetTranscript}>Reset</button>
            <button onClick={SpeechRecognition.startListening}>Start</button>
        </div>
    );
};

const App = () => {
    const [tickets, setTickets] = useState([]);

    // Fetch the tickets when the component mounts
    useEffect(() => {
        fetch(API_ENDPOINT)
            .then((response) => response.json())
            .then(setTickets)
            .catch(console.error);
    }, []);

    return (
        <Grid container spacing={3}>
            <TicketSpeechRecognition tickets={tickets} />
            {tickets.map((ticket) => (
                <Grid item xs={12} sm={6} md={4} key={ticket.duration}>
                    <TicketCard ticket={ticket} />
                </Grid>
            ))}
        </Grid>
    );
};

export default App;
