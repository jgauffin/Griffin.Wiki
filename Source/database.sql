CREATE TABLE [dbo].[Users] (
	[Id] int NOT NULL IDENTITY, 
	[DisplayName] nvarchar(50) NOT NULL, 
	[AccountName] nvarchar(50) NOT NULL,
CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
)
GO

CREATE TABLE [dbo].[WikiPageHistory] (
	[Id] int NOT NULL IDENTITY, 
	[PageId] int NOT NULL, 
	[CreatedBy] int NOT NULL, 
	[CreatedAt] datetime2(7) NOT NULL, 
	[ChangeDescription] ntext NOT NULL, 
	[RawBody] ntext NOT NULL, 
	[HtmlBody] ntext NOT NULL,
CONSTRAINT [PK_WikiPageHistory] PRIMARY KEY ([Id])
)
GO

CREATE TABLE [dbo].[WikiPageLinks] (
	[Page] int NOT NULL, 
	[LinkedPage] int NOT NULL
)
GO

CREATE TABLE [dbo].[WikiPages] (
	[Id] int NOT NULL IDENTITY, 
	[PageId] int,
	[PageName] nvarchar(50) NOT NULL, 
	[Title] nvarchar(50) NOT NULL, 
	[CreatedAt] datetime2(7) NOT NULL, 
	[CreatedBy] int NOT NULL, 
	[UpdatedAt] datetime2(7) NOT NULL, 
	[UpdatedBy] int NOT NULL, 
	[HtmlBody] ntext NOT NULL, 
	[RawBody] ntext NOT NULL,
CONSTRAINT [PK_WikiPages] PRIMARY KEY ([Id])
)
GO

ALTER TABLE WikiPageLinks 
ADD FOREIGN KEY (Page) REFERENCES WikiPages(Id);
ALTER TABLE WikiPageLinks 
ADD FOREIGN KEY (LinkedPage) REFERENCES WikiPages(Id);

ALTER TABLE WikiPageHistory
ADD FOREIGN KEY (PageId) REFERENCES WikiPages(Id);
ALTER TABLE WikiPageHistory
ADD FOREIGN KEY (CreatedBy) REFERENCES Users(Id);

ALTER TABLE WikiPages
ADD FOREIGN KEY (CreatedBy) REFERENCES Users(Id);
ALTER TABLE WikiPages
ADD FOREIGN KEY (ParentId) REFERENCES WikiPages(Id);
ALTER TABLE WikiPages
ADD FOREIGN KEY (UpdatedBy) REFERENCES Users(Id);
