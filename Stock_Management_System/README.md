# Stock Management System - Shree Ganesh Agro Industries

A premium, feature-rich ASP.NET Core MVC Web Application designed for **Shree Ganesh Agro Industries** to manage agricultural stocks, sales, invoices, client ledger balances, and bank transactions. 

This project is built using a decoupled architecture, serving as a highly responsive client-side frontend that communicates with an external secure REST API backend.

---

## 🏗️ Project Architecture & Tech Stack

This project is structured as a standard ASP.NET Core MVC application utilizing **Areas** to enforce domain boundaries and keep the codebase modular, clean, and easily maintainable.

### Backend Communications
The application is entirely decoupled from the database. It communicates with a remote secure REST API hosted at:
`https://stock-manage-api-shree-ganesh-agro-ind.somee.com/api`

Data fetching, posting, updating, and deleting are handled asynchronously through a centralized service class: [Api_Service.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/API_Services/Api_Service.cs).

### Core Technology Stack
* **Framework:** ASP.NET Core (MVC Pattern)
* **Optimization:** [WebOptimizer](https://github.com/ligershark/WebOptimizer) for dynamic JavaScript minification and bundling at runtime
* **PDF Rendering:** [iTextSharp](https://github.com/itext/itextsharp) for custom PDF invoice creation, bilingual text rendering, and watermark designs
* **Excel Reporting:** [ClosedXML](https://github.com/ClosedXML/ClosedXML) for generating standard spreadsheets
* **JSON Serialization:** Newtonsoft.Json
* **Security:** AES-based URL Parameter Encryption (`UrlEncryptor`), JWT Session-based Authorization, and HTTP header Cache-Control prevention.
* **Emailing:** SMTP Client configured for transactional mail (Password recovery)

---

## 📁 Codebase Directory Structure

```
Stock_Management_System/
│
├── 📂 API_Services/          # Helper class for centralizing HTTP API requests 
├── 📂 All_DropDowns/         # Centralized model/class to manage lists of dropdown values (Products, Vehicles, Grades)
├── 📂 Areas/                 # Main business modules grouped by Areas
│   ├── 📂 Accounts/          # Customer profile creation, updating, accounts ledger & PDF/Excel summaries
│   ├── 📂 Information/       # Bank information configurations of company/partners
│   ├── 📂 Invoices/          # Purchase and Sales invoice creation, previewing, and PDF layout rendering
│   ├── 📂 Manage/            # Auth, Dashboard summary, Payments tracking, and Reminders
│   ├── 📂 Sales/             # Sales transactions, seller accounts integration, and exports
│   └── 📂 Stocks/            # Purchased stocks tracking, supplier association, and reporting
│
├── 📂 BAL/                   # Business Access Layer (Access filters, Common Session Variables)
├── 📂 Controllers/           # Application-level Controllers (Home, Index, etc.)
├── 📂 Email_Services/        # Email-sending services & SMTP integrations
├── 📂 Models/                # General shared models
├── 📂 Services/              # Custom attributes (e.g., Preventing browser back caching)
├── 📂 UrlEncryption/         # Security helper for encrypting numerical identifiers in URL parameters
├── 📂 Views/                 # Layouts and shared views (Login Layout, Main Dashboard Layout)
├── 📂 wwwroot/               # Static assets (custom JS scripts, CSS styles, images, plugins)
│
├── 📝 Program.cs             # Application configuration, dependency injection, and request pipeline pipeline
├── 📝 appsettings.json       # App configuration settings (SMTP details, logging)
└── 📝 Stock_Management_System.csproj
```

---

## 🛠️ Detailed Step-by-Step Feature Walkthrough

### Step 1: Authentication & Security
* **JWT Session-Based Auth:** Users authenticate via the `/Auth/Login` endpoint. On success, the API returns a JWT Bearer Token, which the application stores in the session (`JWT_Token`) along with user metadata (`Auth_ID`, `Auth_Name`, `Auth_Email`, `Auth_Phone`). Every outgoing API request is injected with a `Bearer <token>` Authorization Header.
* **Access Filtering:** Pages are protected with a custom filter [CheckAccess.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/BAL/CheckAccess.cs). If a session expires or is missing, users are redirected back to the Login page with a `returnTo` URL parameter to resume where they left off.
* **Cache Control:** The application uses [PreventBackButtonAttribute.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/Services/PreventBackButtonAttribute.cs) and `OnResultExecuting` filters to append HTTP response headers (`no-cache`, `no-store`). This ensures that after logging out, clicking the browser's back button will not display cached dashboard data.
* **URL Parameter Tampering Defense:** ID parameters in URLs are encrypted using AES-128 via the static [UrlEncryptor.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/UrlEncryption/UrlEncryptor.cs) class. Encrypted values use a URL-safe Base64 character set (replacing `+` with `$`, `/` with `_`, and removing trailing `=`).
* **Credentials Recovery:** Contains a complete password reset and recovery loop. The system checks email registration, issues temporary reset tokens (using an SMTP service), and validates tokens to securely load the Reset Password page.

### Step 2: Dashboard Overview & Metrics
* Located inside `Areas/Manage/Views/Manage/Dashboard.cshtml`, this is the command center of the system.
* Displays dynamic count metrics fetched from `/Counts` API endpoint (e.g., totals of stocks, sales, clients).
* Integrates three distinct widgets:
  1. **Ledger Overdue Tracking:** Lists customers with pending payments.
  2. **Audit Trails:** Displays an activity feed of recent operations.
  3. **Task Reminders:** Showcases upcoming schedules and deadlines.

### Step 3: Stock Management
* Managed within `Areas/Stocks/Controllers/StockController.cs`.
* Supports full CRUD operations to track purchased agricultural inventory.
* **Integrated Supplier Addition:** While adding new purchase stock, if a supplier (customer) doesn't exist, the UI allows adding the supplier inline dynamically without breaking the data entry flow.
* **Dropdown Automation:** Dynamically loads grades, products (supports Gujarati and English labels), and vehicles for the transportation details.
* **Exports:** Downloadable summary reports of the stock inventory in PDF and Excel formats.

### Step 4: Sales Management
* Located under `Areas/Sales/Controllers/SaleController.cs`.
* Tracks the selling transactions of agricultural stocks to customers/buyers.
* Employs an autocomplete textbox feature (`GetSellerCustomerData`) to check if a buyer already exists, loading their profile properties instantly.
* Saves transactions and prints sales ledger summaries to Excel and PDF formats.

### Step 5: Advanced Invoicing & Bilingual PDF Layouts
* Created via `Areas/Invoices/Controllers/InvoiceController.cs`.
* **Dynamic Preview:** Invoices are cached in the HTTP session with a unique GUID token so users can preview the rendered format before saving or printing.
* **Custom PDF Generation:** The invoice PDF layout is programmatically generated using the `iTextSharp` library. Key layout details include:
  * **Bilingual Support:** Embeds a local Gujarati Unicode font file (`NotoSansGujarati.ttf`) to print product names in Gujarati.
  * **Indian Currency Symbol:** Embeds a custom symbol font (`Rupee.ttf`) to print the `₹` symbol correctly.
  * **Watermarking:** Automatically scales and renders a watermarked background logo onto the PDF canvas.
  * **Dynamic Layouts:** Generates formatted lines, headers, tables, totals, signatures, and contact numbers.

### Step 6: Payment Tracking & States
* Controlled by `Areas/Manage/Controllers/PaymentController.cs`.
* Classifies payments into three distinct categories:
  1. **Paid Payments:** Accounts with fully cleared invoice balances.
  2. **Pending Payments:** Sales/purchases awaiting payment processing.
  3. **Remaining Payments:** Partials where a user made a partial deposit but carries a remaining balance.
* Payment logs support partial view modals (`_PaymentInfo_Box`, `_RemainPaymentInfo_Box`) for inline updates, along with dedicated Excel and PDF summary sheets.

### Step 7: Bank & Company Information
* Created in `Areas/Information/Controllers/InformationController.cs`.
* Allows administrators to configure and save bank details (Account Numbers, IFSC Codes, Branch names) for easy retrieval during transaction recording.
* Appends official bank logos dynamically to enhance UI representation in select dropdown lists.

### Step 8: Scheduler & Reminders
* Administered by `Areas/Manage/Controllers/ReminderController.cs`.
* Allows creating task alerts, specifying titles, description summaries, target times, and active flags.
* Integrated with the dashboard so upcoming reminders pop up on the manager's screen upon logging in.

---

## ⚙️ Configuration & Setup

### 1. SMTP Email Credentials
To ensure password recovery emails function correctly, edit [appsettings.json](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/appsettings.json) and enter your Gmail App Password credentials:
```json
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "EnableSSL": "true",
  "UserName": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

### 2. Run the Application
Make sure you have the .NET SDK installed (v6.0 or higher recommended). Execute the following commands in the workspace root directory:

```bash
# Restore package dependencies
dotnet restore

# Build the project
dotnet build

# Run the web application locally
dotnet run
```
Once started, the application will bind to the configured local port (typically `https://localhost:7148` or `http://localhost:5148`), which you can inspect in [launchSettings.json](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/Properties/launchSettings.json).
