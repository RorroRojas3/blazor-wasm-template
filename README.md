# RR — Blazor WebAssembly + Entra ID Authentication

A .NET 10 Blazor WebAssembly application with **automatic Microsoft Entra ID authentication** and a **backend-driven permission system**.

## Authentication Flow

```
User opens app
    │
    ▼
App.razor ── AuthorizeRouteView checks auth state
    │
    ├── Authenticated? ──► PermissionState loads permissions from API
    │                           │
    │                           ▼
    │                       CascadingValue<UserPermissions>
    │                           │
    │                           ▼
    │                       Page renders with permission-gated UI
    │
    └── Not authenticated? ──► RedirectToLogin.razor
                                   │
                                   ▼
                               Entra ID login (redirect mode)
                                   │
                                   ▼
                               Authentication.razor handles callback
                                   │
                                   ▼
                               Redirect back to original page
```

If the user has an existing Entra SSO session (domain-joined machine), the redirect is near-instant.

## Project Structure

```
RR.UI/                          (Blazor WebAssembly)
├── Auth/
│   ├── PermissionEnums.cs      AppPermission, AppRole enums
│   ├── UserPermissions.cs      HashSet-based model with convenience methods
│   └── PermissionService.cs    Scoped service, fetches/caches from API
├── Pages/
│   ├── Authentication.razor    MSAL callback handler
│   ├── Home.razor              Welcome + permission summary
│   ├── Counter.razor           Sample counter page
│   ├── Weather.razor           Weather data from API
│   └── Permissions.razor       Debug dashboard
├── Shared/
│   ├── RedirectToLogin.razor   Auto-redirect to Entra login
│   ├── LoginDisplay.razor      User name + logout button
│   ├── RequirePermission.razor Permission-gating wrapper component
│   └── PermissionState.razor   Loads + cascades UserPermissions
├── Layout/
│   ├── MainLayout.razor        Wraps Body in PermissionState
│   └── NavMenu.razor           Navigation links
└── wwwroot/
    ├── appsettings.json        MSAL + API configuration
    └── index.html              Entry point with MSAL script

RR.Api/                         (ASP.NET Core Web API)
├── Auth/
│   └── PermissionEnums.cs      Shared enums (same as frontend)
├── Models/
│   └── UserPermissionData.cs   Response DTO with IReadOnlySet<T>
├── Services/
│   └── PermissionService.cs    Singleton with ConcurrentDictionary
└── Controllers/
    └── PermissionsController.cs  GET/POST permission endpoints
```

## Prerequisites

- .NET 10 SDK
- Two Entra ID app registrations (see below)

## Entra ID App Registration

### 1. API Registration (for `RR.Api`)

1. Go to **Azure Portal > Microsoft Entra ID > App registrations > New registration**
2. Name: `RR API` (or your preference)
3. Supported account types: **Single tenant**
4. After creation, go to **Expose an API**:
   - Set Application ID URI: `api://<API_CLIENT_ID>`
   - Add a scope: `access_as_user` (Admin and user consent)
5. Note the **Application (client) ID** — this is `YOUR_API_CLIENT_ID`
6. Note the **Directory (tenant) ID** — this is `YOUR_TENANT_ID`

### 2. SPA Registration (for `RR.UI`)

1. Create another app registration
2. Name: `RR UI` (or your preference)
3. Supported account types: **Single tenant**
4. Under **Authentication**:
   - Add platform: **Single-page application**
   - Redirect URIs: `https://localhost:7096/authentication/login-callback`
   - Add: `http://localhost:5054/authentication/login-callback`
5. Under **API permissions**:
   - Add permission > My APIs > select `RR API`
   - Select the `access_as_user` scope
   - Grant admin consent
6. Note the **Application (client) ID** — this is `YOUR_CLIENT_ID`

## Configuration

### `RR.UI/wwwroot/appsettings.json`

Replace the placeholders:

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ValidateAuthority": true
  },
  "ApiSettings": {
    "BaseUrl": "https://localhost:7111",
    "Scopes": ["api://YOUR_API_CLIENT_ID/access_as_user"]
  }
}
```

### `RR.Api/appsettings.json`

Replace the placeholders:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_API_CLIENT_ID",
    "Audience": "api://YOUR_API_CLIENT_ID"
  }
}
```

### Seed Data

The backend `PermissionService` includes sample users with Entra Object IDs `00000000-0000-0000-0000-000000000001` through `000...003`. Replace these with real Entra Object IDs from your tenant (found in Entra ID > Users > select user > Object ID).

## Running the Application

Start both projects simultaneously:

```bash
# Terminal 1 — Backend API
cd RR.Api
dotnet run

# Terminal 2 — Frontend
cd RR.UI
dotnet run
```

Or use the solution file:

```bash
dotnet run --project RR.Api &
dotnet run --project RR.UI
```

The frontend runs on `https://localhost:7096` and the API on `https://localhost:7111`.

## Permission System

### Three ways to check permissions in components

1. **`<RequirePermission>` wrapper** (preferred for inline UI):
   ```razor
   <RequirePermission Permission="AppPermission.ReportsExport">
       <button>Export Report</button>
   </RequirePermission>
   ```

2. **Cascading parameter** (for conditional logic in code):
   ```razor
   @code {
       [CascadingParameter]
       private UserPermissions? Perms { get; set; }
   }
   ```

3. **Inject service** (for async checks):
   ```razor
   @inject PermissionService Permissions
   ```

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/permissions` | Get current user's permissions |
| POST | `/api/permissions/{userId}/invalidate` | Remove user from cache |
| POST | `/api/permissions/reload` | Reload all from data store |
| POST | `/api/permissions/{userId}/reload` | Reload single user |
