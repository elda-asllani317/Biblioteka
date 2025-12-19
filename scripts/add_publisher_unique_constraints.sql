-- Add unique constraints to Publishers table
-- This script adds unique constraints for Name and Email columns

USE BibliotekaDB;
GO

-- Check if unique index on Name doesn't exist, then create it
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Publishers_Name' AND object_id = OBJECT_ID('dbo.Publishers'))
BEGIN
    -- First, check for duplicate names
    IF EXISTS (SELECT Name, COUNT(*) as cnt FROM dbo.Publishers GROUP BY Name HAVING COUNT(*) > 1)
    BEGIN
        PRINT 'WARNING: Duplicate names found. Please resolve duplicates before adding unique constraint.';
        SELECT Name, COUNT(*) as cnt FROM dbo.Publishers GROUP BY Name HAVING COUNT(*) > 1;
    END
    ELSE
    BEGIN
        CREATE UNIQUE INDEX IX_Publishers_Name ON dbo.Publishers(Name);
        PRINT 'Unique index IX_Publishers_Name created successfully.';
    END
END
ELSE
BEGIN
    PRINT 'Index IX_Publishers_Name already exists.';
END
GO

-- Check if unique index on Email doesn't exist, then create it
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Publishers_Email' AND object_id = OBJECT_ID('dbo.Publishers'))
BEGIN
    -- First, check for duplicate emails
    IF EXISTS (SELECT Email, COUNT(*) as cnt FROM dbo.Publishers GROUP BY Email HAVING COUNT(*) > 1)
    BEGIN
        PRINT 'WARNING: Duplicate emails found. Please resolve duplicates before adding unique constraint.';
        SELECT Email, COUNT(*) as cnt FROM dbo.Publishers GROUP BY Email HAVING COUNT(*) > 1;
    END
    ELSE
    BEGIN
        CREATE UNIQUE INDEX IX_Publishers_Email ON dbo.Publishers(Email);
        PRINT 'Unique index IX_Publishers_Email created successfully.';
    END
END
ELSE
BEGIN
    PRINT 'Index IX_Publishers_Email already exists.';
END
GO

-- Update Email column to be NOT NULL if it's nullable
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Publishers' 
           AND COLUMN_NAME = 'Email' 
           AND IS_NULLABLE = 'YES')
BEGIN
    -- First, update any NULL emails
    UPDATE dbo.Publishers SET Email = 'unknown' + CAST(Id AS NVARCHAR(10)) + '@example.com' WHERE Email IS NULL;
    ALTER TABLE dbo.Publishers ALTER COLUMN Email NVARCHAR(255) NOT NULL;
    PRINT 'Email column updated to NOT NULL.';
END
GO

PRINT 'Publisher unique constraints script completed.';
GO

