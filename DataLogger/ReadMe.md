Setup notes:

1) The project was created using Visual Studio 2022 dev version and if you face any difficulties in terms of opening in your local machine - then edit the visual studio solution file to match your local version.

2) Packages config contains all the dependecies and the major ones that you would need to add if it is not doing it properly are Moq, Dapper, Nlog 

3) Unit Testing is still work in progress

4) the following hard coded values are used - not ideal - need to solve it (file extension, data logged in the db is about 500 characters long and it truncates anything over that)

5) I have put some random values in the app.config file for database, interval (in milliseconds) and the monitoring folder where the files will be placed.

6) DataLogDb.sql file contains the DBSchema changes that are necessary.

7) To get this up and running in your machine follow the steps as detailed below

	1) download, edit the app.config file under the "DataLoggerWS" Project in accordance with your lcoal machine and make sure you edit the Nlog.config file to configur the path of the log file 
	
	2) run the database scripts against your local machine, verify that a new db with two tables are created and update the connectionstring value in the app.config accordingly.
		
	3)	build the source code locally and you should be able to see the executable created successfully in the bin/debug directory under the project
	
	4) open visual studio command prompt in admin mode and run the following command "installutil DataLoggerWS.exe"
	
	5) this should install the windows service in you local machine with the service name "Data Logger" -- Please note that the service is manual and not automatic.
	
	6) once you start you can place a file or two and verify if the data is getting populated in your local machine.