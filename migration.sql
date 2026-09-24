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
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Country] nvarchar(max) NOT NULL,
    [IsActivated] bit NOT NULL,
    [ActivatedAt] datetime2 NULL,
    [TransactionPasswordHash] nvarchar(max) NULL,
    [Rank] nvarchar(max) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AutoShips] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [ProductName] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [FrequencyDays] int NOT NULL,
    [NextRunDate] datetime2 NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastRunDate] datetime2 NULL,
    CONSTRAINT [PK_AutoShips] PRIMARY KEY ([Id])
);

CREATE TABLE [Commissions] (
    [Id] int NOT NULL IDENTITY,
    [Category] nvarchar(max) NOT NULL,
    [EarnedAt] datetime2 NOT NULL,
    [FromUserId] nvarchar(max) NOT NULL,
    [OwnerUserId] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Vat] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Level] int NULL,
    CONSTRAINT [PK_Commissions] PRIMARY KEY ([Id])
);

CREATE TABLE [KycRecords] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [IdType] nvarchar(max) NOT NULL,
    [IdNumber] nvarchar(max) NOT NULL,
    [IdDocumentName] nvarchar(max) NOT NULL,
    [AddressDocumentName] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [ReviewNotes] nvarchar(max) NULL,
    [SubmittedAt] datetime2 NULL,
    [ReviewedAt] datetime2 NULL,
    CONSTRAINT [PK_KycRecords] PRIMARY KEY ([Id])
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [OrderNumber] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [ShippingFee] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CourierName] nvarchar(max) NOT NULL,
    [TrackingNumber] nvarchar(max) NOT NULL,
    [ShippingStatus] nvarchar(max) NOT NULL,
    [EstimatedDelivery] datetime2 NULL,
    [OrderedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [PayoutSettings] (
    [Id] int NOT NULL IDENTITY,
    [AccountHolder] nvarchar(max) NOT NULL,
    [AccountNumber] nvarchar(max) NOT NULL,
    [BankName] nvarchar(max) NOT NULL,
    [AccountType] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Country] nvarchar(max) NOT NULL,
    [PostalCode] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PayoutSettings] PRIMARY KEY ([Id])
);

CREATE TABLE [PortalContent] (
    [Id] int NOT NULL IDENTITY,
    [Type] nvarchar(max) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [LinkUrl] nvarchar(max) NULL,
    [PublishedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_PortalContent] PRIMARY KEY ([Id])
);

CREATE TABLE [PortalMessages] (
    [Id] int NOT NULL IDENTITY,
    [SenderUserId] nvarchar(max) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [Recipient] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_PortalMessages] PRIMARY KEY ([Id])
);

CREATE TABLE [Purchases] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [ProductName] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [IsBusinessKit] bit NOT NULL,
    [Quantity] int NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [PurchasedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Purchases] PRIMARY KEY ([Id])
);

CREATE TABLE [RankHistory] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [OldRank] nvarchar(max) NOT NULL,
    [NewRank] nvarchar(max) NOT NULL,
    [Incentive] decimal(18,2) NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RankHistory] PRIMARY KEY ([Id])
);

CREATE TABLE [Referrals] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Mobile] nvarchar(max) NOT NULL,
    [SponsorId] nvarchar(max) NOT NULL,
    [Rank] nvarchar(max) NOT NULL,
    [MonthlySales] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [JoinedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Referrals] PRIMARY KEY ([Id])
);

CREATE TABLE [SupportTicketMessages] (
    [Id] int NOT NULL IDENTITY,
    [TicketId] int NOT NULL,
    [SenderUserId] nvarchar(max) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [IsStaff] bit NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SupportTicketMessages] PRIMARY KEY ([Id])
);

CREATE TABLE [SupportTickets] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [TicketNumber] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([Id])
);

CREATE TABLE [TeamChatMessages] (
    [Id] int NOT NULL IDENTITY,
    [SenderUserId] nvarchar(max) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientUserId] nvarchar(max) NOT NULL,
    [RecipientName] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_TeamChatMessages] PRIMARY KEY ([Id])
);

CREATE TABLE [TransactionPasswordLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [Remarks] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [IpAddress] nvarchar(max) NOT NULL,
    [OccurredAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TransactionPasswordLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [WalletTransactions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [OccurredAt] datetime2 NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Comments] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_WalletTransactions] PRIMARY KEY ([Id])
);

CREATE TABLE [Withdrawals] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [RequestedAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Withdrawals] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260924112859_InitialCreate', N'9.0.4');

COMMIT;
GO

