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
