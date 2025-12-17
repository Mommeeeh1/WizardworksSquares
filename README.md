# Wizardworks Squares

A web application that generates colored squares in an expanding grid pattern. Built with .NET/C# backend and React/TypeScript frontend.

## 📋 Overview

This project demonstrates full-stack development capabilities. Users can add colored squares to a grid, with each square automatically positioned using a spiral algorithm and assigned a random color. All data is persisted via a RESTful API and survives page reloads.

## ✨ Features

- **Dynamic Grid System**: Squares are automatically positioned in an expanding spiral pattern
- **Random Colors**: Each square receives a unique random color from a predefined palette
- **Data Persistence**: All squares are saved to a JSON file via API and restored on page load
- **Real-time Updates**: UI updates instantly when squares are added or cleared
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Loading States**: Smart loading indicators that only appear for slow requests (>300ms)
- **Responsive Design**: Clean, modern UI that works across different screen sizes

## 🛠️ Tech Stack

### Backend

- **.NET 9.0** with C#
- **ASP.NET Core Minimal APIs**
- **Swagger/OpenAPI** for API documentation
- **JSON file-based storage**
- **Repository Pattern** for data access
- **Service Layer** for business logic
- **Global Exception Middleware** for error handling
- **xUnit** for unit testing

### Frontend

- **React 18** with TypeScript
- **Vite** for build tooling
- **Custom Hooks** for state management
- **Tailwind CSS** for styling
- **Fetch API** for HTTP requests

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Node.js 18+
- npm or yarn

### Backend Setup

1. Navigate to the backend directory:
```bash
cd WizardworksSquares.Api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5268`
- HTTPS: `https://localhost:7232`

4. Access Swagger documentation:
```
http://localhost:5268
```

### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd wizardworks-squares-frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The application will start on `http://localhost:5173`

### Configuration

The frontend API URL can be configured in `wizardworks-squares-frontend/.env`:
```
VITE_API_URL=http://localhost:5268
```

## 🧪 Running Tests

### Backend Tests

Navigate to the test project directory and run:
```bash
cd WizardworksSquares.Tests
dotnet test
```

The test suite includes:
- **Spiral Algorithm Tests**: Validates the spiral positioning algorithm
  - Ensures unique positions for all squares
  - Verifies correct grid expansion
  - Tests center positioning
- **Service Tests**: Validates business logic
  - First square placement at center
  - Grid reset behavior

## 📡 API Endpoints

### GET `/api/squares`
Retrieves all squares from storage.

**Response**: `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "row": 0,
    "column": 0,
    "color": "#FF5733",
    "createdAt": "2024-01-15T10:30:00Z"
  }
]
```

### POST `/api/squares`
Creates a new square with automatic position and random color.

**Response**: `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "row": 0,
  "column": 1,
  "color": "#33FF57",
  "createdAt": "2024-01-15T10:30:05Z"
}
```

### DELETE `/api/squares`
Removes all squares from storage.

**Response**: `204 No Content`

### GET `/health`
Health check endpoint.

**Response**: `200 OK`
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🎯 Design Decisions

### Architecture

- **Repository Pattern**: Separates data access logic from business logic, making the code more testable and maintainable.
- **Service Layer**: Encapsulates business logic including the spiral positioning algorithm and color selection.
- **DTOs**: Clear separation between domain models and API contracts.
- **Custom Hooks**: React state management is extracted into reusable hooks for cleaner components.

### Spiral Algorithm

Squares are positioned using an expanding spiral algorithm:
- Starts at (0,0)
- Expands outward in rings
- Grid size grows as needed: `ceil(sqrt(count))`
- Ensures a roughly square grid shape

### Error Handling

**Backend**:
- Global exception middleware catches all unhandled exceptions
- Returns RFC 7807 Problem Details format
- Maps exception types to appropriate HTTP status codes (400, 500)

**Frontend**:
- Try-catch blocks in all API calls
- Error state management with dismissible error messages
- Graceful fallbacks for network errors
- User-friendly error messages

### Data Persistence

**JSON File Storage**: Simple, lightweight persistence suitable for a demo application. Easy to inspect and debug.

## 📝 Code Quality

- **Consistent Naming**: PascalCase (C#), camelCase (TypeScript)
- **Type Safety**: TypeScript on frontend, strong typing on backend
- **Documentation**: XML comments (C#), JSDoc (TypeScript)
- **Error Handling**: Comprehensive error handling throughout
- **Clean Code**: SOLID principles, DRY, separation of concerns
- **Tested**: Unit tests for core business logic

## 📄 License

This project is created as a technical assessment.
