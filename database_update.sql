-- Update existing users with passwords for testing
-- Default password: password123

USE BibliotekaDB;
GO

-- Sigurohu që kolona Password ekziston në tabelën Users
IF COL_LENGTH('dbo.Users', 'Password') IS NULL
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [Password] NVARCHAR(255) NULL;
END
GO

UPDATE [dbo].[Users] 
SET [Password] = 'password123' 
WHERE [Email] = 'john.doe@example.com' OR [Email] = 'jane.smith@example.com';
GO

-- Add more test users if needed
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Email] = 'admin@biblioteka.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FirstName], [LastName], [Email], [Password], [Phone], [Address], [RegistrationDate], [IsActive])
    VALUES ('Admin', 'User', 'admin@biblioteka.com', 'password123', '+355 69 0000000', 'Tiranë, Shqipëri', GETDATE(), 1);
END
GO

PRINT 'Databaza është përditësuar me sukses!';
PRINT 'Përdoruesit e testit:';
PRINT 'Email: john.doe@example.com, Password: password123';
PRINT 'Email: jane.smith@example.com, Password: password123';
PRINT 'Email: admin@biblioteka.com, Password: password123';
GO

