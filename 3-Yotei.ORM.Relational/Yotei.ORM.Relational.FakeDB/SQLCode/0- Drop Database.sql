--- =============================================
PRINT 'DROPPING THE TEST DATABASE...'
--- =============================================

USE [Master];

ALTER DATABASE [KeroseneDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE [KeroseneDB];

--- =============================================
PRINT 'TEST DATABASE DROPPED...'
--- =============================================