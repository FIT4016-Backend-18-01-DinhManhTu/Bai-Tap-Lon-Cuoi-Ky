# Blood Bank Manager - Setup & Installation Guide

## Prerequisites

Trước khi cài đặt Blood Bank Manager, đảm bảo bạn có các yêu cầu sau:

### 1. Software Requirements
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- **SQL Server 2019+** hoặc **SQL Server Express 2019+** - [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Git** (optional) - [Download](https://git-scm.com/)
- **Visual Studio Code** hoặc **Visual Studio** (optional, for development)

### 2. Verify Installation
```bash
# Check .NET version
dotnet --version

# Should return: 10.x.x or higher
```

---

## Installation Steps

### Step 1: Clone hoặc Download Project

```bash
# Option 1: Clone from repository (if available)
git clone https://github.com/your-repo-url/BloodBankManager.git
cd BloodBankManager

# Option 2: Extract from ZIP file
# Extract the project folder to desired location
cd BloodBankManager
```

### Step 2: Create SQL Server Database

#### Option A: Automatic (Recommended)
Hệ thống sẽ tự động tạo database khi ứng dụng chạy lần đầu.

```bash
dotnet run
```

#### Option B: Manual
Chạy script SQL Server:

1. Mở **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server
3. Open file: `Database_Setup.sql`
4. Execute the script
5. Verify database `BloodBankDB` was created

```sql
-- Check if database exists
SELECT name FROM sys.databases WHERE name = 'BloodBankDB'
```

### Step 3: Configure Connection String

Edit file `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

#### Connection String Examples:

**Local SQL Server (Windows Authentication):**
```
Server=.;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;
```

**Named SQL Server Instance:**
```
Server=COMPUTER_NAME\SQLEXPRESS;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;
```

**SQL Authentication:**
```
Server=.;Database=BloodBankDB;User Id=sa;Password=YourPassword;Encrypt=false;
```

**Remote SQL Server:**
```
Server=192.168.1.100;Database=BloodBankDB;User Id=sa;Password=YourPassword;Encrypt=false;
```

### Step 4: Restore Dependencies

```bash
dotnet restore
```

### Step 5: Build Project

```bash
dotnet build
```

### Step 6: Apply Database Migrations

```bash
# This will create all database tables
dotnet ef database update
```

If you get error about EF Core tools, install them:
```bash
dotnet tool install --global dotnet-ef
```

### Step 7: Run Application

```bash
dotnet run
```

**Output will show:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

---

## Verify Installation

### Test API with cURL

```bash
# Test 1: Get Blood Types
curl http://localhost:5000/api/bloodtypes

# Expected Response: [{"id":1,"typeName":"O_NEGATIVE",...}]

# Test 2: Get All Patients
curl http://localhost:5000/api/patients

# Expected Response: []
```

### Test with VS Code REST Client

1. Install extension: **REST Client** (humao.rest-client)
2. Open file: `BloodBankManager.http`
3. Click "Send Request" above each API endpoint

---

## Database Schema Overview

### Tables Created:

1. **BloodTypes** - Danh sách nhóm máu
2. **Bloods** - Những túi máu có sẵn
3. **Patients** - Thông tin bệnh nhân
4. **BloodTransfusions** - Lịch sử truyền máu
5. **BloodInventories** - Tồn kho máu theo nhóm
6. **BloodExpiryAlerts** - Cảnh báo hết hạn
7. **__EFMigrationsHistory** - EF Core migration tracking

---

## Troubleshooting

### Issue 1: "Couldn't connect to SQL Server"

**Solution:**
1. Verify SQL Server is running
   ```bash
   # Windows
   services.msc  # Look for "SQL Server (SQLEXPRESS)"
   ```

2. Check connection string in `appsettings.json`

3. Verify database exists:
   ```bash
   sqlcmd -S . -Q "SELECT @@VERSION"
   ```

### Issue 2: "EF Core migration errors"

**Solution:**
```bash
# Remove previous migrations
dotnet ef migrations remove

# Create fresh migrations
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update
```

### Issue 3: "Port 5000 already in use"

**Solution:**
```bash
# Change port in launchSettings.json:
# Change "applicationUrl": "https://localhost:5001;http://localhost:5000"

# Or find process using port:
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Issue 4: Build errors about namespaces

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build --configuration Release
```

---

## Running with Docker (Optional)

Create `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "BloodBankManager.dll"]
```

Build and run:
```bash
docker build -t bloodbank:latest .
docker run -p 5000:5000 -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=BloodBankDB;..." bloodbank:latest
```

---

## Development Workflow

### 1. Make Model Changes

```csharp
// Modify Models/Blood.cs
// Add new property
public int NewProperty { get; set; }
```

### 2. Create Migration

```bash
dotnet ef migrations add AddNewProperty
```

### 3. Review Migration

Edit `Migrations/[timestamp]_AddNewProperty.cs` if needed

### 4. Apply Migration

```bash
dotnet ef database update
```

---

## Deployment Preparation

### Create Release Build

```bash
dotnet publish --configuration Release --output ./publish
```

### Copy Required Files

```bash
# appsettings.json (update connection string)
# appsettings.Production.json (optional)
# bin/Release/net10.0/publish/*
```

### Environment Variables (Production)

```bash
# Set connection string via environment variable
$env:ConnectionStrings__DefaultConnection = "Server=prod-server;Database=BloodBankDB;..."

# Or use appsettings.Production.json
```

---

## Support & Help

For issues and questions:
- Check [README.md](README.md) for API documentation
- Review [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for endpoint details
- Check logs in console output
- Verify database connectivity

---

## Security Notes

⚠️ **Important for Production:**

1. **Never commit connection strings** with passwords
2. Use **environment variables** for sensitive data
3. Enable **HTTPS** in production
4. Use **SQL Authentication** with strong passwords
5. Implement **API authentication** (JWT, OAuth, etc.)
6. Set up **proper error handling** (don't expose internal details)
7. Run **security scans** before deployment

---

## Next Steps

After successful installation:

1. ✅ Create initial blood types (done automatically)
2. ✅ Add blood units to inventory
3. ✅ Register patients
4. ✅ Set up expiry alerts
5. ✅ Perform transfusions with compatibility checks

Good luck! 🩸

