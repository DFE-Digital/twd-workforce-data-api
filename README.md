# Workforce Data API

## Overview

A service that populates a database with workforce related data (potentially obtained from a number of data sources) and exposes it via an API.  
Currently it is anticipated that the service will process a monthly CSV extract file obtained from the Teachers Pensions Service (TPS) in order to populate the database.

## Setup

### Developer setup

The API is an ASP.NET Core 7 web application. To develop locally you will need the following installed:
- Visual Studio 2022 (or the .NET 7 SDK and an alternative IDE/editor);
- a local PostgreSQL 13+ instance;

### Initial setup

#### User Secrets

Install PostgreSQL then add a connection string to user secrets for the `WorkforceDataApi` and `WorkforceDataApi.DevUtils` projects.

```shell
dotnet user-secrets --id WorkforceDataApi set ConnectionStrings:DefaultConnection "Host=localhost;;Port=5432;Username=your_postgres_user;Password=your_postgres_password;Database=your_database;Search Path=workforce_data"
dotnet user-secrets --id WorkforceDataApiDevUtils set ConnectionStrings:DefaultConnection "Host=localhost;;Port=5432;Username=your_postgres_user;Password=your_postgres_password;Database=your_database;Search Path=workforce_data"
```
Where `your_postgres_user` and `your_postgres_password` are the username and password of your Postgres installation, respectively.  
`your_database` can be the name of a new database if you want to create a new one just for workforce_data OR you can specify an existing database as the workforce data will be self-contained in it's own `workforce_data` schema.

If you want to test the end to end process as part of development then you will need to be able to connect to Azure blob storage.

Add a connection string for the blob storage account where test TPS extract CSV files will be located.  

```shell
dotnet user-secrets --id WorkforceDataApi set ConnectionStrings:BlobStorageConnection "DefaultEndpointsProtocol=https;AccountName=your_storage_account;AccountKey=your_access_key;EndpointSuffix=core.windows.net"
dotnet user-secrets --id WorkforceDataApiDevUtils set ConnectionStrings:BlobStorageConnection "DefaultEndpointsProtocol=https;AccountName=your_storage_account;AccountKey=your_access_key;EndpointSuffix=core.windows.net"
```
`your_storage_account` is the name of the Azure blob storage account.  
`your_access_key` is an access key which can be used to connect to the Azure blob storage account.  

If you want to use a container name in blob storage other than the default of `tps-extract` (e.g if using the same account as other developers) then also set the `TpsExtractBlobContainerName` value in user secrets e.g.

```shell
dotnet user-secrets --id WorkforceDataApi set TpsExtractBlobContainerName "whatever-container-name-you-want"
dotnet user-secrets --id WorkforceDataApiDevUtils set TpsExtractBlobContainerName "whatever-container-name-you-want"
```

If you want the background job which downloads the TPS extract CSV files from Azure blob storage and processes them to trigger on a more frequent schedule than one a day at midnight then you can override the `TpsExtractJobSchedule` setting in user secrets too.  
This should be a valid [CRON expression](https://en.wikipedia.org/wiki/Cron#CRON_expression) e.g. to execute every 2 minutes

```shell
dotnet user-secrets --id WorkforceDataApi set TpsExtractJobSchedule "*/2 * * * *"
```

#### Dev Utils CLI

A number of useful utilities have been implemented as a CLI in the WorkforceDataApi.DevUtils project.  
Once this project has been built then open a command line at the following location.  

```shell
twd-workforce-data-api\WorkforceDataApi\src\WorkforceDataApi.DevUtils\bin\Debug\net7.0
```

All commands can then be executed using a pattern similar to the following:

```shell
dotnet WorkforceDataApi.DevUtils.dll <command name> --option1 --option2
```

##### Help

To see what commands are available in the CLI execute the following:

```shell
dotnet WorkforceDataApi.DevUtils.dll --help
```

##### migratedb

This command can be executed to run the Entity Framework migrations which will create the database if it is not already created OR apply additional migrations as needed to an existing database.  

```shell
dotnet WorkforceDataApi.DevUtils.dll migratedb
```

##### generatemockdata

This command can be executed to generate CSV files containing mock data.  

```shell
dotnet WorkforceDataApi.DevUtils.dll generatemockdata
```

It will generate 2 files - one containing mock teacher identity users and one containing TPS extract data for those users.  

Use the `--help` option with this command to see additional options and their default values.

##### importtpscsv

This command can be executed to import a specified CSV file with data in the TPS extract format into the `tps_extract_data_item` table in the workforce_data schema in PostgreSQL.

```shell
dotnet WorkforceDataApi.DevUtils.dll importtpscsv -f <mock TPS extract data CSV filename>
```

You can use this command to import a file previously created using the `generatemockdata` command.  

Use the `--help` option with this command to see additional options and their default values.

##### execjob

This command can be executed to trigger the job which normally runs in a background process and does the following:
- Checks if there are any TPS extract files pending processing in blob storage
- Downloads each file (would normally expect only 1) then:
  - Imports the data into the PostgreSQL database.
  - Archives the file in blob storage.
  - Archives the downloaded local file. 

```shell
dotnet WorkforceDataApi.DevUtils.dll execjob
```

Use the `--help` option with this command to see additional options and their default values.

##### importestablishmentscsv

This command can be executed to download a CSV file containing all establishments from the Get Information About Schools website at https://www.get-information-schools.service.gov.uk/Downloads and import the data into the `establishments_raw` table in the workforce_data schema in PostgreSQL.

```shell
dotnet WorkforceDataApi.DevUtils.dll importestablishmentscsv
```

Use the `--help` option with this command to see additional options and their default values.

##### azblob

This command has several sub-commands related to interacting with blob storage.

```shell
dotnet WorkforceDataApi.DevUtils.dll azblob <sub command>
```

Use the `--help` option with this command to the additional commands.

##### azblob listpending

This command lists the names of TPS extract files pending processing in blob storage.

```shell
dotnet WorkforceDataApi.DevUtils.dll azblob listpending
```

##### azblob downloadpending

This command downloads all TPS extract files pending processing in blob storage.

```shell
dotnet WorkforceDataApi.DevUtils.dll azblob downloadpending
```

##### azblob archivepending

This command archives all TPS extract files pending processing in blob storage.

```shell
dotnet WorkforceDataApi.DevUtils.dll azblob archivepending
```

### Azure Blob Storage

The TPS extract files are expected to be in a `pending` virtual folder in Azure blob storage.  
When they get processed then they will get archived to the `processed` virtual folder within the same container.  
As part of testing you might want to use [Azure Storage Explorer](https://learn.microsoft.com/en-us/azure/vs-azure-tools-storage-manage-with-storage-explorer?tabs=windows) to manually move files to pending / delete from processed etc.

### Import Test Identity Users

The `generatemockdata` command generates a CSV file with the prefix `mock-teacher-identity-users-`.  
This can be imported into a teacher identity database using a utility in the [TeacherIdentity.DevBootstrap](https://github.com/DFE-Digital/get-an-identity/tree/main/dotnet-authserver/src/TeacherIdentity.DevBootstrap) console app within the get-an-identity repo. 

Build the TeacherIdentity solution then open a command line at the following location:

```shell
get-an-identity\dotnet-authserver\src\TeacherIdentity.DevBootstrap\bin\Debug\net7.0
```

Copy the appropriate test user CSV file to this folder and rename it as `test-users.csv`.  
Execute the following command to import the test users (which will also re-run the bootstrap which applies migrations etc. if necessary):

```shell
dotnet TeacherIdentity.DevBootstrap.dll --import-test-users
```