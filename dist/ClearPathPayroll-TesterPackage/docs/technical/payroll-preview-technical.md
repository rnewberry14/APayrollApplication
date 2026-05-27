# Payroll Preview - Technical Documentation

## Component Overview

The Payroll Preview page is a Blazor Server component that displays calculated payroll data and handles approval workflow.

**Location**: `src/ClearPathPayroll/Components/Pages/PayrollPreview.razor`

**Route**: `@page "/payroll/preview/{PayrollRunId:int}"`

**Components Used**: Blazor forms, Bootstrap tables and modals

## Architecture

### Data Flow

```
PayrollRunId (URL Parameter)
    ↓
LoadPayrollRun() → PayrollService.GetPayrollRunByIdAsync()
    ↓
PayrollRun + PayrollRunEmployees (from DB)
    ↓
ValidatePayrollRun() → Build validation/warning lists
    ↓
Render Display with validation state
```

### Approval Flow

```
ShowApprovalConfirmation()
    ↓
Modal dialog displays (on user click)
    ↓
HandleApprove()
    ↓
Update Status: Draft → Approved
    ↓
PayrollService.UpdatePayrollRunAsync()
    ↓
Redirect to payroll list on success
```

## Code Structure

### Component Parameters

```csharp
[Parameter]
public int PayrollRunId { get; set; }
```

Passed via route: `/payroll/preview/123`

### Injected Services

```csharp
@inject PayrollService PayrollService
@inject PayrollCalculationService PayrollCalculationService
@inject NavigationManager NavigationManager
```

### Component State

```csharp
private PayrollRun? payrollRun;              // Main data model
private bool isLoading = true;                // Loading indicator
private bool isProcessing = false;            // Submission processing
private bool showConfirmation = false;        // Approval modal
private string successMessage = string.Empty; // Success notification
private string errorMessage = string.Empty;   // Error notification
private List<string> validationErrors = new();// Validation errors
private List<string> warnings = new();        // Warnings (non-blocking)
private decimal totalCashRequired = 0m;       // Calculated total
```

## Key Methods

### OnInitializedAsync

**Purpose**: Load payroll run on page load

**Logic**:
1. Call `LoadPayrollRun()`
2. Call `ValidatePayrollRun()`

**Error Handling**: Displays error message if load fails

### LoadPayrollRun

**Purpose**: Fetch payroll run data from database

**Logic**:
```csharp
payrollRun = await PayrollService.GetPayrollRunByIdAsync(PayrollRunId);
totalCashRequired = payrollRun.TotalNetPay + payrollRun.TotalEmployerTaxes;
```

**Returns**: Populates `payrollRun` and `totalCashRequired`

**Error Handling**: Catches exceptions and displays error message

### ValidatePayrollRun

**Purpose**: Build validation errors and warnings

**Validation Errors** (prevent approval):
- No employees in payroll run
- Gross pay <= 0
- Status is Approved or Submitted

**Warnings** (do not prevent approval):
- Pay date is in the past
- Total deductions exceed gross pay
- Employees with zero tax withholding
- Minimum wage violations for hourly employees

**Logic**:
```csharp
if (payrollRun.PayrollRunEmployees == null || Count == 0)
    validationErrors.Add("No employees in this payroll run.");

// Minimum wage validation for each employee
foreach (var employee in payrollRun.PayrollRunEmployees)
{
    var minimumWageWarning = MinimumWageValidator.ValidateMinimumWage(
        employeeName,
        employee.Employee.State,
        employee.Employee.PayType,
        employee.GrossPay,
        totalHours,
        employee.EarningLines
    );
    
    if (!string.IsNullOrEmpty(minimumWageWarning))
    {
        warnings.Add(minimumWageWarning);
    }
}
```

**Minimum Wage Validation**:
- Applies only to hourly employees (PayType.Hourly)
- Regular employees: Gross Pay ÷ Hours ≥ State Minimum Wage
- Tipped employees: (Base Wage + Tips) ÷ Hours ≥ State Minimum Wage
  - Tipped status detected by earning line descriptions
  - Base wage can be federal minimum ($2.13/hr)
  - Tips inferred from earning lines containing "tip", "gratuity", etc.
- Uses [MinimumWageValidator](../src/ClearPathPayroll/Components/Pages/MinimumWageValidator.cs) helper class

### HandleRecalculate

**Purpose**: Recalculate payroll from current data

**Workflow**:
1. Check `CanRecalculatePayrollRunAsync()`
2. If cannot recalculate, show error
3. Reload payroll data
4. Re-validate and display updated amounts

**Error Handling**: Shows error if status not Draft/Calculated

### ShowApprovalConfirmation

**Purpose**: Display confirmation modal

**Logic**:
```csharp
if (validationErrors.Count == 0)
{
    showConfirmation = true;  // Display modal
}
```

**Prevents approval if errors exist**

### HandleApprove

**Purpose**: Update status to Approved and save

**Workflow**:
1. Validate no errors
2. Set status to `PayrollStatus.Approved`
3. Call `PayrollService.UpdatePayrollRunAsync()`
4. Show success message
5. Delay 2 seconds for readability
6. Redirect to `/payroll`

**Error Handling**: Catches database errors and shows message

### HandleCancel

**Purpose**: Return to payroll list without changes

**Logic**:
```csharp
NavigationManager.NavigateTo("/payroll");
```

## Data Models

### PayrollRun (Main Model)

**Key Properties**:
- `PayrollRunId`: Primary key
- `CompanyId`: Foreign key to Company
- `PayPeriodStart`, `PayPeriodEnd`: Date range
- `PayDate`: When payment occurs
- `Status`: Draft, Calculated, Approved, Submitted, Completed, Voided
- `TotalGrossPay`, `TotalDeductions`, `TotalEmployeeTaxes`, `TotalEmployerTaxes`, `TotalNetPay`
- `PayrollRunEmployees`: Navigation property with collection

**Navigation**:
- `Company`: Company details
- `PaySchedule`: Pay frequency information
- `PayrollRunEmployees`: Employee payroll details

### PayrollRunEmployee (Per-Employee Details)

**Key Properties**:
- `EmployeeId`: Foreign key
- `GrossPay`: Total earnings
- `TotalDeductions`: Sum of all deductions
- `TotalTaxes`: Sum of all taxes
- `NetPay`: Gross - Deductions - Taxes
- `EarningLines`: Breakdown by earning type
- `DeductionLines`: Breakdown by deduction type
- `TaxLines`: Breakdown by tax type

## UI Components

### Display Sections

1. **Header**: Title and status badge
2. **Validation/Warning Alerts**: Dismissible alerts for errors and warnings
3. **Run Summary Card**: Company, period, dates, run ID
4. **Employee Table**: All employees with payroll amounts
5. **Totals Cards**: Payroll totals and employer costs
6. **Action Buttons**: Recalculate, Approve, Cancel
7. **Confirmation Modal**: Approval confirmation dialog
8. **Success/Error Messages**: Dismissible notifications

### Bootstrap Classes Used

- `.container-fluid`: Full width layout
- `.card`, `.card-header`, `.card-body`: Card components
- `.table`, `.table-striped`: Employee details table
- `.alert`: Error/warning/success messages
- `.modal`, `.modal-dialog`: Confirmation modal
- `.btn`, `.btn-primary`, `.btn-success`: Buttons
- `.badge`: Status badge

### Accessibility

- Semantic HTML (button, table, form elements)
- Color-based status indicators + text labels
- Disabled states prevent unintended actions
- Modal has close button and keyboard support (built-in)

## Security Considerations

### Data Protection

**SSN Handling**: No SSNs displayed
- Employee identified by first/last name and ID only
- SSN never fetched or displayed on preview

**Bank Account Handling**: Tokens only
- If bank accounts displayed, use tokens + Last4
- Current implementation doesn't display accounts

### Access Control

Implement role-based authorization:
- **View Preview**: `payroll:preview_run` permission
- **Approve**: `payroll:approve_run` permission

**Recommendation**: Add to Program.cs:
```csharp
@attribute [Authorize(Roles = "PayrollManager,Administrator")]
```

### Audit Logging

All approvals should log:
- User ID who approved
- Timestamp
- PayrollRunId
- Status change (Draft → Approved)

Implement in `HandleApprove()`:
```csharp
await AuditLogEntry.CreateAsync(
    entityType: "PayrollRun",
    entityId: PayrollRunId,
    action: "Approved",
    details: "Payroll run approved for submission",
    performedByUserId: currentUserId
);
```

## Integration Points

### PayrollService Methods Called

- `GetPayrollRunByIdAsync(id)`: Load payroll run
- `UpdatePayrollRunAsync(payrollRun)`: Save approval

**Expected Return Types**:
- `Task<PayrollRun?>`: Payroll run data with navigation properties loaded
- `Task`: Async save operation

### PayrollCalculationService Methods Called

- `CanRecalculatePayrollRunAsync(id)`: Check if recalculation allowed
- Returns `Task<bool>`

## Error Handling

### Try-Catch Pattern

```csharp
try
{
    isProcessing = true;
    // Business logic
}
catch (Exception ex)
{
    errorMessage = $"Error message: {ex.Message}";
}
finally
{
    isProcessing = false;
}
```

### Common Errors

| Exception | Cause | User Message |
|-----------|-------|--------------|
| `DbUpdateException` | Database constraint violation | "Error approving payroll" |
| `InvalidOperationException` | Status transition not allowed | "Cannot recalculate in current status" |
| `NullReferenceException` | Payroll run not found | "Payroll run not found" |

## Performance Considerations

### Database Queries

**LoadPayrollRun()**:
- Fetches one `PayrollRun` row
- Includes navigation properties: `Company`, `PaySchedule`, `PayrollRunEmployees`
- Nested includes for `Employee` on each PayrollRunEmployee
- **N+1 Risk**: If PayrollRunEmployees not eagerly loaded

**Recommendation**: Verify `PayrollService.GetPayrollRunByIdAsync()` includes:
```csharp
.Include(p => p.PayrollRunEmployees)
    .ThenInclude(pe => pe.Employee)
.Include(p => p.Company)
.Include(p => p.PaySchedule)
```

### Rendering Performance

- Employee table renders all employees
- For 1000+ employees, consider pagination
- Current design assumes < 500 employees per run

### Response Times

- Page load: 1-2 seconds (DB query + render)
- Recalculate: 2-5 seconds (refresh + validate)
- Approve: 1-2 seconds (update + redirect)

## Testing

### Unit Test Coverage

Should test:
1. Load payroll run with valid ID
2. Load payroll run with invalid ID (returns null)
3. Validation errors for each error condition
4. Warnings appear but don't prevent approval
5. Cannot approve with validation errors present
6. Approval updates status correctly
7. Recalculate refreshes data
8. Cancel navigates away

### Integration Test Coverage

Should test:
1. Full workflow: Load → Validate → Approve
2. Database persistence after approval
3. Error scenarios (DB down, invalid data)
4. Authorization checks

### Manual Testing Checklist

- [ ] Page loads with valid payroll ID
- [ ] Error message displays with invalid ID
- [ ] All employee details display correctly
- [ ] Totals are accurate
- [ ] Approval button disabled when validation errors present
- [ ] Approval button enabled when no errors
- [ ] Modal confirms before approval
- [ ] Approval updates database status
- [ ] Recalculate refreshes displayed data
- [ ] Cancel returns to payroll list
- [ ] Success message displays after approval

## Future Enhancements

1. **Batch Approval**: Approve multiple payroll runs at once
2. **Payroll Export**: Download as PDF or CSV
3. **Historical Comparison**: Show vs. previous period
4. **Note Attachment**: Add approval notes or comments
5. **Dual Control**: Require second approver for large payroll
6. **Email Confirmation**: Send confirmation to payroll manager
7. **Scheduled Approval**: Auto-approve at specific time
8. **Partial Recalculation**: Recalculate specific employees only

## Known Limitations

1. **Recalculate**: Refreshes from database; doesn't accept new employee data
   - Would require data entry form integration
   - Current design assumes data already entered before preview

2. **Large Payroll Runs**: No pagination on employee table
   - 1000+ employees may impact rendering
   - Consider implement virtualization for large runs

3. **No Offline Support**: Requires server connection
   - Cannot approve if server unavailable
   - No draft approval capability

4. **No Concurrent Approval**: Last approval wins
   - If two managers approve simultaneously, second overwrites first
   - Consider optimistic locking for enterprise use

## Minimum Wage Validator Helper

**Location**: `src/ClearPathPayroll/Components/Pages/MinimumWageValidator.cs`

**Key Methods**:
- `GetStateMinimumWage(state)`: Returns minimum wage for given state
- `IsTippedEmployee(earningLines)`: Detects if employee has tip income
- `GetTipAmount(earningLines)`: Calculates total tips
- `GetBaseWageAmount(earningLines)`: Calculates wages excluding tips
- `GetTotalHours(earningLines)`: Sums hours from earning lines
- `ValidateMinimumWage(...)`: Performs full validation and returns warning message

**State Minimum Wages**:
- Dictionary of 50 states with current minimum wage rates
- Updated as of May 2026
- Falls back to federal minimum ($7.25) for unmapped states or invalid input

**Tipped Employee Logic**:
- Detects tips from earning line descriptions: "tip", "gratuity", "service charge", "credit card tip", "cash tip"
- Federal tipped minimum: $2.13/hour
- If tips present: Validates (base + tips) meets state minimum
- If no tips but marked as tipped: Warns about low base wage

## Related Documentation

- [PayrollPreview Component Tests](../../tests/ClearPathPayroll.Tests/PayrollPreviewTests.cs)
- [MinimumWageValidator](../src/ClearPathPayroll/Components/Pages/MinimumWageValidator.cs)
- [PayrollService](../../src/ClearPathPayroll/Services/PayrollService.cs)
- [PayrollCalculationService](../../src/ClearPathPayroll/Services/PayrollCalculationService.cs)
- [PayrollRun Domain Model](../../src/ClearPathPayroll/Domain/PayrollRun.cs)
