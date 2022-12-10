from rest_framework import viewsets
from tickets.models import Ticket, TicketSale
from tickets.serializers import TicketSaleSerializer, TicketSerializer

# Create your views here.


class TicketViewSet(viewsets.ModelViewSet):
    queryset = Ticket.objects.all()
    serializer_class = TicketSerializer

class TicketSaleViewSet(viewsets.ModelViewSet):
    queryset = TicketSale.objects.all()
    serializer_class = TicketSaleSerializer
