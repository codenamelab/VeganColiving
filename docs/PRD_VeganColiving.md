# Product Requirements Document (PRD)
## Product: VeganColiving (aka “VeganEverything – Coliving Match”)
## Version: 0.2 (Draft – updated for Blazor + EF Core)
## Last Updated: 2025-08-23
## Author: (TODO: add)

---

## 1. Executive Summary
VeganColiving is a platform that helps vegans with aligned living preferences either:
1. Form new coliving groups; or
2. Fill vacant rooms/spaces in existing vegan households.

Differentiators: deep lifestyle & values alignment (vegan baseline), structured + semantic matching, trust & freshness (active-interest renewal), privacy-preserving disclosure.

MVP builds on a .NET 8 stack with Blazor (Server or Hybrid strategy) and Entity Framework Core for data access.

---

## 2. Problem Statement
(unchanged – see v0.1)  
Fragmented discovery, lack of structured lifestyle filters, friction forming aligned groups, stale data.

---

## 3. Goals & Non‑Goals
(unchanged in substance; technology implications added)

Tech Alignment Notes:
- Optimize for rapid delivery with shared frontend/backend model types (C# records).
- Use EF Core migrations for schema evolution, favoring incremental extensibility (JSON columns or value-owned types for flexible preference dimensions).

---

## 4. Target Users & Personas
(unchanged)

---

## 5. User Stories (Prioritized)
(unchanged list; implementation detail note)
Implementation Note: Requirement vs Preference toggle stored as enum per attribute to enable LINQ filtering + scoring pipeline.

---

## 6. Feature Breakdown (MVP)
(unchanged functionally)  
Implementation Additions:
- Blazor component library for reusable field groups (LocationFilter, LifestyleSection, FacilitiesSelector).
- Server-side validation via FluentValidation (or DataAnnotations baseline) + shared validation attributes for both client/server if using Blazor WebAssembly prerender.

---

## 7. Data Model (Conceptual → EF Core Mapping Notes)
Entity mapping guidelines:
- Use GUID (Ulid or COMB optional) primary keys for scalability.
- Concurrency tokens (rowversion) on mutable aggregate roots (Listing, Group).
- Owned Types for grouped value objects (PriceRange, AvailabilityWindow, IncludedCosts).
- Enum fields stored as strings (HasConversion) for readability unless perf dictates ints.

Key Entities (same semantics):
User
PreferenceSet (Option 1: flattened columns + suffix _Level; Option 2: PreferenceEntry table (attribute key, value, level) – choose Option 1 for performance of primary queries)
Listing
Group / GroupMember
Message / Thread
MatchRecord
Report (Audit)

Flexible Attributes:
- For evolving dimensions (e.g., SustainabilityPractices) use a join table or a JSON column if using PostgreSQL provider; if SQL Server, consider separate table with indexed FK.

Index Strategy (examples):
- IX_Listing_Status_AvailabilityFrom
- IX_Listing_GeoApprox (spatial / geography index if provider supports)
- IX_PreferenceSet_UserId (unique)
- IX_Group_Status
- Full-text index on Listing.Description & User.LifestyleNotes (SQL Server Full-Text or PostgreSQL tsvector; fallback: Lucene/Elastic optional later).

---

## 8. Matching & Scoring (Initial Algorithm Sketch)
(unchanged logic)  
Implementation:
- Precompute normalized numeric fields (e.g., priceNormalized) in Listing.
- LINQ projection to compute hard requirement filtering server-side.
- Soft score pipeline:
  Step 1: Retrieve candidate set with hard filters (IQueryable filters).
  Step 2: For each candidate, compute sub-scores in C# (in-memory) OR translate simple numeric / categorical overlaps to SQL expressions if performance requires early ordering.
- Future semantic similarity: integrate with external vector service (e.g., OpenAI embeddings) storing vector in separate table; not in MVP.

Data Structure for ScoreBreakdown: store JSON (string) column on MatchRecord; use System.Text.Json.

---

## 9. Preference & Filter Taxonomy Mapping
(unchanged)  
EF Implementation: An enum AttributeLevel { Requirement = 2, Preference = 1 }. For each attribute, store twin: e.g., SmokingPolicy, SmokingPolicyLevel. Template codegen can scaffold repetitive pairs.

---

## 10. Key User Flows (MVP)
(unchanged)  
Blazor Components Mapping (examples):
- OnboardingWizard.razor
- PreferenceEditor/*.razor
- ListingWizard/*.razor
- SearchPage.razor (FilterSidebar + ResultsGrid)
- GroupDashboard.razor
- RenewalPromptModal.razor
- MatchExplanationDrawer.razor

---

## 11. Privacy & Security
Additions for .NET Implementation:
- Data Protection API for key management (per environment key ring).
- Optional field-level encryption for email (AES-GCM) before persistence; wrap with ValueConverter.
- Rate limiting via ASP.NET Core rate limiting middleware (per-IP + per-user).
- Address Generalization: store precise address in Listing; expose only approximate geohash (precision reduction) until threshold. Use geohash library in C#.

---

## 12. Internationalization / Localization
Implementation:
- Use .resx resource files + shared resource class injected into components.
- Culture selection via query param or user profile; persisted in cookie.
- For dynamic enumerations, store stable keys (e.g., NoiseLevel.Quiet) and map to localized strings.

---

## 13. Accessibility
Blazor specifics:
- Ensure components set @attributes for aria-*.
- Custom select / multiselect components must manage focus & keyboard navigation (Arrow keys, Enter, Space, Escape).
- Provide semantic headings for screen reader region navigation.

---

## 14. Performance & Scalability Targets
Add .NET specifics:
- P95 end-to-end (Server Render + DB) < 800ms for search.
- Connection pooling config (e.g., Min=5 / Max=50) tuned for expected concurrency.
- Caching:
  - MemoryCache for static enumerations.
  - Optional distributed cache (Redis) for precomputed top N matches per user (invalidate on preference change).
- Async streaming (IAsyncEnumerable) from EF for large result sets (with server paging).
- Consider compiled queries for hottest match queries.

---

## 15. Success Metrics (KPIs)
(unchanged definitions; instrumentation covered below)

---

## 16. Analytics & Instrumentation
Implementation:
- Use OpenTelemetry (otel) for traces & metrics; export to Prometheus / OTLP collector.
- Event Publishing: Interface IEventPublisher; EF SaveChanges interceptor to emit domain events (e.g., ListingCreated).
- Structured Logging with Serilog (JSON) + correlation IDs.
- Frontend event dispatch via JS interop or direct HTTP calls to /api/events (batched).

---

## 17. Constraints & Assumptions
Add:
- Initial choice between Blazor Server vs WebAssembly:
  - Server pros: faster iteration, single deployment, secure secret handling.
  - Server cons: persistent SignalR connections scale cost; must handle disconnected states.
  - WASM Hybrid option later for offline / caching.
(OPEN: decide deployment model before Phase 1 end.)

---

## 18. Technical Architecture (High-Level – Updated for Blazor + EF Core)

Layers:
1. Presentation (Blazor)
   - Pages + Components
   - State Containers (e.g., Scoped MatchStateService)
2. Application Layer
   - CQRS-style commands/queries (MediatR optional)
   - Validation pipeline
3. Domain
   - Entities, Value Objects, Domain Events
4. Infrastructure
   - EF Core DbContext
   - Repositories (if abstraction needed)
   - External services (Email, Geocoding, Vector service future)
5. Matching Engine
   - Service with strategy interface IMatchScorer
   - Hard filter builder => Expression<Func<Listing,bool>>
   - Soft scoring aggregator

Data Persistence:
- Relational DB (SQL Server or PostgreSQL – CHOOSE: If using EF Core + need native JSON + FTS easily, prefer PostgreSQL).
- Migrations: dotnet ef migrations with automated CI checks.

Messaging / Real-time:
- SignalR for:
  - Minimal group messaging
  - Live renewal banners
  - Optional “someone viewed your listing” (defer).

Background Processing:
- Hosted Services (IHostedService) or Hangfire/Quartz for:
  - Renewal scan job
  - Match precomputation
  - Email digests

Security:
- ASP.NET Core Identity or custom minimal auth with Magic Link tokens (signed JWT claims).
- Authorization policies (e.g., MustBeVegan, ListingOwnerPolicy).

Deployment:
- Containerized (Docker) -> Azure App Service / Azure Container Apps / Fly.io.
- DB: Managed PostgreSQL (Azure Flexible Server) or SQL Server (Azure SQL).
- Secrets: Azure Key Vault / environment-managed.

Scalability:
- Horizontal scale via stateless app replicas (if Blazor Server, require sticky sessions or backplane for SignalR – use Redis backplane).
- Caching of enumeration & static match weight configuration.

---

## 19. Risks & Mitigations
(unchanged list; add infra notes)
- Blazor Server scaling risk: Evaluate concurrency (SignalR connections). Mitigate using keep-alive tuning & connection reclamation on idle.

---

## 20. Roadmap (Indicative – Adjusted for .NET)
Phase 0 (Weeks 0–2): Decide Blazor model (Server vs WASM), set solution structure, auth skeleton, initial DbContext + migrations.  
Phase 1 (Weeks 3–5): Preference & Listing entities + CRUD + filters (hard).  
Phase 2 (Weeks 6–8): Matching scorer service + explanatory UI.  
Phase 3 (Weeks 9–11): Group formation, invitations (SignalR minimal).  
Phase 4 (Weeks 12–14): Renewal job, waitlist, moderation tools, i18n.  
Phase 5 (Post-MVP): Semantic search (vector store), E2EE chat (MLS / Double Ratchet), adaptive scoring, sustainability scoring.

---

## 21. Open Questions (Updated)
1. Blazor hosting model final choice? (Server first, WASM later?)
2. Database provider: PostgreSQL (JSONB + FTS) vs SQL Server (FTS, but less flexible JSON indexing).
3. Will we integrate external geocoding (Nominatim, Azure Maps) at MVP or stub?
4. Vector search strategy (.NET library + pgvector vs external service)?
5. Rate limits policy specifics (daily invites cap?).
6. Do we need multi-tenancy support early (org-level separation)?
7. Email delivery provider (SendGrid, Resend, SES)?

---

## 22. Glossary
(unchanged)

---

## 23. Appendices
A. Original Preference Categories Mapping (unchanged)
B. EF Core Implementation Snippets (Illustrative)

```csharp
public enum Level { Preference = 1, Requirement = 2 }

public record Range<T>(T Min, T Max);

public class PreferenceSet {
    public Guid UserId { get; set; }
    public Range<decimal>? PriceRange { get; set; }
    public Level PriceRangeLevel { get; set; }
    public string? Country { get; set; }
    public Level CountryLevel { get; set; }
    // Repeat pattern; consider source generation to reduce boilerplate
}

public class Listing {
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public decimal PriceMonthly { get; set; }
    public DateOnly AvailabilityFrom { get; set; }
    public DateOnly? AvailabilityDeadline { get; set; }
    public string? GeoApproxHash { get; set; }
    public bool Furnished { get; set; }
    public ICollection<ListingFacility> Facilities { get; set; } = new List<ListingFacility>();
}
```

C. Sample Hard Filter Expression

```csharp
Expression<Func<Listing,bool>> BuildHardFilter(User user, PreferenceSet prefs) {
    return l =>
        (prefs.PriceRange == null ||
         (l.PriceMonthly >= prefs.PriceRange.Min && l.PriceMonthly <= prefs.PriceRange.Max) ||
         prefs.PriceRangeLevel == Level.Preference)
        &&
        (prefs.Country == null ||
         l.AddressCountry == prefs.Country ||
         prefs.CountryLevel == Level.Preference);
}
```

---

Change Log:
- v0.2: Replaced prior JS-centric architecture with .NET 8 Blazor + EF Core stack; added EF mapping, performance, hosting model considerations, and implementation snippets.

End of Document.
