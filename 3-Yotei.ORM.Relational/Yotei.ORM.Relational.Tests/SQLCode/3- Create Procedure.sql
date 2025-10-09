--- =============================================
PRINT 'CREATING A STORED PROCEDURE...'
--- =============================================

PRINT 'NOTE: It only works if the Employee ID can be converted to an INT field...'

USE [YoteiDB]
GO

IF EXISTS (
	SELECT name FROM sysobjects
	WHERE name = 'employee_insert' AND type = 'P' )
DROP PROCEDURE employee_insert
GO

CREATE PROCEDURE employee_insert(
	@FirstName nvarchar(50),
	@LastName nvarchar(50) = null,
	@CountryId nvarchar(8) = 'us',
	@ManagerId nvarchar(8) = null,
	@BirthDate date = null,
	@Active bit = null,
	@JoinDate date = null,
	@StartTime time = null,
	@Photo varbinary(max) = null
)
WITH RECOMPILE
AS BEGIN
	--- It will only work if the Employee's Id can be converted to an int
	DECLARE @TempId AS nvarchar(8)
	SET @TempId = ( SELECT TOP 1 Id FROM Employees ORDER BY Id )
	SET @TempId = @TempId + 1
	SET @TempId = RIGHT( REPLICATE( '0', 8-LEN(@TempId)) + @TempId, 8 )

	INSERT INTO Employees ( Id, FirstName, LastName, BirthDate, Active, ManagerId, CountryId, JoinDate, StartTime, Photo )
	OUTPUT INSERTED.*
	VALUES ( @TempId, @FirstName, @LastName, @BirthDate, @Active, @ManagerId, @CountryId, @JoinDate, @StartTime, @Photo )
	
END
GO

--- =============================================
PRINT 'STORED PROCEDURE CREATED...'
--- =============================================