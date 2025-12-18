-- Create a new admin user with simple credentials
USE BibliotekaDB;
GO

-- Delete old admin if exists
DELETE FROM Users WHERE Email = 'admin@test.com';
DELETE FROM Users WHERE Email = 'admin@biblioteka.com';
GO

-- Create new admin with very simple credentials
INSERT INTO Users (FirstName, LastName, Email, Password, Phone, Address, RegistrationDate, IsActive, Role)
VALUES ('Admin', 'System', 'admin', 'admin', '+355 69 0000000', 'Tiranë', GETDATE(), 1, 'Admin');
GO

-- Verify the new admin
SELECT 
    'New Admin Created' as Status,
    Email,
    Password,
    Role,
    IsActive,
    FirstName,
    LastName
FROM Users 
WHERE Email = 'admin';
GO

PRINT '';
PRINT '========================================';
PRINT 'NEW ADMIN CREDENTIALS:';
PRINT 'Email: admin';
PRINT 'Password: admin';
PRINT 'Role: Admin';
PRINT '========================================';
GO

