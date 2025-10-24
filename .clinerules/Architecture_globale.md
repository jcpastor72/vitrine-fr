# ARCHITECTURE COMPLÈTE - LABOR CONTROL

**Date de création** : 23 octobre 2025
**Version** : 1.0
**Analyse approfondie du projet**

---

## 📋 TABLE DES MATIÈRES

1. [Vue d'ensemble](#vue-densemble)
2. [Architecture Backend](#architecture-backend)
3. [Architecture Frontend Vitrine](#architecture-frontend-vitrine)
4. [Architecture Application Mobile](#architecture-application-mobile)
5. [Base de données](#base-de-données)
6. [Infrastructure et Déploiement](#infrastructure-et-déploiement)
7. [Sécurité](#sécurité)
8. [APIs et Intégrations](#apis-et-intégrations)

---

## 1. VUE D'ENSEMBLE

### 1.1 Stack Technologique Globale

```
┌─────────────────────────────────────────────────────────────┐
│                      LABOR CONTROL                          │
│                  Plateforme Multi-Canal                      │
└─────────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                │             │             │
          ┌─────▼────┐  ┌────▼─────┐  ┌───▼────┐
          │ Backend  │  │ Frontend │  │ Mobile │
          │ .NET 9   │  │  Blazor  │  │React   │
          │   API    │  │WebAssembly│ │Native  │
          └─────┬────┘  └────┬─────┘  └───┬────┘
                │            │            │
                └────────┬───┴────────────┘
                         │
                  ┌──────▼───────┐
                  │ PostgreSQL   │
                  │   Azure      │
                  └──────────────┘
```

### 1.2 Composants Principaux

| Composant | Technologie | Port | Environnement |
|-----------|-------------|------|---------------|
| **Backend API** | .NET 9 / ASP.NET Core | 5278 | http://localhost:5278 |
| **Frontend Vitrine** | Blazor WebAssembly (.NET 9) | 5140 | http://localhost:5140 |
| **App Mobile** | React Native / Expo | N/A | iOS/Android |
| **Base de données** | PostgreSQL 15 | 5432 | localhost / Azure |

---

## 2. ARCHITECTURE BACKEND

### 2.1 Structure des Dossiers

```
Backend/LaborControl.API/
├── Controllers/          # API REST Controllers
│   ├── AlertsController.cs
│   ├── AssetsController.cs
│   ├── AuthController.cs
│   ├── CartController.cs
│   ├── ControlPointsController.cs
│   ├── CustomersController.cs
│   ├── IndustriesController.cs
│   ├── MaintenanceSchedulesController.cs
│   ├── OrdersController.cs
│   ├── ProductsController.cs
│   ├── QualificationsController.cs
│   ├── RfidChipsController.cs
│   ├── SectorsController.cs
│   ├── SitesController.cs
│   ├── TaskExecutionsController.cs
│   ├── TasksController.cs
│   ├── TaskTemplatesController.cs
│   ├── TeamsController.cs
│   ├── UsersController.cs
│   └── ZonesController.cs
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── SeedDefaultTaskTemplate.sql # Script SQL gamme par défaut
├── DTOs/                 # Data Transfer Objects
│   ├── AssetDTOs.cs
│   ├── CartDTOs.cs
│   ├── ControlPointDTOs.cs
│   ├── CreateUserRequest.cs
│   ├── CustomerDTOs.cs
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── OrderDTO.cs
│   ├── ProductDTOs.cs
│   ├── RegisterProfessionalDTO.cs
│   ├── RfidChipDTOs.cs
│   ├── SiteDTOs.cs
│   ├── TeamDTOs.cs
│   └── ZoneDTOs.cs
├── Models/               # Entités de domaine
│   ├── Alert.cs
│   ├── Asset.cs
│   ├── CartItem.cs
│   ├── ControlPoint.cs
│   ├── Customer.cs
│   ├── Industry.cs
│   ├── MaintenanceSchedule.cs
│   ├── Order.cs
│   ├── Product.cs
│   ├── Qualification.cs
│   ├── QualificationUser.cs
│   ├── RfidChip.cs
│   ├── ScheduledTask.cs
│   ├── Sector.cs
│   ├── Site.cs
│   ├── TaskExecution.cs
│   ├── TaskTemplate.cs
│   ├── Team.cs
│   ├── User.cs
│   └── Zone.cs
├── Migrations/           # Migrations EF Core (67 migrations)
├── Services/             # Services métier
│   ├── InvoiceService.cs
│   ├── OrderFulfillmentService.cs
│   ├── SiretVerificationService.cs
│   └── StripePaymentService.cs
├── Program.cs            # Point d'entrée de l'application
├── appsettings.json      # Configuration production
└── appsettings.Development.json # Configuration développement
```

### 2.2 Clean Architecture

L'application suit les principes de **Clean Architecture** :

```
┌──────────────────────────────────────────┐
│         Presentation Layer               │
│         (Controllers)                    │
├──────────────────────────────────────────┤
│         Application Layer                │
│         (DTOs, Services)                 │
├──────────────────────────────────────────┤
│         Domain Layer                     │
│         (Models/Entities)                │
├──────────────────────────────────────────┤
│         Infrastructure Layer             │
│   (ApplicationDbContext, Migrations)     │
└──────────────────────────────────────────┘
```

### 2.3 Entités du Domaine (Models)

#### Hiérarchie des Entités

```
Customer (Client)
  └── Site (Site géographique)
       ├── Zone (Zone dans un site)
       │    ├── Asset (Équipement/Machine)
       │    │    └── ControlPoint (Point de contrôle)
       │    │         └── ScheduledTask (Tâche planifiée)
       │    │              └── TaskExecution (Exécution de tâche)
       │    └── ControlPoint (Point de contrôle direct)
       └── Team (Équipe)
            └── User (Utilisateur/Technicien)
```

#### Entités E-Commerce

```
Product (Produit - Puces NFC, Services)
  └── CartItem (Panier)
       └── Order (Commande)
            └── RfidChip (Puce RFID expédiée)
```

#### Système de Qualifications

```
Sector (Secteur d'activité)
  └── Industry (Industrie)
       └── Qualification (Qualification métier)
            └── QualificationUser (Attribution utilisateur)
```

### 2.4 Controllers Principaux

#### AuthController
- **POST** `/api/Auth/register` - Inscription client professionnel
- **POST** `/api/Auth/login` - Connexion
- **POST** `/api/Auth/refresh` - Rafraîchissement token JWT

#### TasksController
- **GET** `/api/tasks` - Liste des tâches
- **POST** `/api/tasks` - Créer tâche
- **PUT** `/api/tasks/{id}` - Modifier tâche
- **DELETE** `/api/tasks/{id}` - Supprimer tâche
- **POST** `/api/tasks/check-availability` - Vérifier disponibilité technicien

#### AssetsController
- **GET** `/api/assets` - Liste équipements
- **GET** `/api/assets/{id}` - Détail équipement
- **POST** `/api/assets` - Créer équipement
- **PUT** `/api/assets/{id}` - Modifier équipement (incluant heures de fonctionnement)
- **DELETE** `/api/assets/{id}` - Supprimer équipement

#### OrdersController (E-Commerce)
- **GET** `/api/orders` - Liste commandes
- **POST** `/api/orders` - Créer commande
- **POST** `/api/orders/{id}/confirm-payment` - Confirmer paiement Stripe

#### RfidChipsController
- **GET** `/api/rfid-chips` - Liste puces RFID
- **POST** `/api/rfid-chips` - Enregistrer puce
- **POST** `/api/rfid-chips/validate/{uid}` - Valider scan NFC

### 2.5 Services

#### InvoiceService
Génération de factures PDF pour les commandes avec logo et informations légales.

#### StripePaymentService
Intégration Stripe pour paiements en ligne (puces RFID, abonnements).

#### SiretVerificationService
Vérification API INSEE pour validation SIRET lors de l'inscription.

#### OrderFulfillmentService
Gestion du processus de commande (préparation, expédition, livraison).

### 2.6 Authentification & Autorisation

- **JWT (JSON Web Tokens)** pour l'authentification
- **BCrypt** pour le hachage des mots de passe
- **Rôles** : Admin, Supervisor, Technician, Customer
- **Multi-tenant** : Isolation totale par `CustomerId`

**Configuration JWT** (appsettings.Development.json) :
```json
{
  "Jwt": {
    "Key": "VotreCleSecreteTresLonguePourLaborControl2025!",
    "Issuer": "LaborControl",
    "Audience": "LaborControlApp"
  }
}
```

### 2.7 Configuration Base de Données

**Connection String Local** :
```
Host=localhost;Port=5432;Database=laborcontrol_local;Username=postgres;Password=postgres
```

**Production Azure** : Configurée dans `appsettings.json`

---

## 3. ARCHITECTURE FRONTEND VITRINE

### 3.1 Technologie

- **Blazor WebAssembly** (.NET 9)
- **Tailwind CSS** pour le styling
- **SignalR Client** pour les notifications temps réel
- **Hébergement** : Azure Static Web Apps

### 3.2 Structure des Dossiers

```
vitrine-fr/Client/ (VitrineFr)
├── Components/           # Composants réutilisables
├── Models/              # Modèles côté client
├── Pages/               # Pages Blazor (.razor)
│   ├── AccountDetails.razor
│   ├── AdminDashboard.razor
│   ├── Alerts.razor
│   ├── AssetDetail.razor
│   ├── Assets.razor
│   ├── Calendar.razor
│   ├── Cart.razor             # E-commerce
│   ├── Catalog.razor          # E-commerce
│   ├── Checkout.razor         # E-commerce
│   ├── ControlPoints.razor
│   ├── CreateTask.razor
│   ├── Dashboard.razor
│   ├── EditAsset.razor
│   ├── EditControlPoint.razor
│   ├── Index.razor            # Page d'accueil
│   ├── Industries.razor       # Gestion industries
│   ├── Login.razor
│   ├── OrderChips.razor       # Commande puces
│   ├── Orders.razor           # Historique commandes
│   ├── Payment.razor          # Paiement Stripe
│   ├── Personnel.razor        # Gestion personnel
│   ├── Qualifications.razor   # Gestion qualifications
│   ├── Register.razor
│   ├── Sectors.razor          # Gestion secteurs
│   ├── Sites.razor
│   ├── TasksDashboard.razor   # Dashboard principal tâches
│   ├── TaskTemplates.razor    # Gammes de contrôle
│   ├── Teams.razor
│   ├── Admin-lc/               # Pages administration
│   │   ├── ActivityLog.razor
│   │   ├── Clients.razor
│   │   ├── Dashboard.razor
│   │   ├── Orders.razor
│   │   ├── Preparation.razor
│   │   ├── RfidChips.razor
│   │   └── Shipments.razor
│   └── ...
├── Services/            # Services HTTP
│   ├── AuthService.cs
│   ├── ApiService.cs
│   └── ...
├── wwwroot/            # Fichiers statiques
│   ├── css/
│   ├── js/
│   ├── images/
│   ├── index.html      # Point d'entrée HTML
│   └── appsettings.json
├── _Imports.razor      # Imports globaux
├── App.razor           # Composant racine
├── Program.cs          # Configuration Blazor WASM
└── VitrineFr.csproj
```

### 3.3 Pages Principales

#### Pages Opérationnelles (Techniciens/Superviseurs)
- **Dashboard.razor** - Vue d'ensemble temps réel
- **TasksDashboard.razor** - Liste des tâches planifiées avec filtres
- **Calendar.razor** - Vue calendrier des tâches
- **CreateTask.razor** - Création de tâche avec gestion week-ends
- **TaskExecutions.razor** - Historique des exécutions

#### Pages Gestion (Admin/Superviseurs)
- **Sites.razor** / **CreateSite.razor** / **EditSite.razor** - Gestion sites
- **Assets.razor** / **CreateAsset.razor** / **EditAsset.razor** - Gestion équipements
- **ControlPoints.razor** / **CreateControlPoint.razor** - Gestion points de contrôle
- **Teams.razor** / **CreateTeam.razor** - Gestion équipes
- **Personnel.razor** / **CreateUser.razor** - Gestion utilisateurs

#### Pages E-Commerce
- **Catalog.razor** - Catalogue produits (puces NFC, services)
- **Cart.razor** - Panier
- **Checkout.razor** - Récapitulatif commande
- **Payment.razor** - Paiement Stripe
- **OrderChips.razor** - Commande rapide de puces
- **Orders.razor** - Historique commandes

#### Pages Administration (Super Admin)
- **Admin/Dashboard.razor** - Dashboard administrateur
- **Admin/Clients.razor** - Gestion clients
- **Admin/Orders.razor** - Gestion commandes
- **Admin/Preparation.razor** - Préparation commandes
- **Admin/Shipments.razor** - Gestion expéditions
- **Admin/RfidChips.razor** - Stock puces RFID

### 3.4 Fonctionnalités Clés

#### Notifications Temps Réel (SignalR)
- Notifications centrées au milieu de l'écran
- Animation fade-in/zoom
- Auto-dismiss après 3 secondes
- Implémenté dans `wwwroot/index.html`

#### Mode Offline
- Service Worker pour mise en cache
- Fonctionnement hors ligne partiel

#### Responsive Design
- Tailwind CSS
- Mobile-first
- Support tablettes

---

## 4. ARCHITECTURE APPLICATION MOBILE

### 4.1 Technologie

- **React Native** avec **Expo SDK 52**
- **TypeScript**
- **Expo Router** (navigation basée sur fichiers)
- **NFC Manager** pour lecture puces NFC
- **Permissions** : NFC (Android), Core NFC (iOS)

### 4.2 Structure des Dossiers

```
Mobile/LaborControlApp/
├── app/                      # Routes Expo Router
│   ├── (tabs)/              # Navigation à onglets
│   │   ├── index.tsx        # Page d'accueil
│   │   ├── explore.tsx      # Page exploration
│   │   ├── register-chips.tsx # Enregistrement puces
│   │   └── _layout.tsx      # Layout onglets
│   ├── modal.tsx            # Modal
│   └── _layout.tsx          # Layout racine
├── components/              # Composants réutilisables
│   ├── ui/                  # Composants UI
│   ├── FormRenderer.tsx     # Rendu dynamique formulaires
│   ├── PhotoEditor.tsx      # Éditeur de photos
│   ├── external-link.tsx
│   ├── haptic-tab.tsx
│   ├── parallax-scroll-view.tsx
│   ├── themed-text.tsx
│   └── themed-view.tsx
├── constants/               # Constantes (couleurs, config)
├── hooks/                   # Hooks React personnalisés
├── scripts/                 # Scripts utilitaires
├── assets/                  # Images, icônes, splash screen
│   └── images/
│       ├── icon.png
│       ├── splash-icon.png
│       ├── android-icon-*.png
│       └── favicon.png
├── android/                 # Configuration Android native
├── app.json                 # Configuration Expo
├── eas.json                 # Configuration EAS Build
├── package.json
└── tsconfig.json
```

### 4.3 Configuration (app.json)

```json
{
  "expo": {
    "name": "LaborControlApp",
    "slug": "LaborControlApp",
    "version": "1.0.0",
    "orientation": "portrait",
    "scheme": "laborcontrolapp",
    "newArchEnabled": true,
    "android": {
      "permissions": ["android.permission.NFC"],
      "package": "com.jcpastor.LaborControlApp"
    },
    "plugins": [
      "expo-router",
      "expo-splash-screen",
      "react-native-nfc-manager"
    ]
  }
}
```

### 4.4 Fonctionnalités

#### Scan NFC
- Utilisation de `react-native-nfc-manager`
- Lecture UID puce RFID
- Validation via API Backend

#### Formulaires Dynamiques
- Composant `FormRenderer.tsx` pour affichage dynamique
- Types de champs : Texte, Nombre, Select, Checkbox, Photo
- Validation temps réel

#### Gestion Photos
- `PhotoEditor.tsx` pour capture et édition
- Compression avant envoi
- OCR potentiel (via API)

#### Navigation
- **Expo Router** (navigation basée sur fichiers)
- Onglets principaux : Accueil, Explorer, Enregistrer puces

---

## 5. BASE DE DONNÉES

### 5.1 PostgreSQL 15

**Hébergement** :
- **Local** : localhost:5432 (développement)
- **Production** : Azure Database for PostgreSQL Flexible Server

### 5.2 Schéma Principal

#### Tables Core

```sql
-- Clients
Customers
  - Id (UUID, PK)
  - Name (VARCHAR)
  - Email (VARCHAR)
  - Siret (VARCHAR)
  - Industry (VARCHAR)
  - SubscriptionPlan (VARCHAR)
  - Website (VARCHAR)
  - ApeCode (VARCHAR)

-- Sites
Sites
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - Name (VARCHAR)
  - Address (VARCHAR)
  - City (VARCHAR)
  - PostalCode (VARCHAR)
  - Siret (VARCHAR)
  - IsActive (BOOL)

-- Zones
Zones
  - Id (UUID, PK)
  - SiteId (UUID, FK → Sites)
  - Name (VARCHAR)
  - Description (TEXT)

-- Assets (Équipements)
Assets
  - Id (UUID, PK)
  - ZoneId (UUID, FK → Zones)
  - Name (VARCHAR)
  - Type (VARCHAR)
  - SerialNumber (VARCHAR)
  - OperatingHours (INT) -- Heures de fonctionnement
  - DisplayOrder (INT)

-- ControlPoints (Points de contrôle)
ControlPoints
  - Id (UUID, PK)
  - AssetId (UUID, FK → Assets, nullable)
  - ZoneId (UUID, FK → Zones, nullable)
  - Name (VARCHAR)
  - Description (TEXT)
  - ChipId (UUID, FK → RfidChips, unique)
  - Latitude (DECIMAL)
  - Longitude (DECIMAL)
```

#### Tables Tâches

```sql
-- ScheduledTasks (Tâches planifiées)
ScheduledTasks
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - ControlPointId (UUID, FK → ControlPoints)
  - AssignedUserId (UUID, FK → Users, nullable)
  - Title (VARCHAR)
  - Frequency (VARCHAR) -- DAILY/WEEKLY/MONTHLY/ONCE
  - ScheduledDate (TIMESTAMP)
  - ScheduledEndDate (TIMESTAMP, nullable)
  - ScheduledStartTime (TIMESPAN, nullable)
  - ScheduledEndTime (TIMESPAN, nullable)
  - WeekendHandling (VARCHAR) -- ALLOW/MOVE_TO_MONDAY/SKIP
  - DelayTolerance (INT) -- Minutes de tolérance
  - RequireDoubleScan (BOOL)
  - TaskType (VARCHAR) -- PREVENTIVE/CORRECTIVE/INSPECTION
  - Status (VARCHAR)
  - IsCancelled (BOOL)
  - CancelledBy (UUID, FK → Users, nullable)
  - CancelledAt (TIMESTAMP, nullable)
  - CancellationReason (TEXT, nullable)

-- TaskTemplates (Gammes de contrôle)
TaskTemplates
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - Name (VARCHAR)
  - Description (TEXT)
  - FormDefinition (JSONB) -- Définition dynamique du formulaire
  - IsPredefined (BOOL)
  - IndustryId (UUID, FK → Industries, nullable)
  - RequireDoubleScan (BOOL)

-- TaskExecutions (Exécutions de tâches)
TaskExecutions
  - Id (UUID, PK)
  - TaskId (UUID, FK → ScheduledTasks)
  - UserId (UUID, FK → Users)
  - StartedAt (TIMESTAMP)
  - CompletedAt (TIMESTAMP, nullable)
  - FirstScanAt (TIMESTAMP, nullable)
  - SecondScanAt (TIMESTAMP, nullable)
  - FormData (JSONB) -- Données saisies
  - Photos (TEXT[]) -- URLs photos
  - Status (VARCHAR) -- OK/WARNING/CRITICAL
```

#### Tables Utilisateurs

```sql
-- Users (Utilisateurs)
Users
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - Email (VARCHAR, UNIQUE)
  - Username (VARCHAR, nullable)
  - PasswordHash (VARCHAR)
  - Role (VARCHAR) -- ADMIN/SUPERVISOR/TECHNICIAN
  - FirstName (VARCHAR)
  - LastName (VARCHAR)
  - Phone (VARCHAR)
  - SectorId (UUID, FK → Sectors, nullable)
  - IndustryId (UUID, FK → Industries, nullable)
  - IsActive (BOOL)
  - MustChangePassword (BOOL)

-- Teams (Équipes)
Teams
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - Name (VARCHAR)
  - Description (TEXT)
  - WorkStartTime (TIMESPAN, nullable)
  - WorkEndTime (TIMESPAN, nullable)
  - WorkDays (VARCHAR, nullable) -- JSON array jours travaillés

-- QualificationUsers (Attribution qualifications)
QualificationUsers
  - UserId (UUID, FK → Users)
  - QualificationId (UUID, FK → Qualifications)
  - AcquiredDate (TIMESTAMP)
  - ExpirationDate (TIMESTAMP, nullable)
```

#### Tables E-Commerce

```sql
-- Products (Produits)
Products
  - Id (UUID, PK)
  - Name (VARCHAR)
  - Description (TEXT)
  - Price (DECIMAL)
  - Type (VARCHAR) -- CHIP/SERVICE
  - StockQuantity (INT)
  - IsActive (BOOL)

-- CartItems (Panier)
CartItems
  - Id (UUID, PK)
  - UserId (UUID, FK → Users)
  - ProductId (UUID, FK → Products)
  - Quantity (INT)
  - AddedAt (TIMESTAMP)

-- Orders (Commandes)
Orders
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - OrderNumber (VARCHAR, UNIQUE)
  - TotalAmount (DECIMAL)
  - Status (VARCHAR) -- PENDING/PAID/PREPARING/SHIPPED/DELIVERED/CANCELLED
  - Service (VARCHAR) -- BASIC/EXPRESS
  - ProductType (VARCHAR) -- CHIP/SERVICE
  - StripePaymentIntentId (VARCHAR)
  - InvoicePdf (TEXT) -- URL facture PDF
  - CreatedAt (TIMESTAMP)
  - UpdatedAt (TIMESTAMP)

-- RfidChips (Puces RFID)
RfidChips
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers, nullable)
  - ChipId (VARCHAR, UNIQUE)
  - Uid (VARCHAR, UNIQUE)
  - Status (VARCHAR) -- STOCK/ACTIVE/INACTIVE
  - OrderId (UUID, FK → Orders, nullable)
  - PackagingType (VARCHAR, nullable)
  - ActivationDate (TIMESTAMP, nullable)
  - Checksum (VARCHAR) -- SHA256
```

#### Tables Référentiels

```sql
-- Sectors (Secteurs d'activité)
Sectors
  - Id (UUID, PK)
  - Name (VARCHAR)
  - Description (TEXT)
  - IsPredefined (BOOL)

-- Industries (Industries)
Industries
  - Id (UUID, PK)
  - SectorId (UUID, FK → Sectors)
  - Name (VARCHAR)
  - Description (TEXT)
  - IsPredefined (BOOL)

-- Qualifications (Qualifications métier)
Qualifications
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers, nullable)
  - SectorId (UUID, FK → Sectors, nullable)
  - Name (VARCHAR)
  - Description (TEXT)
  - ValidityPeriodDays (INT, nullable)
  - IsPredefined (BOOL)
  - IsActive (BOOL)
```

#### Tables Maintenance Préventive

```sql
-- MaintenanceSchedules
MaintenanceSchedules
  - Id (UUID, PK)
  - CustomerId (UUID, FK → Customers)
  - AssetId (UUID, FK → Assets)
  - Name (VARCHAR)
  - Description (TEXT)
  - Frequency (VARCHAR)
  - NextMaintenanceDate (TIMESTAMP)
  - IsActive (BOOL)
```

### 5.3 Migrations

**Total** : 67 migrations depuis le 6 octobre 2025

**Migrations récentes importantes** :
- `20251019052420_AddOperatingHoursToAssets` - Ajout heures de fonctionnement
- `20251019123626_AddInvoicePdfToOrder` - Ajout URL facture PDF
- `20251018150601_AddDelayToleranceToScheduledTasks` - Tolérance de retard
- `20251018101940_AddRequireDoubleScanToScheduledTasks` - Double scan
- `20251017080340_AddWeekendHandlingToScheduledTasks` - Gestion week-ends
- `20251017071643_AddCancellationFieldsToScheduledTasks` - Annulation tâches

### 5.4 Multi-Tenant

**Isolation totale par `CustomerId`** :
- Chaque table principale contient une colonne `CustomerId`
- Les requêtes filtrent systématiquement par `CustomerId`
- Aucune fuite de données entre clients

---

## 6. INFRASTRUCTURE ET DÉPLOIEMENT

### 6.1 Environnements

| Environnement | Backend | Frontend | Base de données |
|---------------|---------|----------|-----------------|
| **Développement** | localhost:5278 | localhost:5140 | PostgreSQL Local |
| **Production** | Azure App Service | Azure Static Web Apps | Azure PostgreSQL |

### 6.2 Azure (Production)

#### Services Utilisés
- **Azure App Service** - Backend API (.NET 9)
- **Azure Static Web Apps** - Frontend Blazor
- **Azure Database for PostgreSQL Flexible Server** - Base de données
- **Azure Blob Storage** - Stockage photos (potentiel)
- **Azure SignalR Service** - Notifications temps réel (optionnel)

#### URLs Production
- Backend API : `https://laborcontrol-api.azurewebsites.net`
- Frontend Web : `https://laborcontrol-web.azurestaticapps.net`

### 6.3 Déploiement

#### Backend
```bash
# Publier l'API
cd Backend/LaborControl.API
dotnet publish -c Release -o ./publish

# Déployer sur Azure (via Azure CLI ou Portal)
az webapp deploy --resource-group LaborControl --name laborcontrol-api --src-path ./publish
```

#### Frontend Blazor
```bash
# Build Blazor WASM
cd vitrine-fr/Client
dotnet publish -c Release -o ./publish

# Déployer sur Azure Static Web Apps (via GitHub Actions ou SWA CLI)
swa deploy ./publish/wwwroot
```

#### Mobile
```bash
# Build avec EAS (Expo Application Services)
cd Mobile/LaborControlApp
eas build --platform android
eas build --platform ios

# Soumettre aux stores
eas submit --platform android
eas submit --platform ios
```

---

## 7. SÉCURITÉ

### 7.1 Authentification

- **JWT (JSON Web Tokens)** avec clé secrète 256 bits
- **Expiration** : 60 minutes (configurable)
- **Refresh Tokens** : Non implémentés (à ajouter)

### 7.2 Autorisation

**Rôles** :
- `ADMIN` - Super administrateur (gestion clients)
- `CUSTOMER` - Propriétaire de compte client
- `SUPERVISOR` - Superviseur (gestion équipes, tâches)
- `TECHNICIAN` - Technicien terrain

**Politiques d'accès** :
- Controllers protégés par `[Authorize]`
- Vérification `CustomerId` systématique
- Endpoints admin limités au rôle `ADMIN`

### 7.3 Protection des Données

- **RGPD** : Consentement, droit à l'oubli, portabilité
- **Chiffrement** :
  - Mots de passe : BCrypt (10 rounds)
  - Transit : HTTPS/TLS 1.3
  - Base de données : Chiffrement au repos (Azure)
- **SIRET** : Vérification via API INSEE
- **Puces RFID** : Checksum SHA256 pour anti-clonage

### 7.4 API Sécurisée

- **CORS** : Configuré pour domaines autorisés uniquement
- **Rate Limiting** : À implémenter (recommandé)
- **Validation** : DTOs avec DataAnnotations
- **SQL Injection** : Protection EF Core (requêtes paramétrées)

---

## 8. APIS ET INTÉGRATIONS

### 8.1 API Stripe (Paiements)

**Service** : `StripePaymentService.cs`

**Fonctionnalités** :
- Création de Payment Intent
- Confirmation de paiement
- Webhooks pour événements Stripe

**Clés** (appsettings.Development.json) :
```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": ""
  }
}
```

### 8.2 API INSEE (Vérification SIRET)

**Service** : `SiretVerificationService.cs`

**Endpoint** : `https://api.insee.fr/entreprises/sirene/V3/siret/{siret}`

**Token** : `6c15a73c-7586-464c-9357-8f78eb5c778b`

**Utilisation** : Validation SIRET lors de l'inscription

### 8.3 API Interne (Backend → Frontend/Mobile)

**Base URL** : `http://localhost:5278/api` (dev)

**Endpoints principaux** :
- `/Auth/*` - Authentification
- `/Tasks/*` - Gestion tâches
- `/Assets/*` - Gestion équipements
- `/ControlPoints/*` - Points de contrôle
- `/RfidChips/*` - Puces RFID
- `/Orders/*` - Commandes
- `/Teams/*` - Équipes
- `/Users/*` - Utilisateurs

**Format** : JSON
**Authentification** : Bearer Token (JWT)

### 8.4 Intégrations Futures

- **GMAO** : SAP PM, Maximo, COSWIN (via API REST)
- **IoT** : Capteurs connectés (température, pression)
- **OCR** : Azure Cognitive Services (reconnaissance texte photos)
- **IA Prédictive** : Détection dérives, maintenance prédictive

---

## 9. MÉTRIQUES ET MONITORING

### 9.1 Logs

- **Backend** : ILogger (.NET) → Azure Application Insights
- **Frontend** : Console.log → À connecter à un service
- **Mobile** : Expo logging → Sentry (recommandé)

### 9.2 KPIs Techniques

- **Temps de réponse API** : <200ms (cible)
- **Disponibilité** : 99.5% (cible)
- **Taux d'erreur** : <1% (cible)

### 9.3 KPIs Métier

- **Tâches complétées/jour**
- **Taux de conformité** : % tâches validées à l'heure
- **Alertes générées/jour**
- **Commandes puces/mois**

---

## 10. ÉVOLUTIONS ARCHITECTURE

### 10.1 Court Terme (3-6 mois)

- ✅ Implémentation double scan NFC (fait)
- ✅ Gestion week-ends (fait)
- ✅ Heures de fonctionnement assets (fait)
- ⏳ Gamme "Contrôle Visuel" par défaut (script SQL créé, exécution manuelle requise)
- ⏳ Filtres Calendar complets (HTML créé, code C# manquant)

### 10.2 Moyen Terme (6-12 mois)

- Photo aléatoire + OCR (détection triche)
- Analyse prédictive ML (dérives)
- Rapports ISO 9001/14001/45001 automatiques
- Intégration GMAO (API publique)
- Mode tournée optimisé (TSP)
- QR code preuve conformité

### 10.3 Long Terme (12+ mois)

- Assistant IA diagnostic (GPT-4)
- Expansion EHPAD (templates spécifiques)
- Marketplace templates sectoriels
- Intégration IoT/capteurs
- Application Windows/Linux (Electron)
- API publique documentée (marketplace)

---

## 11. POINTS D'ATTENTION

### 11.1 Technique

⚠️ **Fichiers de configuration potentiellement obsolètes** :
- `AUTHENTICATION_API_ANALYSIS.md`
- `AUTHENTICATION_DETAILED_ANALYSIS.md`
- `DEPLOIEMENT_AZURE_BETA.md`
- `README.LOCAL.md`
- Dossiers `StaticSite/` et `frontend-temp/` (voir FICHIERS_OBSOLETES.md)

⚠️ **Migrations nombreuses** : 67 migrations peuvent ralentir les builds. Envisager une consolidation.

⚠️ **Tests unitaires** : Aucun test automatisé détecté. À implémenter.

### 11.2 Sécurité

⚠️ **Secrets exposés** : Les clés Stripe et API INSEE sont dans `appsettings.Development.json` (attention au commit Git)

⚠️ **Refresh Tokens** : Non implémentés (les utilisateurs doivent se reconnecter après expiration JWT)

⚠️ **Rate Limiting** : Non implémenté (risque de DDoS)

### 11.3 Performance

⚠️ **Requêtes N+1** : Possible dans certains controllers (à vérifier avec profiling)

⚠️ **Cache** : Aucune stratégie de cache détectée (Redis recommandé)

⚠️ **Indexation BDD** : À vérifier sur les colonnes fréquemment utilisées en WHERE

---

## 12. CONCLUSION

L'architecture de **LABOR CONTROL** est **solide** et suit les **bonnes pratiques** :

✅ **Clean Architecture** (séparation des responsabilités)
✅ **Multi-tenant** sécurisé (isolation par CustomerId)
✅ **Stack moderne** (.NET 9, Blazor WASM, React Native)
✅ **Base de données relationnelle** bien structurée
✅ **API REST** cohérente
✅ **E-commerce** intégré (Stripe)
✅ **Azure** ready (déploiement cloud)

**Points d'amélioration** :
- Tests automatisés
- Gestion secrets (Azure Key Vault)
- Cache distribué (Redis)
- Refresh Tokens
- Consolidation migrations
- Nettoyage fichiers obsolètes

---

**Dernière mise à jour** : 23 octobre 2025
**Maintenu par** : Claude (desktop)
**Version** : 1.0
