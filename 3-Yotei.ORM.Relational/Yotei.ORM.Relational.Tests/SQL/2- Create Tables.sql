--- =============================================
PRINT 'DROPPING TABLES...'

--- =============================================
USE [KeroseneDB];

--- Comment out the creation code if the tables do not exist yet...

--- =============================================
PRINT 'Dropping table EmployeeTalents...'
--- =============================================

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Employees_Countries]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeTalents]'))
	ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Countries];

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeTalents_Talents]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeTalents]'))
	ALTER TABLE [dbo].[EmployeeTalents] DROP CONSTRAINT [FK_EmployeeTalents_Talents];

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeTalents_Employees]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeTalents]'))
	ALTER TABLE [dbo].[EmployeeTalents] DROP CONSTRAINT [FK_EmployeeTalents_Employees];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeTalents]') AND type in (N'U'))
	DROP TABLE [dbo].[EmployeeTalents];

--- =============================================
PRINT 'Dropping table Talents...'
--- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Talents]') AND type in (N'U'))
DROP TABLE [dbo].[Talents];

--- =============================================
PRINT 'Dropping table Employees...'
--- =============================================

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Employees_Employees]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[Employees]'))
	ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Employees];

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Employees_Countries]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[Employees]'))
	ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Countries];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
	DROP TABLE [dbo].[Employees];

--- =============================================
PRINT 'Dropping table Countries...'
--- =============================================

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Countries_Regions]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[Countries]'))
	ALTER TABLE [dbo].[Countries] DROP CONSTRAINT [FK_Countries_Regions];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
	DROP TABLE [dbo].[Countries];

--- =============================================
PRINT 'Dropping table Regions...'
--- =============================================

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Regions_Regions]')
	AND parent_object_id = OBJECT_ID(N'[dbo].[Regions]'))
	ALTER TABLE [dbo].[Regions] DROP CONSTRAINT [FK_Regions_Regions];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Regions]') AND type in (N'U'))
	DROP TABLE [dbo].[Regions];

--- =============================================
PRINT 'CREATING TABLES...'
--- =============================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

--- =============================================
PRINT 'Creating table Regions...'
--- =============================================

CREATE TABLE [dbo].[Regions](
	[Id] [nvarchar](8) NOT NULL,
	[Name] [nvarchar](50) NULL DEFAULT NULL,
	[ParentId] [nvarchar](8) NULL DEFAULT NULL,
	[RowVersion] [ROWVERSION] NOT NULL,

	CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ( [Id] ASC )
	
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	ON [PRIMARY]
)
ON [PRIMARY];

ALTER TABLE [dbo].[Regions] 
	WITH CHECK ADD  CONSTRAINT [FK_Regions_Regions] FOREIGN KEY([ParentId])
	REFERENCES [dbo].[Regions] ([Id]);

ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_Regions_Regions];

--- =============================================
PRINT 'Creating table Countries...'
--- =============================================

CREATE TABLE [dbo].[Countries](
	[Id] [nvarchar](8) NOT NULL,
	[Name] [nvarchar](50) NULL DEFAULT NULL,
	[RegionId] [nvarchar](8) NOT NULL,
	[RowVersion] [ROWVERSION] NOT NULL,
	
	CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ( [Id] ASC )
	
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	ON [PRIMARY]
)
ON [PRIMARY];

ALTER TABLE [dbo].[Countries] 
	WITH CHECK ADD  CONSTRAINT [FK_Countries_Regions] FOREIGN KEY([RegionId])
	REFERENCES [dbo].[Regions] ([Id]);

ALTER TABLE [dbo].[Countries] CHECK CONSTRAINT [FK_Countries_Regions];

--- =============================================
PRINT 'Creating table Employees...'
--- =============================================

CREATE TABLE [dbo].[Employees](
	[Id] [nvarchar](8) NOT NULL,
	[FirstName] [nvarchar](50) NULL DEFAULT NULL,
	[LastName] [nvarchar](50) NULL DEFAULT NULL,
	[BirthDate] [date] NULL DEFAULT NULL,
	[Active] [bit] NULL DEFAULT NULL,
	[JoinDate] [date] NULL DEFAULT NULL,
	[StartTime] [time] NULL DEFAULT NULL,
	[ManagerId] [nvarchar](8) NULL DEFAULT NULL,
	[CountryId] [nvarchar](8) NOT NULL,
	[Photo] [varbinary](max) NULL DEFAULT NULL,
	[FullName] AS (([FirstName] + ' ') + [LastName]),
	[RowVersion] [ROWVERSION] NOT NULL,
	
	CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ( [Id] ASC )
	
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	ON [PRIMARY]
)
ON [PRIMARY];

ALTER TABLE [dbo].[Employees]
	WITH CHECK ADD  CONSTRAINT [FK_Employees_Countries] FOREIGN KEY([CountryId])
	REFERENCES [dbo].[Countries] ([Id]);

ALTER TABLE [dbo].[Employees]
	CHECK CONSTRAINT [FK_Employees_Countries];

ALTER TABLE [dbo].[Employees]
	WITH CHECK ADD CONSTRAINT [FK_Employees_Employees] FOREIGN KEY([ManagerId])
	REFERENCES [dbo].[Employees] ([Id]);

ALTER TABLE [dbo].[Employees]
	CHECK CONSTRAINT [FK_Employees_Employees];

--- =============================================
PRINT 'Creating table Talents...'
--- =============================================

CREATE TABLE [dbo].[Talents](
	[Id] [nvarchar](8) NOT NULL,
	[Description] [nvarchar](50) NULL DEFAULT NULL,
	[RowVersion] [ROWVERSION] NOT NULL,
	
	CONSTRAINT [PK_Talents] PRIMARY KEY CLUSTERED ( [Id] ASC )
	
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	ON [PRIMARY]
)
ON [PRIMARY];

--- =============================================
PRINT 'Creating table EmployeeTalents...'
--- =============================================

CREATE TABLE [dbo].[EmployeeTalents](
	[EmployeeId] [nvarchar](8) NOT NULL,
	[TalentId] [nvarchar](8) NOT NULL,
	[RowVersion] [ROWVERSION] NOT NULL,
	
	CONSTRAINT [PK_EmployeeTalents] PRIMARY KEY CLUSTERED ( [EmployeeId] ASC, [TalentId] ASC )
	
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	ON [PRIMARY]
)
ON [PRIMARY];

ALTER TABLE [dbo].[EmployeeTalents]
	WITH CHECK ADD  CONSTRAINT [FK_EmployeeTalents_Employees] FOREIGN KEY([EmployeeId])
	REFERENCES [dbo].[Employees] ([Id]);

ALTER TABLE [dbo].[EmployeeTalents]
	WITH CHECK ADD  CONSTRAINT [FK_EmployeeTalents_Talents] FOREIGN KEY([TalentId])
	REFERENCES [dbo].[Talents] ([Id]);

--- =============================================
PRINT 'TABLES CREATED...'
--- =============================================
