from django.contrib import admin
from tickets.models import Ticket


@admin.register(Ticket)
class TicketAdmin(admin.ModelAdmin):
    list_display = ["duration", "zone", "price", "reduced_price"]
    readonly_fields = ["reduced_price"]
