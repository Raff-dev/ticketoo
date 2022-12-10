# Ticketoo
Buy bus tickets using voice recognition

---
- [Ticketoo](#ticketoo)
  - [Tech stack](#tech-stack)
  - [Run the application](#run-the-application)
  - [Acces the application locally](#acces-the-application-locally)
  - [Development setup](#development-setup)
  - [Poetry](#poetry)
  - [Pre-commit](#pre-commit)
  - [Django commands](#django-commands)


---

## Tech stack

-   Backend - [Django](https://www.djangoproject.com/) + [DRF](https://www.django-rest-framework.org/)
-   Frontend - [React](https://reactjs.org/)
-   Dependency management - [Poetry](https://python-poetry.org/)
-   Containerization - [Docker](https://www.docker.com/)

---

## Run the application

    docker compose up

## Acces the application locally

* [localhost:8000](http://localhost:8000) for Django Rest Framework Root

* [localhost:8000/admin](http://localhost:8000/admin) for Django Admin Page

* [localhost:3000](http://localhost:3000) for React App
---
## Development setup

Install requirements.

    # install poetry package manager
    pip install poetry

    # go to backend directory and create virtual environment
    cd backend && poetry install && poetry shell

    # install pre-commit hooks for code quality assurance
    pre-commit install

Create the .env file and supply your own secrets and configuration.

    # create .env file
    cp backend/.env.example backend/.env



---
## Poetry
Here are some helpful poetry commands in case you need to alter the dependencies.

    poetry install # installing dependencies
    poetry shell   # run virtual environment shell
    poetry add     # add a dependency
    poetry remove  # remove a dependency
    poetry update  # update dependency list
    poetry show    # list isntalled dependencies

## Pre-commit

    pre-commit                  # runs pre-commit for staged files only
    pre-commit run {hook_id}    # run specified hoot
    pre-commit run --all-files  # run for all files

## Django commands

    docker-compose run backend python manage.py {your_command} {your_args}
---

