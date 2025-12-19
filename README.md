# Biblioteka - Library Management System

Sistem i menaxhimit të bibliotekës i krijuar me React.js, .NET 8, dhe SQL Server.

## Arkitektura

Projekti përdor **Layered Architecture** me shtresat e mëposhtme:

1. **Presentation Layer** (Biblioteka.API) - API Controllers dhe DTOs
2. **Service Layer** (Biblioteka.Infrastructure/Services) - Business Logic
3. **Data Access Layer** (Biblioteka.Infrastructure/Repositories) - Repository Pattern dhe Unit of Work
4. **Domain Layer** (Biblioteka.Core) - Entities dhe Interfaces

## Design Patterns

Projekti implementon dy pattern-e të mesme:

1. **Repository Pattern** - Abstraksion i data access layer për të lehtësuar testimin dhe menaxhimin e të dhënave
2. **Unit of Work Pattern** - Menaxhim i transaksioneve dhe koordinim i repositories

## Struktura e Projektit

```
Biblioteka/
├── src/
│   ├── Backend/
│   │   ├── Biblioteka.API/          # Presentation Layer
│   │   ├── Biblioteka.Core/         # Domain Layer
│   │   └── Biblioteka.Infrastructure/ # Data Access & Services
│   └── Frontend/                     # React.js Application
├── Biblioteka.sln
└── README.md
```

## Tabelat e Databazës (10 tabela)

1. **Users** - Përdoruesit e bibliotekës
2. **Authors** - Autorët e librave
3. **Categories** - Kategoritë e librave
4. **Publishers** - Botuesit
5. **Books** - Librat
6. **BookCopies** - Kopjet e librave
7. **Loans** - Huazimet
8. **Reviews** - Vlerësimet e librave
9. **Fines** - Gjobat
10. **Notifications** - Njoftimet

## Kërkesat

### Backend
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 ose VS Code

### Frontend
- Node.js 18+
- npm ose yarn

## Instalimi dhe Ekzekutimi

### Backend

1. Hapni projektin në Visual Studio ose VS Code
2. Konfiguroni connection string në `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BibliotekaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Krijoni migracionet:
```bash
cd src/Backend/Biblioteka.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Biblioteka.API
dotnet ef database update --startup-project ../Biblioteka.API
```

4. Ekzekutoni API-n:
```bash
cd src/Backend/Biblioteka.API
dotnet run
```

API do të jetë e disponueshme në `https://localhost:7000`

Frontend

1. Instaloni dependencies:
```bash
cd src/Frontend
npm install
```

2. Ekzekutoni aplikacionin:
```bash
npm start
```

Aplikacioni do të hapet në `http://localhost:3000`

API Endpoints

Books
- `GET /api/books` - Merr të gjithë librat
- `GET /api/books/{id}` - Merr librin me ID
- `POST /api/books` - Krijon libër të ri
- `PUT /api/books/{id}` - Përditëson librin
- `DELETE /api/books/{id}` - Fshin librin
- `GET /api/books/search?term={term}` - Kërkon libra

Loans
- `GET /api/loans/{id}` - Merr huazimin me ID
- `POST /api/loans` - Krijon huazim të ri
- `POST /api/loans/{id}/return` - Kthen librin
- `GET /api/loans/user/{userId}` - Merr huazimet e përdoruesit
- `GET /api/loans/overdue` - Merr huazimet e vonuara

 Karakteristikat

- OO Programming
- Layered Architecture
-  Repository Pattern
-  Unit of Work Pattern
-  Entity Framework Core
-  RESTful API
 - React.js Frontend
- Responsive Design

 Autor:
 Elda Asllani
 Merise  Shallci

