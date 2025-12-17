# Testing Guide - Förklaring av Alla Tester

En komplett guide som förklarar alla tester i Wizardworks Squares projektet.

---

## 📋 Översikt

**Vad testar vi?**
- Spiral-algoritmen (hur squares placeras)
- SquareService (grundläggande funktionalitet)
- Grid-storlek beräkningar
- Positioner och unikhet

**Test-ramverk:**
- **xUnit** - Test framework för .NET
- **FluentAssertions** - Mer läsbara assertions
- **NSubstitute** - Mocking library (simulerar dependencies)

---

## 🧪 Testfiler

### Nuvarande struktur:
```
WizardworksSquares.Tests/
├── Services/
│   └── SquareServiceTests.cs
└── SpiralAlgorithmTests.cs  ← I root, inte i mapp
```

### Best Practice: Matcha produktionskodens struktur

**Produktionskod:**
```
WizardworksSquares.Api/
├── Services/
│   ├── ISquareService.cs
│   └── SquareService.cs
├── Repositories/
│   ├── ISquareRepository.cs
│   └── SquareRepository.cs
└── Models/
    └── Square.cs
```

**Rekommenderad test-struktur:**
```
WizardworksSquares.Tests/
├── Services/
│   ├── SquareServiceTests.cs          ← Testar SquareService
│   └── SpiralAlgorithmTests.cs        ← Testar också SquareService (algoritmen)
└── Repositories/
    └── SquareRepositoryTests.cs        ← Om du skulle testa Repository
```

**Varför?**
- ✅ Lätt att hitta tester (samma struktur som produktionskod)
- ✅ Tydligt vilken kod som testas
- ✅ Konsekvent organisation
- ✅ Skalbar (lätt att lägga till fler tester)

### 1. `SpiralAlgorithmTests.cs`
Testar spiral-algoritmen i detalj. **Borde vara i Services/ mappen** eftersom algoritmen är en del av SquareService.

### 2. `SquareServiceTests.cs`
Testar grundläggande funktionalitet i SquareService. **Är korrekt placerad i Services/ mappen.**

---

## 🔍 Detaljerad Förklaring av Tester

### SpiralAlgorithmTests.cs

#### **Test 1: `SpiralAlgorithm_ShouldHaveUniquePositions`**

**Vad testar den?**
Kontrollerar att varje square får en unik position (ingen square hamnar på samma plats som en annan).

**Hur fungerar den?**
```csharp
[Theory]
[InlineData(1)]   // Testar med 1 square
[InlineData(4)]   // Testar med 4 squares
[InlineData(9)]   // Testar med 9 squares
[InlineData(16)]  // Testar med 16 squares
[InlineData(25)]  // Testar med 25 squares
```

**Steg-för-steg:**
1. **Arrange:** Skapar en mock repository och service
2. **Act:** Skapar squares en i taget (1, 4, 9, 16, eller 25 squares)
3. **Assert:** Kontrollerar att alla positioner är unika (ingen duplicering)

**Varför är detta viktigt?**
- Om två squares hamnar på samma position, kommer en att dölja den andra
- Spiral-algoritmen måste garantera unika positioner

**Exempel:**
- Med 4 squares: Positioner ska vara (0,0), (0,1), (1,1), (1,0) - alla unika ✅
- Om två squares hade (0,0): Testet skulle faila ❌

---

#### **Test 2: `SpiralAlgorithm_GridSize_ShouldFollowCeilSqrtFormula`**

**Vad testar den?**
Kontrollerar att grid-storleken följer formeln `ceil(sqrt(count))` korrekt.

**Formeln:**
- 1 square → Grid: 1x1 (ceil(sqrt(1)) = 1)
- 2 squares → Grid: 2x2 (ceil(sqrt(2)) = 2)
- 4 squares → Grid: 2x2 (ceil(sqrt(4)) = 2)
- 5 squares → Grid: 3x3 (ceil(sqrt(5)) = 3)
- 9 squares → Grid: 3x3 (ceil(sqrt(9)) = 3)
- 10 squares → Grid: 4x4 (ceil(sqrt(10)) = 4)

**Hur fungerar den?**
```csharp
var testCases = new[]
{
    (squareCount: 1, expectedGridSize: 1),
    (squareCount: 2, expectedGridSize: 2),
    (squareCount: 4, expectedGridSize: 2),
    (squareCount: 5, expectedGridSize: 3),
    (squareCount: 9, expectedGridSize: 3),
    (squareCount: 10, expectedGridSize: 4),
};
```

**Steg-för-steg:**
1. **Arrange:** För varje test case, rensa och förbered
2. **Act:** Skapa det antal squares som specificeras
3. **Assert:** Kontrollera att alla squares passar inom den förväntade grid-storleken
   - Max row < expectedGridSize
   - Max column < expectedGridSize

**Varför är detta viktigt?**
- Gridet måste vara stort nog för alla squares
- Men inte större än nödvändigt
- Frontend använder samma formel, så de måste matcha!

**Exempel:**
- 5 squares skapas
- Förväntad grid-storlek: 3x3 (ceil(sqrt(5)) = 3)
- Testet kontrollerar att max row < 3 och max column < 3
- Om en square hamnade på row 3 eller column 3 → Testet skulle faila ❌

---

#### **Test 3: `SpiralAlgorithm_FirstNinePositions_ShouldFollowExpectedPattern`**

**Vad testar den?**
Kontrollerar att de första 9 squares placeras i exakt rätt spiral-mönster.

**Förväntat mönster (första 9 squares):**
```
Square 1: (0, 0) - Center
Square 2: (0, 1) - Höger
Square 3: (1, 1) - Ner
Square 4: (1, 0) - Vänster
Square 5: (0, 2) - Start av ring 3
Square 6: (1, 2)
Square 7: (2, 2)
Square 8: (2, 1)
Square 9: (2, 0)
```

**Visuellt:**
```
Square 5   Square 2   Square 1
(0,2)      (0,1)      (0,0)

Square 9   Square 4   Square 3
(2,0)      (1,0)      (1,1)

Square 8   Square 7   Square 6
(2,1)      (2,2)      (1,2)
```

**Hur fungerar den?**
```csharp
var expectedPositions = new List<(int row, int col)>
{
    (0, 0), // Square 1: center
    (0, 1), // Square 2: right
    (1, 1), // Square 3: down
    (1, 0), // Square 4: left
    (0, 2), // Square 5: start of ring 3
    (1, 2), // Square 6
    (2, 2), // Square 7
    (2, 1), // Square 8
    (2, 0), // Square 9
};
```

**Steg-för-steg:**
1. **Arrange:** Förbered service och mock repository
2. **Act:** Skapa exakt 9 squares
3. **Assert:** För varje square, kontrollera att row och column matchar exakt förväntat värde

**Varför är detta viktigt?**
- Detta är "smoke test" för spiral-algoritmen
- Om de första 9 squares är fel, är hela algoritmen fel
- Detta är det mest specifika testet - det kontrollerar exakt positioner

**Exempel:**
- Square 1 ska vara på (0, 0) - om den är på (0, 1) → Testet failar ❌
- Square 5 ska vara på (0, 2) - om den är på (1, 0) → Testet failar ❌

---

#### **Test 4: `SpiralAlgorithm_PositionsShouldNotChange_AfterClearAndRestart`**

**Vad testar den?**
Kontrollerar att spiral-algoritmen startar om från början efter att alla squares rensats.

**Hur fungerar den?**
1. **Första körningen:** Skapar 5 squares
2. **Rensa:** Anropar `ClearAllSquaresAsync()`
3. **Andra körningen:** Skapar 5 squares igen
4. **Assert:** De nya 5 squares ska ha exakt samma positioner som de första 5 squares

**Steg-för-steg:**
```csharp
// Första körningen
for (int i = 0; i < 5; i++)
{
    await service.CreateSquareAsync();
}
var firstRunPositions = firstRunSquares.Select(s => (s.Row, s.Column)).ToList();

// Rensa
await service.ClearAllSquaresAsync();

// Andra körningen
for (int i = 0; i < 5; i++)
{
    await service.CreateSquareAsync();
}
var secondRunPositions = secondRunSquares.Select(s => (s.Row, s.Column)).ToList();

// Assert - Positioner ska vara identiska
secondRunPositions.Should().BeEquivalentTo(firstRunPositions);
```

**Varför är detta viktigt?**
- Användaren förväntar sig att när de rensar och skapar nya squares, ska de börja från början
- Om positionerna ändras efter clear, är det förvirrande
- Detta testar att state hanteras korrekt

**Exempel:**
- Första körningen: Squares på (0,0), (0,1), (1,1), (1,0), (0,2)
- Efter clear och andra körningen: Squares ska vara på exakt samma positioner
- Om de är på olika positioner → Testet failar ❌

---

### SquareServiceTests.cs

#### **Test 1: `CreateSquareAsync_FirstSquare_ShouldBePlacedAtCenter`**

**Vad testar den?**
Kontrollerar att den första squaren alltid placeras i mitten (position 0,0).

**Hur fungerar den?**
```csharp
// Arrange
_mockRepository.GetAllAsync().Returns(new List<Square>()); // Inga squares ännu

// Act
var result = await _service.CreateSquareAsync();

// Assert
result.Row.Should().Be(0);
result.Column.Should().Be(0);
```

**Steg-för-steg:**
1. **Arrange:** Mock repository returnerar tom lista (inga squares finns)
2. **Act:** Skapa första squaren
3. **Assert:** Kontrollera att row = 0 och column = 0

**Varför är detta viktigt?**
- Detta är grundläggande beteende - första squaren ska alltid vara i mitten
- Om detta inte fungerar, fungerar ingenting
- Detta är ett enkelt "happy path" test

**Exempel:**
- Första squaren skapas → Position: (0, 0) ✅
- Om den hamnade på (1, 1) → Testet failar ❌

---

#### **Test 2: `CreateSquareAsync_AfterClear_ShouldStartFromCenter`**

**Vad testar den?**
Kontrollerar att efter att alla squares rensats, börjar spiralen om från mitten igen.

**Hur fungerar den?**
```csharp
// Skapa en square
await _service.CreateSquareAsync();

// Rensa alla
await _service.ClearAllSquaresAsync();

// Skapa första squaren igen
var result = await _service.CreateSquareAsync();

// Assert - Ska vara på (0, 0) igen
result.Row.Should().Be(0);
result.Column.Should().Be(0);
```

**Steg-för-steg:**
1. **Arrange:** Skapa en square (den hamnar på (0,0))
2. **Clear:** Rensa alla squares
3. **Act:** Skapa första squaren igen
4. **Assert:** Den nya squaren ska vara på (0,0) igen

**Varför är detta viktigt?**
- Efter clear ska systemet "glömma" alla tidigare squares
- Nästa square ska börja från början (center)
- Detta testar att clear-funktionaliteten fungerar korrekt

**Exempel:**
- Skapa square → (0, 0)
- Clear all
- Skapa square igen → Ska vara (0, 0) igen ✅
- Om den hamnade på (0, 1) → Testet failar ❌

---

## 🛠️ Test-tekniker som Används

### 1. **Mocking med NSubstitute**

**Vad är mocking?**
Mocking betyder att vi "simulerar" dependencies (som repository) istället för att använda riktiga.

**Varför?**
- Vi vill testa logiken, inte databasen
- Tester ska vara snabba (ingen databas-anrop)
- Vi kan kontrollera exakt vad som händer

**Exempel:**
```csharp
var mockRepository = Substitute.For<ISquareRepository>();
mockRepository.GetAllAsync().Returns(new List<Square>());
```

Detta betyder: "När någon anropar `GetAllAsync()`, returnera en tom lista."

---

### 2. **Theory och InlineData**

**Theory:**
En test som körs flera gånger med olika data.

**InlineData:**
Specificerar vilka värden testet ska köras med.

**Exempel:**
```csharp
[Theory]
[InlineData(1)]
[InlineData(4)]
[InlineData(9)]
public async Task Test(int squareCount)
{
    // Testet körs 3 gånger: med 1, 4, och 9 squares
}
```

**Fördelar:**
- Skriv ett test, testa många scenarion
- Mindre kod att skriva
- Lätt att lägga till fler test cases

---

### 3. **FluentAssertions**

**Vad är det?**
Ett bibliotek som gör assertions mer läsbara.

**Jämförelse:**

**Utan FluentAssertions:**
```csharp
Assert.Equal(0, result.Row);
Assert.Equal(0, result.Column);
```

**Med FluentAssertions:**
```csharp
result.Row.Should().Be(0);
result.Column.Should().Be(0);
```

**Fördelar:**
- Mer läsbart (läs som en mening)
- Bättre felmeddelanden
- Mer expressivt

---

### 4. **Arrange-Act-Assert Pattern**

Alla tester följer detta mönster:

**Arrange:** Förbered testet
- Skapa mocks
- Sätt upp data
- Konfigurera dependencies

**Act:** Kör koden som ska testas
- Anropa metoden
- Spara resultatet

**Assert:** Kontrollera resultatet
- Verifiera att det är korrekt
- Kontrollera side effects

**Exempel:**
```csharp
// Arrange
var mockRepository = Substitute.For<ISquareRepository>();
var service = new SquareService(mockRepository, mockLogger);

// Act
var result = await service.CreateSquareAsync();

// Assert
result.Row.Should().Be(0);
```

---

## 📊 Test Coverage

**Vad täcker testerna?**

✅ **Spiral-algoritmen:**
- Unika positioner
- Grid-storlek beräkningar
- Exakt positioner för första 9 squares
- Restart efter clear

✅ **SquareService:**
- Första squaren i center
- Restart efter clear

❌ **Vad täcker testerna INTE?**
- API endpoints (integration tests)
- Frontend (React komponenter)
- Database operations (använder mocks)
- Error handling i detalj

---

## 🚀 Hur Kör Man Testerna?

### I Visual Studio:
1. Öppna Test Explorer (Test → Test Explorer)
2. Klicka "Run All Tests"
3. Se resultat

### I Terminal:
```bash
dotnet test
```

### Kör specifik test:
```bash
dotnet test --filter "SpiralAlgorithm_ShouldHaveUniquePositions"
```

---

## 💡 Varför Dessa Tester?

**1. Säkerställer korrekt funktionalitet**
- Om algoritmen är fel, kommer testerna att faila
- Tydliga felmeddelanden visar vad som är fel

**2. Dokumentation**
- Testerna visar hur systemet ska fungera
- Nya utvecklare kan läsa testerna för att förstå

**3. Refactoring-säkerhet**
- Om du ändrar kod, kan du köra testerna
- Om testerna passerar, har du inte förstört något

**4. Regression prevention**
- Om något går sönder i framtiden, kommer testerna att upptäcka det
- "Safety net" för framtida ändringar

---

## 📝 Sammanfattning

**Totalt antal tester:** 6
- 4 tester för spiral-algoritmen
- 2 tester för grundläggande funktionalitet

**Vad testar vi?**
- ✅ Unika positioner
- ✅ Grid-storlek formel
- ✅ Exakt spiral-mönster
- ✅ Restart efter clear
- ✅ Första squaren i center

**Test-tekniker:**
- Mocking (NSubstitute)
- Theory/InlineData
- FluentAssertions
- Arrange-Act-Assert pattern

**Nästa steg:**
- Lägg till fler edge cases
- Integration tests för API
- Frontend tests (React Testing Library)

---

## 🔗 Relaterade Guider

- `FRONTEND_GUIDE.md` - Förklaring av frontend
- `CONCEPTS_GUIDE.md` - Allmänna koncept
- `INTERVIEW_GUIDE.md` - Förberedelse för intervjuer
