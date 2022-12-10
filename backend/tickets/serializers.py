from rest_framework import serializers
from tickets.models import Ticket, TicketSale


class TicketSerializer(serializers.ModelSerializer):
    reduced_price = serializers.ReadOnlyField()

    class Meta:
        model = Ticket
        fields = "__all__"

class TicketSaleSerializer(serializers.ModelSerializer):
    total_price = serializers.ReadOnlyField()

    class Meta:
        model = TicketSale
        fields = "__all__"
