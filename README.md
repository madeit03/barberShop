# BarberShop Rezerwacje - Aplikacja ASP.NET Core MVC

## Opisl Projektu

Aplikacja webowa do rezerwacji usług fryzjerskich oparta na wzorcu MVC (Model-View-Controller). System umożliwia użytkownikom przeglądanie dostępnych usług, rezerwację terminów i zarządzanie swoimi rezerwacjami. Administratorzy mogą zarządzać usługami, terminami oraz zatwierdzać rezerwacje.

## Wymagania

- .NET 8.0 SDK lub nowszy
- SQL Server LocalDB
- Visual Studio, VS Code lub inny edytor kodu

## Technologie

- **Backend**: ASP.NET Core 8.0 MVC
- **Baza danych**: SQL Server z Entity Framework Core 8.0
- **Autoryzacja**: ASP.NET Core Identity
- **Frontend**: HTML5, Bootstrap 5, CSS3, JavaScript

## Struktura projektu

```
BarberShopReservation/
├── Models/
│   ├── AppUser.cs          # Model użytkownika (Identity)
│   ├── Service.cs          # Model usługi fryzjerskiej
│   ├── TimeSlot.cs         # Model terminu dostępności
│   └── Reservation.cs      # Model rezerwacji
├── Controllers/
│   ├── HomeController.cs   # Strona główna, lista usług
│   ├── ReservationController.cs # Zarządzanie rezerwacjami użytkownika
│   ├── AdminController.cs  # Panel administratora
│   └── AccountController.cs # Rejestracja i logowanie
├── Data/
│   ├── ApplicationDbContext.cs # Entity Framework DbContext
│   └── DataSeeder.cs       # Seeding przykładowych danych
├── Views/
│   ├── Home/               # Widoki strony głównej
│   ├── Account/            # Widoki logowania i rejestracji
│   ├── Reservation/        # Widoki rezerwacji
│   ├── Admin/              # Widoki panelu administratora
│   └── Shared/             # Layout i wspólne widoki
└── Program.cs              # Konfiguracja aplikacji
```

## Modele Danych

### AppUser
- Rozszerza IdentityUser
- Pole: FirstName, LastName, CreatedAt
- Relacja: Wiele rezerwacji

### Service
- Id, Name, Description, Price, DurationMinutes, IsActive
- Relacja: Wiele czasów (TimeSlots) i rezerwacji (Reservations)

### TimeSlot
- Id, ServiceId, StartTime, EndTime, IsAvailable
- Relacja: Jedna rezerwacja

### Reservation
- Id, ServiceId, UserId, TimeSlotId, Status, Notes, CreatedAt, ApprovedAt
- Statuses: Pending, Approved, Cancelled, Completed

## Instalacja i Uruchomienie

### 1. Klonowanie repozytorium
```bash
git clone [URL-repozytorium]
cd BarberShopReservation
```

### 2. Przywrócenie pakietów NuGet
```bash
dotnet restore
```

### 3. Aplikowanie migracji do bazy danych
```bash
dotnet ef database update
```

Aplikacja automatycznie:
- Tworzy role: Admin, User
- Tworzy konto administratora (email: admin@barbershop.com, hasło: Admin123)
- Seeduje przykładowe usługi i terminy

### 4. Uruchomienie aplikacji
```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem: `https://localhost:7121`

## Funkcjonalności

### Dla zwykłych użytkowników:
- ✅ Rejestracja i logowanie
- ✅ Przeglądanie dostępnych usług
- ✅ Rezerwacja terminu usługi
- ✅ Przeglądanie swoich rezerwacji
- ✅ Edycja rezerwacji
- ✅ Anulowanie rezerwacji

### Dla administratora:
- ✅ Zarządzanie usługami (CRUD)
- ✅ Zarządzanie terminami dostępności
- ✅ Przeglądanie wszystkich rezerwacji
- ✅ Zatwierdzanie/odrzucanie rezerwacji
- ✅ Dashboard ze statystykami
- ✅ Filtrowanie rezerwacji po statusie

## Dane testowe

### Konto administratora:
- Email: `admin@barbershop.com`
- Hasło: `Admin123`

### Usługi dostępne:
1. Strzyżenie męskie - 30 zł
2. Golenie - 25 zł
3. Strzyżenie + Golenie - 50 zł
4. Stylizacja brody - 20 zł
5. Farbowanie włosów - 40 zł

## Diagram bazy danych

```
┌─────────────┐
│  AspNetUsers│
│ (AppUser)   │
└──────┬──────┘
       │ 1:N
       ├──────────────────┐
       │                  │
┌──────▼──────┐   ┌──────▼─────────┐
│ Reservations│   │ AspNetUserRoles│
│   1:N       │   └─────────────────┘
└──────┬──────┘
       │ N:1
┌──────▼─────────────┐
│ Services & TimeSlots
│      1:N          │
└───────────────────┘
```

## Migracje

Migracje są przechowywane w folderze `Migrations/`. Aby dodać nową migrację:

```bash
dotnet ef migrations add NazwaMigracji
dotnet ef database update
```

Aby cofnąć migrację:
```bash
dotnet ef migrations remove
```

## Autoryzacja i Role

- Role `Admin` - dostęp do panelu administracyjnego
- Rola `User` - standardowy użytkownik

Polityki autoryzacji:
- `[Authorize]` - wymaga zalogowania
- `[Authorize(Roles = "Admin")]` - wymaga roli Admin
- `[AllowAnonymous]` - brak wymaganień

## Stylowanie

- Używamy Bootstrap 5 z CDN
- Niestandardowe kolory: 
  - Główny: #f39c12 (pomarańczowy)
  - Tło: #1a1a1a (czarny)
- Responsywny design mobilny

## Testy

Możesz przetestować aplikację poprzez:

1. **Rejestracja nowego użytkownika** - przycisk "Rejestracja"
2. **Rezerwacja usługi** - Strona główna → Zarezerwuj
3. **Panel admina** - Zaloguj się jako admin@barbershop.com

## Rozwiązywanie problemów

### Błąd bazy danych
```bash
dotnet ef database drop --force
dotnet ef database update
```

### Czyszczenie cache
```bash
dotnet clean
dotnet build
```

## Autor
Projekt akademicki - Rezerwacja usług fryzjerskich

## Licencja
MIT

## Kontakt
W razie pytań lub problemów, prosimy o kontakt poprzez Issues w repozytorium.
