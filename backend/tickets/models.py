from django.db import models

REDUCED_MODIFIER = 0.5


class Ticket(models.Model):
    duration = models.IntegerField()
    zone = models.CharField(max_length=255)
    price = models.IntegerField()

    @property
    def reduced_price(self):
        return self.price * REDUCED_MODIFIER


class TicketSale(models.Model):
    ticket = models.ForeignKey(Ticket, on_delete=models.CASCADE)
    quantity = models.IntegerField()
    reuced = models.BooleanField()
    date = models.DateField()

    @property
    def total_price(self):
        price = self.ticket.reduced_price if self.reuced else self.ticket.price
        return price * self.quantity
