# Projekt APBD s28016 - Stanisław Stepka

## Opis projektu:
Kod źródłowy backendu aplikacji do zarządzania danymi firm. Aplikacja umożliwia przechowywanie, przetwarzanie i zarządzanie informacjami o firmach oraz powiązanych danych.

## Techstack i wzorce:
- **Język programowania:** C#
- **Framework:** ASP.NET Core
- **Baza danych:** PostgreSQL / SQL Server
- **ORM:** Entity Framework Core
- **Autoryzacja i uwierzytelnianie:** JWT
- **Konteneryzacja:** Docker
- **Wzorce projektowe:** Repository Pattern, Unit of Work, Dependency Injection

## Schemat bazodanowy:
(Schemat można dodać w postaci obrazu, np. `![img.png](sciezka_do_obrazka.png)`, jeśli go posiadasz)

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

## API Endpoints:
- **GET** `/api/companies` - Pobranie listy firm
- **GET** `/api/companies/{id}` - Pobranie szczegółów firmy
- **POST** `/api/companies` - Dodanie nowej firmy (wymaga autoryzacji)
- **PUT** `/api/companies/{id}` - Aktualizacja danych firmy (wymaga autoryzacji)
- **DELETE** `/api/companies/{id}` - Usunięcie firmy (wymaga autoryzacji administratora)

## Informacje dot. testowania:
- Przykładowe dane i skrypt tworzenia bazy danych znajdują się w katalogu `/backend/src/assets`
- Testowanie API można przeprowadzić za pomocą Postmana lub Swaggera (`/swagger` po uruchomieniu serwera)

## Instrukcja uruchomienia:
1. Sklonuj repozytorium:
   ```sh
   git clone https://github.com/StanslawStepkas28016/CompanyProjectApp.git
   cd CompanyProjectApp
   ```
2. Skonfiguruj zmienne środowiskowe (np. w pliku `.env` lub `appsettings.json`).
3. Uruchom aplikację przy użyciu Dockera lub lokalnie:
   ```sh
   dotnet run --project backend
   ```
4. Otwórz przeglądarkę i przejdź do `http://localhost:5000/swagger` w celu testowania API.

## Dodatkowe informacje:
- Aplikacja wykorzystuje JWT do autoryzacji.
- Obsługuje paginację oraz filtrowanie danych.
- Logi aplikacji są przechowywane w plikach logów w katalogu `logs/`.
- Możliwość integracji z systemami zewnętrznymi poprzez API.
