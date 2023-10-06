# Dokumentation för Web-API för Bokplattform

### Innehållsförteckning

1. Introduktion
2. Komma igång
   - Bygga koden
   - Konfigurera och köra applikationen
3. Funktioner
4. Arkitektur
5. Tekniska detaljer
6. Felsökning

---

### 1. Introduktion

Projektet innefattar utvecklandet av ett web-API för att hantera data och kommunikation mellan databasen och front-end-applikationen för en social plattform. Plattformen möjliggör för användare att hålla reda på vilka böcker de vill läsa och har läst, samt att diskutera dem genom bokcirklar.

#### Bakgrund

Trots att konceptet inte är nyskapande ser beställaren potential genom att kombinera funktioner för bokuppföljning och bokcirkeldiskussioner, något som de anser saknas på den nuvarande marknaden.

#### Målgrupp

Målgruppen innefattar bokälskare av alla åldrar, vilket gör det essentiellt att web-API:t kan ansluta till olika enheter, såsom läsplattor för seniorer och mobila enheter för yngre generationer.

### 2. Komma igång

#### 2.1 Bygga koden

För att bygga applikationen, följ dessa steg:

1. Klona projektet från det önskade repositoriet.
2. Öppna projektet i din föredragna utvecklingsmiljö (t.ex. Visual Studio).
3. Kompilera koden utifrån .sln-filen.
4. Lös eventuella beroenden eller saknade referenser om det uppmanas.

#### 2.2 Konfigurera och köra applikationen

Konfigurering och exekvering av applikationen kräver:

1. Säkerställande av korrekt kompilering av koden.
2. Konfiguration av databas och API-inställningar (om det behövs).
3. Starta API och kontrollera att det är tillgängligt via webbläsaren eller API-testverktyg som Postman.

### 3. Funktioner

API:et tillhandahåller diverse endpoint för att:

- Hämta alla böcker.
- Hämta, skapa, uppdatera och radera en specifik bok baserat på ID.
- Hantera användare och deras bibliotek av böcker.
- Skapa och hantera evenemang (bokcirklar), inklusive tillägga en bok till en cirkel.

### 4. Arkitektur

API:et är utvecklat för att hantera all datamanipulation och kommunikation med databasen. Strukturen är uppbyggd för att hantera olika typer av data, såsom böcker och evenemang, och relationer mellan dessa, med hjälp av olika controllers och endpoints.

### 5. Tekniska detaljer

Datastrukturer och kommunikation realiseras genom olika metoder och endpoint. Exempelvis är metoderna för att hämta, lägga till, uppdatera och radera böcker eller evenemang skrivna med syftet att erbjuda dessa funktioner på ett säkert och optimerat sätt genom att använda olika HTTP-metoder (GET, POST, PUT, DELETE, etc.)

### 6. Felsökning

Om ett anrop till API:et misslyckas eller returnerar oväntad data, bör följande steg följas för felsökning:

- Kontrollera API-loggar för eventuella felmeddelanden eller varningar.
- Verifiera att anropets struktur och payload är korrekt formaterade.
- Verifiera att eventuella autentiseringsuppgifter eller tokens är korrekta och giltiga.
- Säkerställ att databasen är tillgänglig och att datan är korrekt strukturerad.

Hela API:et är uppbyggt för att hantera CRUD-operationer på de dataentiteter som är nödvändiga för att fungera som en backend till en applikation för att hålla reda på och diskutera böcker. Denna dokumentation kan sedan vidareutvecklas med mer detaljerade instruktioner för varje endpoint och exempelanrop för att assistera framtida utvecklare och användare av API:et.

### 7. Versionsinformation

0.1 - Ett första utkast för att demonstrera grundläggande funktionalitet.

- Nyckeln för signering och validering av tokens ligger öppen i appsettings.development.json.
- Två roller, user och admin skapas första gången man startar applikationen om databasen är tom.
- Det skapas en admin- och en user-användare första gången man kör applikationen om databasen är tom.
