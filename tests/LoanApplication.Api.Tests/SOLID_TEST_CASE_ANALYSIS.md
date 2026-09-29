## SOLID.cs Test Case Analysis - Comprehensive Specification

### Overview
This document analyzes `SOLID.cs` (LoanProcessor class) and identifies all required test cases covering:
- Boundary values and exact thresholds
- Zero, null, negative, and invalid inputs
- Missing/non-existent data
- Dependency returns null or unexpected data
- Dependency exceptions or timeouts
- Duplicate or unexpected requests

---

### 🔍 ANALYSIS BY VALIDATION LAYER

---

## 1. NULL & EMPTY INPUT VALIDATION

### 1.1 Null Loan Object
| Aspect | Details |
|--------|---------|
| **Scenario** | Process null loan object |
| **Input Condition** | `loan = null` |
| **Expected Behavior** | Output: "Loan application cannot be null", method returns early without processing |
| **Validation Layer** | Line 43: `if (loan == null)` |
| **Test Type** | Null handling |

### 1.2 Null/Empty Applicant Name
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with null applicant name |
| **Input Condition** | `loan.ApplicantName = null` OR `loan.ApplicantName = ""` |
| **Expected Behavior** | Output: "Applicant name is required", method returns early |
| **Validation Layer** | Line 49: `if (string.IsNullOrEmpty(loan.ApplicantName))` |
| **Test Type** | Empty/null string handling |
| **Test Cases** |  `null`, `""` (empty string), `"  "` (whitespace only)* |

*Note: Code uses `IsNullOrEmpty()`, not `IsNullOrWhiteSpace()`, so whitespace-only string may pass validation

### 1.3 Null/Empty Email
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with null email address |
| **Input Condition** | `loan.Email = null` OR `loan.Email = ""` |
| **Expected Behavior** | Output: "Email is required", method returns early |
| **Validation Layer** | Line 55: `if (string.IsNullOrEmpty(loan.Email))` |
| **Test Type** | Empty/null string handling |
| **Test Cases** | `null`, `""` (empty string) |
| **Potential Bug** | No email format validation; accepts invalid formats like "notanemail" |

---

## 2. NUMERIC BOUNDARY VALIDATION

### 2.1 Annual Income Boundaries

#### 2.1.1 Negative Annual Income
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with negative annual income |
| **Input Condition** | `loan.AnnualIncome < 0` (e.g., -50000) |
| **Expected Behavior** | Output: "Annual income must be greater than zero", method returns early |
| **Validation Layer** | Line 61: `if (loan.AnnualIncome <= 0)` |
| **Test Type** | Boundary (zero) and invalid input |
| **Exact Threshold** | Must be `> 0` (greater than zero, not equal to zero) |

#### 2.1.2 Zero Annual Income
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with zero annual income |
| **Input Condition** | `loan.AnnualIncome = 0` |
| **Expected Behavior** | Output: "Annual income must be greater than zero", method returns early |
| **Validation Layer** | Line 61: `if (loan.AnnualIncome <= 0)` |
| **Test Type** | Boundary condition (exact threshold) |
| **Exact Threshold** | `0` is REJECTED (uses `<=` not `<`) |

#### 2.1.3 Minimum Valid Annual Income
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with minimum valid annual income |
| **Input Condition** | `loan.AnnualIncome = 0.01m` (smallest decimal > 0) |
| **Expected Behavior** | Passes income validation, proceeds to DTI calculation |
| **Edge Case** | Very small income (e.g., $0.01/year) may pass but likely create DTI issues later |
| **Potential Bug** | No upper limit on annual income; accepts unrealistic values like `decimal.MaxValue` |

#### 2.1.4 Extreme Annual Income
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with extremely high annual income |
| **Input Condition** | `loan.AnnualIncome = decimal.MaxValue` |
| **Expected Behavior** | Passes income validation; may cause arithmetic overflow in DTI calculation |
| **Potential Bug** | No overflow protection in DTI calculation: `loan.AnnualIncome / 12` could overflow |

---

### 2.2 Loan Amount Boundaries

#### 2.2.1 Negative Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Request negative loan amount |
| **Input Condition** | `loan.LoanAmount < 0` (e.g., -10000) |
| **Expected Behavior** | Output: "Loan amount must be greater than zero", method returns early |
| **Validation Layer** | Line 67: `if (loan.LoanAmount <= 0)` |
| **Test Type** | Boundary and invalid input |
| **Exact Threshold** | Must be `> 0` |

#### 2.2.2 Zero Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Request zero loan amount |
| **Input Condition** | `loan.LoanAmount = 0` |
| **Expected Behavior** | Output: "Loan amount must be greater than zero", method returns early |
| **Validation Layer** | Line 67: `if (loan.LoanAmount <= 0)` |
| **Test Type** | Boundary condition |
| **Exact Threshold** | `0` is REJECTED |

#### 2.2.3 Minimum Valid Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Request minimum valid loan amount |
| **Input Condition** | `loan.LoanAmount = 0.01m` |
| **Expected Behavior** | Passes loan amount validation, proceeds to DTI check |
| **Edge Case** | Monthly payment = 0.01 * 0.01 = 0.0001; May not be realistic but technically valid |

#### 2.2.4 Extreme Loan Amount (Overflow Risk)
| Aspect | Details |
|--------|---------|
| **Scenario** | Request extremely large loan amount |
| **Input Condition** | `loan.LoanAmount = decimal.MaxValue` |
| **Expected Behavior** | Passes validation; may cause overflow in DTI: `LoanAmount * 0.01` |
| **Potential Bug** | Arithmetic overflow not handled |

---

### 2.3 Credit Score Boundaries

#### 2.3.1 Credit Score Below Minimum
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score below minimum range |
| **Input Condition** | `loan.CreditScore < 300` (e.g., 299, 0, -100) |
| **Expected Behavior** | Output: "Invalid credit score", method returns early |
| **Validation Layer** | Line 73: `if (loan.CreditScore < 300 || loan.CreditScore > 900)` |
| **Test Type** | Boundary condition |
| **Exact Threshold** | Minimum accepted: `300` |
| **Test Cases** | 299, 0, -1, -999 |

#### 2.3.2 Credit Score at Minimum Boundary
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score exactly at minimum |
| **Input Condition** | `loan.CreditScore = 300` |
| **Expected Behavior** | Passes credit score range validation; may still be rejected at approval threshold (< 650) |
| **Validation Layer** | Line 73: Lower bound check (boundary inclusive) |
| **Exact Threshold** | `300` is ACCEPTED |

#### 2.3.3 Credit Score Above Maximum
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score above maximum range |
| **Input Condition** | `loan.CreditScore > 900` (e.g., 901, 1000, 9999) |
| **Expected Behavior** | Output: "Invalid credit score", method returns early |
| **Validation Layer** | Line 73: Upper bound check |
| **Test Type** | Boundary condition |
| **Exact Threshold** | Maximum accepted: `900` |
| **Test Cases** | 901, 999, 1000, int.MaxValue |

#### 2.3.4 Credit Score at Maximum Boundary
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score exactly at maximum |
| **Input Condition** | `loan.CreditScore = 900` |
| **Expected Behavior** | Passes credit score range validation; proceeds to approval threshold check |
| **Validation Layer** | Line 73: Upper bound check (boundary inclusive) |
| **Exact Threshold** | `900` is ACCEPTED |

#### 2.3.5 Valid Range but Below Approval Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score in valid range but too low for approval |
| **Input Condition** | `loan.CreditScore < 650` (e.g., 649, 600, 300) |
| **Expected Behavior** | Passes range validation (300-900), but rejected at approval: Output "Credit score too low", Status = "Rejected" |
| **Validation Layer** | Line 98: `if (loan.CreditScore < 650)` |
| **Test Type** | Business rules check |
| **Exact Threshold** | Approval requires `>= 650` |
| **Test Cases** | 649, 600, 500, 300 |

#### 2.3.6 Credit Score at Approval Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with credit score exactly at approval minimum |
| **Input Condition** | `loan.CreditScore = 650` |
| **Expected Behavior** | Passes both range and approval threshold; proceeds to DTI check |
| **Validation Layer** | Line 98: Threshold check (boundary exclusive) |
| **Exact Threshold** | `650` is ACCEPTED |

#### 2.3.7 Excellent Credit Score
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with excellent credit score |
| **Input Condition** | `loan.CreditScore = 800` (or higher) |
| **Expected Behavior** | Passes all credit score checks; proceeds to DTI and external service checks |
| **Test Type** | Happy path for credit score |

---

## 3. DEBT-TO-INCOME (DTI) RATIO VALIDATION

### 3.1 DTI Calculation Details
```csharp
// Line 80-82:
decimal monthlyIncome = loan.AnnualIncome / 12;
decimal estimatedMonthlyPayment = loan.LoanAmount * 0.01m;  // 1% of loan amount
decimal dti = estimatedMonthlyPayment / monthlyIncome;

// Line 84:
if (dti > 0.50m)  // Rejects if DTI exceeds 50%
```

### 3.2 DTI Boundary Cases

#### 3.2.1 DTI Just Below Threshold (0.50 Boundary)
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with DTI just below maximum allowed |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 450_000` → DTI = (450000 * 0.01) / (100000/12) = 4500 / 8333.33 = 0.54` |
| **Expected Behavior** | DTI = 0.54 > 0.50 → REJECTED |
| **Note** | This example exceeds 0.50; need calculation that stays just under |
| **Correct Example** | `AnnualIncome = 100_000`, `LoanAmount = 300_000` → DTI = 3000 / 8333.33 = 0.36 → APPROVED |

#### 3.2.2 DTI Exactly at Threshold (0.50)
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with DTI exactly at maximum |
| **Input Condition** | Need: `(LoanAmount * 0.01) / (AnnualIncome / 12) = 0.50` → `LoanAmount * 0.01 * 12 / AnnualIncome = 0.50` → `LoanAmount = AnnualIncome * 0.50 / 0.12` |
|  | Example: `AnnualIncome = 120_000`, `LoanAmount = 500_000` → DTI = 5000 / 10000 = 0.50 |
| **Expected Behavior** | DTI = 0.50 is NOT > 0.50 → APPROVED (boundary is exclusive on upper end) |
| **Validation Layer** | Line 84: `if (dti > 0.50m)` (uses `>`, not `>=`) |
| **Exact Threshold** | `0.50` is ACCEPTED; `0.501` is REJECTED |

#### 3.2.3 DTI Just Above Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with DTI just above maximum |
| **Input Condition** | `AnnualIncome = 120_000`, `LoanAmount = 500_001` → DTI = 5000.01 / 10000 = 0.500001 |
| **Expected Behavior** | DTI > 0.50 → Output "DTI ratio too high", Status = "Rejected", email sent |
| **Validation Layer** | Line 84-95 |

#### 3.2.4 DTI Well Above Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with significantly high DTI |
| **Input Condition** | `AnnualIncome = 12_000`, `LoanAmount = 60_000` → DTI = 600 / 1000 = 0.60 |
| **Expected Behavior** | DTI = 0.60 > 0.50 → REJECTED with audit log |
| **Test Type** | Business rules verification |

#### 3.2.5 DTI with Very Small Income (Division Close to Zero)
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with very small annual income |
| **Input Condition** | `AnnualIncome = 0.01m`, `LoanAmount = 1000` → MonthlyIncome = 0.01/12 ≈ 0.000833 |
| **Expected Behavior** | DTI = (1000 * 0.01) / 0.000833 = 10 / 0.000833 ≈ 12000 → REJECTED (very high DTI) |
| **Potential Bug** | Near-zero division hazard if income approaches 0 |

#### 3.2.6 DTI with Extreme Loan Amount (Overflow Risk)
| Aspect | Details |
|--------|---------|
| **Scenario** | Apply with extremely large loan amount |
| **Input Condition** | `AnnualIncome = 1_000_000`, `LoanAmount = decimal.MaxValue` |
| **Expected Behavior** | `LoanAmount * 0.01m` may overflow decimal |
| **Potential Bug** | Arithmetic overflow not handled |

---

## 4. CREDIT BUREAU EXTERNAL SERVICE

### 4.1 HTTP Dependency Failures

#### 4.1.1 Credit Bureau API Call Succeeds
| Aspect | Details |
|--------|---------|
| **Scenario** | External credit bureau API call succeeds |
| **Input Condition** | Valid applicant name, HTTP request succeeds with 200 status |
| **Expected Behavior** | Logs response, continues to loan amount limit check |
| **Dependency Layer** | Line 115-142: HttpClient call |
| **Notes** | Code uses `.Result` (blocking), not `await` (async) |

#### 4.1.2 Credit Bureau API Returns HTTP Error (Non-Success Status)
| Aspect | Details |
|--------|---------|
| **Scenario** | External API call fails with non-2xx HTTP status |
| **Input Condition** | API returns 404, 500, or other error status |
| **Expected Behavior** | `response.IsSuccessStatusCode = false` → Status = "Manual Review", loan saved, method returns |
| **Validation Layer** | Line 123-129 |
| **Test Type** | Dependency error handling |

#### 4.1.3 Credit Bureau API Throws HttpRequestException
| Aspect | Details |
|--------|---------|
| **Scenario** | HTTP request throws exception (network failure, timeout, invalid URL) |
| **Input Condition** | HttpClient.GetAsync() throws `HttpRequestException`, `TaskCanceledException`, or others |
| **Expected Behavior** | Exception caught at line 136, Status = "Manual Review", audit + SaveLoan, method returns |
| **Validation Layer** | Line 115-142: try-catch block |
| **Test Type** | Dependency exception handling |
| **Specific Cases** |  HttpRequestException, TaskCanceledException (timeout), others |

#### 4.1.4 Credit Bureau API Call Times Out
| Aspect | Details |
|--------|---------|
| **Scenario** | HTTP request times out |
| **Input Condition** | `httpClient.GetAsync(url).Result` timeout (default HttpClient timeout ~100 seconds) |
| **Expected Behavior** | Throws `TaskCanceledException` → Caught as exception → Status = "Manual Review" |
| **Validation Layer** | Line 136-142: catch block |
| **Test Type** | Timeout handling |

#### 4.1.5 Invalid Applicant Name Causes URL Encoding Issue
| Aspect | Details |
|--------|---------|
| **Scenario** | Applicant name contains characters that cause HTTP request failure |
| **Input Condition** | Name like "Jane%2" or "O'Brien" → URL becomes `https://creditbureau.example.com/api/check/Jane%2` |
| **Expected Behavior** | Malformed URL causes HttpRequestException → Status = "Manual Review" |
| **Validation Layer** | Line 117-119: URL construction without encoding |
| **Potential Bug** | Applicant name not URL-encoded; special characters break API call |
| **Test Type** | Edge case dependency failure |

#### 4.1.6 Credit Bureau Returns Null or Empty Response
| Aspect | Details |
|--------|---------|
| **Scenario** | Credit bureau API returns 200 with null/empty body |
| **Input Condition** | `response.Content.ReadAsStringAsync().Result = null` or `""` |
| **Expected Behavior** | Code logs empty response; continues to loan limits check |
| **Validation Layer** | Line 131-134: Response logged but not validated |
| **Test Type** | Unexpected data from dependency |
| **Potential Bug** | No validation of response content; empty response treated as success |

#### 4.1.7 Credit Bureau Response Malformed JSON
| Aspect | Details |
|--------|---------|
| **Scenario** | Credit bureau returns 200 but invalid JSON body |
| **Input Condition** | `response.Content = "invalid json {]}"` |
| **Expected Behavior** | Code logs malformed JSON; silently continues (no JSON parsing in code) |
| **Validation Layer** | Line 131-134 |
| **Test Type** | Unexpected data format |
| **Note** | Code logs response but never parses/validates it |

---

## 5. LOAN AMOUNT LIMIT VALIDATION (By Type)

### 5.1 Loan Type Limits
```csharp
// Line 211-226:
if (loan.Type == LoanType.Home) {
	maximumLoanAmount = loan.AnnualIncome * 5;
} else if (loan.Type == LoanType.Personal) {
	maximumLoanAmount = loan.AnnualIncome * 2;
} else if (loan.Type == LoanType.Auto) {
	maximumLoanAmount = loan.AnnualIncome * 3;
} else if (loan.Type == LoanType.Education) {
	maximumLoanAmount = loan.AnnualIncome * 4;
}

// Line 228-242:
if (loan.LoanAmount > maximumLoanAmount) {
	// REJECT
}
```

### 5.2 Home Loan Limits

#### 5.2.1 Home Loan Within Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Home loan request within 5x annual income limit |
| **Input Condition** | `LoanType = Home`, `AnnualIncome = 100_000`, `LoanAmount = 500_000` |
| **Expected Behavior** | `500_000 <= 500_000` → APPROVED, proceeds to fraud check |
| **Exact Threshold** | Max = Income × 5 |

#### 5.2.2 Home Loan Exactly at Limit (Boundary)
| Aspect | Details |
|--------|---------|
| **Scenario** | Home loan request exactly at maximum |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 500_000` |
| **Expected Behavior** | `500_000 > 500_000` is FALSE → APPROVED |
| **Validation Layer** | Line 228: `if (loan.LoanAmount > maximumLoanAmount)` |
| **Exact Threshold** | `500_000` is ACCEPTED; `500_001` is REJECTED |

#### 5.2.3 Home Loan Exceeding Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Home loan request exceeds 5x limit |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 500_001` |
| **Expected Behavior** | Output "Requested loan amount exceeds maximum limit", Status = "Rejected", email sent |
| **Validation Layer** | Line 228-242 |

### 5.3 Personal Loan Limits

#### 5.3.1 Personal Loan Within Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Personal loan within 2x annual income limit |
| **Input Condition** | `LoanType = Personal`, `AnnualIncome = 100_000`, `LoanAmount = 200_000` |
| **Expected Behavior** | `200_000 <= 200_000` → APPROVED |
| **Exact Threshold** | Max = Income × 2 |

#### 5.3.2 Personal Loan Exactly at Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Personal loan exactly at maximum |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 200_000` |
| **Expected Behavior** | `200_000 > 200_000` is FALSE → APPROVED |
| **Exact Threshold** | `200_000` is ACCEPTED |

#### 5.3.3 Personal Loan Exceeding Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Personal loan exceeds 2x limit |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 200_001` |
| **Expected Behavior** | REJECTED |

### 5.4 Auto Loan Limits

#### 5.4.1 Auto Loan Within Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Auto loan within 3x annual income limit |
| **Input Condition** | `LoanType = Auto`, `AnnualIncome = 100_000`, `LoanAmount = 300_000` |
| **Expected Behavior** | APPROVED |
| **Exact Threshold** | Max = Income × 3 |

#### 5.4.2 Auto Loan Exactly at Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Auto loan exactly at maximum |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 300_000` |
| **Expected Behavior** | `300_000 > 300_000` is FALSE → APPROVED |
| **Exact Threshold** | `300_000` is ACCEPTED |

#### 5.4.3 Auto Loan Exceeding Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Auto loan exceeds 3x limit |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 300_001` |
| **Expected Behavior** | REJECTED |

### 5.5 Education Loan Limits

#### 5.5.1 Education Loan Within Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Education loan within 4x annual income limit |
| **Input Condition** | `LoanType = Education`, `AnnualIncome = 100_000`, `LoanAmount = 400_000` |
| **Expected Behavior** | APPROVED |
| **Exact Threshold** | Max = Income × 4 |

#### 5.5.2 Education Loan Exactly at Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Education loan exactly at maximum |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 400_000` |
| **Expected Behavior** | `400_000 > 400_000` is FALSE → APPROVED |
| **Exact Threshold** | `400_000` is ACCEPTED |

#### 5.5.3 Education Loan Exceeding Limit
| Aspect | Details |
|--------|---------|
| **Scenario** | Education loan exceeds 4x limit |
| **Input Condition** | `AnnualIncome = 100_000`, `LoanAmount = 400_001` |
| **Expected Behavior** | REJECTED |

### 5.6 Unknown Loan Type
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan type not recognized (undefined enum value or edge case) |
| **Input Condition** | `LoanType = (LoanType)99` or invalid enum value |
| **Expected Behavior** | `maximumLoanAmount` remains 0 → Any loan amount > 0 fails limit check |
| **Potential Bug** | Default case not handled; `maximumLoanAmount = 0` means all loans rejected as "exceeding limit" |
| **Test Type** | Edge case handling |

---

## 6. FRAUD DETECTION (Large Loan Flag)

### 6.1 Fraud Threshold Details
```csharp
// Line 245:
if (loan.LoanAmount > 5000000)  // $5 million threshold
{
	// Move to Manual Review
}
```

### 6.2 Fraud Detection Cases

#### 6.2.1 Loan Amount Below Fraud Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Large loan but below fraud threshold |
| **Input Condition** | `LoanAmount = 4_999_999` |
| **Expected Behavior** | `4_999_999 > 5_000_000` is FALSE → APPROVED (passes fraud check) |
| **Exact Threshold** | Max approved = $5,000,000 (boundary exclusive) |

#### 6.2.2 Loan Amount Exactly at Fraud Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan exactly at fraud boundary |
| **Input Condition** | `LoanAmount = 5_000_000` |
| **Expected Behavior** | `5_000_000 > 5_000_000` is FALSE → APPROVED |
| **Validation Layer** | Line 245: Boundary condition (uses `>`, not `>=`) |
| **Exact Threshold** | `5_000_000` is ACCEPTED |

#### 6.2.3 Loan Amount Above Fraud Threshold
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan exceeds fraud detection threshold |
| **Input Condition** | `LoanAmount = 5_000_001` |
| **Expected Behavior** | Status = "Manual Review", output "Large loan amount - fraud review required", audit log created |
| **Validation Layer** | Line 245-258 |
| **Test Type** | Fraud prevention business rule |

#### 6.2.4 Extremely Large Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan far above fraud threshold |
| **Input Condition** | `LoanAmount = decimal.MaxValue` (or 1 billion+) |
| **Expected Behavior** | Status = "Manual Review" |
| **Test Type** | Extreme value handling |

---

## 7. INTEREST RATE CALCULATION

### 7.1 Interest Rate Rules
```csharp
// Line 147-186:
// Home: 7.5% base, -0.25% for existing customer
// Personal: 11.5% base, -0.50% for existing customer
// Auto: 9.25% base, -0.25% for existing customer
// Education: 6.5% base, -0.50% for existing customer
// Unknown: 15.0% default
```

### 7.2 Interest Rate Calculation Cases

#### 7.2.1 Home Loan - New Customer
| Aspect | Details |
|--------|---------|
| **Scenario** | Home loan for non-existing customer |
| **Input Condition** | `LoanType = Home`, `IsExistingCustomer = false` |
| **Expected Behavior** | Interest rate = 7.5% |
| **Validation Layer** | Line 147-154 |

#### 7.2.2 Home Loan - Existing Customer
| Aspect | Details |
|--------|---------|
| **Scenario** | Home loan for existing customer |
| **Input Condition** | `LoanType = Home`, `IsExistingCustomer = true` |
| **Expected Behavior** | Interest rate = 7.25% (7.5% - 0.25% discount) |
| **Validation Layer** | Line 151-153 |

#### 7.2.3-7.2.8 All Other Loan Type Combinations
Similar pattern for Personal (11.5%/11.0%), Auto (9.25%/9.0%), Education (6.5%/6.0%)

#### 7.2.9 Unknown Loan Type
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan type not matched to any category |
| **Input Condition** | `LoanType = (LoanType)99` or unrecognized value |
| **Expected Behavior** | Interest rate = 15.0% (default fallback) |
| **Validation Layer** | Line 183-186 |
| **Test Type** | Fallback/default behavior |

---

## 8. PROCESSING FEE CALCULATION

### 8.1 Fee Rules
```csharp
// Line 191-206:
// Home: 0.5% of loan amount
// Personal: 2.0% of loan amount
// Auto: 1.0% of loan amount
// Education: 0.25% of loan amount
// Unknown: 0% default (not explicitly set)
```

### 8.2 Processing Fee Cases

#### 8.2.1 Home Loan Fee Calculation
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate fee for home loan |
| **Input Condition** | `LoanAmount = 500_000`, `LoanType = Home` |
| **Expected Behavior** | Fee = 500_000 * 0.005 = 2,500 |
| **Exact Formula** | Fee = LoanAmount × 0.005 |

#### 8.2.2 Personal Loan Fee Calculation
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate fee for personal loan |
| **Input Condition** | `LoanAmount = 200_000`, `LoanType = Personal` |
| **Expected Behavior** | Fee = 200_000 * 0.02 = 4,000 |
| **Exact Formula** | Fee = LoanAmount × 0.02 |

#### 8.2.3 Auto Loan Fee Calculation
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate fee for auto loan |
| **Input Condition** | `LoanAmount = 300_000`, `LoanType = Auto` |
| **Expected Behavior** | Fee = 300_000 * 0.01 = 3,000 |
| **Exact Formula** | Fee = LoanAmount × 0.01 |

#### 8.2.4 Education Loan Fee Calculation
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate fee for education loan |
| **Input Condition** | `LoanAmount = 400_000`, `LoanType = Education` |
| **Expected Behavior** | Fee = 400_000 * 0.0025 = 1,000 |
| **Exact Formula** | Fee = LoanAmount × 0.0025 |

#### 8.2.5 Unknown Loan Type Fee
| Aspect | Details |
|--------|---------|
| **Scenario** | Fee for unknown loan type |
| **Input Condition** | `LoanType = (LoanType)99` |
| **Expected Behavior** | `processingFee` remains 0 (not explicitly set) |
| **Potential Bug** | Default fee is 0, which is counterintuitive |

---

## 9. MONTHLY STATEMENT GENERATION

### 9.1 Statement Generation Rules
```csharp
// Line 383-420:
// Only generates if loan.Status = "Approved"
// Calculates monthly interest = LoanAmount * (InterestRate / 100) / 12
// Sends email notification
```

### 9.2 Monthly Statement Cases

#### 9.2.1 Statement for Approved Loan
| Aspect | Details |
|--------|---------|
| **Scenario** | Generate statement for approved loan |
| **Input Condition** | `loan.Status = "Approved"` |
| **Expected Behavior** | Statement generated with loan details, monthly interest calculated, email sent |
| **Validation Layer** | Line 390: Guard clause check |

#### 9.2.2 Statement for Rejected Loan
| Aspect | Details |
|--------|---------|
| **Scenario** | Attempt to generate statement for rejected loan |
| **Input Condition** | `loan.Status = "Rejected"` |
| **Expected Behavior** | Output "Statement cannot be generated.", method returns without processing |
| **Validation Layer** | Line 390 |
| **Guard Clause** | Early return, no email sent |

#### 9.2.3 Statement for Manual Review Loan
| Aspect | Details |
|--------|---------|
| **Scenario** | Attempt to generate statement for manual review loan |
| **Input Condition** | `loan.Status = "Manual Review"` |
| **Expected Behavior** | Output "Statement cannot be generated." |
| **Guard Clause** | Early return |

#### 9.2.4 Statement for Pending Loan
| Aspect | Details |
|--------|---------|
| **Scenario** | Attempt to generate statement for pending loan |
| **Input Condition** | `loan.Status = "Pending"` (or any non-"Approved" status) |
| **Expected Behavior** | Guard clause rejects, output "Statement cannot be generated." |

#### 9.2.5 Statement with Null Status
| Aspect | Details |
|--------|---------|
| **Scenario** | Generate statement for loan with null status |
| **Input Condition** | `loan.Status = null` |
| **Expected Behavior** | `null != "Approved"` is true → Statement cannot be generated |
| **Potential Bug** | No null check; relies on != behavior |

#### 9.2.6 Statement with Empty Status String
| Aspect | Details |
|--------|---------|
| **Scenario** | Generate statement for loan with empty status |
| **Input Condition** | `loan.Status = ""` |
| **Expected Behavior** | `"" != "Approved"` is true → Statement cannot be generated |

#### 9.2.7 Statement Status String Case Sensitivity
| Aspect | Details |
|--------|---------|
| **Scenario** | Status string different case |
| **Input Condition** | `loan.Status = "approved"` (lowercase) |
| **Expected Behavior** | `"approved" != "Approved"` is true (case mismatch) → Statement NOT generated |
| **Potential Bug** | Case-sensitive string comparison; "approved" ≠ "Approved" |
| **Test Type** | String comparison edge case |

---

## 10. MONTHLY INTEREST CALCULATION

### 10.1 Monthly Interest Calculation
```csharp
// Line 418-421:
decimal monthlyInterest = loan.LoanAmount * (interestRate / 100) / 12;
```

### 10.2 Monthly Interest Cases

#### 10.2.1 Standard Monthly Interest (Approved Loan)
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate monthly interest for approved loan |
| **Input Condition** | `LoanAmount = 12_000`, `InterestRate = 9.0%` |
| **Expected Behavior** | Monthly Interest = 12_000 * (9.0 / 100) / 12 = 90 |
| **Formula** | Monthly Interest = Loan Amount × (Annual Rate / 100) / 12 |

#### 10.2.2 Monthly Interest with Zero Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate interest for zero loan (shouldn't reach here due to earlier validation) |
| **Input Condition** | `LoanAmount = 0` |
| **Expected Behavior** | Monthly Interest = 0 |
| **Note** | Zero loan should be rejected earlier; this is theoretical |

#### 10.2.3 Monthly Interest with Extreme Loan Amount
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate interest for very large loan |
| **Input Condition** | `LoanAmount = decimal.MaxValue`, `InterestRate = 7.5%` |
| **Expected Behavior** | Arithmetic overflow possible |
| **Potential Bug** | No overflow protection |

#### 10.2.4 Monthly Interest with Different Interest Rates
| Aspect | Details |
|--------|---------|
| **Scenario** | Calculate interest with all possible interest rates |
| **Input Condition** | Rates: 7.25, 7.5, 9.0, 9.25, 11.0, 11.5, 6.0, 6.5, 15.0 |
| **Expected Behavior** | Correct calculation for each rate |

---

## 11. FILE I/O & SIDE EFFECTS

### 11.1 Audit File Creation

#### 11.1.1 Audit File Created on Loan Decision
| Aspect | Details |
|--------|---------|
| **Scenario** | Loan decision logged to audit file |
| **Input Condition** | Loan rejected or moved to manual review |
| **Expected Behavior** | File `loan-audit.txt` created with decision log |
| **Validation Layer** | Line 252-255, 308-314, 343-346 |
| **File Location** | Current working directory |
| **Potential Bug** | Hardcoded filename; no error handling if file write fails |

#### 11.1.2 Audit File Append Multiple Entries
| Aspect | Details |
|--------|---------|
| **Scenario** | Multiple loans processed create multiple audit entries |
| **Input Condition** | Process multiple loans sequentially |
| **Expected Behavior** | Audit file appended with each entry |
| **Test Type** | Multiple request handling |

#### 11.1.3 Audit File Permission Denied
| Aspect | Details |
|--------|---------|
| **Scenario** | Write to audit file fails due to permissions |
| **Input Condition** | File locked or no write permissions |
| **Expected Behavior** | Exception thrown, unhandled |
| **Potential Bug** | No exception handling for file I/O |
| **Test Type** | Dependency failure (file system) |

### 11.2 Loan Document Creation

#### 11.2.1 JSON Loan Document Generated
| Aspect | Details |
|--------|---------|
| **Scenario** | Approved loan generates JSON document |
| **Input Condition** | Loan approved |
| **Expected Behavior** | File `loan-{id}.json` created with loan details |
| **Validation Layer** | Line 283-287, 349-381 |
| **File Contents** | JSON with LoanId, Applicant, Amount, InterestRate, ProcessingFee, Status, GeneratedAt |

#### 11.2.2 JSON Document File Overwrite
| Aspect | Details |
|--------|---------|
| **Scenario** | Process same loan twice (same ID) |
| **Input Condition** | Loan with ID = 1 processed, then processed again |
| **Expected Behavior** | File `loan-1.json` overwritten |
| **Test Type** | Duplicate request handling |
| **Potential Bug** | No version control or backup of previous document |

---

## 12. EMAIL NOTIFICATIONS

### 12.1 Email Sending (Mocked in Tests)

#### 12.1.1 Rejection Email Sent
| Aspect | Details |
|--------|---------|
| **Scenario** | Email sent on loan rejection |
| **Input Condition** | Loan rejected (DTI, credit score, or amount limit) |
| **Expected Behavior** | Email to applicant with rejection reason |
| **Validation Layer** | Line 89-93, 105-109, 235-239 |

#### 12.1.2 Approval Email Sent
| Aspect | Details |
|--------|---------|
| **Scenario** | Email sent on loan approval |
| **Input Condition** | Loan approved |
| **Expected Behavior** | Email to applicant with interest rate and processing fee |
| **Validation Layer** | Line 273-281 |

#### 12.1.3 Monthly Statement Email Sent
| Aspect | Details |
|--------|---------|
| **Scenario** | Email sent with monthly statement |
| **Input Condition** | GenerateMonthlyStatement() called for approved loan |
| **Expected Behavior** | Email sent with monthly interest details |
| **Validation Layer** | Line 447-452 |

#### 12.1.4 Email to Invalid Address
| Aspect | Details |
|--------|---------|
| **Scenario** | Send email to invalid email address |
| **Input Condition** | Email = "notanemail" (no validation conducted) |
| **Expected Behavior** | Email send attempted (no validation in code) |
| **Potential Bug** | No email format validation |
| **Test Type** | Dependency data validation |

#### 12.1.5 Email Address Is Null or Empty
| Aspect | Details |
|--------|---------|
| **Scenario** | Attempt to send email with null/empty address |
| **Input Condition** | Email = null or "" (caught by earlier validation) |
| **Expected Behavior** | Should not reach email sending (rejected earlier), but if it did, exception likely |
| **Test Type** | Null data handling |

---

## 13. DATABASE INTERACTIONS (Hard-Coded)

### 13.1 Connection String & SaveLoan

#### 13.1.1 Hard-Coded Connection String
| Aspect | Details |
|--------|---------|
| **Scenario** | Database connection| **Input Condition** | Line 33-34: Hard-coded credentials |
| **Expected Behavior** | Connection string: `"Server=localhost;Database=Loans;User Id=sa;Password=Password123;"` |
| **Security Issue** | Credentials exposed in source code |
| **Potential Bug** | Connection will fail if SQL Server not available |

#### 13.1.2 SaveLoan Connection Failure
| Aspect | Details |
|--------|---------|
| **Scenario** | Database connection fails |
| **Input Condition** | SQL Server unavailable or wrong credentials |
| **Expected Behavior** | Exception thrown, unhandled |
| **Potential Bug** | No exception handling for database operations |
| **Test Type** | Dependency failure (database) |

#### 13.1.3 SaveLoan SQL Injection Risk
| Aspect | Details |
|--------|---------|
| **Scenario** | Applicant name contains SQL injection payload |
| **Input Condition** | ApplicantName = `"'; DROP TABLE Loans; --"` |
| **Expected Behavior** | SQL injection executed (vulnerable code) |
| **Security Issue** | SQL injection vulnerability |
| **Severity** | Critical |

---

## SUMMARY: Total Test Cases by Category

| Category | Count | High Priority? |
|----------|-------|---|
| Null/Empty Input | 2 | ✅ Yes |
| Annual Income Boundaries | 4 | ✅ Yes |
| Loan Amount Boundaries | 4 | ✅ Yes |
| Credit Score Boundaries | 7 | ✅ Yes |
| DTI Ratio Boundaries | 6 | ✅ Yes |
| Credit Bureau Failures | 7 | ✅ Yes |
| Loan Type Limits | 11 | ✅ Yes |
| Fraud Detection | 4 | ✅ Yes |
| Interest Rate Calculations | 9 | ✅ Yes |
| Processing Fee Calculations | 5 | ✅ Yes |
| Monthly Statements | 7 | ✅ Yes |
| Monthly Interest | 4 | ✅ Yes |
| File I/O & Audit | 3 | ✅ Yes |
| Email Notifications | 5 | ✅ Yes |
| Database (Hard-coded) | 3 | ✅ Yes (Security) |
| **TOTAL** | **82** | **Comprehensive** |

---

## Key Testing Priorities

1. **Boundary Conditions** (Most Critical)
   - All threshold checks: Credit score (300, 650, 900), DTI (0.50), Loan limits, Fraud ($5M)
   - Exact equality cases (000) vs. inequality (>)

2. **Dependency Failures** (Critical for Robustness)
   - HTTP timeouts, non-2xx responses, exceptions
   - File I/O errors, database connection failures

3. **Null/Invalid Data**
   - Null objects, empty strings, negative numbers
   - Unexpected string formats (case sensitivity, special characters)

4. **Security Issues**
   - SQL injection risks
   - Hard-coded credentials
   - Email validation

5. **Edge Cases**
   - Arithmetic overflow (decimal.MaxValue)
   - Near-zero division (very small income)
   - Unknown enum values

---

**Next Step:** Implement executable test cases using this specification
