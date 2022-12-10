FROM python:3.10

ENV PYTHONUNBUFFERED 1

WORKDIR /app/

RUN pip install poetry && \
    poetry config virtualenvs.create false --local

COPY ./pyproject.toml ./poetry.lock* /app/

RUN poetry install --no-dev

COPY . /app/

EXPOSE 8000

CMD ./entrypoint.sh
