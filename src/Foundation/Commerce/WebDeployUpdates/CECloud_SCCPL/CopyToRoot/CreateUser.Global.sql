
IF  NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'PlaceholderForGlobalDatabaseUserName')
BEGIN
	IF (LEN('PlaceholderForGlobalDatabasePassword') = 0)
		CREATE USER [PlaceholderForGlobalDatabaseUserName] FOR LOGIN [PlaceholderForGlobalDatabaseUserName]
	ELSE
		CREATE USER [PlaceholderForGlobalDatabaseUserName] WITH PASSWORD = 'PlaceholderForGlobalDatabasePassword';
END
GO

IF (NOT EXISTS (SELECT '*' FROM sys.database_role_members rm
	JOIN sys.database_principals p ON rm.role_principal_id = p.principal_id
	JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
	WHERE p.name = 'db_datareader' AND m.name = 'PlaceholderForGlobalDatabaseUserName'))
BEGIN
	EXEC sp_addrolemember 'db_datareader', [PlaceholderForGlobalDatabaseUserName];
END

IF (NOT EXISTS (SELECT '*' FROM sys.database_role_members rm
	JOIN sys.database_principals p ON rm.role_principal_id = p.principal_id
	JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
	WHERE p.name = 'db_datawriter' AND m.name = 'PlaceholderForGlobalDatabaseUserName'))
BEGIN
	EXEC sp_addrolemember 'db_datawriter', [PlaceholderForGlobalDatabaseUserName];
END

GO

GRANT EXECUTE TO [PlaceholderForGlobalDatabaseUserName];
GO
