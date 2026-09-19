IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Roles] (
    [RoleId] int NOT NULL IDENTITY,
    [RoleName] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
);

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Username] nvarchar(30) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

CREATE TABLE [Customers] (
    [CustomerId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [DateOfBirth] date NOT NULL,
    [Street] nvarchar(100) NOT NULL,
    [City] nvarchar(50) NOT NULL,
    [State] nvarchar(2) NOT NULL,
    [Country] nvarchar(50) NOT NULL,
    [ZipCode] nvarchar(10) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [PhoneNumber] nvarchar(10) NOT NULL,
    [Email] nvarchar(254) NOT NULL,
    [SocialSecurityNumberLastFour] nvarchar(4) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId]),
    CONSTRAINT [FK_Customers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [UserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [Accounts] (
    [AccountId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [AccountNumber] nvarchar(20) NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [OpenedDate] datetime2 NOT NULL,
    [AccountKind] nvarchar(20) NOT NULL,
    [OverdraftLimit] decimal(18,2) NULL,
    [AnnualInterestRate] decimal(9,6) NULL,
    [MinimumBalance] decimal(18,2) NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId]),
    CONSTRAINT [FK_Accounts_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE NO ACTION
);

CREATE TABLE [ServiceRequests] (
    [RequestId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [RequestedDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [RequestKind] nvarchar(30) NOT NULL,
    [AccountId] int NULL,
    [StartDate] date NULL,
    [EndDate] date NULL,
    [Reason] int NULL,
    [NewAddressStreet] nvarchar(100) NULL,
    [NewAddressCity] nvarchar(50) NULL,
    [NewAddressState] nvarchar(2) NULL,
    [NewAddressCountry] nvarchar(50) NULL,
    [NewAddressZipCode] nvarchar(10) NULL,
    CONSTRAINT [PK_ServiceRequests] PRIMARY KEY ([RequestId]),
    CONSTRAINT [FK_ServiceRequests_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([AccountId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ServiceRequests_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE NO ACTION
);

CREATE TABLE [Transactions] (
    [TransactionId] int NOT NULL IDENTITY,
    [AccountId] int NOT NULL,
    [RelatedAccountId] int NULL,
    [Amount] decimal(18,2) NOT NULL,
    [TransactionType] int NOT NULL,
    [TransactionDate] datetime2 NOT NULL,
    [BalanceAfterTransaction] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK_Transactions_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([AccountId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Transactions_Accounts_RelatedAccountId] FOREIGN KEY ([RelatedAccountId]) REFERENCES [Accounts] ([AccountId]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [RoleName])
VALUES (1, N'Customer'),
(2, N'Admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

CREATE UNIQUE INDEX [IX_Accounts_AccountNumber] ON [Accounts] ([AccountNumber]);

CREATE INDEX [IX_Accounts_CustomerId] ON [Accounts] ([CustomerId]);

CREATE UNIQUE INDEX [IX_Customers_UserId] ON [Customers] ([UserId]);

CREATE UNIQUE INDEX [IX_Roles_RoleName] ON [Roles] ([RoleName]);

CREATE INDEX [IX_ServiceRequests_AccountId] ON [ServiceRequests] ([AccountId]);

CREATE INDEX [IX_ServiceRequests_CustomerId] ON [ServiceRequests] ([CustomerId]);

CREATE INDEX [IX_Transactions_AccountId] ON [Transactions] ([AccountId]);

CREATE INDEX [IX_Transactions_RelatedAccountId] ON [Transactions] ([RelatedAccountId]);

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260919185633_InitialCreate', N'10.0.12');

COMMIT;
GO

