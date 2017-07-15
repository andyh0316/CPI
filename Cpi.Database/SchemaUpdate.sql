UPDATE [LookUp].[UserRole] SET DisplayOrder = DisplayOrder + 1;
GO

SET IDENTITY_INSERT [LookUp].[UserRole] ON
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (4, '老子', 1, 0)
SET IDENTITY_INSERT [LookUp].[UserRole] OFF

GO