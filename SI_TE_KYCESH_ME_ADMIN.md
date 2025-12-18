# 🔐 Si të Kyçesh si Admin

## ✅ Kredencialet e Adminit:

**Email/Username:** `admin`  
**Password:** `admin`

## 📋 HAPAT PËR TË KYÇUR:

### 1. Sigurohuni që Backend është i rikthyer:
- Në terminal ku po ekzekutohet backend, shtypni `Ctrl+C` për ta ndalur
- Pastaj ekzekutoni:
  ```bash
  cd src/Backend/Biblioteka.API
  dotnet run
  ```
- Prisni derisa të shihni: `Now listening on: http://localhost:5000`

### 2. Sigurohuni që Frontend është duke u ekzekutuar:
- Në terminal të frontend, ekzekutoni:
  ```bash
  cd src/Frontend
  npm start
  ```
- Prisni derisa të hapet `http://localhost:3000`

### 3. Hapni Browser:
- Shkoni te: `http://localhost:3000`
- Ose klikoni link-un që shfaqet në terminal

### 4. Klikoni "Kyçu":
- Në faqen kryesore, klikoni butonin "Kyçu" ose shkoni direkt te `/login`

### 5. Shkruani Kredencialet:
- **Email ose Username:** `admin`
- **Password:** `admin`
- **Kujdes:** Mos shtoni hapësira para ose pas!

### 6. Klikoni "Kyçu":
- Pas plotësimit, klikoni butonin "Kyçu"

## ✅ Çfarë duhet të ndodhë:

1. **Nëse login-i është i suksesshëm:**
   - Do të ridrejtoheni automatikisht në `/dashboard`
   - Do të shihni Dashboard-in e Adminit me të gjitha opsionet
   - Në navbar do të shfaqet "(Admin)" pranë emrit tuaj

2. **Nëse shfaqet gabim "Email ose password i gabuar":**
   - Kontrolloni nëse backend-i është i rikthyer
   - Kontrolloni nëse kredencialet janë të sakta: `admin` / `admin`
   - Kontrolloni terminalin e backend-it për mesazhe logging

## 🔍 Debug nëse nuk funksionon:

### Kontrolloni në terminal të backend-it:
Duhet të shihni mesazhe si:
- `Login attempt for email: admin`
- `Total users in database: X`
- `User found: admin, Role: Admin, IsActive: True`
- `Login successful for user: admin, Role: Admin`

### Nëse shihni "User not found":
- Kontrolloni nëse admin ekziston në databazë:
  ```sql
  SELECT Email, Password, Role, IsActive 
  FROM Users 
  WHERE Email = 'admin'
  ```

### Nëse shihni "Password mismatch":
- Sigurohuni që password-i është saktësisht: `admin` (pa hapësira)

## 🆘 Nëse ende nuk funksionon:

1. **Rinisni backend-in:**
   - `Ctrl+C` për ta ndalur
   - `dotnet run` për ta rikthyer

2. **Rinisni frontend-in:**
   - `Ctrl+C` për ta ndalur
   - `npm start` për ta rikthyer

3. **Pastroni localStorage në browser:**
   - Hapni Developer Tools (F12)
   - Shkoni te Application > Local Storage
   - Fshini të gjitha çelësat

4. **Provoni përsëri login-in**

## ✅ Pas login të suksesshëm:

Do të shihni Dashboard-in e Adminit me:
- 📚 Books
- ✍️ Authors
- 📂 Categories
- 🏢 Publishers
- 📖 Book Copies
- 📋 Loans
- 💰 Fines
- 👥 Users
- 🔔 Notifications
- ⭐ Reviews

