-- Add Role column to Users table
USE BibliotekaDB;
GO

-- Check if Role column exists, if not add it
IF COL_LENGTH('dbo.Users', 'Role') IS NULL
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [Role] NVARCHAR(50) NOT NULL DEFAULT 'User';
    PRINT 'Role column added successfully.';
END
ELSE
BEGIN
    PRINT 'Role column already exists.';
END
GO

-- Update existing users to have default role 'User'
UPDATE [dbo].[Users] 
SET [Role] = 'User' 
WHERE [Role] IS NULL OR [Role] = '';
GO

-- Create an admin user if it doesn't exist
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Email] = 'admin@biblioteka.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FirstName], [LastName], [Email], [Password], [Phone], [Address], [RegistrationDate], [IsActive], [Role])
    VALUES ('Admin', 'User', 'admin@biblioteka.com', 'admin123', '+355 69 0000000', 'Tiranë, Shqipëri', GETDATE(), 1, 'Admin');
    PRINT 'Admin user created: admin@biblioteka.com / admin123';
END
ELSE
BEGIN
    -- Update existing admin user to have Admin role
    UPDATE [dbo].[Users] 
    SET [Role] = 'Admin' 
    WHERE [Email] = 'admin@biblioteka.com';
    PRINT 'Admin user role updated.';
END
GO

-- Update test users to have User role
UPDATE [dbo].[Users] 
SET [Role] = 'User' 
WHERE [Email] IN ('john.doe@example.com', 'jane.smith@example.com');
GO

PRINT 'Role column setup completed.';
PRINT 'Admin credentials: admin@biblioteka.com / admin123';
GO

