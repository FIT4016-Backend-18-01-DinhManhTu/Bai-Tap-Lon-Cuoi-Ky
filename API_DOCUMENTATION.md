# Blood Bank Manager - API Documentation

## API Endpoints Overview

### Base URL
- Development: `http://localhost:5000`
- Production: Update based on your deployment

---

## 1. Blood Types API

### GET /api/bloodtypes
Retrieve all blood types in the system.

**Response:**
```json
[
  {
    "id": 1,
    "typeName": "O_NEGATIVE",
    "description": "O âm tính - Nhóm máu phổ biến"
  },
  {
    "id": 2,
    "typeName": "O_POSITIVE",
    "description": "O dương tính"
  }
]
```

### POST /api/bloodtypes
Create a new blood type.

**Request Body:**
```json
{
  "typeName": "O_NEGATIVE",
  "description": "O âm tính"
}
```

---

## 2. Bloods API (Túi máu)

### POST /api/bloods
Add a new blood unit to inventory.

**Request Body:**
```json
{
  "unitNumber": "U001",
  "bloodTypeId": 1,
  "collectionDate": "2026-03-12T10:00:00Z",
  "expiryDate": "2026-04-16T10:00:00Z",
  "volume": 450,
  "donorName": "Nguyễn Văn A",
  "location": "Kho A1"
}
```

**Response:**
```json
{
  "id": 1,
  "unitNumber": "U001",
  "bloodTypeId": 1,
  "collectionDate": "2026-03-12T10:00:00Z",
  "expiryDate": "2026-04-16T10:00:00Z",
  "volume": 450,
  "status": "Available",
  "donorName": "Nguyễn Văn A",
  "location": "Kho A1",
  "createdAt": "2026-03-12T10:30:00Z"
}
```

### GET /api/bloods
Retrieve all blood units.

### GET /api/bloods/{id}
Retrieve a specific blood unit.

### GET /api/bloods/available/{bloodTypeId}
Get available blood units for a specific blood type (ordered by FIFO - oldest first).

**Response:**
```json
[
  {
    "id": 1,
    "unitNumber": "U001",
    "bloodTypeId": 1,
    "collectionDate": "2026-03-12T10:00:00Z",
    "expiryDate": "2026-04-16T10:00:00Z",
    "volume": 450,
    "status": "Available"
  }
]
```

### GET /api/bloods/expiring/alert
Get blood units expiring within 7 days.

### POST /api/bloods/create-expiry-alerts
Create alerts for all blood units expiring within 7 days.

### POST /api/bloods/check-expired
Check and mark all expired blood units automatically.

### PUT /api/bloods/{id}/discard
Mark a blood unit as discarded.

**Request Body:**
```json
{
  "reason": "Hết hạn"
}
```

---

## 3. Patients API

### POST /api/patients
Register a new patient.

**Request Body:**
```json
{
  "name": "Trần Văn B",
  "patientCode": "BN001",
  "bloodTypeId": 1,
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": "Nam",
  "hospital": "Bệnh viện Lồi",
  "ward": "Phòng 101",
  "admissionDate": "2026-03-12T00:00:00Z",
  "phoneNumber": "0987654321",
  "email": "patient@example.com",
  "medicalCondition": "Chảy máu nội tạng"
}
```

### GET /api/patients
Get all patients.

### GET /api/patients/{id}
Get specific patient details.

### GET /api/patients/{id}/compatible-bloods
Get all blood units compatible with the patient's blood type (automatic compatibility check).

**Example Response:**
```json
[
  {
    "id": 1,
    "unitNumber": "U001",
    "bloodTypeId": 1,
    "volume": 450,
    "expiryDate": "2026-04-16T10:00:00Z",
    "status": "Available"
  }
]
```

### POST /api/patients/{id}/transfuse
Perform blood transfusion for a patient with automatic compatibility verification.

**Request Body:**
```json
{
  "bloodId": 1,
  "quantity": 200,
  "performedBy": "Bác sĩ Nguyễn",
  "notes": "Truyền thành công"
}
```

**Response:**
```json
{
  "transfusionId": 1,
  "patientName": "Trần Văn B",
  "bloodType": "O_NEGATIVE",
  "quantityTransfused": 200,
  "transfusionDate": "2026-03-12T10:30:00Z",
  "message": "Transfusion completed successfully"
}
```

**Error Response (Incompatible blood):**
```json
{
  "message": "Blood type O_NEGATIVE is not compatible with patient blood type AB_NEGATIVE",
  "compatibleDonors": ["AB_NEGATIVE"]
}
```

### GET /api/patients/{id}/transfusions
Get transfusion history for a specific patient.

**Response:**
```json
[
  {
    "id": 1,
    "patientId": 1,
    "bloodId": 1,
    "quantity": 200,
    "transfusionDate": "2026-03-12T10:30:00Z",
    "performedBy": "Bác sĩ Nguyễn",
    "notes": "Truyền thành công",
    "blood": {
      "id": 1,
      "unitNumber": "U001",
      "bloodTypeId": 1
    }
  }
]
```

---

## 4. Blood Inventory API

### GET /api/bloodinventory
Get inventory status for all blood types.

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

### GET /api/bloodinventory/{bloodTypeId}
Get inventory for specific blood type.

### GET /api/bloodinventory/low-stock/alert
Get all blood types with low stock (below threshold).

### PUT /api/bloodinventory/{bloodTypeId}/threshold
Update low stock threshold for a specific blood type.

**Request Body:**
```json
{
  "newThreshold": 1500
}
```

---

## 5. Blood Expiry Alerts API

### GET /api/bloodexpiryalerts
Get all expiry alerts.

**Response:**
```json
[
  {
    "id": 1,
    "bloodUnitNumber": "U001",
    "bloodType": "O_NEGATIVE",
    "daysUntilExpiry": 5,
    "expiryDate": "2026-03-17T10:00:00Z",
    "alertDate": "2026-03-12T10:30:00Z",
    "isAcknowledged": false,
    "acknowledgedAt": null,
    "acknowledgedBy": null,
    "status": "Active"
  }
]
```

### GET /api/bloodexpiryalerts/active
Get all active, unacknowledged alerts.

### POST /api/bloodexpiryalerts/refresh
Refresh all alerts (check for expired bloods and create new alerts).

### PUT /api/bloodexpiryalerts/{alertId}/acknowledge
Acknowledge an expiry alert.

**Request Body:**
```json
{
  "acknowledgedBy": "Bác sĩ Nguyễn"
}
```

### GET /api/bloodexpiryalerts/statistics
Get alert statistics.

**Response:**
```json
{
  "totalAlerts": 15,
  "activeAlerts": 5,
  "acknowledgedAlerts": 7,
  "resolvedAlerts": 3,
  "unrecognizedAlerts": 2
}
```

---

## Error Handling

### Common Error Responses

**400 - Bad Request:**
```json
{
  "message": "Requested quantity exceeds available volume"
}
```

**404 - Not Found:**
```json
{
  "message": "Blood not found"
}
```

**500 - Internal Server Error:**
```json
{
  "message": "Error processing request"
}
```

---

## Blood Type Compatibility Matrix

| Recipient | Compatible Donor Types |
|-----------|------------------------|
| O- | O- |
| O+ | O-, O+ |
| A- | O-, A- |
| A+ | O-, O+, A-, A+ |
| B- | O-, B- |
| B+ | O-, O+, B-, B+ |
| AB- | O-, A-, B-, AB- |
| AB+ | O-, O+, A-, A+, B-, B+, AB-, AB+ |

---

## 6. Authentication API

### POST /api/account/register/staff
Register a new staff member.

**Request Body:**
```json
{
  "email": "staff@example.com",
  "password": "password123",
  "fullName": "Nguyễn Văn Staff"
}
```

**Response:**
```json
{
  "message": "Staff registered successfully"
}
```

### POST /api/account/register/patient
Register a new patient (creates both user account and patient record).

**Request Body:**
```json
{
  "email": "patient@example.com",
  "password": "password123",
  "fullName": "Trần Thị Patient",
  "patientCode": "P001",
  "bloodTypeId": 1,
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": "Female",
  "hospital": "Bệnh viện Việt Đức",
  "ward": "Khoa Nội",
  "admissionDate": "2026-03-12T00:00:00Z",
  "phoneNumber": "0123456789",
  "medicalCondition": "Thiếu máu"
}
```

**Response:**
```json
{
  "message": "Patient registered successfully"
}
```

### POST /api/account/login
Login for both staff and patients.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "message": "Login successful",
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "fullName": "User Name",
    "role": "Staff",
    "patientId": null
  },
  "roles": ["Staff"]
}
```

### POST /api/account/logout
Logout the current user.

**Response:**
```json
{
  "message": "Logged out successfully"
}
```

---

## Response Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 201 | Created - Resource created successfully |
| 400 | Bad Request - Invalid parameters |
| 401 | Unauthorized - Authentication required |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error |

---

## Notes

- All timestamps are in UTC format (ISO 8601)
- FIFO logic ensures oldest blood units are used first
- Automatic compatibility checking prevents transfusion errors
- Expiry alerts are generated for blood expiring within 7 days
- Low stock alerts help maintain adequate blood inventory
- Authentication is required for certain operations (to be implemented in future updates)

