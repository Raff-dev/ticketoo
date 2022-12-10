#!/bin/sh

echo "Updating dependencies"
poetry install --no-dev

# configure static files before uncommenting it
# echo "Collect static files"
# python manage.py collectstatic --noinput

echo "Applying migrations"
python manage.py migrate

echo "Starting server"
python manage.py runserver 0.0.0.0:8000
