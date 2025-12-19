-- Fix Admin User - Ensure admin exists with correct credentials
USE BibliotekaDB;
GO

-- Check if admin exists
IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Email] = 'admin@biblioteka.com')
BEGIN
    -- Update existing admin
    UPDATE [dbo].[Users] 
    SET 
        [Password] = 'admin123',
        [Role] = 'Admin',
        [IsActive] = 1,
        [FirstName] = 'Admin',
        [LastName] = 'User'
    WHERE [Email] = 'admin@biblioteka.com';
    
    PRINT 'Admin user updated successfully.';
    PRINT 'Email: admin@biblioteka.com';
    PRINT 'Password: admin123';
    PRINT 'Role: Admin';
END
ELSE
BEGIN
    -- Create admin if doesn't exist
    INSERT INTO [dbo].[Users] ([FirstName], [LastName], [Email], [Password], [Phone], [Address], [RegistrationDate], [IsActive], [Role])
    VALUES ('Admin', 'User', 'admin@biblioteka.com', 'admin123', '+355 69 0000000', 'Tiranë, Shqipëri', GETDATE(), 1, 'Admin');
    
    PRINT 'Admin user created successfully.';
    PRINT 'Email: admin@biblioteka.com';
    PRINT 'Password: admin123';
    PRINT 'Role: Admin';
END
GO

-- Verify admin user
SELECT 
    Email,
    Password,
    Role,
    IsActive,
    FirstName,
    LastName
FROM [dbo].[Users] 
WHERE [Email] = 'admin@biblioteka.com';
GO

PRINT 'Admin user verification completed.';
GO

