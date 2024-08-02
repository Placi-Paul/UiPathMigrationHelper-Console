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
List all packages & dependencies from a feed:
```
> .\UiPathMigrationHelper-Console.exe list -f {feed}
```


## Search
Used to search packages and versions:
```
> .\UiPathMigrationHelper-Console.exe search -f {feed} -n {packageId}
```

Returns all packages with similar name and available versions
```
> .\UiPathMigrationHelper-Console.exe search -f {UiPathOfficialFeed} -n uipath.system
```

Used to check a specific version of the provided package.
```
.\UiPathMigrationHelper-Console.exe search -f {feed} -n {packageId} -v {version}
```

> [!WARNING]
> The command requires to provide the fully qualified name of the package when searching for specific version.

-n "uipath.system" -> partial package name

-n "uipath.system.activities" -> fully qualified name

Example:
```
> .\UiPathMigrationHelper-Console.exesearch -f {UiPathOfficialFeed} -n uipath.database.activities" -v 1.7.0
```

## Check
```
> .\UiPathMigrationHelper-Console.execheck -f {feed} -l {libraryFeed} --target Legacy|Windows|CrossPlatform
```
Used to check all available packages from a feed & dependencies from another feed if they are available to migrate.<br>
'--target' parameter is used to determine the target project.

> [!WARNING]
> The tool uses the exact match of the dependency to determine if it's compatible. It does not look for the latest version.
