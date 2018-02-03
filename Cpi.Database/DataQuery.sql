/* Find out roles and permissions */
select a.Id as UserRoleId, a.Name as UserRole, c.Id as PermissionId, c.Name as Permission, b.[Create], b.[Edit], b.[Delete] from [LookUp].[UserRole] a
join [UserRolePermission] b
on a.Id = b.UserRoleId
join [LookUp].[Permission] c
on b.PermissionId = c.Id