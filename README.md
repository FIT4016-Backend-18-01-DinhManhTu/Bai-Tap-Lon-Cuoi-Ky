# Blood Bank Manager - Hệ thống Quản lý Ngân hàng Máu

## 📋 Mô tả Hệ thống

Blood Bank Manager là một hệ thống ASP.NET Core Backend được thiết kế để quản lý ngân hàng máu hiệu quả, bao gồm:

- **Quản lý tồn kho máu**: Theo dõi các túi máu theo nhóm máu, ngày thu, ngày hết hạn
- **FIFO Logic**: Nhập trước xuất trước, đảm bảo máu được sử dụng đúng thứ tự
- **Expiry Tracking**: Cảnh báo máu sắp hết hạn trong 7 ngày
- **Blood Compatibility**: Kiểm tra tương thích nhóm máu tự động
- **Patient Management**: Quản lý bệnh nhân cần truyền máu
- **Transfusion Records**: Ghi chép toàn bộ quá trình truyền máu

## 🏗️ Kiến trúc Hệ thống

```
BloodBankManager/
├── Models/              # Entity Models
│   ├── BloodType.cs
│   ├── Blood.cs
│   ├── Patient.cs
│   ├── BloodTransfusion.cs
│   ├── BloodInventory.cs
│   └── BloodExpiryAlert.cs
├── Data/               # Database Context
│   └── BloodBankContext.cs
├── Services/           # Business Logic
│   ├── BloodCompatibilityService.cs
│   └── BloodExpiryService.cs
├── Controllers/        # API Endpoints
│   ├── BloodTypesController.cs
│   ├── BloodsController.cs
│   ├── PatientsController.cs
│   ├── BloodInventoryController.cs
│   └── BloodExpiryAlertsController.cs
├── Migrations/         # Database Migrations
├── Program.cs          # Main Configuration
└── appsettings.json    # Database Connection String
```

## 🗄️ Cấu hình Database

### SQL Server Connection String
Cập nhật file `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BloodBankDB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**Các tham số:**
- `Server`: Tên SQL Server (VD: `.` cho local, `COMPUTER_NAME` cho remote)
- `Database`: Tên cơ sở dữ liệu
- `Trusted_Connection`: `true` nếu dùng Windows Authentication, `false` nếu dùng SQL login

### Tạo Database

```bash
dotnet ef database update
```

## 🚀 Chạy Ứng dụng

```bash
# Build project
dotnet build

# Run application
dotnet run
```

Ứng dụng sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

## 📡 API Endpoints

### 1. Blood Types (Nhóm máu)

#### GET /api/bloodtypes
Lấy tất cả nhóm máu
```bash
curl https://localhost:5001/api/bloodtypes
```

**Response:**
```json
[
  {
    "id": 1,
    "typeName": "O_NEGATIVE",
    "description": "O âm tính - Nhóm máu phổ biến"
  }
]
```

### 2. Bloods (Túi máu)

#### POST /api/bloods
Thêm túi máu mới
```bash
curl -X POST https://localhost:5001/api/bloods \
  -H "Content-Type: application/json" \
  -d '{
    "unitNumber": "U001",
    "bloodTypeId": 1,
    "collectionDate": "2026-03-12",
    "expiryDate": "2026-04-16",
    "volume": 450,
    "donorName": "Nguyễn Văn A",
    "location": "Kho A1"
  }'
```

#### GET /api/bloods
Lấy tất cả túi máu
```bash
curl https://localhost:5001/api/bloods
```

#### GET /api/bloods/available/{bloodTypeId}
Lấy máu có sẵn theo nhóm máu (FIFO)
```bash
curl https://localhost:5001/api/bloods/available/1
```

#### GET /api/bloods/expiring/alert
Lấy máu sắp hết hạn trong 7 ngày
```bash
curl https://localhost:5001/api/bloods/expiring/alert
```

#### POST /api/bloods/create-expiry-alerts
Tạo cảnh báo hết hạn
```bash
curl -X POST https://localhost:5001/api/bloods/create-expiry-alerts
```

#### PUT /api/bloods/{id}/discard
Hủy bỏ túi máu
```bash
curl -X PUT https://localhost:5001/api/bloods/1/discard \
  -H "Content-Type: application/json" \
  -d '{"reason": "Hết hạn"}'
```

### 3. Patients (Bệnh nhân)

#### POST /api/patients
Thêm bệnh nhân mới
```bash
curl -X POST https://localhost:5001/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Trần Văn B",
    "patientCode": "BN001",
    "bloodTypeId": 1,
    "dateOfBirth": "1990-01-01",
    "gender": "Nam",
    "hospital": "Bệnh viện Lồi",
    "ward": "Phòng 101",
    "admissionDate": "2026-03-12",
    "phoneNumber": "0987654321",
    "email": "patient@example.com",
    "medicalCondition": "Chảy máu nội tạng"
  }'
```

#### GET /api/patients
Lấy tất cả bệnh nhân
```bash
curl https://localhost:5001/api/patients
```

#### GET /api/patients/{id}/compatible-bloods
Lấy máu tương thích với bệnh nhân
```bash
curl https://localhost:5001/api/patients/1/compatible-bloods
```

#### POST /api/patients/{id}/transfuse
Truyền máu cho bệnh nhân (với kiểm tra tương thích tự động)
```bash
curl -X POST https://localhost:5001/api/patients/1/transfuse \
  -H "Content-Type: application/json" \
  -d '{
    "bloodId": 1,
    "quantity": 200,
    "performedBy": "Bác sĩ Nguyễn",
    "notes": "Truyền thành công"
  }'
```

### 4. Blood Inventory (Tồn kho)

#### GET /api/bloodinventory
Lấy tồn kho tất cả nhóm máu
```bash
curl https://localhost:5001/api/bloodinventory
```

**Response:**
```json
[
  {
    "id": 1,
    "bloodType": "O_NEGATIVE",
    "totalUnits": 10,
    "totalVolume": 4500,
    "availableUnits": 8,
    "reservedUnits": 2,
    "lowStockThreshold": 1000,
    "isLowStock": false,
    "lastUpdated": "2026-03-12T10:30:00Z"
  }
]
```

#### GET /api/bloodinventory/low-stock/alert
Lấy các nhóm máu tồn kho thấp
```bash
curl https://localhost:5001/api/bloodinventory/low-stock/alert
```

#### PUT /api/bloodinventory/{bloodTypeId}/threshold
Cập nhật ngưỡng cảnh báo tồn kho
```bash
curl -X PUT https://localhost:5001/api/bloodinventory/1/threshold \
  -H "Content-Type: application/json" \
  -d '{"newThreshold": 1500}'
```

### 5. Blood Expiry Alerts (Cảnh báo hết hạn)

#### GET /api/bloodexpiryalerts
Lấy tất cả cảnh báo
```bash
curl https://localhost:5001/api/bloodexpiryalerts
```

#### GET /api/bloodexpiryalerts/active
Lấy cảnh báo hoạt động chưa xác nhận
```bash
curl https://localhost:5001/api/bloodexpiryalerts/active
```

#### PUT /api/bloodexpiryalerts/{alertId}/acknowledge
Xác nhận cảnh báo
```bash
curl -X PUT https://localhost:5001/api/bloodexpiryalerts/1/acknowledge \
  -H "Content-Type: application/json" \
  -d '{"acknowledgedBy": "Bác sĩ Nguyễn"}'
```

#### GET /api/bloodexpiryalerts/statistics
Lấy thống kê cảnh báo
```bash
curl https://localhost:5001/api/bloodexpiryalerts/statistics
```

## 🩸 Quy tắc Tương thích Nhóm máu

| Nhóm máu nhận | Có thể nhận từ |
|--------------|----------------|
| O-           | O- (Universal Donor) |
| O+           | O-, O+ |
| A-           | O-, A- |
| A+           | O-, O+, A-, A+ |
| B-           | O-, B- |
| B+           | O-, O+, B-, B+ |
| AB-          | O-, A-, B-, AB- |
| AB+          | Tất cả (Universal Recipient) |

## ⚠️ Cảnh báo Hết hạn

- Máu được thu trong khoảng **35-42 ngày**
- Hệ thống cảnh báo khi máu sắp hết hạn trong **7 ngày**
- Máu quá ngày hết hạn sẽ được tự động đánh dấu là **Expired**

## 📊 FIFO Logic

Khi lấy máu từ kho, hệ thống sẽ:
1. Sắp xếp theo ngày thu máu (cũ nhất trước)
2. Chọn túi máu cũ nhất còn hiệu lực
3. Đảm bảo không lãng phí máu do hết hạn

## 🔧 Công nghệ Sử dụng

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server 2019+
- **ORM**: Entity Framework Core 10.0
- **Language**: C# 12

## 📝 Dependencies

Các gói NuGet cần thiết:
- `Microsoft.EntityFrameworkCore.SqlServer` (10.0.0)
- `Microsoft.EntityFrameworkCore.Design` (10.0.0)

## 🛠️ Phát triển thêm

### Thêm Migration mới
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Build Release
```bash
dotnet publish -c Release -o ./publish
```

## 📞 Support

Để báo cáo lỗi hoặc yêu cầu tính năng, hãy liên hệ với nhóm phát triển.

## 📄 License

Dự án này được cấp phép theo MIT License
