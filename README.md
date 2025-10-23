# Site Vitrine Labor Control

Site web Labor control et application web, déployé sur Azure Static Web Apps.

## Table des matières

- [Vue d'ensemble](#vue-densemble)
- [Stack technique](#stack-technique)
- [Architecture](#architecture)
- [Workflow de développement](#workflow-de-développement)
- [Structure du projet](#structure-du-projet)
- [Configuration](#configuration)
- [Déploiement](#déploiement)
- [URLs importantes](#urls-importantes)
- [Fonctionnalités](#fonctionnalités)

---

## Vue d'ensemble

Site vitrine et application Labor Control permettant aux clients de :
- Découvrir la solution Labor Control
- Tester l'interface de démonstration
- Commander des puces NFC
- S'inscrire pour un essai
- Exploiter leur interface

**URL Production** : `https://site.labor-control.fr'

---

## Stack technique

### Frontend (Client)
- **Framework** : Blazor WebAssembly .NET 9.0
- **Language** : C# avec support nullable activé
- **UI Framework** : Tailwind CSS v3 (via CDN)
- **State Management** : Blazored.LocalStorage (v4.5.0)
- **Hosting** : Azure Static Web Apps

### Backend API
- **API principale** : https://laborcontrol-api.azurewebsites.net
- **Base de données** : PostgreSQL Azure (partagée avec l'application principale)

### Packages NuGet
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.9" />
<PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
```

---

## 🏗 Architecture

```
vitrine-fr/
│
├── Api/                          # Placeholder API (requis par Azure Static Web Apps)
│   └── package.json
│
├── Client/                       # Application Blazor WebAssembly
│   ├── Components/               # Composants réutilisables
│   ├── Models/                   # Modèles de données (DTOs)
│   ├── Pages/                    # Pages Razor (50+ pages)
│   │   ├── Index.razor          # Page d'accueil
│   │   ├── Login.razor          # Connexion
│   │   ├── Register.razor       # Inscription
│   │   ├── Dashboard.razor      # Tableau de bord
│   │   ├── Catalog.razor        # Catalogue puces
│   │   ├── Cart.razor           # Panier
│   │   └── ...                  # Autres pages
│   ├── Services/                # Services (AuthService, ApiService)
│   ├── Shared/                  # Composants partagés (Layout, NavMenu)
│   ├── wwwroot/                 # Ressources statiques
│   │   ├── css/                 # Styles personnalisés
│   │   ├── js/                  # Scripts JavaScript
│   │   ├── index.html          # Point d'entrée HTML
│   │   └── staticwebapp.config.json
│   ├── App.razor                # Composant racine
│   ├── Program.cs               # Point d'entrée application
│   └── LaborControl.Web.csproj
│
├── Legal/                        # Documents légaux
├── Models/                       # Modèles partagés
└── README.md                     # Ce fichier
```

---

## 🚀 Workflow de développement

**IMPORTANT** : Pas de tests locaux. Le workflow est :

```bash
# 1. Développer dans VS Code ou Visual Studio
# Modifier les fichiers .razor, .cs, etc.

# 2. Commit Git
git add .
git commit -m "Description des modifications"

# 3. Push vers GitHub
git push origin main

# 4. GitHub Actions déploie automatiquement sur Azure
# Attendre 2-5 minutes

# 5. Tester en ligne sur Azure
# https://laborcontrol-web.azurestaticapps.net
```

### Pas de `dotnet run` local
- ❌ Pas de développement local
- ❌ Pas de `dotnet watch run`
- ❌ Pas de tests en localhost

### Pourquoi ce workflow ?
- Environnement de production = environnement de test
- API Azure toujours utilisée
- Pas de configuration locale nécessaire
- Déploiement automatique via GitHub Actions

---

## 📁 Structure du projet

### Pages principales

| Page | Route | Description |
|------|-------|-------------|
| `Index.razor` | `/` | Page d'accueil vitrine |
| `Login.razor` | `/login` | Authentification utilisateur |
| `Register.razor` | `/register` | Inscription nouveau client |
| `Dashboard.razor` | `/dashboard` | Tableau de bord principal |
| `Sites.razor` | `/sites` | Gestion des sites industriels |
| `Equipment.razor` | `/equipment` | Gestion des équipements |
| `ControlPoints.razor` | `/controlpoints` | Points de contrôle |
| `TasksDashboard.razor` | `/tasksdashboard` | Planning des tâches |
| `Calendar.razor` | `/calendar` | Vue calendrier |
| `Catalog.razor` | `/catalog` | Catalogue puces NFC |
| `Cart.razor` | `/cart` | Panier d'achat |
| `Checkout.razor` | `/checkout` | Paiement |

### Services

#### `AuthService.cs`
Gère l'authentification et la gestion des tokens JWT :
- Login / Logout
- Enregistrement nouveaux clients
- Stockage token dans LocalStorage
- Vérification authentification

#### `ApiService.cs`
Service centralisant les appels API :
- Communication avec backend Azure
- Gestion headers authentification
- Endpoints pour toutes les entités (Sites, Zones, Assets, Tasks, etc.)

### Configuration Tailwind CSS

Tailwind est chargé via CDN dans `index.html` :

```html
<script src="https://cdn.tailwindcss.com?plugins=forms"></script>
```

**Configuration** :
```javascript
tailwind = { corePlugins: { preflight: false } }
```

Le preflight est désactivé pour éviter les conflits avec les styles Blazor par défaut.

### Composants notables

#### Notification système
JavaScript personnalisé dans `index.html` pour afficher des notifications centrées :

```javascript
window.showNotification = function(message) { ... }
```

Utilisé depuis C# via :
```csharp
await JSRuntime.InvokeVoidAsync("showNotification", "Message ici");
```

#### Auto-capitalisation
Script automatique qui met la première lettre en majuscule dans les champs de texte (sauf email, password, etc.)

---

## Configuration

### `Program.cs`

```csharp
// URL Backend API Azure (Production uniquement)
var apiBaseUrl = "https://laborcontrol-api.azurewebsites.net/";

builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri(apiBaseUrl)
});

// Services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>();
```

### `staticwebapp.config.json`

Configuration pour Azure Static Web Apps :

```json
{
  "navigationFallback": {
    "rewrite": "/index.html"
  },
  "responseOverrides": {
    "404": {
      "rewrite": "/index.html",
      "statusCode": 200
    }
  },
  "globalHeaders": {
    "cache-control": "no-cache, no-store, must-revalidate"
  }
}
```

**Important** :
- Toutes les routes 404 redirigent vers `/index.html` (routing SPA)
- Cache désactivé pour forcer le rechargement des nouvelles versions
- Support WASM et fichiers .NET

---

## Déploiement

### Déploiement automatique via GitHub Actions

Le déploiement se fait **automatiquement** lors d'un push sur la branche principale.

#### Workflow
1. Push du code sur GitHub
2. GitHub Actions détecte le push
3. Build du projet Blazor (.NET 9)
4. Publication vers Azure Static Web Apps
5. Disponible sur https://laborcontrol-web.azurestaticapps.net (2-5 minutes)

#### Fichier GitHub Actions
Le fichier `.github/workflows/azure-static-web-apps-*.yml` gère tout automatiquement.

### Variables d'environnement Azure

Aucune variable d'environnement n'est nécessaire car :
- L'API backend est codée en dur : `https://laborcontrol-api.azurewebsites.net`
- Pas de secrets côté frontend (tout est dans l'API)

---

## URLs importantes

| Environnement | URL | Description |
|---------------|-----|-------------|
| **Frontend Prod** | https://laborcontrol-web.azurestaticapps.net | Site vitrine production |
| **Backend API** | https://laborcontrol-api.azurewebsites.net | API REST Labor Control |
| **Base de données** | Azure PostgreSQL | (accès restreint backend uniquement) |

---

##  Fonctionnalités

###  Site vitrine
- Page d'accueil présentant Labor Control
- Sections : Problème, Solution, Fonctionnalités, Pricing
- Design moderne avec Tailwind CSS
- Responsive mobile-first

###  Authentification
- Inscription nouveau client (formulaire professionnel complet)
- Connexion avec email/password
- JWT stocké dans LocalStorage
- Auto-logout si token expiré

###  Gestion multi-tenant
- Chaque client a ses propres Sites > Zones > Équipements > Points de contrôle
- Isolation complète des données par `CustomerId`

###  Planning et tâches
- Dashboard temps réel des tâches
- Création tâches ponctuelles ou récurrentes
- Affectation techniciens
- Gestion des alertes
- Vue calendrier interactive

###  E-commerce puces NFC
- Catalogue de puces RFID/NFC
- Panier avec calcul prix dynamique
- Workflow de commande complet
- Historique des commandes

###  Gestion d'équipe
- Création utilisateurs (Admin, Superviseur, Technicien)
- Équipes et affectations
- Gestion des qualifications

###  Rapports et statistiques
- Tableau de bord avec KPIs temps réel
- Statistiques par site/zone/équipement
- Export rapports (fonctionnalité à venir)

---

## Développement

### Conventions de code
- **Naming** : PascalCase pour classes/méthodes, camelCase pour variables privées
- **Async/Await** : Toujours utiliser `async Task` pour appels API
- **Nullable** : Activé, utiliser `?` pour types nullable
- **Routing** : `@page "/route"` en haut de chaque page Razor

### Bonnes pratiques
1. **State Management** : Utiliser LocalStorage pour données persistantes locales
2. **API Calls** : Toujours passer par `ApiService`
3. **Error Handling** : Try/catch autour des appels API avec feedback utilisateur
4. **Loading States** : Afficher un spinner pendant chargement données
5. **Validation** : Valider côté client ET serveur

### Ajout d'une nouvelle page

1. Créer le fichier dans `Client/Pages/`
```razor
@page "/ma-nouvelle-page"

<h1>Ma Nouvelle Page</h1>

@code {
    protected override async Task OnInitializedAsync()
    {
        // Initialisation
    }
}
```

2. Ajouter le lien dans le menu si nécessaire (`Client/Shared/NavMenu.razor`)

3. Commit et push → Test en ligne

---

## Débogage

### Logs Azure
- Consulter les logs de déploiement dans GitHub Actions
- Vérifier le statut du déploiement dans Azure Portal

### Logs navigateur
Ouvrir la console développeur (F12) pour voir :
- Erreurs JavaScript
- Appels réseau (onglet Network)
- Logs Blazor

### Erreurs courantes

#### 1. **401 Unauthorized**
- Token JWT expiré ou invalide
- Solution : Se reconnecter sur le site

#### 2. **404 Not Found (API)**
- Backend Azure inaccessible
- Vérifier URL API dans `Program.cs`
- Vérifier que le backend Azure est en ligne

#### 3. **Tailwind classes ne s'appliquent pas**
- CDN Tailwind non chargé
- Vérifier connexion internet
- Vérifier `<script>` dans `index.html`

#### 4. **Déploiement échoue**
- Vérifier les logs GitHub Actions
- Vérifier les erreurs de compilation .NET
- Vérifier les packages NuGet

---

## Notes importantes

### Environnement de production
- **Tout se teste en production** : Pas d'environnement local
- L'API Azure est toujours utilisée
- Les modifications backend nécessitent un déploiement Azure séparé

### Performance
- Blazor WebAssembly charge ~2-3 MB au premier chargement (framework .NET)
- Cache navigateur améliore les chargements suivants
- Tailwind CSS via CDN = ~40 KB

### Sécurité
- Pas de secrets dans le code frontend (tout est public)
- Tokens JWT stockés en LocalStorage
- CORS géré côté backend Azure
- Isolation multi-tenant via `CustomerId`

---

## Support

Pour toute question ou problème :
1. Vérifier les logs GitHub Actions pour le déploiement
2. Vérifier les logs dans la console développeur du navigateur
3. Consulter les issues GitHub du projet
4. Contacter l'équipe de développement

---

## Licence

© 2025 Labor Control - Tous droits réservés

---

**Dernière mise à jour** : 23 octobre 2025
