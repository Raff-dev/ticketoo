from django.urls import include, path
from rest_framework.routers import DefaultRouter

from tickets.views import TicketSaleViewSet, TicketViewSet

router = DefaultRouter()
router.register("tickets", TicketViewSet)
router.register("orders", TicketSaleViewSet)


urlpatterns = [
    path("", include(router.urls)),
]
