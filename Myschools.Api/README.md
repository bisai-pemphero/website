# Myschools API

This is the .NET 8 ASP.NET Core API migration surface for the legacy Web Forms application. It keeps the existing SQL Server schema and ADO.NET-style SQL operations; the Web Forms site remains available while clients move to HTTP APIs.

## Run

Set the connection string without committing credentials:

```powershell
$env:ConnectionStrings__Myschools = 'Data Source=...;Initial Catalog=MyschoolsSMS;User ID=...;Password=...;TrustServerCertificate=True'
$env:Jwt__Key = 'a-long-random-secret-at-least-32-characters'
dotnet run --project .\Myschools.Api
```

The login endpoint preserves the legacy TripleDES password transformation used by `CommonPages/Login.aspx.cs`:

`POST /api/auth/login`

Swagger UI is available at `/swagger/index.html`. Call the login endpoint, copy the returned `accessToken`, click **Authorize**, and enter `Bearer <accessToken>` to test protected endpoints.

The returned bearer token is used with the protected resource endpoints:

`GET /api/users`, `GET /api/schools`, `GET /api/students`, `GET /api/invoices`, `GET /api/payments`, `GET /api/fees/outstanding`, and `GET /api/dashboard`.

Additional workflows include `POST /api/student-registration`, `PUT|DELETE /api/student-management/{studentId}`, `POST /api/class-fee-invoices`, `POST /api/fee-payments`, `GET /api/receipts/{transactionId}`, `GET|POST|PUT /api/teacher-assignments`, `GET /api/grades/roster`, `POST /api/grades`, `GET /api/exam-results`, `GET /api/report-cards/marks`, `POST /api/sms`, `GET /api/licences`, and `POST /api/licences/{schoolId}`.

Reporting and receipt data are available from `GET /api/reports/payments`, `GET /api/reports/payments/by-category`, `GET /api/reports/payments/by-mode`, `GET /api/reports/payments/by-class`, `GET /api/reports/payments/total`, and `GET /api/bulk-receipts`. These return JSON so clients can render or print them as needed.

Browser origins are configured under `Cors:Origins`; mobile clients only need the bearer token.
