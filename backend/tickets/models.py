from django.db import models

REDUCED_MODIFIER = 0.5


class Ticket(models.Model):
    duration = models.CharField(max_length=255)
    zone = models.CharField(max_length=255)
    price = models.IntegerField()

    class Meta:
        unique_together = ["duration", "zone", "price"]

    @property
    def reduced_price(self) -> int:
        if self.price:
            return int(self.price * REDUCED_MODIFIER)
        return None


class TicketSale(models.Model):
    ticket = models.ForeignKey(Ticket, on_delete=models.CASCADE, null=False, blank=False)
    quantity = models.IntegerField(null=False, blank=False)
    reduced = models.BooleanField(null=False, blank=False)

    @property
    def total_price(self):
        price = self.ticket.reduced_price if self.reduced else self.ticket.price
        return price * self.quantity
