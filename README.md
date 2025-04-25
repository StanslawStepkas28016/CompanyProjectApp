# Projekt APBD s28016 - Stanisław Stepka

## Opis projektu:
Kod źródłowy backendu aplikacji do zarządzania danymi firm. Aplikacja umożliwia przechowywanie, przetwarzanie i zarządzanie informacjami o firmach oraz powiązanych danych.

## Techstack i wzorce:
- **Język programowania:** C#
- **Framework:** ASP.NET Core
- **Baza danych:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Autoryzacja i uwierzytelnianie:** JWT
- **Konteneryzacja:** Docker
- **Wzorce projektowe:** Repository Pattern, Unit of Work, Dependency Injection

## Dostępne funkcjonalności:
### Dla gościa:
- Pobieranie listy firm
- Pobieranie szczegółowych informacji o firmie

### Dla użytkownika zarejestrowanego:
- Rejestracja i logowanie
- Dodawanie nowej firmy
- Edytowanie danych firmy
- Usuwanie firmy

### Dla administratora:
- Wszystkie funkcjonalności użytkownika
- Zarządzanie użytkownikami
- Usuwanie dowolnych firm i powiązanych danych
- Przegląd i edycja połączeń między tabelami

## Informacje dot. testowania:
- Przykładowe dane i skrypt tworzenia bazy danych znajdują się w katalogu `/backend/src/assets`
- Testowanie API można przeprowadzić za pomocą Postmana lub Swaggera (`/swagger` po uruchomieniu serwera)

## Dodatkowe informacje:
- Aplikacja wykorzystuje JWT do autoryzacji.
- Obsługuje paginację oraz filtrowanie danych.
- Logi aplikacji są przechowywane w plikach logów w katalogu `logs/`.
- Możliwość integracji z systemami zewnętrznymi poprzez API.
