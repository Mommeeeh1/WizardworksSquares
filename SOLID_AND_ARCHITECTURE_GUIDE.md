# SOLID Principles & Architecture Guide

En komplett guide som förklarar SOLID-principerna och design/arkitektur som används i Wizardworks Squares projektet.

---

## 📋 Innehåll

1. [SOLID Principles](#solid-principles)
2. [Design Patterns](#design-patterns)
3. [Architecture Overview](#architecture-overview)
4. [Layered Architecture](#layered-architecture)
5. [Separation of Concerns](#separation-of-concerns)
6. [Dependency Injection](#dependency-injection)
7. [Frontend Architecture](#frontend-architecture)

---

## 🎯 SOLID Principles

SOLID är fem design-principer som hjälper till att skriva ren, underhållbar och skalbar kod.

### **S** - Single Responsibility Principle (SRP)

**Vad betyder det?**
En klass eller modul ska bara ha en anledning att ändras. Den ska bara ha ett ansvar.

**I ditt projekt:**

#### ✅ **SquareService** - Har bara ett ansvar
```csharp
public class SquareService : ISquareService
{
    // Ansvar: Business logic för squares
    // - Spiral-algoritmen
    // - Färgval
    // - Positionering
    // INTE: Data persistence (det gör Repository)
}
```

**Varför är detta bra?**
- Om du behöver ändra business logic → Ändra bara SquareService
- Om du behöver ändra data storage → Ändra bara Repository
- Lättare att testa (mock repository)
- Lättare att förstå

#### ✅ **SquareRepository** - Har bara ett ansvar
```csharp
public class SquareRepository : ISquareRepository
{
    // Ansvar: Data persistence
    // - Läsa från JSON
    // - Skriva till JSON
    // INTE: Business logic (det gör Service)
}
```

#### ✅ **SquareEndpoints** - Har bara ett ansvar
```csharp
public static class SquareEndpoints
{
    // Ansvar: HTTP endpoints
    // - Mappa routes
    // - Hantera HTTP requests/responses
    // INTE: Business logic (anropar Service)
}
```

**Exempel på vad som skulle vara FEL:**
```csharp
// ❌ BAD - Flera ansvar
public class SquareManager
{
    // Ansvar 1: Business logic
    public void CreateSquare() { ... }
    
    // Ansvar 2: Data persistence
    public void SaveToFile() { ... }
    
    // Ansvar 3: HTTP handling
    public HttpResponse HandleRequest() { ... }
}
```

**I ditt projekt:**
- ✅ SquareService = Business logic
- ✅ SquareRepository = Data persistence
- ✅ SquareEndpoints = HTTP handling
- ✅ GlobalExceptionHandlerMiddleware = Error handling

---

### **O** - Open/Closed Principle (OCP)

**Vad betyder det?**
Klasser ska vara öppna för utökning men stängda för modifiering. Du ska kunna lägga till ny funktionalitet utan att ändra befintlig kod.

**I ditt projekt:**

#### ✅ **Interface-baserad design**
```csharp
// Interface - "Öppen" för nya implementationer
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
}

// Implementation - "Stängd" för modifiering
public class SquareRepository : ISquareRepository
{
    // JSON-baserad implementation
}

// Framtida utökning - INTE ändra SquareRepository!
public class DatabaseSquareRepository : ISquareRepository
{
    // SQL-baserad implementation
    // Samma interface, annan implementation
}
```

**Varför är detta bra?**
- Du kan byta implementation (JSON → Database) utan att ändra Service
- Du kan lägga till nya implementationer (t.ex. MongoDB) utan att ändra befintlig kod
- Service koden förblir oförändrad

**Viktigt: När får man ändra ett interface?**

**❌ Generellt sett: INTE ofta!**
Om du ändrar ett interface måste ALLA implementationer ändras också. Detta kan bryta befintlig kod.

**Exempel på problem:**
```csharp
// Om du ändrar interface:
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
    Task<Square> UpdateAsync(Square square); // ← NY metod
}

// Måste ändra ALLA implementationer:
public class SquareRepository : ISquareRepository
{
    // Måste implementera UpdateAsync() även om du inte använder den
    public Task<Square> UpdateAsync(Square square)
    {
        throw new NotImplementedException(); // ❌ Problem!
    }
}

public class DatabaseSquareRepository : ISquareRepository
{
    // Måste också implementera UpdateAsync()
}
```

**✅ När det är OK att ändra interface:**
1. **När du lägger till ny funktionalitet som alla behöver**
   - T.ex. om alla repositories behöver en `UpdateAsync()` metod
   - Men tänk på konsekvenserna!

2. **När projektet är nytt och inte i produktion än**
   - Mindre risk att bryta befintlig kod

3. **När du använder default implementations (C# 8.0+)**
   ```csharp
   public interface ISquareRepository
   {
       Task<List<Square>> GetAllAsync();
       Task<Square> CreateAsync(Square square);
       Task ClearAllAsync();
       
       // Default implementation - gamla implementationer behöver inte ändras
       Task<bool> ExistsAsync(Guid id) => Task.FromResult(false);
   }
   ```

**✅ Bättre alternativ: Skapa nytt interface**
```csharp
// Istället för att ändra ISquareRepository:
public interface ISquareRepository
{
    // Befintliga metoder förblir oförändrade
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
}

// Skapa nytt interface för utökad funktionalitet:
public interface IUpdatableSquareRepository : ISquareRepository
{
    Task<Square> UpdateAsync(Square square);
}

// Bara implementationer som behöver update implementerar det nya interfacet
public class DatabaseSquareRepository : IUpdatableSquareRepository
{
    // Implementerar både ISquareRepository och UpdateAsync()
}
```

**Sammanfattning:**
- ✅ **Lägg till nya implementationer** - OCP följs
- ❌ **Ändra befintligt interface** - Kan bryta OCP (måste ändra alla implementationer)
- ✅ **Skapa nytt interface** - Bättre än att ändra befintligt
- ✅ **Default implementations** - Om språket stödjer det

#### ✅ **Middleware pattern**
```csharp
// GlobalExceptionHandlerMiddleware - "Öppen" för utökning
public class GlobalExceptionHandlerMiddleware
{
    // Kan lägga till fler exception types utan att ändra kärnan
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentNullException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            // Lätt att lägga till fler:
            // NotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
```

**Exempel på vad som skulle vara FEL:**
```csharp
// ❌ BAD - Måste modifiera koden för att lägga till ny funktionalitet
public class SquareService
{
    public async Task<SquareDto> CreateSquareAsync()
    {
        // Om vi vill lägga till email-notifikation:
        // Måste ändra denna metod → Bryter OCP
        if (someCondition)
        {
            SendEmail(); // Ny kod i befintlig metod
        }
    }
}

// ✅ GOOD - Utöka istället för att modifiera
public class SquareService
{
    // Befintlig kod förblir oförändrad
}

public class NotificationSquareService : ISquareService
{
    private readonly ISquareService _baseService;
    
    public async Task<SquareDto> CreateSquareAsync()
    {
        var square = await _baseService.CreateSquareAsync();
        SendEmail(); // Ny funktionalitet utan att ändra befintlig kod
        return square;
    }
}
```

**Praktisk regel för interfaces:**
- ✅ **Lägg till nya implementationer** - Följer OCP
- ⚠️ **Ändra interface försiktigt** - Kan kräva ändringar i alla implementationer
- ✅ **Skapa nytt interface** - Bättre än att ändra befintligt (t.ex. `IUpdatableSquareRepository`)
- ✅ **Använd default implementations** - Om språket stödjer det (C# 8.0+)

---

### **L** - Liskov Substitution Principle (LSP)

**Vad betyder det?**
Objekt av en subklass ska kunna ersätta objekt av basklassen utan att bryta programmet. Om du har en interface, ska alla implementationer kunna användas på samma sätt.

**I ditt projekt:**

#### ✅ **ISquareRepository implementations**
```csharp
// Interface - Kontrakt
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
}

// Implementation 1: JSON
public class SquareRepository : ISquareRepository
{
    // Kan ersättas med DatabaseSquareRepository
}

// Implementation 2: Database (hypotetisk)
public class DatabaseSquareRepository : ISquareRepository
{
    // Måste följa samma kontrakt
    // Kan användas på exakt samma sätt
}
```

**I Program.cs:**
```csharp
// Byt implementation utan att ändra Service
builder.Services.AddScoped<ISquareRepository, SquareRepository>();
// eller
builder.Services.AddScoped<ISquareRepository, DatabaseSquareRepository>();
// Service koden förblir oförändrad!
```

**Varför är detta viktigt?**
- SquareService kan använda vilken implementation som helst
- Tester kan använda mock implementations
- Lätt att byta implementation (JSON → Database)

**Exempel på vad som skulle vara FEL:**
```csharp
// ❌ BAD - Implementation bryter kontraktet
public class DatabaseSquareRepository : ISquareRepository
{
    public async Task<List<Square>> GetAllAsync()
    {
        // Returnerar null istället för tom lista
        // Service förväntar sig en lista, inte null
        return null; // ❌ Bryter LSP
    }
}

// ✅ GOOD - Följer kontraktet
public class DatabaseSquareRepository : ISquareRepository
{
    public async Task<List<Square>> GetAllAsync()
    {
        // Returnerar alltid en lista (tom om inga squares)
        return squares ?? new List<Square>(); // ✅ Följer kontraktet
    }
}
```

---

### **I** - Interface Segregation Principle (ISP)

**Vad betyder det?**
Klienter ska inte tvingas implementera interfaces de inte använder. Skapa små, specifika interfaces istället för stora, generella.

**I ditt projekt:**

#### ✅ **Specifika interfaces**
```csharp
// ISquareRepository - Specifikt för squares
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
}

// ISquareService - Specifikt för square business logic
public interface ISquareService
{
    Task<List<SquareDto>> GetAllSquaresAsync();
    Task<SquareDto> CreateSquareAsync();
    Task<bool> ClearAllSquaresAsync();
}
```

**Varför är detta bra?**
- Varje interface har ett tydligt syfte
- Lätt att förstå vad varje interface gör
- Lätt att implementera (bara implementera det du behöver)

**Exempel på vad som skulle vara FEL:**
```csharp
// ❌ BAD - För stort interface
public interface IDataRepository
{
    // Squares
    Task<List<Square>> GetAllSquaresAsync();
    Task<Square> CreateSquareAsync(Square square);
    
    // Users
    Task<List<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    
    // Products
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> CreateProductAsync(Product product);
}

// Om du bara behöver squares, måste du ändå implementera alla metoder
public class SquareRepository : IDataRepository
{
    // Måste implementera GetAllUsersAsync() även om du inte använder det
    public Task<List<User>> GetAllUsersAsync() 
    {
        throw new NotImplementedException(); // ❌ ISP violation
    }
}

// ✅ GOOD - Separerade interfaces
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
}

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
}

// Nu implementerar du bara det du behöver
public class SquareRepository : ISquareRepository
{
    // Bara square-metoder - ✅ Följer ISP
}
```

---

### **D** - Dependency Inversion Principle (DIP)

**Vad betyder det?**
Högnivå-moduler ska inte bero på lågnivå-moduler. Båda ska bero på abstraktioner (interfaces). Abstraktioner ska inte bero på detaljer, detaljer ska bero på abstraktioner.

**I ditt projekt:**

#### ✅ **Dependency Injection med interfaces**
```csharp
// SquareService (högnivå) beror på ISquareRepository (abstraktion)
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository; // Interface, inte konkret klass
    
    public SquareService(ISquareRepository repository, ILogger<SquareService> logger)
    {
        _repository = repository; // Dependency injection
    }
}
```

**I Program.cs:**
```csharp
// Registrera implementations
builder.Services.AddScoped<ISquareRepository, SquareRepository>();
builder.Services.AddScoped<ISquareService, SquareService>();
```

**Varför är detta viktigt?**
- SquareService beror på interface, inte konkret implementation
- Lätt att byta implementation (JSON → Database)
- Lätt att testa (mock repository)
- Låg koppling (loose coupling)

**Arkitektur-diagram:**
```
┌─────────────────┐
│ SquareEndpoints │  (Högnivå)
└────────┬────────┘
         │ beror på
         ▼
┌─────────────────┐
│ ISquareService  │  (Abstraktion)
└────────┬────────┘
         │ beror på
         ▼
┌─────────────────┐
│  SquareService  │  (Högnivå)
└────────┬────────┘
         │ beror på
         ▼
┌─────────────────┐
│ISquareRepository│  (Abstraktion)
└────────┬────────┘
         │ implementeras av
         ▼
┌─────────────────┐
│SquareRepository │  (Lågnivå)
└─────────────────┘
```

**Exempel på vad som skulle vara FEL:**
```csharp
// ❌ BAD - Direkt beroende på konkret klass
public class SquareService
{
    private readonly SquareRepository _repository; // Konkret klass
    
    public SquareService()
    {
        _repository = new SquareRepository(); // ❌ DIP violation
    }
}

// Problem:
// - Kan inte byta implementation
// - Kan inte testa (svårt att mocka)
// - Tätt kopplad (tight coupling)

// ✅ GOOD - Beroende på interface
public class SquareService
{
    private readonly ISquareRepository _repository; // Interface
    
    public SquareService(ISquareRepository repository) // Dependency injection
    {
        _repository = repository; // ✅ Följer DIP
    }
}
```

---

## 🏗️ Design Patterns

### 1. **Repository Pattern**

**Vad är det?**
Ett pattern som abstraherar data access-lagret. Ger ett enhetligt interface för att komma åt data.

**I ditt projekt:**
```csharp
// Interface - Abstraktion
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
    Task<Square> CreateAsync(Square square);
    Task ClearAllAsync();
}

// Implementation - Konkret data access
public class SquareRepository : ISquareRepository
{
    // JSON-baserad implementation
    // Kan bytas till Database, MongoDB, etc.
}
```

**Fördelar:**
- ✅ Separation of concerns (data access separerad från business logic)
- ✅ Lätt att testa (mock repository)
- ✅ Lätt att byta data storage (JSON → Database)
- ✅ Följer DIP (Service beror på interface)

---

### 2. **Service Layer Pattern**

**Vad är det?**
En lager som innehåller business logic, separerad från data access och presentation.

**I ditt projekt:**
```csharp
// Service - Business logic
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository;
    
    public async Task<SquareDto> CreateSquareAsync()
    {
        // Business logic:
        // 1. Beräkna position (spiral-algoritm)
        // 2. Välj färg
        // 3. Skapa square
        // 4. Spara via repository
    }
}
```

**Fördelar:**
- ✅ Business logic på ett ställe
- ✅ Lätt att testa
- ✅ Kan återanvändas (API, console app, etc.)
- ✅ Följer SRP (bara business logic)

---

### 3. **Dependency Injection Pattern**

**Vad är det?**
Ett pattern där dependencies injiceras från utsidan istället för att skapas inuti klassen.

**I ditt projekt:**
```csharp
// Program.cs - Registrera dependencies
builder.Services.AddScoped<ISquareRepository, SquareRepository>();
builder.Services.AddScoped<ISquareService, SquareService>();

// SquareService - Får dependencies via constructor
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository;
    
    public SquareService(ISquareRepository repository, ILogger<SquareService> logger)
    {
        _repository = repository; // Injekterad, inte skapad här
    }
}
```

**Fördelar:**
- ✅ Låg koppling (loose coupling)
- ✅ Lätt att testa (mock dependencies)
- ✅ Lätt att byta implementation
- ✅ Följer DIP

---

### 4. **Middleware Pattern**

**Vad är det?**
Ett pattern där requests går genom en pipeline av middleware-komponenter.

**I ditt projekt:**
```csharp
// GlobalExceptionHandlerMiddleware
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Gå vidare i pipeline
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Hantera fel
        }
    }
}

// Program.cs
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
```

**Fördelar:**
- ✅ Centraliserad error handling
- ✅ Separation of concerns
- ✅ Kan lägga till fler middleware (logging, authentication, etc.)

---

### 5. **DTO Pattern (Data Transfer Object)**

**Vad är det?**
Objekt som används för att överföra data mellan lager, utan att exponera interna modeller.

**I ditt projekt:**
```csharp
// Model - Intern representation
public class Square
{
    public Guid Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public string Color { get; set; }
    public DateTime CreatedAt { get; set; }
}

// DTO - Exponerad till API
public record SquareDto(
    Guid Id,
    int Row,
    int Column,
    string Color,
    DateTime CreatedAt
);

// Service returnerar DTO, inte Model
public async Task<SquareDto> CreateSquareAsync()
{
    var square = new Square { ... }; // Intern model
    await _repository.CreateAsync(square);
    return new SquareDto(...); // Returnera DTO
}
```

**Fördelar:**
- ✅ Kontroll över vad som exponeras
- ✅ Kan ändra intern model utan att påverka API
- ✅ Separation of concerns

---

### 6. **Container/Presentation Pattern (Frontend)**

**Vad är det?**
Ett pattern som separerar state/logik (Container) från rendering (Presentation).

**I ditt projekt:**
```typescript
// Container - State och logik
export const SquaresContainer: React.FC = () => {
  const { squares, isLoading, error, addSquare, clearAll } = useSquares();
  
  return (
    <div>
      <Button onClick={addSquare}>Add Square</Button>
      <Grid squares={squares} /> {/* Presentation */}
    </div>
  );
};

// Presentation - Bara rendering
export const Grid: React.FC<GridProps> = ({ squares }) => {
  // Ingen state, bara renderar props
  return <div>{/* Render squares */}</div>;
};
```

**Fördelar:**
- ✅ Separation of concerns
- ✅ Lätt att testa (Grid är pure function)
- ✅ Återanvändbar (Grid kan användas överallt)
- ✅ Följer SRP

---

### 7. **Custom Hooks Pattern (Frontend)**

**Vad är det?**
Extrahera logik till återanvändbara hooks.

**I ditt projekt:**
```typescript
// Custom hook - Logik
export const useSquares = () => {
  const [squares, setSquares] = useState<Square[]>([]);
  
  const addSquare = async () => {
    const newSquare = await apiClient.createSquare();
    setSquares(prev => [...prev, newSquare]);
  };
  
  return { squares, addSquare, clearAll };
};

// Komponent - Använder hook
export const SquaresContainer = () => {
  const { squares, addSquare } = useSquares();
  // Komponenten är enkel, logiken är i hooken
};
```

**Fördelar:**
- ✅ Återanvändbar logik
- ✅ Separation of concerns
- ✅ Lätt att testa (testa hook separat)

---

## 🏛️ Architecture Overview

### **Layered Architecture**

Din app följer en layered architecture (lager-arkitektur):

```
┌─────────────────────────────────────┐
│      Presentation Layer             │
│  (Endpoints, HTTP handling)         │
│  SquareEndpoints                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Business Logic Layer            │
│  (Services, Business rules)          │
│  SquareService                       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Access Layer               │
│  (Repositories, Data persistence)    │
│  SquareRepository                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Storage                    │
│  (JSON file, Database, etc.)         │
└──────────────────────────────────────┘
```

**Varje lager:**
- ✅ Har ett tydligt ansvar (SRP)
- ✅ Beror på abstraktioner, inte konkreta klasser (DIP)
- ✅ Kan bytas ut utan att påverka andra lager

---

## 📐 Separation of Concerns

### **Backend Separation**

```
┌─────────────────────────────────────┐
│  SquareEndpoints                    │
│  Ansvar: HTTP requests/responses     │
└──────────────┬──────────────────────┘
               │ anropar
┌──────────────▼──────────────────────┐
│  SquareService                       │
│  Ansvar: Business logic              │
│  - Spiral-algoritm                   │
│  - Färgval                           │
└──────────────┬──────────────────────┘
               │ anropar
┌──────────────▼──────────────────────┐
│  SquareRepository                    │
│  Ansvar: Data persistence            │
│  - Läsa/skriva JSON                  │
└──────────────┬──────────────────────┘
               │ använder
┌──────────────▼──────────────────────┐
│  JSON File                           │
│  Data storage                        │
└──────────────────────────────────────┘
```

### **Frontend Separation**

```
┌─────────────────────────────────────┐
│  SquaresContainer                    │
│  Ansvar: Orchestrera UI             │
│  - State management                  │
│  - Koordinera komponenter            │
└──────────────┬──────────────────────┘
               │
       ┌───────┴────────┐
       │                │
┌──────▼──────┐  ┌──────▼──────┐
│  useSquares │  │    Grid     │
│  Hook        │  │ Presentation│
│  Logik       │  │ Rendering   │
└──────┬───────┘  └─────────────┘
       │
┌──────▼──────┐
│  api.ts     │
│  API calls  │
└─────────────┘
```

---

## 💉 Dependency Injection

### **Hur det fungerar i ditt projekt:**

**1. Registrera dependencies (Program.cs):**
```csharp
builder.Services.AddScoped<ISquareRepository, SquareRepository>();
builder.Services.AddScoped<ISquareService, SquareService>();
```

**2. Injecta i konstruktor:**
```csharp
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository;
    
    public SquareService(ISquareRepository repository, ILogger<SquareService> logger)
    {
        _repository = repository; // DI container injicerar automatiskt
    }
}
```

**3. Använd i endpoints:**
```csharp
group.MapGet("/", async (ISquareService squareService) =>
{
    // DI container injicerar automatiskt
    var squares = await squareService.GetAllSquaresAsync();
    return Results.Ok(squares);
});
```

**Fördelar:**
- ✅ Låg koppling
- ✅ Lätt att testa
- ✅ Centraliserad konfiguration
- ✅ Följer DIP

---

## 🎨 Frontend Architecture

### **Struktur:**

```
src/
├── components/
│   ├── squares/
│   │   ├── SquaresContainer.tsx  (Container)
│   │   └── Grid.tsx               (Presentation)
│   └── ui/
│       ├── Button.tsx             (Reusable)
│       ├── ErrorMessage.tsx       (Reusable)
│       └── LoadingOverlay.tsx     (Reusable)
├── hooks/
│   └── useSquares.ts              (Custom hook)
├── services/
│   └── api.ts                     (API client)
├── types/
│   └── square.ts                  (TypeScript types)
├── utils/
│   └── validation.ts              (Validation logic)
└── constants/
    └── gridConstants.ts           (Constants)
```

### **Design Patterns i Frontend:**

1. **Container/Presentation Pattern**
   - SquaresContainer = Container (state, logik)
   - Grid = Presentation (rendering)

2. **Custom Hooks Pattern**
   - useSquares = Extraherar logik

3. **Service Layer Pattern**
   - api.ts = Centraliserad API-kommunikation

4. **Constants Pattern**
   - gridConstants.ts = Centraliserade konstanter

---

## 📊 SOLID Summary i Ditt Projekt

| Princip | Var det appliceras | Exempel |
|---------|-------------------|---------|
| **S** - Single Responsibility | Varje klass har ett ansvar | SquareService = Business logic, SquareRepository = Data access |
| **O** - Open/Closed | Interface-baserad design | ISquareRepository kan ha flera implementationer |
| **L** - Liskov Substitution | Alla implementations kan ersätta interface | SquareRepository och DatabaseSquareRepository (hypotetisk) |
| **I** - Interface Segregation | Små, specifika interfaces | ISquareRepository, ISquareService (separerade) |
| **D** - Dependency Inversion | Dependency injection med interfaces | SquareService beror på ISquareRepository, inte SquareRepository |

---

## 🎯 Best Practices Använda i Projektet

### ✅ **1. Interface-baserad design**
- Alla dependencies är interfaces
- Lätt att byta implementation
- Lätt att testa

### ✅ **2. Dependency Injection**
- Alla dependencies injiceras
- Centraliserad konfiguration
- Låg koppling

### ✅ **3. Separation of Concerns**
- Varje lager har ett ansvar
- Tydlig separation mellan lager
- Lätt att förstå

### ✅ **4. Error Handling**
- GlobalExceptionHandlerMiddleware
- Centraliserad error handling
- Konsistenta error responses

### ✅ **5. Logging**
- ILogger<T> i alla klasser
- Structured logging
- Lätt att debugga

### ✅ **6. DTO Pattern**
- Separerade modeller och DTOs
- Kontroll över vad som exponeras
- Säkerhet

---

## 🔍 Code Examples - SOLID i Praktiken

### **Exempel 1: SRP - Single Responsibility**

```csharp
// ✅ GOOD - Varje klass har ett ansvar
public class SquareService : ISquareService
{
    // Ansvar: Business logic
    public async Task<SquareDto> CreateSquareAsync()
    {
        // Business logic här
    }
}

public class SquareRepository : ISquareRepository
{
    // Ansvar: Data access
    public async Task<Square> CreateAsync(Square square)
    {
        // Data access här
    }
}
```

### **Exempel 2: DIP - Dependency Inversion**

```csharp
// ✅ GOOD - Beroende på interface
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository; // Interface
    
    public SquareService(ISquareRepository repository) // DI
    {
        _repository = repository;
    }
}

// ❌ BAD - Beroende på konkret klass
public class SquareService
{
    private readonly SquareRepository _repository; // Konkret klass
    
    public SquareService()
    {
        _repository = new SquareRepository(); // Direkt instantiering
    }
}
```

### **Exempel 3: OCP - Open/Closed**

```csharp
// ✅ GOOD - Öppen för utökning
public interface ISquareRepository
{
    Task<List<Square>> GetAllAsync();
}

// Kan lägga till nya implementationer utan att ändra interface
public class DatabaseSquareRepository : ISquareRepository
{
    // Ny implementation
}

// Service koden förblir oförändrad
```

---

## 📝 Sammanfattning

**SOLID-principerna i ditt projekt:**

1. ✅ **SRP** - Varje klass har ett tydligt ansvar
2. ✅ **OCP** - Interface-baserad design, öppen för utökning
3. ✅ **LSP** - Alla implementations kan ersätta interface
4. ✅ **ISP** - Små, specifika interfaces
5. ✅ **DIP** - Dependency injection med interfaces

**Design Patterns:**
- Repository Pattern
- Service Layer Pattern
- Dependency Injection Pattern
- Middleware Pattern
- DTO Pattern
- Container/Presentation Pattern (Frontend)
- Custom Hooks Pattern (Frontend)

**Arkitektur:**
- Layered Architecture
- Separation of Concerns
- Dependency Injection
- Interface-baserad design

**Resultat:**
- ✅ Ren, underhållbar kod
- ✅ Lätt att testa
- ✅ Lätt att utöka
- ✅ Låg koppling
- ✅ Hög kohesion

---

## 🔗 Relaterade Guider

- `FRONTEND_GUIDE.md` - Detaljerad frontend-förklaring
- `TESTING_GUIDE.md` - Förklaring av tester
- `CONCEPTS_GUIDE.md` - Allmänna koncept
- `INTERVIEW_GUIDE.md` - Förberedelse för intervjuer
