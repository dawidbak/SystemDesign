# URL Shortener

A high-performance URL shortening service built with .NET 9 and PostgreSQL, featuring distributed ID generation using Snowflake algorithm, in-memory caching, and rate limiting.

## 🚀 Features

- **URL Shortening**: Convert long URLs into short, shareable links
- **URL Redirection**: Fast redirection from short URLs to original URLs
- **Snowflake ID Generation**: Distributed, time-based unique ID generation for scalability
- **Caching Layer**: In-memory cache with decorator pattern for improved performance
- **Rate Limiting**: Token bucket algorithm to prevent abuse (100 requests per minute per IP)
- **Vertical Slice Architecture**: Features organized by business capability with all layers in one place for better maintainability
- **PostgreSQL Database**: Reliable data persistence with Entity Framework Core
- **OpenAPI/Scalar**: Interactive API documentation

## 🏗️ Architecture

The solution follows Vertical Slice Architecture principles, where features are organized by business capability rather than technical layers. Each feature contains mostly all the code needed to fulfill a specific use case.

**Benefits:**
- **High Cohesion**: Related code stays together
- **Low Coupling**: Features are independent and easy to modify
- **Easy Navigation**: Everything for a feature is almost in one place(we are sharing i.e. repositories, models, etc.)
- **Scalability**: People can work on different features without conflicts
- **Testability**: Each slice can be tested independently

```
├── Api/                          # Presentation Layer
│   ├── Program.cs                # Application entry point with rate limiting
│   ├── appsettings.json          # Configuration
│   └── Dockerfile                # Container configuration
│
└── Application/                  # Application Layer
    ├── Common/                   # Shared components
    │   ├── IEndpoint.cs          # Endpoint interface
    │   ├── Handlers.cs           # Command/Query handlers
    │   └── Options/
    │       └── ShortenerOptions.cs
    │
    ├── Domain/                   # Domain models
    │   └── UrlMapping.cs
    │
    ├── Features/                 # Feature-based organization (Vertical Slices)
    │   └── Url/
    │       ├── GetShortUrl/      # Complete slice: Endpoint → Handler → Repository
    │       │   ├── Endpoint.cs
    │       │   ├── GetShortUrl.cs
    │       │   └── GetShortUrlHandler.cs
    │       │
    │       └── ShortenUrl/       # Complete slice: Endpoint → Handler → Repository
    │           ├── Endpoint.cs
    │           ├── ShortenUrl.cs
    │           ├── ShortenUrlDto.cs
    │           └── ShortenUrlHandler.cs
    │
    └── Infrastructure/           # Shared infrastructure
        ├── Persistence/          # Database context
        ├── Repositories/         # Data access with caching
        │   └── UrlMappingRepository.cs (Cache Decorator Pattern)
        ├── Services/
        │   └── SnowflakeService.cs
        └── Migrations/           # EF Core migrations
```

## 🛠️ Technology Stack

- **.NET 9** - Latest .NET framework
- **ASP.NET Core Minimal APIs** - Lightweight HTTP APIs
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL 18** - Primary database
- **Docker & Docker Compose** - Containerization
- **IdGen** - Snowflake ID generation library
- **Scalar** - Modern API documentation
- **In-Memory Cache** - Performance optimization

## 📋 Prerequisites

- .NET 9 SDK
- Docker Desktop (for PostgreSQL)
- Visual Studio 2022 / JetBrains Rider / VS Code

## 📊 Business Requirements & Capacity Planning

### Traffic Estimates

**Write Operations:**
- **Daily URL Generation**: 100 million URLs per day
- **Write Operations per Second**: 100,000,000 ÷ 24 ÷ 3,600 ≈ **1,160 writes/sec**
- **Peak Load (2x)**: ~2,320 writes/sec

**Read Operations:**
- **Read-to-Write Ratio**: 10:1 (typical for URL shorteners)
- **Read Operations per Second**: 1,160 × 10 = **11,600 reads/sec**
- **Peak Load (2x)**: ~23,200 reads/sec

### Storage Capacity (10-Year Projection)

**Data Growth:**
```
Daily records:     100,000,000 URLs
Yearly records:    100,000,000 × 365 = 36,500,000,000 URLs
10-year records:   36,500,000,000 × 10 = 365,000,000,000 URLs (365 billion)
```

**Storage Calculation:**

Assuming each URL mapping requires approximately **520 bytes**:
- Snowflake ID (8 bytes)
- Short URL (50 bytes average)
- Original URL (400 bytes average)
- Indexes and metadata (62 bytes)

```
Raw data:          365,000,000,000 × 520 bytes = 189,800,000,000,000 bytes
                   189.8 TB

With overhead:     189.8 TB × 2 (indexes, replication, overhead)
                   ≈ 380 TB
```

**Result: ~190 TB of raw data, ~380 TB total with overhead after 10 years**

## 📐 System Design Diagrams

### High-Level Architecture Diagram

**Description:**
A comprehensive system architecture diagram showing the complete flow from client requests through load balancer to multiple API instances, caching layer (Redis), database cluster (PostgreSQL with read replicas), and monitoring/logging infrastructure.

---

### API Endpoints Flow Diagram

## 🚦 Getting Started

### 1. Clone the repository

### 2. Start PostgreSQL with Docker Compose

```bash
cd src
docker-compose up -d
```

This will start a PostgreSQL instance on `localhost:5432` with:
- Database: `urlshortener`
- Username: `postgres`
- Password: `postgres`

### 3. Run Database Migrations

```bash
cd Application
dotnet ef database update
```

### 4. Run the Application

```bash
cd ../Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7041`
- API Documentation: `http://localhost:7041/scalar`

## 📡 API Endpoints

### Create Short URL

**POST** `/api/shorten`

**Request Body:**
```json
{
  "originalUrl": "https://www.example.com/very/long/url/that/needs/shortening"
}
```

**Response:**
```json
{
  "shortUrl": "http://localhost:7041/api/abc123",
  "originalUrl": "https://www.example.com/very/long/url/that/needs/shortening"
}
```

### Redirect to Original URL

**GET** `/api/{shortCode}`

Redirects to the original URL associated with the short code.

**Example:**
```
GET http://localhost:7041/api/abc123
→ Redirects to https://www.example.com/very/long/url/that/needs/shortening
```

## 🔧 Configuration

Edit `appsettings.json` to configure the application:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=urlshortener;Username=postgres;Password=postgres"
  },
  "Shortener": {
    "BaseUrl": "http://localhost:7041/api/"
  }
}
```

## 🎯 Design Patterns Used

### 1. **Decorator Pattern (Cache Proxy)**
The `CacheUrlMappingRepository` wraps the `UrlMappingRepository` to add caching functionality without modifying the original repository:
- First checks in-memory cache
- Falls back to database if not cached
- Automatically caches results for subsequent requests

### 2. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: `ShortenUrl` - Modifies state (creates new mappings)
- **Queries**: `GetShortUrl` - Reads state (retrieves URLs)

### 3. **Vertical Slice Architecture**
Features are organized by use case rather than technical layers

## ⚡ Performance Features

### Snowflake ID Generation
- **Distributed**: Generates unique IDs across multiple instances
- **Time-based**: IDs are chronologically sortable
- **Compact**: Efficient 64-bit unsigned integers
- **Custom Epoch**: Started from 2025-10-18 for extended lifespan

### Caching Strategy
- In-memory cache for frequently accessed URLs
- Cache-aside pattern with automatic fallback
- Reduces database load for popular URLs

### Rate Limiting
- Token Bucket algorithm
- 100 requests per minute per IP address
- Prevents abuse and ensures fair usage

## 📊 Database Schema

**UrlMappings Table:**
- `Id` (bigint, PK) - Snowflake-generated unique ID
- `ShortUrl` (varchar) - Shortened URL code
- `OriginalUrl` (varchar) - Original long URL
- Indexes on `ShortUrl` and `OriginalUrl` for fast lookups

## 📈 Scalability Considerations

1. **Horizontal Scaling**: Snowflake IDs support multiple API instances
2. **Caching**: Reduces database load for popular URLs
3. **Database Indexing**: Fast lookups by short URL and original URL
4. **Stateless API**: Easy to load balance across multiple instances
