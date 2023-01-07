from django.contrib import admin

from tickets.models import Ticket, TicketSale


@admin.register(Ticket)
class TicketAdmin(admin.ModelAdmin):
    list_display = ["duration", "zone", "price", "reduced_price"]
    readonly_fields = ["reduced_price"]


@admin.register(TicketSale)
class TicketSaleAdmin(admin.ModelAdmin):
    list_display = ["ticket", "quantity", "reduced", "total_price"]
    readonly_fields = ["ticket", "quantity", "reduced", "total_price"]
