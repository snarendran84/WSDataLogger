use master;

if not exists (select * from sys.databases where name = 'DataLogs')
begin 
	create database DataLogs;
end;
go

if not exists 
	(select name from master.sys.server_principals where name = 'dataloguser')
	begin
		create login dataloguser with password = 'TestUserPassword';
	end
go

use DataLogs;
if not exists 
	(select name from sys.database_principals where name = 'dataloguser')
	begin
		create user dataloguser for Login dataloguser;
	end
go

alter role db_datawriter add member dataloguser;
alter role db_datareader add member dataloguser;


if not exists (select 1 from sys.tables where name = 'FileDetails' and type = 'U') 
begin
	create table FileDetails (
		FileId int primary key identity(1,1),
		[Name] nvarchar(250) not null,
		Created DateTime not null
		)
end

if not exists (select 1 from sys.default_constraints where parent_object_id = object_id('dbo.FileDetails') and name = 'co_filedetails_created')
begin
	alter table FileDetails 
		add constraint co_filedetails_created default getdate() for Created;
end

if not exists (select 1 from sys.tables where name = 'FileDataLog' and type = 'U') 
begin
	create table FileDataLog (
		FileId int,
		LogData nvarchar(500),
		constraint fk_FileId foreign key (FileId) references FileDetails(FileId)
		)
end

