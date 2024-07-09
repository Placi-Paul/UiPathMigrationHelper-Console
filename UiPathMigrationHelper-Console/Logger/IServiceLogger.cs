using NuGet.Common;

namespace UiPathMigrationHelper_Console.Logger
{
    public interface IServiceLogger : ILogger
    {
        public void SetMininumLogLevel(LogLevel minLogLevel);
    }
}
