SET IDENTITY_INSERT [LookUp].[Location] ON
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Phnom Penh', 1, 0)
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Other', 2, 0)
SET IDENTITY_INSERT [LookUp].[Location] OFF

GO

SET IDENTITY_INSERT [LookUp].[Permission] ON
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Call', 1, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Invoice', 2, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Expense', 3, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (4, 'Finance Overview', 4, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (5, 'Finance List', 5, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (6, 'Performance', 6, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (7, 'Staff', 7, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (8, 'Product', 8, 0)
SET IDENTITY_INSERT [LookUp].[Permission] OFF

GO

SET IDENTITY_INSERT [LookUp].[UserRole] ON
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Administrator', 2, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Data Specialist', 3, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Staff', 4, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (4, '老子', 1, 0)
SET IDENTITY_INSERT [LookUp].[UserRole] OFF

GO

SET IDENTITY_INSERT [LookUp].[CallStatus] ON
INSERT INTO [LookUp].[CallStatus] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Sent To Call Center', 1, 0)
SET IDENTITY_INSERT [LookUp].[CallStatus] OFF

GO


SET IDENTITY_INSERT [LookUp].[InvoiceStatus] ON
INSERT INTO [LookUp].[InvoiceStatus] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Sold', 2, 0)
INSERT INTO [LookUp].[InvoiceStatus] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Cancelled', 3, 0)
INSERT INTO [LookUp].[InvoiceStatus] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Delivered', 1, 0)
SET IDENTITY_INSERT [LookUp].[InvoiceStatus] OFF

GO

SET IDENTITY_INSERT [LookUp].[UserOccupation] ON
INSERT INTO [LookUp].[UserOccupation] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Operator', 1, 0)
INSERT INTO [LookUp].[UserOccupation] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Delivery', 2, 0)
SET IDENTITY_INSERT [LookUp].[UserOccupation] OFF

GO

-- Data Specialist																						    
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 1, 1, 1, 1, 0)
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 2, 1, 1, 1, 0)
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 3, 1, 1, 1, 0)

-- Call Specialist Role
SET IDENTITY_INSERT [LookUp].[UserRole] ON
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (5, 'Call Specialist', 5, 0)
SET IDENTITY_INSERT [LookUp].[UserRole] OFF

INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (5, 1, 1, 1, 1, 0)

UPDATE [UserRolePermission] SET UserRoleId = 2 WHERE UserRoleId = 3

GO

UPDATE [LookUp].[Permission] SET Name = 'Call' WHERE ID = 1
UPDATE [LookUp].[Permission] SET Name = 'Invoice' WHERE ID = 2
UPDATE [LookUp].[Permission] SET Name = 'Expense' WHERE ID = 3
UPDATE [LookUp].[Permission] SET Name = 'Finance Overview' WHERE ID = 4
UPDATE [LookUp].[Permission] SET Name = 'Finance List' WHERE ID = 5
UPDATE [LookUp].[Permission] SET Name = 'Performance' WHERE ID = 6
UPDATE [LookUp].[Permission] SET Name = 'Staff' WHERE ID = 7
UPDATE [LookUp].[Permission] SET Name = 'Product' WHERE ID = 8

GO

DELETE FROM [UserRolePermission] WHERE UserRoleId = 2 AND PermissionId = 3

GO

INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (2, 4, 0, 0, 0, 0)
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (2, 6, 0, 0, 0, 0)

GO
