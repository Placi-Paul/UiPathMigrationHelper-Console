using Cocona;

namespace UiPathMigrationHelper_Console.Parameters
{
    public record PaginationParameters(
        [Option('s', Description ="Skip package")]
        int Skip = 0,
        [Option('t', Description ="Take packages after skip parameter")]
        int Take = 10
    ) : ICommandParameterSet;
}
