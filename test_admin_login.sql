-- Test Admin Login - Verify admin credentials
USE BibliotekaDB;
GO

-- Check admin user
SELECT 
    'Admin User Check' as Test,
    Email,
    Password,
    Role,
    IsActive,
    FirstName,
    LastName,
    CASE 
        WHEN Email = 'admin@biblioteka.com' AND Password = 'admin123' AND Role = 'Admin' AND IsActive = 1 
        THEN '✅ Admin credentials are correct'
        ELSE '❌ Admin credentials are incorrect'
    END as Status
FROM Users 
WHERE Email = 'admin@biblioteka.com';
GO

-- If admin doesn't exist or is incorrect, fix it
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@biblioteka.com' AND Password = 'admin123' AND Role = 'Admin')
BEGIN
    -- Delete existing admin if wrong
    DELETE FROM Users WHERE Email = 'admin@biblioteka.com';
    
    -- Create correct admin
    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, Address, RegistrationDate, IsActive, Role)
    VALUES ('Admin', 'User', 'admin@biblioteka.com', 'admin123', '+355 69 0000000', 'Tiranë, Shqipëri', GETDATE(), 1, 'Admin');
    
    PRINT 'Admin user created/fixed successfully!';
END
ELSE
BEGIN
    PRINT 'Admin user is correct!';
END
GO

-- Final verification
SELECT 
    Email,
    Password,
    Role,
    IsActive
FROM Users 
WHERE Email = 'admin@biblioteka.com';
GO

PRINT '';
PRINT '========================================';
PRINT 'Admin Login Credentials:';
PRINT 'Email: admin@biblioteka.com';
PRINT 'Password: admin123';
PRINT 'Role: Admin';
PRINT '========================================';
GO

