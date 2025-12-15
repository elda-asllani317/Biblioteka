# Si të Startoni Aplikacionin

## Hapi 1: Startoni Backend (.NET API)

Hapni një terminal të ri PowerShell dhe ekzekutoni:

```powershell
cd src/Backend/Biblioteka.API
dotnet run --urls "http://localhost:5000"
```

Backend do të jetë i disponueshëm në: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

## Hapi 2: Startoni Frontend (React.js)

Hapni një terminal tjetër PowerShell dhe ekzekutoni:

```powershell
cd src/Frontend
npm start
```

Frontend do të hapet automatikisht në browser në: `http://localhost:3000`

## Nëse browser-i nuk hapet automatikisht:

1. Hapni manualisht browser-in tuaj (Chrome, Edge, Firefox)
2. Shkruani në adresë: `http://localhost:3000`

## Kontrolloni nëse aplikacionet po ekzekutohen:

```powershell
# Kontrolloni portin 5000 (Backend)
netstat -ano | findstr :5000

# Kontrolloni portin 3000 (Frontend)
netstat -ano | findstr :3000
```

## Problemet e mundshme:

1. **CORS Error**: Sigurohuni që backend-i po ekzekutohet në portin 5000
2. **Port tashmë në përdorim**: Mbyllni aplikacionet e tjera që përdorin portet 3000 ose 5000
3. **Node modules**: Nëse ka probleme, ekzekutoni `npm install` përsëri në folder-in Frontend

## URL-të e rëndësishme:

- Frontend: http://localhost:3000
- Backend API: http://localhost:5000/api
- Swagger: http://localhost:5000/swagger

