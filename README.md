# UiPathMigrationHelper-Console
CLI project aimed to help with the migration task

# Commands
Commands:
- **list**      List all packages and dependencies from a nuget feed
- **search**    Search package based on identifier (package name)
- **check**     Checks all packages and dependencies to provide full compatibility matrix.

Options:

-h, --help    Show help message

  --version     Show version

# Feed Configuration
The cli works with a hosted feed or folder feed.

## Orchestrator Feed
The current version does not support api keys for secured feeds.
In order to use UiPath Orchestrator package/library feed, disable the security:

Tenant > Settings > Deployment > Security

Select API Key and leave it blank

### Cloud Feeds
Check the network page in automation ops for this API response to get the nuget feeds:

https://cloud.uipath.com/{organization-name}/roboticsops_/feeds_/api/Feeds/orchestrator/paged?pageSize=10&pageIndex=0

# Examples

All commands will allow a **skip** & **take** parameter to limits the search services

## List
> list -f {feed}
<br>
Command to list all packages & dependencies from a feed.

## Search
> search -f {feed} -n {packageId}
<br>
Used to return all packages with similar name and list all version
<br>

 Example:  
  > search -f {UiPathOfficialFeed} -n uipath.system
  <br>
  ![image](https://github.com/user-attachments/assets/db9d2ca2-d121-43b1-ad3d-929674bb246c)

> search -f {feed} -n {packageId} -v {version}
<br>
  Used to check a specific version of the provided package.
  
  !The command requires to provide the fully qualified name of the package.
  
  -n "uipath.system" -> partial package name
  
  -n "uipath.system.activities" -> fully qualified name

  Example:
  > search -f {UiPathOfficialFeed} -n uipath.database.activities" -v 1.7.0
<br>
  ![image](https://github.com/user-attachments/assets/8e4708a6-ab32-4fa5-8fbe-f9e2977c5ec6)

## Check
> check -f {feed} -l {libraryFeed} --target Legacy|Windows|CrossPlatform
<br>
Used to check all available packages from a feed & dependencies from another feed if they are available to migrate.<br>
'--target' parameter is used to determine the target project.
