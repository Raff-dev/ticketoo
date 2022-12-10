import React, { useState, useEffect } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";

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
                    <p className={classes.value}>{ticket.duration}</p>
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
            {tickets.map((ticket) => (
                <Grid item xs={12} sm={6} md={4} key={ticket.duration}>
                    <TicketCard ticket={ticket} />
                </Grid>
            ))}
        </Grid>
    );
};

export default App;
