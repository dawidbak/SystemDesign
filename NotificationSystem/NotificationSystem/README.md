# Notification System

A simulated notification system that aims to mimic a real-world microservices-based architecture for handling user and system driven notifications. Instead of being a fully modular monolith by design, this solution consolidates concerns that in production would typically be split across separate microservices(FakeService, NotificationSystem service and Workers).

The solution consists of three projects that represent slices of such an environment:

1. **FakeService** – Simulates an external domain service in the ecosystem (publishes events like user registration) to test flows locally.
2. **NotificationSystem** – (`Notification.Api` + `Notification.Application`) Core service responsible for managing users, templates, devices, settings and issuing notification requests.
3. **NotificationEmailJob** – Background processor consuming events (e.g. email requests) and integrating with external email providers.
## Requirements

Building a scalable system that sends millions of notifications per day is challenging and requires clarity on functional and non-functional requirements.

### Functional Requirements
- Channels: Mobile Push, SMS, Email.
- Delivery Goal: Soft real-time (as fast as possible; slight delay acceptable under high load).
- Devices: iOS, Android, Laptop/Desktop (web / native clients).
- Triggers:
  - Client-initiated (immediate user or system actions).

### Scale & Volumes (Daily)
| Channel | Daily Notifications | Avg / Second (≈/86400) |
|---------|---------------------|------------------------|
| Push    | 10,000,000          | ~116 /s               |
| SMS     | 1,000,000           | ~12 /s                |
| Email   | 5,000,000           | ~58 /s                |
| Total   | 16,000,000          | ~186 /s               |

Peak traffic often exceeds averages (e.g. ×2–×5 burst). Design should assume >900 notifications/sec sustained in peak windows.

### Non-Functional Requirements
- Reliability: No lost notifications (at-least-once with idempotency at channel provider layer).
- Latency: Sub‑second enqueue, channel-specific delivery SLA (e.g. Push <2s typical; Email minutes acceptable).
- Scalability: Horizontal scaling of API, workers, and queues.
- Resilience: Retries with backoff; dead-letter queues for poisoned messages.

## 📐 System Design

### High-Level Architecture Diagram
<img width="1710" height="872" alt="image" src="https://github.com/user-attachments/assets/9cfeda5c-54de-43a6-b9a9-71e3279540ba" />

### User contact info gathering flow
It's beyond the scope of the task, just how it would look in practice.

<img width="1694" height="574" alt="image" src="https://github.com/user-attachments/assets/79bb98f6-e08b-406b-a036-98e6a6fe454a" />

### Notification service database schema
<img width="778" height="569" alt="image" src="https://github.com/user-attachments/assets/ba5c3291-8abb-4419-8fbc-4d9697a446a2" />

### Notification Email Worker flows
#### Inbox
<img width="1178" height="861" alt="image" src="https://github.com/user-attachments/assets/df366036-459f-4ae9-b643-7116879c72ef" />
#### Outbox
<img width="1424" height="776" alt="image" src="https://github.com/user-attachments/assets/54eeb143-551a-4791-8fe6-e6651f88aea3" />
#### Tables schema
<img width="725" height="152" alt="image" src="https://github.com/user-attachments/assets/5189e374-f7ea-4d81-9cdb-eef6d819eaa1" />


### Scalability Considerations
**Identifier Generation:** UUID v7 is used for generating IDs for users, devices, and settings. This ensures globally unique identifiers with temporal ordering, enabling efficient partitioning and indexing.
Using calculator: https://devina.io/collision-calculator it will take 5 centuries to get a collision :)

**API:** The Notification System accepts events in two ways: via HTTP requests from services or as a consumer of message queues. In both cases, it publishes events to workers for processing. Thanks for that, we can scale it horizontally or vertically depending on usage.

**Queue Layer:** RabbitMQ handles asynchronous processing via separate queues for iOS, Android, SMS, and Email notifications. Queues decouple producers from consumers, allowing workers to scale horizontally based on workload and throughput.

**Worker Layer:** Workers for each queue can be independently scaled, processing events published by the Notification System. This allows the system to handle spikes in notifications efficiently without overloading the database or external services.

**Caching Layer**: Redis is used to store user-related data and templates. This reduces database reads and improves response times. Vertical scaling should be enough here, because the data will be very stable and therefore there won't be much exchange. It will also be cheaper to maintain. If it got to be bigger, then I would switch to horizontal and if multiple Redis nodes are used, consistent hashing ensures balanced distribution of cache entries and smooth scaling when nodes are added or removed(virtual nodes) or we can add i.e. 4 nodes using Id of node to hash function.

**Database Layer**: PostgreSQL stores persistent user data, devices, settings, and templates. When the Notification System writes to the database, the primary node receives the write, and it is then replicated to replicas. Read replicas handle most worker and API queries, reducing load on the primary. This setup ensures scalability while maintaining data durability and eventual consistency.

**External Integration Layer**: Third-party services (APNS, Firebase, SMS gateways, Email providers) are accessed asynchronously via workers. This allows the system to scale independently of external service latency or throughput limits.

---
## 🚀 Features

- Email notifications (extensible to SMS / Push / Other channels)
- Template management (subject + content; extendable to placeholders/render engines)
- User channel settings (opt‑in / opt‑out per channel)
- Device tracking (foundation for push/mobile channels)
- Event‑Driven Architecture (RabbitMQ + MassTransit)
- Redis distributed caching (cache‑aside patterns, namespaced keys)
- Outbox / Inbox patterns for resilient event processing in the job
- Vertical Slice / Feature‑based organization in `Notification.Application`
- Minimal APIs with custom endpoint registration builder
- Docker Compose infrastructure (PostgreSQL, RabbitMQ, Redis)
- OpenAPI + Scalar API documentation

## 🎯 Design Patterns & Approaches

| Pattern / Approach        | Purpose / Benefit |
|---------------------------|-------------------|
| Vertical Slice Architecture | High cohesion, low coupling; feature self‑containment |
| Decorator (Cache Proxy)   | Adds caching without altering repository core logic |
| Outbox / Inbox            | Reliable event publishing & idempotent processing |
| Event‑Driven              | Loose coupling between producers and consumers |
| Cache‑Aside               | Controlled hydration + explicit invalidation |


---
## 🏗️ Architecture Overview

Vertical Slice Architecture: each feature encapsulates endpoint + handler. Shared abstractions kept minimal.

```
├── compose.yaml                        # Local infra: PostgreSQL, RabbitMQ, Redis
├── Notification.Api/                   # HTTP layer (Minimal API)
│   ├── Program.cs                      # DI, MassTransit, Redis cache registration
│   ├── appsettings*.json               # Connection strings & configuration
│
├── Notification.Application/           # Core business, application and data logic
│   ├── ConfigureServices.cs            # DI registrations
│   ├── Common/                         # Endpoint abstractions, handler base utilities
│   ├── Domain/                         # Models: User, Template, Setting, Device, ChannelType
│   ├── Features/                       # Vertical slices (Notification, User, Template, Setting)
│   ├── Infrastructure/                 # Persistence, Repositories (+ cache decorators), Migrations
│   └── ...                             # Extensions / additional services
│
├── NotificationEmailJob/               # Background worker / event consumer
│   ├── Program.cs                      # Host builder & subscriptions
│   ├── In/Outbox/                      # Outbox & inbox patterns
│   ├── Handlers/                       # Consumers & processing logic
│   └── Services/                       # External provider adapters
|
└── Shared/                             # Contains shared contracts and events for MassTransit
```

### Communication Layer
- **RabbitMQ**: Responsible for Event-Driven architecture in our ecosystem.
- **MassTransit**: Consumer registration, endpoint configuration, retry policies, serialization.

### Cache Layer
- **Redis**: `IDistributedCache` implementation (prefix `notificationsystem:`). Decorator pattern applied to repositories for cache‑aside.

### Data Layer
- **PostgreSQL**: Primary persistence for notification service and worker.
- **EF Core Migrations**: Schema evolution (located under `Infrastructure/Migrations`).

### Event Flow (Email Example)
1. `FakeService` publishes `UserRegisteredEvent`.
2. `Notification.Api` consumes the event → creates or updates User + default channel settings.
3. User(employee) invokes Email Notification endpoint → `SendEmailNotificationHandler` loads User + Template (cache first, DB fallback).
4. Publishes `EmailNotificationRequestedEvent`.
5. `NotificationEmailJob` consumes the request → composes payload → sends email (or logs in dev).
6. Delivery status can be persisted via Outbox pattern for guaranteed processing.

---
## 📦 Technology Stack

| Area            | Technology / Library                                   |
|-----------------|---------------------------------------------------------|
| Platform        | .NET 9                                                  |
| API             | ASP.NET Core Minimal APIs                               |
| EDA             | MassTransit + RabbitMQ                                  |
| Cache           | Redis (StackExchange.Redis + DistributedCache)          |
| Database        | PostgreSQL 18                                           |
| Documentation   | OpenAPI + Scalar                                        |
| Containerization | Docker / Docker Compose                                |

---
## 📋 Prerequisites

- .NET 9 SDK
- Docker Desktop (for PostgreSQL, RabbitMQ, Redis)
- IDE: Rider / VS Code / Visual Studio 2022

---
## 🚦 Getting Started

### 1. Start Infrastructure
```bash
docker compose up -d
```
Starts containers: PostgreSQL (`notificationsystem`), RabbitMQ (5672 / 15672), Redis (6379).

### 2. Run API
```bash
cd Notification.Api
dotnet run
```

### 3. Run Fake Service
```bash
cd ../FakeService
dotnet run
```

### 4. Run Email Job
```bash
cd ../NotificationEmailJob
dotnet run
```

### 5. API Documentation
In Development: `http://localhost:<api-port>/scalar`.

---
## 🔧 Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=notificationsystem;Username=postgres;Password=postgres",
    "RabbitMQ": "amqp://rabbitmq:rabbitmq@localhost:5672",
    "Redis": "localhost:6379"
  }
}
```
---
