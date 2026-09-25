SELECT
    c.CustomerId,
    c.FirstName,
    c.LastName,
    c.Email,
    c.Phone,
    c.Address,
    c.DateOfBirth,
    l.LoanId,
    l.LoanAmount,
    l.InterestRate,
    l.LoanStatus,
    l.ApplicationDate,
    b.BranchName,
    b.BranchAddress,
    t.TransactionId,
    t.TransactionDate,
    t.TransactionAmount,
    t.TransactionType
FROM Customers c
INNER JOIN Loans l
    ON c.CustomerId = l.CustomerId
INNER JOIN Branches b
    ON l.BranchId = b.BranchId
LEFT JOIN Transactions t
    ON c.CustomerId = t.CustomerId
WHERE
    YEAR(l.ApplicationDate) = 2025
    AND LOWER(l.LoanStatus) = 'approved'
    AND c.CustomerId IN (
        SELECT CustomerId
        FROM Transactions
        WHERE TransactionAmount > 10000
    )
ORDER BY
    c.LastName,
    c.FirstName,
    l.ApplicationDate DESC;
