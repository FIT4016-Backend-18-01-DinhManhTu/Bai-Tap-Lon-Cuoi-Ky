# Blood Bank Manager - Business Rules & System Logic

## 1. Blood Type Compatibility Rules

### Universal Rules
- **O- (Negative)**: Universal Donor - có thể hiến cho tất cả các nhóm máu
- **AB+ (Positive)**: Universal Recipient - có thể nhận từ tất cả các nhóm máu

### Compatibility Matrix

#### Red Blood Cell (RBC) Compatibility

```
┌─────────────┬────────────────────────────────────────────────┐
│ Recipient   │ Compatible Donors                              │
├─────────────┼────────────────────────────────────────────────┤
│ O-          │ O-                                             │
│ O+          │ O-, O+                                         │
│ A-          │ O-, A-                                         │
│ A+          │ O-, O+, A-, A+                                 │
│ B-          │ O-, B-                                         │
│ B+          │ O-, O+, B-, B+                                 │
│ AB-         │ O-, A-, B-, AB-                                │
│ AB+         │ O-, O+, A-, A+, B-, B+, AB-, AB+ (All types)   │
└─────────────┴────────────────────────────────────────────────┘
```

### ABO Blood Group System
- **O (Zero)**: No A or B antigens
- **A**: Has A antigen
- **B**: Has B antigen
- **AB**: Has both A and B antigens

### Rh Factor (D Antigen)
- **Rh+** (Positive): Has Rh D antigen
- **Rh-** (Negative): No Rh D antigen

### Rules:
- A person with Rh- blood can only receive Rh- blood
- A person with Rh+ blood can receive both Rh+ and Rh- blood

---

## 2. Blood Unit Management

### Blood Unit Status
```
Available   → Blood is ready for transfusion
InUse       → Blood is currently being transfused
Reserved    → Blood is reserved for specific patient
Expired     → Blood has past expiry date
Discarded   → Blood has been manually discarded
```

### Blood Unit Lifecycle

```
Collection Date
      ↓
  Collection (Day 0)
      ↓
Collection Date + 35-42 days
      ↓
  Expiry Date
      ↓
Status: Available → InUse/Reserved → Expired/Discarded
```

### Standard Blood Unit Volume
- **Standard single unit**: 420-450 mL
- **Pediatric unit**: 250-300 mL
- **Apheresis unit**: 200-600 mL

---

## 3. Expiry Management System

### Timeline & Alerts

```
Collection Date          → Day 0 (Blood collected)
           ├─ Day 28: System begins monitoring
           ├─ Day 32: Can still transfuse normally
           ├─ Day 35: FIFO preference (older units transfused first)
           ├─ Day 37: RED ZONE - Alert active (7 days remaining)
           │           └─ Alert Status: ACTIVE
           │           └─ Action Required: Use or discard
           ├─ Day 40: CRITICAL ZONE
           └─ Day 42: Expiry Date (Must not use after this)
                      Status: EXPIRED (auto-marked)
                      Alert: RESOLVED
```

### Alert Rules

1. **Minimum Threshold**: 7 days before expiry
2. **Alert Generation**: Automatic when blood is 7 days from expiry
3. **Alert Acknowledgment**: Staff must acknowledge receipt of alert
4. **Auto-Expiry Check**: System checks daily at scheduled time
5. **Expired Blood Status**: Automatically marked when current date >= expiry date

### Alert Status Transitions

```
Active (Not Acknowledged)
    ↓ (Manual acknowledgment or auto-expiry)
Resolved (Expired) OR Acknowledged (Not used yet)
    ↓ (If blood is used)
Resolved (Used successfully)
```

---

## 4. FIFO (First In, First Out) Logic

### Principle
**Máu được thu trước (cũ hơn) phải được sử dụng trước (xuất trước)**

### Implementation

```
When requesting blood of type O-:

Step 1: Filter available O- blood units
        Status = Available AND ExpiryDate > Today

Step 2: Sort by CollectionDate (oldest first)
        Collection Date: 2026-03-10 ← First choice
        Collection Date: 2026-03-12
        Collection Date: 2026-03-14

Step 3: Select oldest unit for transfusion
        Unit: U001 (collected 2026-03-10)

Step 4: Update inventory
        Status: InUse
        UsedAt: Current timestamp
        Volume: Original - Transfused amount
```

### Benefits
- ✅ Minimize waste from expired blood
- ✅ Ensure freshest blood remains in storage
- ✅ Comply with blood bank standards
- ✅ Reduce disposal costs

---

## 5. Inventory Management

### Inventory Tracking

```
BloodInventory Record per Blood Type:
├─ Total Units: Sum of all blood units (any status)
├─ Total Volume: Total ml available (All)
├─ Available Units: Count of Available units
├─ Reserved Units: Count of Reserved units
├─ Low Stock Threshold: Minimum volume in ml
└─ Last Updated: Timestamp of last update
```

### Low Stock Alerts

```
If (Total Volume < Low Stock Threshold)
  → Generate Low Stock Alert
  → Notify blood bank manager
  → Recommend urgent collection

Example:
  Blood Type: O-
  Total Volume: 800 ml
  Threshold: 1000 ml
  Status: ⚠️ LOW STOCK (collect more O- blood)
```

### Default Low Stock Threshold
- 1000 mL per blood type (configurable)

### Stock Status Levels

```
🟢 Green (Good)   : Total Volume >= Threshold
🟡 Yellow (Warning): Total Volume = 75-100% of Threshold
🔴 Red (Critical) : Total Volume < 75% of Threshold
```

---

## 6. Transfusion Process

### Transfusion Workflow

```
1. Patient Registration
   ├─ Record blood type
   ├─ Medical condition
   └─ Hospital information

2. Request Blood
   ├─ Get compatible blood types
   ├─ Query available units (FIFO ordered)
   └─ Select specific unit

3. Compatibility Check (Automatic)
   ├─ Verify donor blood type compatible with recipient
   │  └─ If NOT compatible → REJECT (Error: Blood incompatible)
   └─ If compatible → PROCEED

4. Volume Check
   ├─ Verify available volume >= requested quantity
   │  └─ If NOT enough → REJECT (Error: Insufficient volume)
   └─ If enough → PROCEED

5. Perform Transfusion
   ├─ Update blood status: InUse
   ├─ Record transfusion details
   ├─ Update blood volume: original - transfused
   └─ Record timestamp

6. Post-Transfusion
   ├─ If (remaining volume == 0) → Status: Discarded
   ├─ If (remaining volume > 0) → Status: Available
   └─ Update inventory totals

7. Record Keeping
   └─ BloodTransfusion record includes:
      ├─ Patient info
      ├─ Blood unit info
      ├─ Quantity transfused
      ├─ Performed by (staff name)
      ├─ Timestamp
      └─ Notes/Observations
```

### Transfusion Compatibility Check Example

```
Patient: Trần Văn B
Blood Type: A+

Compatible Donors: [O-, O+, A-, A+]

Blood Units Available:
├─ U001: O-   [✓ Compatible] ← Will be selected (FIFO)
├─ U002: O+   [✓ Compatible]
├─ U003: B+   [✗ Not Compatible]
└─ U004: A+   [✓ Compatible]
```

---

## 7. Data Validation Rules

### Blood Unit Validation
- ✓ Unit Number: Unique, max 50 characters
- ✓ Collection Date: Cannot be in future
- ✓ Expiry Date: Must be 35-42 days after collection
- ✓ Volume: Must be > 0 ml
- ✓ Donor Name: Required, max 200 characters

### Patient Validation
- ✓ Patient Code: Unique, max 50 characters
- ✓ Blood Type: Must be valid enum
- ✓ Date of Birth: Cannot be >= admission date
- ✓ Email: Valid email format (if provided)
- ✓ Phone: Valid format (if provided)

### Transfusion Validation
- ✓ Blood compatibility check (primary validation)
- ✓ Blood not expired check
- ✓ Blood status = Available
- ✓ Sufficient volume available
- ✓ Patient exists

---

## 8. Error Handling & Business Rules

### Blood Compatibility Errors

```
Error: Blood type O_NEGATIVE is not compatible with patient blood type A_NEGATIVE
Response:
{
  "message": "Incompatible blood type",
  "patientBloodType": "A_NEGATIVE",
  "requestedBloodType": "O_NEGATIVE",
  "compatibleTypes": ["O_NEGATIVE", "A_NEGATIVE"]
}
```

### Insufficient Volume Error

```
Error: Requested quantity 300ml exceeds available 150ml
Response:
{
  "message": "Insufficient blood volume",
  "requested": 300,
  "available": 150,
  "suggestion": "Split transfusion or use additional units"
}
```

### Expired Blood Error

```
Error: Blood unit has expired
Response:
{
  "message": "Blood has expired",
  "unitNumber": "U001",
  "expiryDate": "2026-03-12T10:00:00Z",
  "action": "Unit auto-marked as Expired"
}
```

---

## 9. Scheduled Operations

### Daily Tasks (Recommended Schedule)

```
⏰ 00:00 (Midnight): Check expired bloods
   ├─ Run CheckAndMarkExpiredBloodsAsync()
   └─ Update blood status automatically

⏰ 08:00 (Morning): Generate expiry alerts
   ├─ Run CreateExpiryAlertsAsync()
   └─ Notify staff of units expiring soon

⏰ 16:00 (Afternoon): Inventory review
   ├─ Verify low stock levels
   └─ Recommend collection from donors
```

---

## 10. Audit & Compliance

### Data Logging

Every operation records:
- ✓ Timestamp (UTC)
- ✓ User/Staff name
- ✓ Operation type (Create, Update, Transfuse, Discard)
- ✓ Blood unit information
- ✓ Patient information
- ✓ Status changes

### Compliance Records Kept

```
Blood Unit History:
├─ Collection info
├─ Storage location history
├─ Each transfusion record
├─ Expiry tracking
└─ Discard information

Patient Transfusion History:
├─ All transfusions performed
├─ Blood types received
├─ Date and time
├─ Staff who performed
└─ Notes/Observations
```

---

## 11. Performance Considerations

### Database Indexes (Auto-created)
```sql
IX_Blood_ExpiryDate        -- Fast expiry lookups
IX_Blood_Status            -- Fast status filtering
IX_BloodTransfusion_Date   -- Fast history queries
IX_BloodExpiryAlert_Status -- Fast alert filtering
```

### FIFO Optimization
- CollectionDate index ensures efficient ordering
- Reduces scan time for large inventories
- Enables real-time availability checks

---

## Summary: Key Business Rules

| Rule | Implementation |
|------|-----------------|
| **FIFO** | Order by CollectionDate ASC |
| **Expiry Check** | >= 7 days before expiry |
| **Compatibility** | Automatic validation before transfusion |
| **Inventory Tracking** | Real-time updates |
| **Low Stock** | Configurable threshold per blood type |
| **Auto-Expiry** | Automatic status update at/after expiry date |
| **Alerts** | Manual acknowledgment required |
| **Audit Trail** | All operations logged with timestamp |

