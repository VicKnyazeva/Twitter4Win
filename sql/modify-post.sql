USE [TwitterApp]
GO

UPDATE [dbo].[Posts]
   SET 
      [Text] = 'New Text',
      [CreatedDate] = '2019-09-29 19:07:36.037',
      [RetweetCount] = 5
 WHERE Id = 14
GO


