--- =============================================
PRINT 'DROPPING THE TEST DATABASE...'
--- =============================================

USE [Master];

ALTER DATABASE [YoteiDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE [YoteiDB];

--- =============================================
PRINT 'TEST DATABASE DROPPED...'
--- =============================================