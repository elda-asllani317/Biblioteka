-- Direct test of admin login
USE BibliotekaDB;
GO

-- Verify admin exists and check exact values
SELECT 
    'Admin Verification' as Test,
    Email,
    Password,
    LEN(Password) as PasswordLength,
    Role,
    IsActive,
    CASE 
        WHEN Email = 'admin@biblioteka.com' THEN 'Email OK'
        ELSE 'Email WRONG'
    END as EmailCheck,
    CASE 
        WHEN Password = 'admin123' THEN 'Password OK'
        ELSE 'Password WRONG'
    END as PasswordCheck,
    CASE 
        WHEN Role = 'Admin' THEN 'Role OK'
        ELSE 'Role WRONG'
    END as RoleCheck,
    CASE 
        WHEN IsActive = 1 THEN 'Active OK'
        ELSE 'Active WRONG'
    END as ActiveCheck
FROM Users 
WHERE Email = 'admin@biblioteka.com';
GO

-- If admin doesn't exist or is wrong, recreate it
IF NOT EXISTS (
    SELECT * FROM Users 
    WHERE Email = 'admin@biblioteka.com' 
    AND Password = 'admin123' 
    AND Role = 'Admin' 
    AND IsActive = 1
)
BEGIN
    PRINT 'Admin is incorrect. Fixing...';
    
    -- Delete existing admin
    DELETE FROM Users WHERE Email = 'admin@biblioteka.com';
    
    -- Create correct admin
    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, Address, RegistrationDate, IsActive, Role)
    VALUES ('Admin', 'User', 'admin@biblioteka.com', 'admin123', '+355 69 0000000', 'Tiranë', GETDATE(), 1, 'Admin');
    
    PRINT 'Admin fixed successfully!';
END
ELSE
BEGIN
    PRINT 'Admin is correct!';
END
GO

-- Final check
SELECT 
    Email,
    Password,
    Role,
    IsActive
FROM Users 
WHERE Email = 'admin@biblioteka.com';
GO

