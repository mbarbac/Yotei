--- =============================================
PRINT 'DELETING CONTENTS...'
--- =============================================

USE YoteiDB;

PRINT 'Deleting contents from EmployeeTalents...'
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeTalents]') AND type in (N'U'))
	DELETE FROM [dbo].[EmployeeTalents];

PRINT 'Deleting contents from Talents...'
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Talents]') AND type in (N'U'))
	DELETE FROM [dbo].[Talents];

PRINT 'Deleting contents from Employees...'
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
	DELETE FROM [dbo].[Employees];

PRINT 'Deleting contents from Countries...'
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
	DELETE FROM [dbo].[Countries];

PRINT 'Deleting contents from Regions...'
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Regions]') AND type in (N'U'))
	DELETE FROM [dbo].[Regions];

--- =============================================
PRINT 'SETTING INITIAL CONTENTS...'
--- =============================================

--- REGIONS
PRINT 'Inserting contents into Regions...';

INSERT INTO [dbo].[Regions] ( Id, Name, ParentId )
VALUES
	( '000', 'World', null )

	,( '100', 'Americas', '000'  )
		,( '110', 'North America', '100' )
			,( '111', 'Canada', '110' )
			,( '112', 'US Administration', '110' )
			,( '113', 'US Commercial', '110' )
		,( '120', 'Central America', '100' )
	
	,( '200', 'Europe, Middle East & Africa', '000' )
		,( '210', 'EMEA North', '200' )
		,( '220', 'EMEA South', '200' )
			,( '221', 'West Mediterranean', '220' )
			,( '223', 'Middle East', '220' )
			,( '225', 'Africa', '220' )
		,( '230', 'EMEA Central', '200' )
		,( '240', 'EMEA East', '200' )
	
	,( '300', 'Asia and Pacific', '000' )
		,( '310', 'Japan', '300' )
;

--- COUNTRIES
PRINT 'Inserting contents into Countries...';

INSERT INTO [dbo].[Countries] ( Id, Name, RegionId )
VALUES
	( 'ca', 'Canada', '111' )

	,( 'usx', 'US Administration', '112' )
	,( 'us', 'United States of America', '113' )

	,( 'mx', 'Mexico', '120' )
	
	,( 'uk', 'United Kingdom', '210' )
	,( 'ie', 'Ireland', '210' )
	
	,( 'es', 'España', '221' )	
	,( 'pt', 'Portugal', '221' )
	,( 'it', 'Italy', '221' )

	,( 'ae', 'United Arab Emirates', '223' )
	
	,( 'za', 'Republic of South Africa', '225' )
	
	,( 'jp', 'Japan', '310' )
;

--- EMPLOYEES
PRINT 'Inserting contents into Employees...';

INSERT INTO [dbo].[Employees]
	( Id, Active, ManagerId, CountryId, FirstName, LastName, BirthDate, JoinDate, StartTime, Photo )
VALUES
	( '1001', 1, null, 'us', 'Tom', 'Thomsom', '1969-9-12', '2002-1-24', '10:12:45', null )
		,('1002', 1, '1001', 'us', 'Dave', 'Alistair', '1959-11-23', '2001-11-9', '18:00:45', null )
		,('2001', 1, '1001', 'uk', 'Mohammed', 'Ifasi', null, null, null, null )
			,('2002', 1, '2001', 'uk', 'Andrew', 'Mc Quanty', '1970-11-23', '2001-11-9', '18:00:45', null )
			,('2003', 1, '2001', 'es', 'David', 'Perez de Manto', null, null, null, null )
				,('2005', 1, '2003', 'es', 'Juan', 'Perez Gomez', null, null, null, null )
					,('2008', null, '2005', 'es', 'Fernando', 'Quesero Villaverde', null, null, null, null )
					,('2009', null, '2005', 'es', 'Antonio', 'Martinez del Alamo', null, null, null, null )
				,('2006', 1, '2005', 'za', 'Richard', 'Mc Donnel', null, null, null, null )
					,('2010', 1, '2006', 'za', 'Paul', 'Brown', null, null, null, null )
					,('2011', 1, '2006', 'za', 'Nicole', 'Weather', null, null, null, null )
			,('2004', 1, '2001', 'ae', 'Hassan', 'El Auly', '1969-1-15', '2001-11-9', '18:00:45', null )
				,('2007', 1, '2005', 'ae', 'John', 'Burrogough', null, null, null, null )
		,('3001', 1, '1001', 'jp', 'Asira', 'Yamamoto', '1967-2-20', '2003-8-23', '8:00:00', null )
;

--- Talents
PRINT 'Inserting contents into Talents...';

INSERT INTO [dbo].[Talents]
	( [Id], [Description] )
VALUES
	( 'sales', 'Sales Talent' ),
	( 'tech', 'Tech Talent' )
;

--- EMPLOYEE Talents
PRINT 'Inserting contents into EmployeeTalents...';

INSERT INTO [dbo].[EmployeeTalents]
	( [EmployeeId], [TalentId] )
VALUES
	( '1002', 'sales' ),
	( '2006', 'sales' ), ( '2006', 'tech' ),
	( '2010', 'tech' ),
	( '2011', 'tech' )
;

--- =============================================
PRINT 'SET OF INITIAL CONTENTS LOADED...'
--- =============================================
