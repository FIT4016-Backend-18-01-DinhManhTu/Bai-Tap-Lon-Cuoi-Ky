# Blood Bank Manager - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- ✅ .NET 10 SDK installed
- ✅ SQL Server 2019+ running
- ✅ Git (optional)

### Quick Setup

#### 1. Clone/Extract Project
```bash
cd BloodBankManager
```

#### 2. Configure Database (Windows Authentication)
Open `appsettings.json` - already configured for local SQL Server:
```json
"DefaultConnection": "Server=.;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;"
```

#### 3. Build & Run
```bash
dotnet restore
dotnet build
dotnet run
```

**Success message:**
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
```

---

## � User Authentication

### For Staff (Nhân Viên)
1. **Login:** `http://localhost:5000/login.htm`
2. **Register:** `http://localhost:5000/register.htm` → Select "👨‍💼 Nhân Viên"

### For Patients (Bệnh Nhân)
1. **Login:** `http://localhost:5000/patient-login.htm`
2. **Register:** `http://localhost:5000/patient-register.htm`
3. **Dashboard:** Auto-redirect after login to view personal medical info

### Test Patient Account
After registration, patients can:
- View personal information (name, blood type, medical history)
- See transfusion history
- Access medical records

---

## �📡 Test API in VS Code

### Install Extension
1. Open VS Code Extensions (Ctrl+Shift+X)
2. Search: "REST Client"
3. Install by Huachao Mao
4. Reload VS Code

### Run Requests
1. Open `BloodBankManager.http`
2. Click "Send Request" link above each API call
3. View response in right panel

### First Test Request
```bash
GET http://localhost:5000/api/bloodtypes
```

Expected: List of 8 blood types

---

## 🩸 Common API Examples

### 1. Add Blood Unit
```bash
curl -X POST http://localhost:5000/api/bloods \
  -H "Content-Type: application/json" \
  -d '{
    "unitNumber":"U001",
    "bloodTypeId":1,
    "collectionDate":"2026-03-01T10:00:00Z",
    "expiryDate":"2026-04-12T10:00:00Z",
    "volume":450,
    "donorName":"Nguyễn Văn A",
    "location":"Kho A1"
  }'
```

### 2. Register Patient
```bash
curl -X POST http://localhost:5000/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Trần Văn B",
    "patientCode":"BN001",
    "bloodTypeId":1,
    "dateOfBirth":"1990-01-01T00:00:00Z",
    "gender":"Nam",
    "hospital":"Bệnh viện Lồi",
    "ward":"Phòng 101",
    "admissionDate":"2026-03-12T00:00:00Z",
    "phoneNumber":"0987654321",
    "email":"patient@example.com",
    "medicalCondition":"Chảy máu"
  }'
```

### 3. Check Compatible Blood
```bash
curl http://localhost:5000/api/patients/1/compatible-bloods
```

### 4. Transfuse Blood (with auto-compatibility check)
```bash
curl -X POST http://localhost:5000/api/patients/1/transfuse \
  -H "Content-Type: application/json" \
  -d '{
    "bloodId":1,
    "quantity":200,
    "performedBy":"Bác sĩ Nguyễn",
    "notes":"Thành công"
  }'
```

### 5. Check Inventory
```bash
curl http://localhost:5000/api/bloodinventory
```

### 6. Get Expiring Blood (7 days)
```bash
curl http://localhost:5000/api/bloods/expiring/alert
```

### 7. Create Expiry Alerts
```bash
curl -X POST http://localhost:5000/api/bloods/create-expiry-alerts
```

---

## 📊 Database Features

### Automatic Features
- ✅ Blood types pre-populated (8 types)
- ✅ Database auto-created on first run
- ✅ All tables created automatically
- ✅ Indexes created for performance
- ✅ Expired blood auto-marked daily

### Manual Features (via API)
- Create blood units
- Register patients
- Perform transfusions
- Acknowledge alerts
- Update inventory thresholds

---

## 🔒 Blood Type Compatibility

**Quick Reference:**
```
O-  → Can give to ALL (Universal Donor)
AB+ → Can receive from ALL (Universal Recipient)

A≠B (ABO groups don't mix)
Rh-(negative) can only receive Rh-
Rh+(positive) can receive both Rh+ and Rh-
```

**Example:**
- Patient blood type: A+
- Can receive from: O-, O+, A-, A+
- Cannot receive from: B-, B+, AB-, AB+

---

## 📁 Project Structure

```
BloodBankManager/
├── Models/              # Entity classes (7 models)
├── Controllers/         # API endpoints (5 controllers)
├── Services/            # Business logic (2 services)
├── Data/               # Database context
├── Migrations/         # Database version control
├── appsettings.json    # Configuration
├── Program.cs          # App startup
├── README.md           # Documentation
└── BloodBankManager.http  # API test requests
```

---

## 🛠️ Development Tips

### Add New Field to Model
```csharp
// Models/Blood.cs
public string NewField { get; set; }
```

Create migration:
```bash
dotnet ef migrations add AddNewField
dotnet ef database update
```

### Debug API Request
```bash
# Enable detailed logging
# appsettings.json:
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

### Check Database
```bash
# SQL Server
sqlcmd -S . -d BloodBankDB -Q "SELECT * FROM BloodTypes"
```

---

## ❌ Troubleshooting

### Error: "Connection refused"
```
✓ Solution: Verify SQL Server running
  - Windows: Services > SQL Server (SQLEXPRESS)
  - Or start: net start MSSQLSERVER
```

### Error: "Database does not exist"
```
✓ Solution: Run migration
  dotnet ef database update
```

### Error: "Port 5000 in use"
```
✓ Solution: Use different port
  # launchSettings.json:
  "applicationUrl": "https://localhost:5001;http://localhost:5002"
```

### Error: "EF Core tools not found"
```
✓ Solution: Install tool
  dotnet tool install --global dotnet-ef
```

---

## 📚 Learn More

- **Full API Docs**: [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Setup Guide**: [SETUP_GUIDE.md](SETUP_GUIDE.md)
- **Business Rules**: [BUSINESS_RULES.md](BUSINESS_RULES.md)
- **ASP.NET Core**: [docs.microsoft.com](https://docs.microsoft.com/aspnet/core)
- **Entity Framework Core**: [ef.readthedocs.io](https://ef.readthedocs.io)

---

## 🎯 Next Steps

1. ✅ Start application: `dotnet run`
2. ✅ Test API: Open `BloodBankManager.http`
3. ✅ Read [README.md](README.md) for details
4. ✅ Check [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for all endpoints
5. ✅ Review [BUSINESS_RULES.md](BUSINESS_RULES.md) for system logic

---

## 💡 Pro Tips

- 🔍 Use `http://localhost:5000/api/bloodtypes` to check if API is running
- 📝 Test requests are in `BloodBankManager.http` (20+ examples)
- 💾 Database changes auto-apply on startup
- ⚡ FIFO logic ensures no wasted blood
- 🔔 7-day expiry alerts automatic
- ✔️ Blood compatibility checked automatically

---

## Support

Have questions? Check:
- ✓ README.md - Overview
- ✓ API_DOCUMENTATION.md - All endpoints
- ✓ SETUP_GUIDE.md - Installation
- ✓ BUSINESS_RULES.md - System logic
- ✓ BloodBankManager.http - Example requests

Happy coding! 🩸

