# Byway API — ASP.NET Core Web API (Onion Architecture)

Backend for the Byway e-learning platform, built per the finalized domain
model and business rules from the architecture review. No frontend code
here — pairs with the separately-built `byway-react` project.

> **Important — this was written without a local .NET SDK / NuGet access.**
> I was not able to run `dotnet build` or `dotnet ef` while writing this, so
> treat the first build as the real verification step. See "First build"
> below for the most likely rough edges.

## Stack

- ASP.NET Core 8 Web API, Onion Architecture (Domain / Application /
  Infrastructure / API — no MVC, per requirement)
- EF Core 8 + SQL Server
- ASP.NET Core Identity (real `Admin`/`Student` roles) + JWT bearer auth
- Swagger / OpenAPI

## Project structure

```
src/
  Byway.Domain/           Entities, enums — framework-free, no EF/Identity refs
  Byway.Application/      Interfaces, DTOs, use-case services (business rules live here)
  Byway.Infrastructure/    EF Core, Identity, repositories, JWT, file/email services
  Byway.API/               Controllers, middleware, Program.cs, appsettings
```

## First build — do this first

I could not run these commands myself (no .NET SDK / no NuGet access in my
environment), so this is genuinely untested. Start here:

```bash
cd Byway
dotnet new sln -n Byway
dotnet sln add src/Byway.Domain/Byway.Domain.csproj
dotnet sln add src/Byway.Application/Byway.Application.csproj
dotnet sln add src/Byway.Infrastructure/Byway.Infrastructure.csproj
dotnet sln add src/Byway.API/Byway.API.csproj

dotnet restore
dotnet build
```

If `dotnet build` reports errors, they are most likely one of:
- A NuGet package version bump needed (the versions in each `.csproj` were
  current as of my training data, not verified against today's NuGet feed).
- A missing `using` or small typo in one file — nothing here is
  architecturally complex, so these should be quick fixes.

## Database setup

1. Set a real `SeedAdmin:Password` in `appsettings.Development.json` (already
   scaffolded with a placeholder `Admin@12345` — change it). This file is
   gitignored, so it never reaches source control.
2. Set a real `Jwt:SigningKey` (32+ random characters) the same way.
3. Create the initial migration and database:

```bash
cd src/Byway.API
dotnet tool install --global dotnet-ef   # if you don't have it already
dotnet ef migrations add InitialCreate --project ../Byway.Infrastructure --startup-project .
dotnet ef database update --project ../Byway.Infrastructure --startup-project .
```

`Program.cs` also calls `EnsureCreatedAsync()` as a fallback for quick local
runs — but once you have real migrations, prefer `dotnet ef database update`
and consider removing the `EnsureCreatedAsync()` call (the two approaches
don't mix well long-term: `EnsureCreated` bypasses the migrations table).

4. Run it:

```bash
dotnet run --project src/Byway.API
```

Swagger UI opens at `/swagger` in Development. Log in as
`admin@byway.local` with whatever password you set in step 1 to get an
Admin JWT for the admin-only endpoints.

## What's implemented vs. flagged as an assumption

Everything in the finalized domain model and business rules from the
architecture review is implemented, including:
- Cart as an explicit entity (`Cart 1→* CartItem`), created lazily
- Course.Rating computed as `AVG(Review.Rating)` at query time — never stored
- One Review per `(UserId, CourseId)`, upsert instead of duplicate
- Course/Instructor edit-delete guards (only a **Paid** order blocks a course)
- Checkout as two commit points: `Pending` persisted first, then resolved to
  `Paid` (clears cart) or `Failed` (cart preserved) via a swappable
  `IPaymentSimulator`
- Explicit FK delete behavior per the architecture review (`Restrict` on
  Course→OrderItem and Course→Review, `Cascade` on Course→CartItem and
  Course→CourseContent, `Restrict` on Category/Instructor→Course)
- Decimal precision set explicitly on every money/rating column
- Bonus features: auto-login after registration, welcome email, purchase
  confirmation intent (see note below)

**Left as a deliberate gap, matching an unresolved item from the review:**
- The **"Cancelled" order status has no trigger anywhere in this code** —
  exactly as flagged in the architecture review. Nothing sets it. Don't wire
  it up without deciding what should trigger it first.
- **Purchase-confirmation email** is not sent from `CheckoutService` — only
  the registration welcome email is wired up. Wiring the purchase email in
  is a one-line addition (call `IEmailService.SendAsync` after `OrderStatus`
  resolves to `Paid`) once you want it; I left it out rather than guessing
  at whether you want it fired even on top of the 90%-success simulation.
- **Concurrency guard on double-checkout** (flagged as B4 in the review) is
  not implemented — nothing currently stops two near-simultaneous checkout
  requests from the same cart producing two Orders.

## Connecting the React frontend

Set `Cors:AllowedOrigins` in `appsettings.Development.json` to wherever
`byway-react`'s dev server runs (already set to `http://localhost:5173`).
The React app's `AuthContext`/`CartContext` from the earlier mockup would
need to be pointed at these real endpoints and switched from local state to
real `fetch`/`axios` calls — that wiring wasn't done as part of this backend
work.
