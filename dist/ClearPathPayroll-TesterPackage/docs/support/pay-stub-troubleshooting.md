# Pay Stub Printing Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. This document covers software behavior only.

## Common Errors

- **Pay stub not available**: Confirm the payroll run employee ID in the route.
- **Missing company data**: Confirm the payroll run is linked to a company.
- **Missing employee data**: Confirm the payroll run employee is linked to an employee.
- **Direct deposit last 4 unavailable**: Confirm the employee has an active verified direct deposit token record.
- **Check number missing**: Confirm the net pay line payment method is `Check`.

## Troubleshooting Steps

1. Confirm payroll has been calculated.
2. Open `/payroll/paystub/{payrollRunEmployeeId}`.
3. Review display settings.
4. Confirm the desired sections are enabled.
5. Select **Print Pay Stub**.

## Safety Cautions

The pay stub displays SSN last 4 only and direct deposit last 4 only. Review applicable pay statement requirements before distribution.
