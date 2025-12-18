# 🔧 Rregullimi i Admin Login

## ✅ Admin është i konfiguruar saktë në databazë:
- **Email:** `admin@biblioteka.com`
- **Password:** `admin123`
- **Role:** `Admin`
- **IsActive:** `1` (aktiv)

## 🚨 Nëse admin nuk po funksionon, ndiqni këto hapa:

### Hapi 1: Ndaloni Backend
Në terminal ku po ekzekutohet backend, shtypni `Ctrl+C` për ta ndalur.

### Hapi 2: Ekzekutoni Skriptin për të Siguruar Admin
```bash
sqlcmd -S localhost -E -i test_admin_login.sql
```

Ose ekzekutoni direkt:
```bash
sqlcmd -S localhost -E -Q "USE BibliotekaDB; UPDATE Users SET Password = 'admin123', Role = 'Admin', IsActive = 1 WHERE Email = 'admin@biblioteka.com'; IF @@ROWCOUNT = 0 INSERT INTO Users (FirstName, LastName, Email, Password, Phone, Address, RegistrationDate, IsActive, Role) VALUES ('Admin', 'User', 'admin@biblioteka.com', 'admin123', '+355 69 0000000', 'Tiranë', GETDATE(), 1, 'Admin');"
```

### Hapi 3: Rinisni Backend
```bash
cd src/Backend/Biblioteka.API
dotnet run
```

### Hapi 4: Testoni Login
1. Hapni `http://localhost:3000`
2. Klikoni "Kyçu"
3. Shkruani:
   - **Email:** `admin@biblioteka.com`
   - **Password:** `admin123`

## 🔍 Debug nëse ende nuk funksionon:

### Kontrolloni në databazë:
```sql
SELECT Email, Password, Role, IsActive 
FROM Users 
WHERE Email = 'admin@biblioteka.com';
```

Duhet të shihni:
- Email: `admin@biblioteka.com`
- Password: `admin123`
- Role: `Admin`
- IsActive: `1`

### Kontrolloni Backend Logs
Kur provoni të kyçeni, shikoni në terminal të backend-it për çfarë gabimi shfaqet.

### Testoni direkt API
Mund të testoni direkt API-n me Postman ose curl:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@biblioteka.com\",\"password\":\"admin123\"}"
```

## ✅ Pas rregullimit, admin duhet të funksionojë!

