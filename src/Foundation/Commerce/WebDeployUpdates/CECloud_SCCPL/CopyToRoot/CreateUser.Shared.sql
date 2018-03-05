
IF  NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'PlaceholderForSharedDatabaseUserName')
BEGIN
	IF (LEN('PlaceholderForSharedDatabasePassword') = 0)
		CREATE USER [PlaceholderForSharedDatabaseUserName] FOR LOGIN [PlaceholderForSharedDatabaseUserName]
	ELSE
		CREATE USER [PlaceholderForSharedDatabaseUserName] WITH PASSWORD = 'PlaceholderForSharedDatabasePassword';
END
GO

IF (NOT EXISTS (SELECT '*' FROM sys.database_role_members rm
	JOIN sys.database_principals p ON rm.role_principal_id = p.principal_id
	JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
	WHERE p.name = 'db_datareader' AND m.name = 'PlaceholderForSharedDatabaseUserName'))
BEGIN
	EXEC sp_addrolemember 'db_datareader', [PlaceholderForSharedDatabaseUserName];
END

IF (NOT EXISTS (SELECT '*' FROM sys.database_role_members rm
	JOIN sys.database_principals p ON rm.role_principal_id = p.principal_id
	JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
	WHERE p.name = 'db_datawriter' AND m.name = 'PlaceholderForSharedDatabaseUserName'))
BEGIN
	EXEC sp_addrolemember 'db_datawriter', [PlaceholderForSharedDatabaseUserName];
END

GO

GRANT EXECUTE TO [PlaceholderForSharedDatabaseUserName];
GO
