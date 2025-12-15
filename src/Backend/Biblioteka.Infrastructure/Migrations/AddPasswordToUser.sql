-- Add Password column to Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [Password] NVARCHAR(MAX) NOT NULL DEFAULT '';
END
GO

-- Update existing users with default password (in production, use hashed passwords)
UPDATE [dbo].[Users] SET [Password] = 'password123' WHERE [Password] = '' OR [Password] IS NULL;
GO

