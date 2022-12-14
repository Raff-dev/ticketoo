# Ticketoo
Buy bus tickets using voice recognition

---
## Run the application

Create the .env file and supply your own secrets and configuration, then simply compose up.

    docker compose up
---
## Issues
If you are running on Windows and get `entrypoint.sh not found`, you might need to change the end of line sequence of entrypoint.sh CRLF -> LF.

---
## Acces the application locally

* [localhost:8000](http://localhost:8000) for Django Rest Framework Root

* [localhost:8000/admin](http://localhost:8000/admin) for Django Admin Page

* [localhost:8000/tickets](http://localhost:8000/tickets) `GET` for listing available tickets

* [localhost:8000/orders](http://localhost:8000/orders) `POST` for registering ticket sale
```json
{
    "ticket": 1,
    "reduced": "true",
    "quantity": 5
}
```
---
## Django commands

    docker compose run backend python manage.py {your_command} {your_args}
---

