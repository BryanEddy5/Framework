using System;
using System.Diagnostics.CodeAnalysis;
using Serilog.Events;

namespace HumanaEdge.Webcore.Core.Logging
{
    /// <summary>
    /// Stackdriver Severity Mapper.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StackdriverSeverityMapper
    {
        /// <summary>
        /// Maps Serilog severity to stackdriver severity.
        /// </summary>
        /// <param name="comparison">the comparison.</param>
        /// <param name="value">the value.</param>
        /// <returns>the mapped value.</returns>
        public static LogEventPropertyValue? MapSeverity(
            StringComparison comparison,
            LogEventPropertyValue? value)
        {
            if (value is ScalarValue { Value: LogEventLevel serilogSeverity })
            {
                // Stackdriver: https://jira.humanaedge.com/secure/attachment/19634/screenshot-2.png
                // Serilog Levels: https://jira.humanaedge.com/secure/attachment/19635/screenshot-3.png
                string? severity = serilogSeverity switch
                {
                    LogEventLevel.Information => "INFO",
                    LogEventLevel.Error => "ERROR",
                    LogEventLevel.Fatal => "ALERT",
                    LogEventLevel.Warning => "WARNING",
                    LogEventLevel.Debug => "DEBUG",
                    LogEventLevel.Verbose => "DEBUG",
                    _ => null // default to null.
                };

                return new ScalarValue(severity);
            }

            // Undefined - argument was not a string.
            return null;
        }
    }
}