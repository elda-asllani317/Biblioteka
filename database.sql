-- =============================================
-- Biblioteka Database Script
-- Sistem i menaxhimit të bibliotekës
-- =============================================

-- Krijimi i databazës
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BibliotekaDB')
BEGIN
    CREATE DATABASE BibliotekaDB;
END
GO

USE BibliotekaDB;
GO

-- =============================================
-- Tabela: Authors
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Authors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Authors] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Biography] NVARCHAR(MAX) NOT NULL,
        [DateOfBirth] DATETIME2 NULL,
        [Nationality] NVARCHAR(MAX) NOT NULL
    );
END
GO

-- =============================================
-- Tabela: Categories
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL UNIQUE,
        [Description] NVARCHAR(MAX) NOT NULL
    );
END
GO

-- =============================================
-- Tabela: Publishers
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Publishers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Publishers] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(MAX) NOT NULL,
        [Phone] NVARCHAR(MAX) NOT NULL,
        [Email] NVARCHAR(MAX) NOT NULL
    );
END
GO

-- =============================================
-- Tabela: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [Phone] NVARCHAR(MAX) NOT NULL,
        [Address] NVARCHAR(MAX) NOT NULL,
        [RegistrationDate] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
    
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
END
GO

-- =============================================
-- Tabela: Books
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Books]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Books] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(500) NOT NULL,
        [ISBN] NVARCHAR(20) NOT NULL UNIQUE,
        [Description] NVARCHAR(MAX) NOT NULL,
        [PublicationYear] INT NOT NULL,
        [Pages] INT NOT NULL,
        [Language] NVARCHAR(MAX) NOT NULL,
        [AuthorId] INT NOT NULL,
        [CategoryId] INT NOT NULL,
        [PublisherId] INT NOT NULL,
        CONSTRAINT [FK_Books_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Authors]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Books_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Books_Publishers_PublisherId] FOREIGN KEY ([PublisherId]) REFERENCES [dbo].[Publishers]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_Books_AuthorId] ON [dbo].[Books]([AuthorId]);
    CREATE INDEX [IX_Books_CategoryId] ON [dbo].[Books]([CategoryId]);
    CREATE INDEX [IX_Books_PublisherId] ON [dbo].[Books]([PublisherId]);
    CREATE UNIQUE INDEX [IX_Books_ISBN] ON [dbo].[Books]([ISBN]);
END
GO

-- =============================================
-- Tabela: BookCopies
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BookCopies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BookCopies] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CopyNumber] NVARCHAR(50) NOT NULL,
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        [Condition] NVARCHAR(MAX) NOT NULL,
        [PurchaseDate] DATETIME2 NOT NULL,
        [BookId] INT NOT NULL,
        CONSTRAINT [FK_BookCopies_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_BookCopies_BookId] ON [dbo].[BookCopies]([BookId]);
END
GO

-- =============================================
-- Tabela: Loans
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Loans]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Loans] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [LoanDate] DATETIME2 NOT NULL,
        [DueDate] DATETIME2 NOT NULL,
        [ReturnDate] DATETIME2 NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
        [UserId] INT NOT NULL,
        [BookCopyId] INT NOT NULL,
        CONSTRAINT [FK_Loans_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Loans_BookCopies_BookCopyId] FOREIGN KEY ([BookCopyId]) REFERENCES [dbo].[BookCopies]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_Loans_UserId] ON [dbo].[Loans]([UserId]);
    CREATE INDEX [IX_Loans_BookCopyId] ON [dbo].[Loans]([BookCopyId]);
END
GO

-- =============================================
-- Tabela: Reviews
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Reviews] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Rating] INT NOT NULL,
        [Comment] NVARCHAR(MAX) NOT NULL,
        [ReviewDate] DATETIME2 NOT NULL,
        [UserId] INT NOT NULL,
        [BookId] INT NOT NULL,
        CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books]([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_Reviews_UserId] ON [dbo].[Reviews]([UserId]);
    CREATE INDEX [IX_Reviews_BookId] ON [dbo].[Reviews]([BookId]);
END
GO

-- =============================================
-- Tabela: Fines
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Fines]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Fines] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Reason] NVARCHAR(MAX) NOT NULL,
        [IssueDate] DATETIME2 NOT NULL,
        [PaymentDate] DATETIME2 NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [UserId] INT NOT NULL,
        [LoanId] INT NULL,
        CONSTRAINT [FK_Fines_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Fines_Loans_LoanId] FOREIGN KEY ([LoanId]) REFERENCES [dbo].[Loans]([Id]) ON DELETE SET NULL
    );
    
    CREATE INDEX [IX_Fines_UserId] ON [dbo].[Fines]([UserId]);
    CREATE INDEX [IX_Fines_LoanId] ON [dbo].[Fines]([LoanId]);
END
GO

-- =============================================
-- Tabela: Notifications
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Notifications] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [UserId] INT NOT NULL,
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Notifications_UserId] ON [dbo].[Notifications]([UserId]);
END
GO

-- =============================================
-- Të dhëna fillestare (Sample Data)
-- =============================================

-- Shtimi i autorëve
IF NOT EXISTS (SELECT * FROM [dbo].[Authors] WHERE [FirstName] = 'Ismail' AND [LastName] = 'Kadare')
BEGIN
    INSERT INTO [dbo].[Authors] ([FirstName], [LastName], [Biography], [DateOfBirth], [Nationality])
    VALUES 
    ('Ismail', 'Kadare', 'Shkrimtar shqiptar i njohur botërisht', '1936-01-28', 'Shqiptar'),
    ('Dritëro', 'Agolli', 'Poet dhe shkrimtar shqiptar', '1931-10-13', 'Shqiptar');
END
GO

-- Shtimi i kategorive
IF NOT EXISTS (SELECT * FROM [dbo].[Categories] WHERE [Name] = 'Fiction')
BEGIN
    INSERT INTO [dbo].[Categories] ([Name], [Description])
    VALUES 
    ('Fiction', 'Letërsi artistike'),
    ('Non-Fiction', 'Letërsi jo-artistike'),
    ('Science', 'Shkencë'),
    ('History', 'Histori'),
    ('Biography', 'Biografi');
END
GO

-- Shtimi i botuesve
IF NOT EXISTS (SELECT * FROM [dbo].[Publishers] WHERE [Name] = 'Onufri')
BEGIN
    INSERT INTO [dbo].[Publishers] ([Name], [Address], [Phone], [Email])
    VALUES 
    ('Onufri', 'Tiranë, Shqipëri', '+355 4 1234567', 'info@onufri.com'),
    ('Toena', 'Tiranë, Shqipëri', '+355 4 7654321', 'info@toena.com');
END
GO

-- Shtimi i përdoruesve
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Email] = 'john.doe@example.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FirstName], [LastName], [Email], [Phone], [Address], [RegistrationDate], [IsActive])
    VALUES 
    ('John', 'Doe', 'john.doe@example.com', '+355 69 1234567', 'Tiranë, Shqipëri', GETDATE(), 1),
    ('Jane', 'Smith', 'jane.smith@example.com', '+355 69 7654321', 'Durrës, Shqipëri', GETDATE(), 1);
END
GO

PRINT 'Databaza BibliotekaDB është krijuar me sukses!';
PRINT 'Të gjitha tabelat janë krijuar dhe të dhënat fillestare janë shtuar.';
GO

