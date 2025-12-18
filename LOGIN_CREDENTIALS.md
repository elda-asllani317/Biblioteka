# 📋 Credentials për Login

## 🔐 Credentials për Testim

### 👑 Admin (Administrator)
**Email:** `admin`  
**Password:** `admin`  
**Role:** Admin  
**Aksesi:** Të gjitha funksionalitetet (Books, Authors, Categories, Publishers, Book Copies, Loans, Fines, Users, Notifications, Reviews)

---

### 👤 Përdorues (User)
**Email:** `john.doe@example.com`  
**Password:** `password123`  
**Role:** User  
**Aksesi:** 
- ✅ Dashboard
- ✅ Books (vetëm shikim)
- ✅ Loans (vetëm shikim)
- ✅ Notifications
- ✅ Reviews
- ❌ Fines (nuk ka akses)
- ❌ Users Management (nuk ka akses)
- ❌ Authors, Categories, Publishers Management (nuk ka akses)
- ❌ Book Copies (nuk ka akses)
- ❌ Add Book (nuk ka akses)
- ❌ New Loan (nuk ka akses)

---

### 👤 Përdorues tjetër (User)
**Email:** `jane.smith@example.com`  
**Password:** `password123`  
**Role:** User  
**Aksesi:** I njëjtë si përdoruesi i mësipërm

---

## 📝 Shënime

1. **Regjistrim i ri:** Çdo regjistrim i ri krijon automatikisht një përdorues me role "User" (jo Admin).

2. **Krijimi i Admin:** Vetëm një admin ekzistues mund të krijojë përdorues të tjerë me role "Admin" përmes Users Management.

3. **Ndryshimi i Role:** Admin mund të ndryshojë role-in e përdoruesve përmes Users Management.

---

## 🚀 Si të kyçeni

1. Hapni aplikacionin në `http://localhost:3000`
2. Klikoni "Kyçu" ose shkoni te `/login`
3. Shkruani email dhe password nga lista e mësipërme
4. Pas login, do të ridrejtoheni në Dashboard
5. Dashboard do të tregojë opsione të ndryshme bazuar në role tuaj

---

## ⚠️ Nëse keni probleme me login - "Invalid column name 'Role'"

Nëse merrni gabim "Invalid column name 'Role'", ndiqni këto hapa:

### Hapi 1: Ndaloni aplikacionin backend
Ndalo aplikacionin që po ekzekutohet (Ctrl+C në terminal)

### Hapi 2: Ekzekutoni skriptin SQL
```bash
sqlcmd -S localhost -E -i add_role_to_users.sql
```

### Hapi 3: Aplikoni migracionin
```bash
cd src/Backend/Biblioteka.Infrastructure
dotnet ef database update --startup-project ../Biblioteka.API
```

### Hapi 4: Rinisni backend-in
```bash
cd ../Biblioteka.API
dotnet run
```

---

## 📊 Tabela e Aksesit

| Funksionalitet | Admin | User |
|----------------|-------|------|
| Dashboard | ✅ | ✅ |
| Books (Shikim) | ✅ | ✅ |
| Books (Shtim/Edit) | ✅ | ❌ |
| Authors | ✅ | ❌ |
| Categories | ✅ | ❌ |
| Publishers | ✅ | ❌ |
| Book Copies | ✅ | ❌ |
| Loans (Shikim) | ✅ | ✅ |
| Loans (Krijim) | ✅ | ❌ |
| Fines | ✅ | ❌ |
| Users | ✅ | ❌ |
| Notifications | ✅ | ✅ |
| Reviews | ✅ | ✅ |

