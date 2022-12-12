from rest_framework.viewsets import GenericViewSet, mixins

from tickets.models import Ticket, TicketSale
from tickets.serializers import TicketSaleSerializer, TicketSerializer


class TicketViewSet(GenericViewSet, mixins.ListModelMixin):
    queryset = Ticket.objects.all()
    serializer_class = TicketSerializer


class TicketSaleViewSet(GenericViewSet, mixins.CreateModelMixin):
    queryset = TicketSale.objects.all()
    serializer_class = TicketSaleSerializer
