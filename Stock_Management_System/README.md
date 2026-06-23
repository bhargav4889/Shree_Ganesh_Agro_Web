# Stock Management System - Shree Ganesh Agro Industries

A premium, feature-rich web application designed for **Shree Ganesh Agro Industries** to manage agricultural stocks, sales, invoices, client ledger balances, and bank transactions. 

The application serves as a highly responsive client-side interface that communicates asynchronously with a secure external REST API backend database. The backend services code repository is hosted at [Shree_Ganesh_Agro_API](https://github.com/bhargav4889/Shree_Ganesh_Agro_API).

---

## 🧭 Project Features & Functionality (Simple Explanation)

Below is an overview of what the application does and how it works, explained in simple, everyday terms.

### 1. Dashboard Command Center
* **What it is:** The main overview page that you see as soon as you log in.
* **How it helps:** It summarizes key numbers at a glance (such as total stock purchases, sales, and active clients) and alerts the manager about overdue payments, upcoming task reminders, and recent system actions.

### 2. Stock Management (Buying Inventory)
* **What it is:** A portal to record raw grain stock purchased from suppliers.
* **How it helps:** You can enter the purchase date, grain types, bag counts, and net weight. It also records transport details (vehicle license numbers) and the operating weighbridge operator.
* **Smart Feature:** If a supplier is new, the manager can create their profile directly inside the stock form without having to leave the page.

### 3. Customer Directory & Running Ledgers
* **What it is:** A digital directory of all buyers and suppliers (referred to as "parties").
* **How it helps:** It classifies profiles to ensure suppliers only show up in purchase forms and buyers in sales forms.
* **Ledger Engine:** Every purchase and sale automatically updates the customer's chronological ledger. You can view their full history of transactions and download clean account statements in PDF or Excel sheets at any time.

### 4. Sales & Orders (Selling Inventory)
* **What it is:** A page to record sales made to buyers.
* **How it helps:** To save time, typing a buyer's name triggers a smart autocomplete suggestion, pulling their contact information instantly.
* **Calculations:** The system dynamically calculates the total price and bags weight based on the input rate as the operator types.

### 5. Bilingual PDF Invoices
* **What it is:** A professional billing engine.
* **How it helps:** It programmatically generates bills in PDF format.
* **Key Features:** It supports regional languages, rendering grain names in Gujarati alongside English billing labels, embeds the Indian Rupee symbol (₹) correctly, and includes a preview cache so users can review the document in the browser before printing.

### 6. Payment States & Financial Locks
* **What it is:** A system to track paid, unpaid, and partially paid balances.
* **How it works:** It categorizes transactions into **Pending** (no payment made), **Remain** (partial payment logged, carrying an outstanding balance), and **Paid** (fully cleared invoice).
* **Audit Protection:** To prevent fraud or ledger tampering, once a payment is saved, it is locked. To correct a mistake, the payment record must be deleted (which resets the state to Pending) and re-entered with correct values.

### 7. Task Reminders
* **What it is:** A scheduling calendar for the management team.
* **How it helps:** The manager can create reminders with a specific title, description, and target time. Active reminders automatically pop up as warnings on the dashboard.

### 8. Bank Details Configuration
* **What it is:** A list of corporate bank profiles (Account numbers, IFSC, branch names).
* **How it helps:** Stored bank details link directly to payment forms, auto-mapping accounts with official bank logos in dropdown selectors for clean UI visuals.

### 9. Security & URL Protection
* **What it is:** Built-in safeguards to protect sensitive business data.
* **How it works:** 
  * It encrypts database IDs inside URLs to prevent query tampering.
  * It logs out inactive users and uses a no-cache filter, making it impossible for someone to hit the back button and view private dashboard info after logging out.
  * It requires JWT security tokens to authorize every database action.

---

## 📁 Project Directory Structure

The project is organized in a modular structure using **Areas** to separate different business features:

```
Stock_Management_System/
│
├── 📂 API_Services/          # Service classes that route HTTP requests to the REST API database
├── 📂 All_DropDowns/         # Parameter models for dropdown listings (Products, Vehicles, Operators)
├── 📂 Areas/                 # Modular functional components of the application
│   ├── 📂 Accounts/          # Customer profiles directory, ledger calculations, and account exports
│   ├── 📂 Information/       # Stored corporate bank accounts configuration details
│   ├── 📂 Invoices/          # Billing layouts, bilingual PDF generation, and preview cache maps
│   ├── 📂 Manage/            # Auth/Login, Dashboard widgets, Payment state logs, and Reminders
│   ├── 📂 Sales/             # Sales transactions logging, autocomplete searches, and data sheets
│   └── 📂 Stocks/            # Stock registrations, inventory logs, and inline supplier setup
│
├── 📂 BAL/                   # Business Access Layer (Access checks & User Session helper models)
├── 📂 Controllers/           # Default page routing (HomeController, public Tour showcase page)
├── 📂 Email_Services/        # SMTP mail services for transactional password recovery links
├── 📂 Models/                # Shared data transfer objects (DTOs)
├── 📂 Services/              # Custom attributes (Cache-control attributes for logout protection)
├── 📂 UrlEncryption/         # AES-128 security helper class for URL parameter safeguarding
├── 📂 Views/                 # Layout wrappers and shared pages (Login Layout, Sidebar wrapper)
├── 📂 wwwroot/               # Static assets (custom CSS files, JS files, images, third-party plug-ins)
│
├── 📝 Program.cs             # Application configuration, dependency injection, and routing pipeline
├── 📝 appsettings.json       # App configuration settings (SMTP settings, logging levels)
└── 📝 Stock_Management_System.sln
```

---

## 💻 Code Standards & Design Architecture

The codebase follows clean software engineering principles, ensuring that the application remains maintainable, scalable, and secure.

### 1. MVC Architecture (Model-View-Controller)
The project strictly enforces separation of concerns:
* **Models:** Strongly typed classes mapping data structures.
* **Views:** Dynamic HTML/Razor views that render the user interface.
* **Controllers:** Coordinate input requests, call backend services, and return views or JSON results.

### 2. Asynchronous API Client Service
* The presentation layer is fully decoupled from data storage.
* All data requests are channeled asynchronously through a centralized service helper class ([Api_Service.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/API_Services/Api_Service.cs)) using C#'s `async/await` keywords.
* Outgoing requests automatically inherit and inject active JWT Bearer authentication headers from the user session.

### 3. Coding Style & Naming Conventions
* **Classes & Interfaces:** PascalCase (e.g. `AccountController`, `IEmailSender`).
* **Variables & Method Arguments:** camelCase (e.g. `information_Model`, `returnTo`).
* **Regions:** Group related functionalities (e.g., `#region Section: Create Customer` ... `#endregion`) to maintain readable files.
* **Strong Typing:** Models utilize validation data annotations (such as `[Required]`, `[EmailAddress]`) to ensure database sanitization before form submissions.

### 4. Custom Filters & Middleware
* **Authorization checks:** Protected controller methods utilize a custom `[CheckAccess]` filter attribute, checking session states and automatically handling unauthorized redirections.
* **Browser Caching Prevention:** Utilizes a custom `[PreventBackButtonAttribute]` that overrides HTTP response headers (`Cache-Control`, `Expires`, `Pragma`) to disable browser back-button history caches for secure workspaces.

### 5. Query Parameter Encryption
* Database primary keys are never exposed in URL strings.
* Identifiers are obfuscated into URL-safe Base64 strings using AES-128 encryption through [UrlEncryptor.cs](file:///e:/DOTNET/Stock_Management_System/Stock_Management_System/UrlEncryption/UrlEncryptor.cs) before rendering links.

### 6. Dynamic Script Optimization
* Utilizes the `WebOptimizer` pipeline in `Program.cs` to bundle and minify JavaScript and stylesheet files dynamically at startup, optimizing client-side delivery.
