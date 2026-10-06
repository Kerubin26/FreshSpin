# FreshSpin - Authentication + Role-Based Access

This finished version adds JWT authentication and server-side authorization to the original FreshSpin API.

## Roles

- **Admin**: view all bookings, assign riders, update status, view audit logs.
- **Staff**: view all bookings and update status.
- **Customer**: register/login, create bookings, and view only their own bookings.
- Rider login is intentionally not included yet.

## Demo accounts

- Admin: `admin` / `Admin123!`
- Staff: `staff` / `Staff123!`
- Customer: `customer` / `Customer123!`

These are classroom/demo credentials. Change them before deploying.

## Run

1. Install .NET 8 SDK.
2. Open this folder in VS Code or Visual Studio.
3. Run:
   `dotnet restore`
4. Then:
   `dotnet run`
5. Open:
   `http://localhost:5096`

The frontend is served by the same ASP.NET Core project.

## Important

The project still uses EF Core's **InMemory** database to remain compatible with the original backend. Data and newly registered users reset whenever the API restarts.

The JWT signing key in `appsettings.json` is a development key. For production, put a strong secret in environment variables or a secrets manager instead of committing it to source control.

## Automatic Pricing

The customer no longer enters the total price manually.

- Wash & Fold: PHP 50/kg
- Dry Cleaning: PHP 100/kg
- Steam Iron: PHP 30/kg

The browser displays the estimated total instantly as the weight/service changes.
For security, the ASP.NET backend independently recalculates the total and ignores client pricing.

## Updated Recommended Pricing

### Wash & Fold
PHP 35 per kilogram.

### Dry Cleaning (base price per garment)
- Dress Shirt / Polo: PHP 250
- Pants / Slacks: PHP 255
- Suit Jacket / Blazer: PHP 335
- Two-Piece Suit: PHP 590
- Barong Tagalog (Jusi/Pina): PHP 250
- Casual Dress: PHP 300
- Winter / Wool Coat: PHP 455
- Evening Gown: PHP 800
- Wedding Gown: PHP 1,800

Dry cleaning is calculated by garment type x quantity, not by kilograms.
The listed base/minimum price from the provided ranges is used for predictable automatic checkout.

### Steam Iron
PHP 25 per kilogram.

## Server-Side Login Lockout

The 3-attempt / 30-second lockout is now enforced by the ASP.NET backend for known user accounts.
Refreshing the browser, clearing sessionStorage, or opening another browser does not bypass the server lockout.

The frontend still shows a countdown for user experience, but the backend is authoritative and returns HTTP 429 while the account is locked.

Because this project still uses EF Core InMemory, restarting the ASP.NET server resets the lockout and all other in-memory data. A persistent SQL database is required for lockout persistence across server restarts.


## Clean Windows Build Package

This ZIP intentionally does not contain `bin`, `obj`, or prebuilt FreshSpinApi.dll files.
Your computer creates FreshSpinApi.dll locally when you run the project.

Recommended first run:
1. Extract the ZIP.
2. Open the FreshSpin-Finished folder.
3. Right-click FIRST_RUN.ps1 and choose Run with PowerShell, or run it from a PowerShell terminal.
4. If PowerShell script execution is restricted, double-click RUN_FRESHSPIN.cmd instead.

If Windows organizational Application Control still blocks a DLL that was compiled locally, that policy is controlled by Windows/your administrator and cannot be removed by changing FreshSpin source code.
