SET IDENTITY_INSERT [LookUp].[Location] ON
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Phnom Penh', 1, 0)
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Other', 2, 0)
SET IDENTITY_INSERT [LookUp].[Location] OFF

GO

SET IDENTITY_INSERT [LookUp].[Permission] ON
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Call 外来电话', 1, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Invoice 订单', 2, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Expense 支出', 3, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (4, 'Finance Overview 财务概况', 4, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (5, 'Finance List 财务表格', 5, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (6, 'Performance 员工评估', 6, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (7, 'Staff 员工', 7, 0)
INSERT INTO [LookUp].[Permission] (Id, Name, DisplayOrder, Deleted) VALUES (8, 'Product 产品', 8, 0)
SET IDENTITY_INSERT [LookUp].[Permission] OFF

GO

SET IDENTITY_INSERT [LookUp].[UserRole] ON
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Administrator', 2, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Data Specialist', 3, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Staff', 4, 0)
INSERT INTO [LookUp].[UserRole] (Id, Name, DisplayOrder, Deleted) VALUES (4, '老子', 1, 0)
SET IDENTITY_INSERT [LookUp].[UserRole] OFF

GO

-- Data Specialist																						    
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 1, 1, 1, 1, 0)
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 2, 1, 1, 1, 0)
INSERT INTO [UserRolePermission] (UserRoleId, PermissionId, [Create], [Edit], [Delete], Deleted) VALUES (3, 3, 1, 1, 1, 0)

