# Technical Specification
## Project: VeganColiving (aka “VeganEverything – Coliving Match”)
## Version: 0.1 (Draft)
## Source PRD Version Referenced: PRD v0.2 (2025-08-23)
## Last Updated: 2025-08-23
## Authors: (TODO: add)

---

## 1. Purpose & Scope
This Technical Specification translates the PRD into an actionable engineering blueprint covering architecture, data design, component/module responsibilities, APIs, security, operations, and implementation phases for the MVP (Phases 0–4) plus foundations for Post-MVP enhancements.

---

## 2. High-Level Architecture
Pattern: Layered + vertical feature slices (optionally CQRS-lite).  
Layers:
1. Presentation: Blazor (initially Server hosting model recommended; WASM later).
2. Application: Orchestrates use cases (commands & queries), validation, authorization.
3. Domain: Entities, value objects, domain events (pure C#).
4. Infrastructure: EF Core (PostgreSQL), repositories (optional), services (Email, Geocoding, Crypto).
5. Matching Engine: Hard filter expression builder + soft scoring strategies.
6. Background Processing: HostedServices / Quartz / Hangfire (pluggable).
7. Observability: OpenTelemetry + Serilog -> (OTLP + JSON logs).
8. Integration Gateways: Email provider, (future) Vector similarity, Geocoding.

Primary Data Store: PostgreSQL 16.x (with extensions: pg_trgm, pgcrypto (optional), PostGIS (optional), pgvector (post-MVP)).

Communication:
- Browser ↔ Server: Blazor Server (SignalR circuits) + REST/JSON endpoints for some APIs (easier external integration).
- Real-time: SignalR Hub(s) for messaging and renewal prompts.
- Background job triggers: internal scheduler.

---

## 3. Technology Stack
- Language: C# 12 (.NET 8 LTS)
- UI: Blazor Server (Phase 0–2), evaluate Hybrid/WASM at Phase 3.
- ORM: Entity Framework Core 8 with code-first migrations.
- DB: PostgreSQL
- Caching: In-memory (IMemoryCache) + optional Redis (Phase 3+).
- Messaging / Real-time: SignalR
- Auth: Magic Link (JWT) OR ASP.NET Core Identity (pluggable strategy) – choose final in Phase 0 decision.
- Validation: FluentValidation (application layer).
- Serialization: System.Text.Json (with source generation for performance-critical DTOs).
- Logging: Serilog (JSON console + optional sink to Seq / Loki).
- Tracing/Metrics: OpenTelemetry exporters (OTLP).
- Testing: xUnit + FluentAssertions + Playwright (E2E) + BenchmarkDotNet (micro-bench scoring).
- CI: GitHub Actions
- Containerization: Docker (multi-stage build).
- Infrastructure (example target): Azure (App Service or Container Apps) or Fly.io.

---

## 4. Module / Project Structure (Solution Layout)
Solution: VeganColiving.sln  
Projects:
- WebApp (Blazor + minimal APIs)  
- Application (CQRS-like handlers, DTOs, validators)  
- Domain (entities, value objects, domain events)  
- Infrastructure (EF Core, repositories, migrations, external service adapters)  
- Matching (scoring strategies, expression builders)  
- Background (job processors, scheduling)  
- Contracts (shared DTOs for public API)  
- Tests.Unit  
- Tests.Integration  
- Tests.E2E  
- Benchmarks (optional)

Namespace Convention: VeganColiving.[Layer].[Feature]

Feature Folders (inside Application & WebApp):
- Users
- Preferences
- Listings
- Matching
- Groups
- Messaging
- Moderation
- Renewal
- Analytics

---

## 5. Domain Model Summary (Entities & Aggregates)
Aggregates & Ownership:
- User (owns PreferenceSet)
- Listing (owned by User; can have Facilities, Availability windows)
- Group (Members, Invitations)
- Thread (Messages)
- MatchRecord (User ↔ Listing or User ↔ Group)
- Report (moderation)
- AuditEvent (append-only)
Value Objects:
- PriceRange, AvailabilityWindow, ScoreBreakdown, Geolocation (if using PostGIS later), Encrypted<T> wrapper.

Event Examples:
- UserRegistered
- ListingCreated
- PreferenceUpdated
- MatchComputed
- ListingRenewalDue
- ReportFiled

---

## 6. Data Schema (Initial Tables)
Conventions:
- snake_case table names optional; default EF PascalCase -> map to lower snake_case via naming convention if desired.
- Primary keys: UUID (generate server-side using `gen_random_uuid()` or C# `Guid.NewGuid()`).
- Timestamps: created_utc, updated_utc (trigger or SaveChanges interceptor).
- Soft delete only where necessary (e.g., Messages? Usually hard delete allowed for GDPR unless legal retention needed).
- Row version: xmin or explicit bytea/timestamp column concurrency token.

Tables (MVP Core):
1. users (id, email, email_normalized, display_name, lifestyle_notes, is_active, created_utc, updated_utc)
2. preference_sets (user_id PK/FK, price_min, price_max, price_level, country, country_level, ... repeated pattern)
3. listings (id, owner_user_id FK, title, description, price_monthly, availability_from, availability_deadline, geo_approx_hash, address_country, is_furnished, status, created_utc, updated_utc)
4. listing_facilities (listing_id FK, facility_type enum)
5. groups (id, name, description, status, created_by_user_id, created_utc, updated_utc)
6. group_members (group_id FK, user_id FK, role enum, joined_utc)
7. threads (id, subject, context_type enum (Listing|Group|Direct), context_id, created_utc)
8. messages (id, thread_id FK, sender_user_id FK, body, body_rich (optional), created_utc)
9. match_records (id, user_id, target_type enum (Listing|Group), target_id, hard_passed bool, score numeric, score_breakdown_json, computed_utc, expires_utc)
10. reports (id, reporter_user_id, target_type, target_id, reason_code, notes, status enum, created_utc, updated_utc)
11. audit_events (id, event_type, actor_user_id nullable, entity_type, entity_id, payload_json, created_utc)
12. renewal_tokens (id, listing_id, token, expires_utc, consumed_utc nullable)
13. outbox_events (id, event_type, payload_json, created_utc, processed_utc nullable, retry_count)
14. user_login_tokens (id, user_id, token_hash, issued_utc, expires_utc, consumed_utc)

Indexes (selected):
- listings(status, availability_from)
- listings(geo_approx_hash)
- listings USING GIN (to_tsvector('simple', description)) (FTS)
- preference_sets(user_id) UNIQUE
- match_records(user_id, target_type, target_id) UNIQUE
- groups(status)
- messages(thread_id, created_utc)
- outbox_events(processed_utc NULLS FIRST, created_utc)
- user_login_tokens(token_hash)

Enums (PostgreSQL enum types or mapped text check constraints):
- listing_status (Draft, Active, Paused, Filled, Archived)
- attribute_level (Preference, Requirement)
- facility_type (...)
- group_status (Forming, Active, Dormant, Closed)
- member_role (Owner, Member, Pending)
- report_status (Open, UnderReview, Resolved, Rejected)
- target_type (Listing, Group)
- context_type (Listing, Group, Direct)

Future Tables (Post-MVP):
- vectors (entity_type, entity_id, embedding vector)
- sustainability_practices (listing_id, key)
- notifications (id, user_id, type, payload_json, read_utc)

---

## 7. Matching Engine Specification
Responsibilities:
- Build hard filter expression from PreferenceSet.
- Generate candidate query (IQueryable<Listing>).
- Apply pre-sorting (e.g., price distance, availability proximity).
- Compute soft scoring in memory OR via SQL projection.

Hard Filter Rules:
- Any attribute marked Requirement must match strictly.
- If attribute level is Preference, non-match allowed without exclusion (score impact only).

Soft Score Components (initial weights example):
- Price alignment (20%)
- Availability overlap (15%)
- Location proximity (20%) – approximate via geohash prefix match or future geodistance
- Lifestyle tags alignment (30%) (initially simple categorical intersections)
- Facilities satisfaction (15%)

Score Formula (example):
score = Σ(weight_i * component_i_normalized)
Normalization:
- Price: 1 - min( |candidatePrice - midPreferred| / preferredRangeWidth, 1 )
- Availability: intersection_days / preferred_window_days
- Location: geohash prefix length / max_prefix (tunable) OR (1 - distanceKm / thresholdKm)
- Facilities: matched_required / total_required (if all required matched; else 0) plus preference contribution.

ScoreBreakdown JSON schema:
{ "price":0.87, "availability":0.66, "location":0.92, "facilities":1.0, "lifestyle":0.75, "total":0.84, "weights": { ... } }

Extensibility:
- IMatchScorer with method:
  Task<MatchResult> ScoreListingsAsync(User user, PreferenceSet prefs, IEnumerable<Listing> candidates, CancellationToken)
- Strategy pattern for future vector-based similarity.

Precomputation:
- Option (Phase 3): Nightly job caches top N matches per active user in Redis keyed by user:{id}:matches.

---

## 8. Public API (REST + SignalR)
Base URL: /api

Endpoints (MVP):
- POST /auth/magic-link/request  (email) → 202
- POST /auth/magic-link/consume  (token) → JWT cookie/session
- GET  /users/me
- PATCH /users/me (profile updates)
- GET  /preferences/me
- PUT  /preferences/me
- POST /listings
- GET  /listings/{id}
- PATCH /listings/{id}
- GET  /listings?filters...
- POST /listings/{id}/renew (generates token or immediate if policy)
- GET  /matches (server computes on demand OR returns cached list)
- GET  /groups/{id}
- POST /groups
- POST /groups/{id}/invite
- POST /messages/threads (create)
- GET  /threads/{id}/messages
- POST /threads/{id}/messages
- POST /reports
- GET  /health/live
- GET  /health/ready

Response Format: JSON { data: ..., meta: ... } or problem+json for errors.

SignalR Hubs:
- /hubs/notifications: events (MatchUpdated, ListingRenewalDue)
- /hubs/messages: new message push

Message Contracts (sample):
{ "type":"MessageCreated", "threadId":"...", "messageId":"...", "sentUtc":"..." }

---

## 9. Blazor Component Hierarchy (Selected)
Pages:
- /onboarding (OnboardingWizard)
- /preferences (PreferenceEditor)
- /listings/new (ListingWizard)
- /listings/{id} (ListingDetail + MatchExplanationDrawer)
- /search (SearchPage)
- /groups/{id} (GroupDashboard)
- /inbox (ThreadList + MessagePane)
- /matches (MatchResultsPage)

Reusable Components:
- PreferenceField<T>
- RequirementToggle
- FacilitySelector
- PriceRangeSlider
- AvailabilityPicker
- MatchScoreBadge
- ScoreBreakdownPanel
- RenewalPromptModal

State Management:
- Scoped UserStateService
- MatchStateService (cache last query results)
- Circuit-safe ephemeral state (avoid heavy memory usage per connection)

---

## 10. Authentication & Authorization
Option Chosen (tentative): Magic Link (email token) → Issue JWT (short-lived) + refresh cookie OR server-side session.

Flow:
1. Request magic link → store hashed single-use token (user_login_tokens).
2. User clicks email link → token validated → session established.
3. Renewal: If WASM in future, use refresh endpoint.

Authorization Policies:
- AuthenticatedUser
- ListingOwner (resource-based)
- GroupMember / GroupOwner
- VeganBaseline (trivial now; could extend with verification steps)

Token Claims:
- sub (user id), ver (schema), roles (if group-role expansions added), iss, exp.

Anti-CSRF:
- If using JWT in cookie, enforce CSRF token for state-changing REST endpoints (double-submit cookie). Blazor Server circuits typically internal, but REST endpoints still secured.

---

## 11. Security & Privacy Implementation
Data Protection:
- ASP.NET Data Protection keys stored in persisted volume or Key Vault.

PII Handling:
- Email possibly encrypted at rest (Encrypted<string>) using envelope key (master key in Key Vault).
- Logging: Never log raw tokens or emails (hash or partial mask).
- Rate Limiting: /auth/magic-link/request (per IP + per email) e.g., 5 per hour.

Geolocation:
- Store precise only if needed later; MVP uses coarse geohash (precision 6–7) and optionally full address in protected table if added later.

Secrets:
- Inject via environment variables + Key Vault (production).
- DO NOT bake into images.

---

## 12. Internationalization
- Resource files: Resources/Shared.resx + culture-specific.
- Culture resolver: Query param ?lang=xx overrides user profile → cookie.
- Enum Localization: mapping dictionary from enum value to resource key.
- Date/Number formatting: <LocalizedNumber Value="decimal" /> component wrapper.

---

## 13. Performance & Scalability Strategy
Key Targets (from PRD):
- P95 search < 800ms server time.

Approaches:
- Use AsNoTracking for read queries.
- Precompile query delegates for top listing searches.
- Limit result sets (pagination default 20).
- Lazy load NONE – explicit includes.
- Circuit Memory Budget: drop heavy state after inactivity (timer).
- Connection scaling: Each Blazor Server circuit ~ memory footprint; load test expected concurrency to set max instance count.

---

## 14. Caching Strategy
Short-Term:
- IMemoryCache: enumeration lists, configuration weights.
- Output caching for public listing details (60s) if allowed.

Phase 3+:
- Redis: user match lists, rate limit counters (if cluster), SignalR backplane.

Invalidation:
- On preference update: evict user:{id}:matches.

---

## 15. Background Jobs
Jobs (HostedService or Quartz):
- ListingRenewalScan (daily)
- MatchPrecompute (optional nightly)
- OutboxDispatcher (every few seconds)
- CleanupExpiredLoginTokens (hourly)
- ReportStaleListings (weekly summary)

Retry Policy (Outbox):
- Exponential backoff (2^attempt * base 5s, max 10m).
- Move to dead-letter log after N=10 failures.

---

## 16. Outbox & Eventing
Outbox Pattern:
- Insert rows in outbox_events within same transaction as domain changes.
- Dispatcher reads unprocessed rows, publishes (e.g., to internal in-memory bus or external queue future).
- Mark processed with timestamp.

Event Consumption:
- For MVP mainly for email notifications & analytics ingestion.

---

## 17. Migrations & Schema Versioning
Commands:
- dotnet ef migrations add <Name> -p Infrastructure -s WebApp
- dotnet ef database update

Automations:
- CI: Validate migrations compile + apply to ephemeral PostgreSQL.
- Production: Manual approval (or automatic with baseline backup).
Backward Compatibility:
- Avoid destructive drops until feature toggles disabled / data migrated.

---

## 18. Configuration Matrix
| Setting | Local | Dev | Staging | Prod |
|---------|-------|-----|---------|------|
| DB_CONN | docker pg | managed pg | managed pg | managed pg (HA) |
| REDIS   | in-memory fallback | optional | yes | yes |
| LOG LEVEL | Debug | Info | Info | Warning |
| EMAIL PROVIDER | Console stub | Sandbox (e.g., Resend test) | Real (low rate) | Real |
| FEATURE FLAGS | all on | selective | selective | stable only |

Feature Flags (simple): IFeatureFlagService + JSON file or LaunchDarkly (optional later).

---

## 19. Observability
Logging:
- CorrelationIdMiddleware attaches X-Correlation-ID.
Tracing:
- Use OpenTelemetry instrumentation for ASP.NET Core, EF Core, HttpClient.
Metrics:
- Custom counters: matches_computed_total, renewals_sent_total, listings_active_gauge.
Dashboards:
- Grafana / Azure Monitor.

Alerting Thresholds:
- Error rate > 2% over 5m
- P95 search latency > 1200ms over 10m
- Active matches job failure > 3 consecutive.

---

## 20. CI/CD Pipeline (GitHub Actions)
Workflows:
1. build-and-test.yml
   - Restore, build, run unit & integration tests (docker-compose for pg)
   - Run lint/style analyzers
   - Upload coverage
2. security-scan.yml
   - Dotnet list package --vulnerable
   - Trivy image scan
3. deploy.yml (on tag / main merge)
   - Build Docker image
   - Run migrations (Idempotent script)
   - Deploy to environment (with slot for staging)

Quality Gates:
- Coverage threshold (e.g., 70% unit; 50% integration early).
- No critical vulnerabilities open.

---

## 21. Testing Strategy
Test Pyramid:
- Unit: Domain logic (matching calculations, validation).
- Integration: Repositories, migrations, API endpoints with test server + test DB.
- E2E: Playwright scripts (sign-in, create listing, view matches).
- Performance: BenchmarkDotNet for scoring function baseline (target < 1ms per listing scoring).
- Load Testing: k6 / Locust for listing search & match endpoints.

Test Data Management:
- Builders for entities.
- Database state reset per test (Respawner library) or transactional test pattern.

---

## 22. Code Quality & Conventions
- Nullable reference types enabled.
- StyleCop / EditorConfig: 120-char line, file-scoped namespaces.
- Guard clauses for public method inputs.
- Avoid static mutable state.
- Use records for immutable DTOs.
- Avoid leaking EF entities outside Application layer (map to DTO).

---

## 23. Deployment Topology (Initial)
Single Region (Prod):
- App Service Plan (2–3 instances, autoscale CPU > 65%).
- PostgreSQL Flexible Server (General Purpose, HA optional after traction).
- Redis Cache (Basic/Standard later).
- Key Vault (secrets, DPAPI keys).
- Log Storage (Blob or Log Analytics).

Network:
- HTTPS enforced
- Optional WAF / reverse proxy
- Future: CDN for static assets (if WASM adopted).

---

## 24. Resilience & Failure Modes
Failure Scenarios:
- DB outage: App returns 503 on critical operations; background jobs paused.
- Redis down: Fallback to direct compute (matches), metrics degrade gracefully.
- Email provider failure: Outbox retry; escalate after N attempts.
- Circuit Overload: Apply max concurrent SignalR connections per instance; reject with user-friendly message.

Timeouts:
- DB command timeout default 30s (search queries targeted < 2s).
- External HTTP (email, geocode) 5s.

Circuit Breakers:
- Polly policies around external HTTP calls (retry + circuit break after consecutive failures).

---

## 25. Risk Mapping (From PRD)
| Risk | Mitigation |
|------|------------|
| Blazor Server scaling | Early load test, connection metrics dashboard, plan WASM fallback |
| Stale data | Renewal job + UI prompts |
| Algorithm opacity | ScoreBreakdownPanel UI with weights |
| Privacy of location | Geohash + withheld precise address until trust |
| Feature creep | Phase gating + scope lock per phase start |
| Performance drift | Benchmark scoring each release + latency SLO alerts |

---

## 26. Decision Log (Initial)
| Decision | Status | Date | Rationale |
|----------|--------|------|-----------|
| Use PostgreSQL | Proposed | Phase 0 | JSONB + FTS + future vector |
| Hosting Model: Blazor Server first | Proposed | Phase 0 | Faster iteration; revisit after scale test |
| Magic Link Auth vs Identity | OPEN | Phase 0 | Magic link simplifies initial UX; evaluate audit/security needs |
| Redis introduction phase | Pending | Phase 2/3 | Only after caching ROI established |
| Vector Search timing | Post-MVP | | Avoid premature complexity |

---

## 27. Open Questions (Carry + Expanded)
1. Confirm database provider (PostgreSQL recommended) — finalize before first migration.
2. Auth approach finalization (Magic Link vs full Identity).
3. Geocoding provider (Nominatim self-host vs commercial API) timing.
4. Minimum viable lifestyle attribute set for MVP filtering (lock list).
5. Are group chats required initially or can threads suffice?
6. Listing moderation workflow: manual review required or reactive (report-based)?
7. Email sending volumes & provider pricing constraints.
8. Data retention policy & GDPR deletion process (scrub or pseudonymize?).

---

## 28. Initial Task Breakdown (Phases 0–2 Detail)

Phase 0 (Environment + Skeleton):
- Task: Solution & project scaffolding
- Task: Configure EF Core + initial migration (users, listings minimal)
- Task: Magic link prototype endpoint
- Task: Logging & telemetry baseline
- Task: CI build-and-test pipeline

Phase 1 (Preferences & Listings CRUD):
- Task: PreferenceSet entity + migration
- Task: Preference editor UI & validation
- Task: Listing entity extended fields + facilities
- Task: Search endpoint (hard filters only)
- Task: Basic Blazor pages (create listing, edit listing)
- Task: Indexes & FTS integration

Phase 2 (Matching + Explanation):
- Task: Matching expression builder
- Task: Scoring service + weights configuration (JSON)
- Task: Match API endpoint + caching interface
- Task: Score breakdown UI component
- Task: Outbox pattern for match analytics events
- Task: Benchmark scoring (BenchmarkDotNet project)

Phase 3+ (Preview):
- Group formation, messaging hub, renewal job.

---

## 29. Example Pseudocode & Snippets

Hard Filter Builder (extended):
```csharp
public Expression<Func<Listing,bool>> Build(User user, PreferenceSet p) {
    // Base parameter
    var l = Expression.Parameter(typeof(Listing), "l");
    var clauses = new List<Expression>();

    // Price
    if (p.PriceRange is not null && p.PriceRangeLevel == Level.Requirement) {
        clauses.Add(Expression.AndAlso(
            Expression.GreaterThanOrEqual(
                Expression.Property(l, nameof(Listing.PriceMonthly)),
                Expression.Constant(p.PriceRange.Min)),
            Expression.LessThanOrEqual(
                Expression.Property(l, nameof(Listing.PriceMonthly)),
                Expression.Constant(p.PriceRange.Max))));
    }
    // Country requirement
    if (!string.IsNullOrEmpty(p.Country) && p.CountryLevel == Level.Requirement) {
        clauses.Add(Expression.Equal(
            Expression.Property(l, "AddressCountry"),
            Expression.Constant(p.Country)));
    }
    var body = clauses.Any()
        ? clauses.Aggregate(Expression.AndAlso)
        : Expression.Constant(true);
    return Expression.Lambda<Func<Listing,bool>>(body, l);
}
```

Scoring (simplified):
```csharp
decimal PriceComponent(decimal listingPrice, Range<decimal>? prefRange) {
    if (prefRange is null) return 1m;
    var mid = (prefRange.Min + prefRange.Max) / 2m;
    var width = (prefRange.Max - prefRange.Min);
    if (width <= 0) return listingPrice == mid ? 1m : 0m;
    var dist = Math.Min(Math.Abs(listingPrice - mid), width);
    return 1m - (dist / width);
}
```

---

## 30. Sample Migration (Excerpt)
```sql
CREATE TABLE users (
  id uuid PRIMARY KEY,
  email text NOT NULL UNIQUE,
  email_normalized text NOT NULL UNIQUE,
  display_name text,
  lifestyle_notes text,
  is_active boolean NOT NULL DEFAULT true,
  created_utc timestamptz NOT NULL,
  updated_utc timestamptz NOT NULL
);

CREATE TABLE preference_sets (
  user_id uuid PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
  price_min numeric,
  price_max numeric,
  price_level text,
  country text,
  country_level text,
  updated_utc timestamptz NOT NULL
);

CREATE INDEX ix_listings_status_availabilityfrom ON listings(status, availability_from);
```

---

## 31. Configuration (appsettings.*.json Keys)
```json
{
  "Database": { "ConnectionString": "Host=...;Database=vegancoliving;..." },
  "Auth": { "MagicLink": { "Issuer": "VeganColiving", "JwtMinutes": 60 } },
  "Matching": { "Weights": { "Price": 0.2, "Availability": 0.15, "Location": 0.2, "Lifestyle": 0.3, "Facilities": 0.15 } },
  "RateLimiting": { "AuthRequestsPerHour": 5 },
  "Email": { "Provider": "Console", "From": "no-reply@vegancoliving.example" },
  "Telemetry": { "OtlpEndpoint": "http://otel-collector:4317" }
}
```

---

## 32. Rollout & Feature Flag Strategy
- Flag: MatchingV1 (off until Phase 2 complete)
- Flag: GroupChat (post-phase 3)
- Flag: SemanticSearch (post-MVP)
- Implementation: Simple JSON config + reloadOnChange; later integrate LaunchDarkly or OpenFeature.

---

## 33. Deletion & Data Lifecycle
User Deletion:
- Hard delete: user, preference_set, matches for user, group membership.
- Messages: either anonymize sender (replace with “DeletedUser”) or delete (choose based on product/legal).
Listing Lifecycle:
- Draft → Active → Paused/Filled → Archived (immutable except unarchive path).
Match TTL:
- Recompute or expire after 14 days (configurable).

---

## 34. Accessibility Implementation Notes
- All interactive components must carry role and aria-label where implicit semantics unclear.
- Keyboard navigation tests in Playwright (focus order).
- Color contrast check CI job (Pa11y / axe integration optional).

---

## 35. Future Enhancements (Technical Hooks Ready)
- Vector embeddings table with (entity_type, entity_id, embedding vector) referencing listings & user preference text → used to augment soft scoring.
- Multi-tenancy header X-Tenant-ID for future segmentation (schema or row discriminator).
- Event-driven re-score on listing update -> push updated match suggestions.

---

## 36. Summary
This specification provides the blueprint to implement the MVP reliably with clear modular boundaries, extensibility points, and operational considerations. Pending decisions (auth model, provider choices) must be finalized at Phase 0 checkpoint to avoid rework.

---

## 37. Next Steps
1. Approve DB provider & auth approach.
2. Confirm Phase 0 backlog tasks & assign owners.
3. Implement solution skeleton + initial migration.
4. Set up CI pipeline with baseline tests.
5. Begin Phase 1 development upon sign-off.

---

End of Document.
