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
    [PageName] nvarchar(50) NOT NULL, 
    [Title] nvarchar(50) NOT NULL, 
    [CreatedAt] datetime2(7) NOT NULL, 
    [CreatedBy] int NOT NULL, 
    [UpdatedAt] datetime2(7) NOT NULL, 
    [UpdatedBy] int NOT NULL, 
    [HtmlBody] ntext NOT NULL, 
    [RawBody] ntext NOT NULL,
    [TemplateId] int,
CONSTRAINT [PK_WikiPages] PRIMARY KEY ([Id])
)
GO

CREATE TABLE [dbo].[WikiTemplates] (
    [Id] int NOT NULL IDENTITY, 
    [Title] nvarchar(50) NOT NULL, 
    [CreatedAt] datetime2(7) NOT NULL, 
    [CreatedBy] int NOT NULL, 
    [UpdatedAt] datetime2(7) NOT NULL, 
    [UpdatedBy] int NOT NULL, 
    [Body] ntext NOT NULL, 
CONSTRAINT [PK_WikiTemplates] PRIMARY KEY ([Id])
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
ALTER TABLE WikiPages
ADD FOREIGN KEY (TemplateId) REFERENCES WikiTemplates(Id);


ALTER TABLE WikiTemplates
ADD FOREIGN KEY (CreatedBy) REFERENCES Users(Id);
ALTER TABLE WikiTemplates
ADD FOREIGN KEY (UpdatedBy) REFERENCES Users(Id);


CREATE TABLE [dbo].[WikiPageTree] (
    [PageId] int NOT NULL, 
    [Title] nvarchar(1000) NOT NULL, 
    [Lineage] nvarchar(1000) NOT NULL,
CONSTRAINT [PK_WikiPageTree] PRIMARY KEY ([PageId])
)
GO

ALTER TABLE [WikiPageTree]
ADD FOREIGN KEY (PageId) REFERENCES WikiPages(Id);


CREATE TABLE [dbo].[WikiMissingPageLinks](
    [Id] int NOT NULL IDENTITY, 
    [PageId] [int] NOT NULL,
    [MissingPageName] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_WikiMissingPageLinks] PRIMARY KEY ([Id])
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WikiMissingPageLinks]  WITH CHECK ADD  CONSTRAINT [FK_WikiMissingPageLinks_WikiPages] FOREIGN KEY([PageId])
REFERENCES [dbo].[WikiPages] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WikiMissingPageLinks] CHECK CONSTRAINT [FK_WikiMissingPageLinks_WikiPages]