using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathMigrationHelper_Console.Logger
{
    internal class ConsoleLogger : ILogger
    {
        private LogLevel _minLogLevel = LogLevel.Warning;
        public void Log(LogLevel level, string data)
        {
            if (level < _minLogLevel) return;

            Console.WriteLine($"{level}: {data}");
        }

        public void Log(ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        public Task LogAsync(LogLevel level, string data)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string data)
        {
            Log(LogLevel.Debug, data);
        }

        public void LogError(string data)
        {
            Log(LogLevel.Error, data);
        }

        public void LogInformation(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogInformationSummary(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogMinimal(string data)
        {
            Log(LogLevel.Minimal, data);
        }

        public void LogVerbose(string data)
        {
            Log(LogLevel.Verbose, data);
        }

        public void LogWarning(string data)
        {
            Log(LogLevel.Warning, data);
        }

        public void SetMininumLogLevel(LogLevel minLogLevel)
        {
            _minLogLevel = minLogLevel;
        }
    }
}
