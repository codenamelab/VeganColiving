Potensielt nyttige data:

- Aktivitetsfornyelse “aktiv søknad”, må bekrefte fortsatt interesse ved jevne mellomrom.

- Boligtype og sted
- Etasje

- Konkrete til bolig/Finn-lenker ol
- Innflyttstid / oppsigelsestid nåværende bolig

Teknisk nettside:
Semantisk søk
Kryptering, end to end.

Formål: 
Bidra til at veganere med lignende preferanser, som ønsker å opprette kollektiv, eller som har ledig plass i eksisterende kollektiv, finner sammen. 

VeganEverything

Målgruppe:
“Alle” veganere.

Matching: Oppretting av "lukket" gruppe am matchede personer - etter de har bekreftet. Alternativ for å åpne for flere gruppemedlemmer. Venteliste. Visning av preferanser (hva som er likt hva som er ulikt).

Markedsundersøkelser: 
Spørre om innspill i veganplattformer og i samtaler.

Eksisterende vegankollektiver eller visninger: Reviewsfunksjonalitet.


Preferanser og Filtre (- Ønsker vs krav) (obligatorisk vs frivillig)
1. Beliggenhet

    Land
    By/Område “Fylke - by - bydel”
    Nabolag “bydel”

Avstand til offentlig transport

Nærhet til universiteter eller arbeidsplasser

2. Pris per person

    Månedlig leiepris
        Minimumspris
        Maksimumspris

Leilighet   
 Inkluderte kostnader
        Strøm 	ja 	nei	fleksibel	pris
        Internett
        Vann og avløp
        Fellesutgifter

3. Romspesifikasjoner

    Type rom
        Enkeltrom
        Dobbeltrom
   
    Møblert/Uten møbler
    Størrelse på rom (kvadratmeter)
    Størrelse på leilighet (kvadratmeter)
    Antall bad i leiligheten
    Balkong/Terrasse
    Støyisolering

4. Kollektivdetaljer

    Antall beboere totalt
    Kjønn på beboere
        Alle kvinner
        Alle menn
        Blandet
    Alder på beboere
        Aldersgruppe (f.eks. 20-30 år)
    Yrkesstatus
        Studenter
        Yrkesaktive

5. Livsstil og Verdier
- Du er veganer (forced)

    Røykeregler
        Røykfritt
        Røyking tillatt ute
        Røyking tillatt
    Alkohol
        Alkoholfritt husholdning
        Alkohol tillatt i moderate mengder
  Festing tillatt?
- Aldri, sjelden, åpent
    Støynivå
Helt stille
Rolig atmosfære
Livlig
Høylytt og dynamisk
        
    Besøkende
        Begrensninger på overnattingsgjester
        Åpent for besøk

6. Fasiliteter

    Internett-hastighet (minimum mbit)
    Vaskemaskin/Tørketrommel
    Oppvaskmaskin
    Parkering
        Gateparkering
        Privat parkering
    Sykkeloppbevaring
    Lagringsplass (bod i m2 per person)
    Fellesarealer
       Stue
       Hage
       Balkong
       Treningsrom
       Stort kjøkken (induksjonskomfyr)

7. Husdyr

    Dyrevennlig
        Tillater vegansk kjæledyr
        Tillater alle kjæledyr
        Ingen kjæledyr
    Allergivennlig
        Visse allergier mot enkelte dyr?

8. Innflyttingsdetaljer

    Tilgjengelig fra (dato)
Må ha bolig innen: (dato, fleksibel)
	Varslingstid før innflytt: (i uker) 
    Leieperiode
Ultrakort (Under 6 måneder)        
Korttidsleie (under 12 måneder)
      	Langtidsleie (over 12 måneder)
Akseptabel bindingstid bolig
Akseptabel Oppsigelsestid bolig

9. Språk og Kultur

    Språk som snakkes i kollektivet
        Norsk
        Engelsk
        Andre språk

10. Personlighet og Interesser

    Interesser
Interesse for
- Aktivisme

Matlaging
- Whole food plant based

        Utendørsaktiviteter
        Kunst og kultur
        Musikk
        Sport

    Personlighetstype
        Introvert
        Ekstrovert
        Balansert

 Ryddevaner
- Ekstremt ryddig
- Ganske ryddig
- Moderat ryddig
- Avslappet til rydding



11. Arbeid og Studier

    Arbeidstid
        Dagarbeid
        Kvelds-/Nattarbeid
        Uregelmessige tider/turnus
    Hjemmekontor
        Jobber mest hjemmefra
        Jobber mest utenfor hjemmet
    Studenter
        Fulltidsstudent
        Deltidsstudent

12. Spesielle Behov

    Tilgjengelighet
        Rullestolvennlig
        Tilpasset for funksjonshemmede
    Allergier
    	Beskriv frifelt

13. Teknologi og Underholdning

    Streaming-tjenester
        Felles abonnement (Netflix, Spotify osv.)
    Spillkonsoller
        Tilgjengelig i fellesområdet
    Hjemmeautomatisering
        Smart lys
        Smart termostat

14. Miljøpraksis

    Resirkulering
        Streng resirkulering
        Kompostering
    Energibesparende tiltak
        Bruk av LED-lys
        Solcellepaneler
    Deling av ressurser
        Felles matinnkjøp
        Deling av husholdningsartikler

15. Kommunikasjonspreferanser

    Foretrukket kontaktmetode
        E-post
        Telefon
        Meldingssystem på nettsiden (varsling epost)
    Responsivitet
        Rask til å svare
        Avslappet kommunikasjon

---

# 🚀 Improvement Ideas & Implementation Roadmap

## Current Status Summary (as of December 2025)

The VeganColiving app currently supports:
- ✅ User authentication and basic profile management
- ✅ Home listings (browse, register, edit, view details)
- ✅ Interest/commitment tracking per home
- ✅ Map view of homes (Leaflet-based)
- ✅ Localization (English/Norwegian)
- ✅ Basic community member directory
- ✅ Multiple images per home
- ✅ External URL links for homes

---

## 🎯 High Priority Improvements (MVP Enhancement)

### 1. User Preferences & Matching System
**Impact**: High | **Effort**: Medium

This is the core feature that would differentiate VeganColiving from generic housing sites.

*Note: ApplicationUser already has basic preferences (MinMonthlyRentalPrice, MaxMonthlyRentalPrice, MinFloor, MaxFloor). This enhancement would extend those into a comprehensive system.*

- [ ] Extend existing user preferences or create dedicated `PreferenceSet` entity
- [ ] Implement preference categories from the list above:
  - Location (city, country, proximity to transit)
  - Price range (min/max monthly rent) - *extend existing fields*
  - Floor preferences - *extend existing fields*
  - Room specs (furnished, size, availability dates)
  - Lifestyle (smoking, alcohol, noise, visitors)
  - Coliving details (residents count, gender mix, age range)
  - Environmental practices
- [ ] Add preference editor UI component with "Requirement" vs "Preference" toggle
- [ ] Build matching engine that scores homes against user preferences
- [ ] Display match percentage and breakdown on home cards
- [ ] "Top Matches For You" section on homes page

### 2. Search & Filtering for Homes
**Impact**: High | **Effort**: Low

Currently the homes page shows all listings without filtering capability.

- [ ] Add filter sidebar/panel with:
  - City/country dropdown
  - Price range slider
  - Capacity filter
  - Status filter (Potential vs Active)
  - Availability date picker
- [ ] Full-text search across titles and descriptions
- [ ] Sort options: price (low/high), date listed, match score
- [ ] URL-based filters for shareable searches

### 3. Active Listing Renewal System  
**Impact**: Medium | **Effort**: Low

Ensures data freshness and user engagement.

*Note: Home entity already has DateListed and DateActivatedUtc. Consider adding renewal tracking either as a field on Home or as a separate audit entity.*

- [ ] Add `LastRenewalUtc` field to Home entity (or create RenewalRecord for audit trail)
- [ ] Require monthly confirmation that listings are still available
- [ ] Show "last confirmed" date on listings
- [ ] Background job to send renewal reminder emails
- [ ] Auto-archive listings not renewed within 60 days
- [ ] "Renew" button on home details for owners

### 4. Group Formation & Collaboration
**Impact**: High | **Effort**: High

Enables the core use case of forming new vegan collectives.

- [ ] Create `Group` entity with members and status (Forming/Active/Closed)
- [ ] "Create a Group" wizard for users seeking roommates
- [ ] Show compatibility scores between potential group members
- [ ] Group chat/discussion thread
- [ ] Shared wishlist of homes the group is interested in
- [ ] Waitlist functionality for full groups

---

## 🔧 Medium Priority Improvements

### 5. Enhanced Home Details
**Impact**: Medium | **Effort**: Low

- [ ] Add floor number field
- [ ] Structured amenities/facilities (washer, dishwasher, parking, etc.)
- [ ] Transportation proximity info
- [ ] Lease term preferences (short/long term)
- [ ] Availability window (from/to dates)
- [ ] Square meters / size field for the full home

### 6. User Profile Enhancements  
**Impact**: Medium | **Effort**: Medium

- [ ] Profile photo upload (replace randomuser.me placeholders)
- [ ] Bio / "About me" text field
- [ ] Lifestyle interests tags (activism, cooking, sports, etc.)
- [ ] Personality indicator (introvert/extrovert/balanced)
- [ ] Work schedule (day/night/remote)
- [ ] Languages spoken
- [ ] Privacy controls (what's visible to non-members)
- [ ] Optional verification badge

### 7. Direct Messaging
**Impact**: High | **Effort**: Medium

Essential for users to communicate about listings.

- [ ] User-to-user direct messaging
- [ ] Thread-based conversations with read status
- [ ] Email notifications for new messages
- [ ] Message from home detail page ("Contact owner")
- [ ] Inbox/conversation list page

### 8. Reviews & Reputation
**Impact**: Medium | **Effort**: Medium

- [ ] Reviews for existing/active coliving homes
- [ ] Rating dimensions: cleanliness, location, value, community
- [ ] "Verified resident" badge for reviews
- [ ] Review moderation workflow

### 9. Notifications System
**Impact**: Medium | **Effort**: Low

- [ ] In-app notification center
- [ ] Notification types: new messages, matches, renewal reminders, interest in your home
- [ ] Email notification preferences
- [ ] Real-time updates via SignalR (already available in Blazor Server)

---

## 💡 Lower Priority / Future Ideas

### 10. Semantic Search
- [ ] Vector embeddings for listings and user preferences
- [ ] Natural language search queries
- [ ] AI-powered matching recommendations
- [ ] Integration with pgvector or similar

### 11. Map Improvements
- [ ] Marker clustering for dense areas
- [ ] Filter controls on map view
- [ ] User location display (with permission)
- [ ] Distance-based search
- [ ] Toggle between list and map view seamlessly

### 12. Mobile Experience
- [ ] Progressive Web App (PWA) support with offline capability
- [ ] Mobile-optimized navigation
- [ ] Consider Blazor Hybrid for native mobile app

### 13. Community Features
- [ ] Events calendar for vegan meetups
- [ ] Discussion forum or Q&A
- [ ] Resource library (vegan living tips, recipes)
- [ ] Success stories section

### 14. Sustainability Scoring
- [ ] Home sustainability rating system
- [ ] Criteria: recycling, composting, solar, LED, shared purchasing
- [ ] Display badge on high-sustainability homes

### 15. Analytics Dashboard
- [ ] Home owner dashboard (views, clicks, interest count)
- [ ] Community statistics
- [ ] Demand heat maps

---

## 🛡️ Technical Improvements

### Security & Privacy
- [ ] Rate limiting on auth endpoints
- [ ] Field-level encryption for sensitive data
- [ ] GDPR data export and deletion features
- [ ] Magic link authentication option

### Performance
- [ ] Redis caching for frequently accessed data
- [ ] Pagination with "load more" for homes list
- [ ] Image optimization and lazy loading
- [ ] Consider migrating to PostgreSQL for better JSON/FTS/vector support (currently using SQL Server; Tech Spec recommends PostgreSQL)

### Code Quality
- [ ] Unit tests for matching/scoring logic
- [ ] Integration tests for API endpoints
- [ ] GitHub Actions CI/CD pipeline
- [ ] OpenTelemetry observability setup

---

## 📊 Suggested Implementation Order

**Phase 1 (Weeks 1-4)**: Foundation
1. Search & Filtering for Homes
2. User Profile Enhancements (photo, bio)
3. Active Listing Renewal System

**Phase 2 (Weeks 5-8)**: Core Matching
1. User Preferences entity & editor
2. Basic matching engine
3. Match score display

**Phase 3 (Weeks 9-12)**: Communication
1. Direct Messaging system
2. Notifications
3. Email integration

**Phase 4 (Weeks 13-16)**: Advanced
1. Group Formation features
2. Reviews system
3. Map improvements

**Post-MVP**: Semantic search, mobile app, analytics

