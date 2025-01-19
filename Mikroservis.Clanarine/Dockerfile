# Koristimo najnoviju Go sliku
FROM golang:1.22 AS builder

# Postavljamo radni direktorijum u kontejneru
WORKDIR /app

# Kopiramo zavisnosti
COPY Mikroservis.Clanarine/go.mod Mikroservis.Clanarine/go.sum ./
RUN go mod download

# Kopiramo ostatak aplikacije
COPY Mikroservis.Clanarine/ ./

# Buildujemo aplikaciju
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o main .
RUN ls -l /app

# Drugi stage - minimalna slika
FROM alpine:latest

WORKDIR /app

# Instaliramo potrebne sertifikate
RUN apk --no-cache add ca-certificates

# Kopiramo binarni fajl
COPY --from=builder /app/main .

# Kopiramo .env fajl
COPY Mikroservis.Clanarine/.env ./

# Postavljamo dozvole za izvršavanje
RUN chmod +x /app/main

# Postavljamo port na kojem će aplikacija slušati
EXPOSE 8082 8083

# Pokrećemo aplikaciju
CMD ["./main"]
