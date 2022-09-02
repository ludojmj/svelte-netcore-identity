# All-In-One CRUD

```bash
git clone https://github.com/ludojmj/svelte-netcore-identity.git
```

The aim of this project is to gather, in a single place, useful front and back ends development tools:

- A Database with SQLite;
- A Web API server with .NET Core 6.x;
- A Svelte JS client App;
- A link to an external service for identity management, authorization, and API security.

## Quick start (Development)

Server

```bash
cd <myfolder>/Server
dotnet run
```

Client

```bash
cd <myfolder>/client
npm install
npm run dev
```

---

## Inspiration

- SQLite database powered by: <https://www.sqlite.org>
- Server based on API mechanisms of: <https://reqres.in/api/whatever>
- Svelte template client borrowed from: <https://github.com/sveltejs/template.git>
- Identity service powered by: <https://demo.duendesoftware.com>
- Identity client borrowed from: <https://github.com/dopry/svelte-oidc>
- CSS borrowed from: <https://getbootstrap.com>
- SVG borrowed from: <https://creativecommons.org>

---

## Manufacturing process steps

### >>>>> SQLite database

#### Overwrite database if needed

```bash
cd <myfolder>
sqlite3 Server/App_Data/stuff.db < Server/App_Data/create_tables.sql
```

### >>>>> .NET Core 6.x Web API server

#### Create the server project

```bash
cd <myfolder>
dotnet new gitignore
dotnet new webapi -n Server
dotnet new xunit -n Server.UnitTest
```

#### Generate the model from the database for the Web API server

```bash
dotnet tool install --global dotnet-ef
cd <myfolder>/Server
dotnet ef dbcontext scaffold "Data Source=App_Data/stuff.db" Microsoft.EntityFrameworkCore.Sqlite \
--output-dir DbModels --context-dir DbModels --context StuffDbContext --force
```

#### Run the tests

```bash
cd <myfolder>/Server.UnitTest
dotnet restore
dotnet build
dotnet test
dotnet test /p:CollectCoverage=true
```

#### Run the Web API server

```bash
cd <myfolder>/Server
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### >>>>> Svelte client App

#### Create the client project

```bash
cd <myfolder>
npm create vite@latest client -- --template svelte
```

#### Run the client App

```bash
cd <myfolder>/client
npm install
npm run dev
```

#### In development, you can either run a standalone version of the client App (mocking)...

File to edit:

 > ```<myfolder>/client/.env.development```

```bash
VITE_REDIRECT_URI="http://localhost:5173"
VITE_API_URL="http://localhost:5173/mock/stuff"
```

#### ...Or call the .NET API hosted locally with the client App...

File to edit:

 > ```<myfolder>/client/.env.development```

```bash
VITE_REDIRECT_URI="hhttps://localhost:5001"
VITE_API_URL="https://localhost:5001/api/stuff"
```

#### ...And in Production (client and API will be hosted together)...

File to edit:

 > ```<myfolder>/client/.env.production```

```bash
VITE_REDIRECT_URI="<your server URI>"
VITE_API_URL="<your server URI>/api/stuff"
```

---

## Troubleshooting

### _An error occured. Please try again later._

**When?**

- Creating a record in the SQLite database _stuff.db_ running Linux on Azure;
- The "real" error (not displayed in Production) is: _SQLite Error 5: 'database is locked'_;
- There is a restricted write access to the file on Linux web app when running on Azure.

**How to solve:**

- ==> Either use a real database or deploy the web app on Azure choosing Windows OS.

### _SQLite Error 1: 'no such table: t_stuff'_

**When?**

- Running the Svelte client App (```npm run dev```);
- Connecting to: <http://localhost:5173/>.

**How to solve:**

- ==> Create the database _stuff.db_ (```sqlite3 Server/App_Data/stuff.db < Server/App_Data/create_tables.sql```).

### _Network Error_

**When?**

- Running the Svelte client App (```npm run dev```);
- Connecting to: <http://localhost:5173/>.

**How to solve:**

- ==> Start the .NET server (```dotnet run```) before the Svelte client App (```npm run dev```).

### _Your connection is not private_ (NET::ERR_CERT_AUTHORITY_INVALID)

**When?**

- Running the .NET Core server (dotnet run);
- Connecting to: <http://localhost:5000/swagger>;
- Or connecting to its redirection: <https://localhost:5001/swagger>.

**How to solve:**

- ==> Click "Advanced settings" button;
- ==> Click on the link to continue to the assumed unsafe localhost site;
- ==> Accept self-signed localhost certificate.

### _You do not have permission to view this directory or page._

**When?**

- Browsing the web site on a Azure Windows instance.

**How to solve:**

- ==> Add the web.config file since you've got IIS running;
- ==> On Linux, the web.config file is useless
(Update your http headers according to the suitable Web Server configuration file).
