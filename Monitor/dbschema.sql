use master;

if not exists (select * from sys.databases where name = 'DataLogs')
begin 
	create database DataLogs;
end;
go

create login dataloguser with password = 'TestUserPassword';
go

use DataLogs;

create user dataloguser for Login dataloguser;
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

alter table FileDetails 
	add constraint co_filedetails_created default getdate() for Created;

if not exists (select 1 from sys.tables where name = 'FileDataLog' and type = 'U') 
begin
	create table FileDataLog (
		FileId int,
		LogData nvarchar(500),
		constraint fk_FileId foreign key (FileId) references FileDetails(FileId)
		)
end

