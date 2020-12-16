using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Serilog.Expressions;
using Serilog.Templates;

namespace HumanaEdge.Webcore.Core.Logging
{
    /// <summary>
    /// A custom serilog formatter, using Serilog Expressions to match Stackdriver Logging expectations.
    /// https://github.com/serilog/serilog-expressions.
    /// </summary>
    /// <remarks>
    /// appsettings.json:
    ///
    /// "Serilog": {
    ///   "Using": [
    ///     "Serilog.Sinks.Console",
    ///     "Serilog.Expressions",
    ///     "HumanaEdge.Webcore.Core.Logging"
    ///   ],
    ///   "MinimumLevel": {
    ///     "Default": "Debug",
    ///     "Override": {
    ///       "Microsoft": "Debug",
    ///       "System": "Warning"
    ///     }
    ///   },
    ///   "WriteTo": {
    ///     "0": {
    ///       "Name": "Console",
    ///       "Args": {
    ///         "formatter": "HumanaEdge.Webcore.Core.Logging.StackdriverFormatter, HumanaEdge.Webcore.Core.Logging"
    ///       }
    ///     }
    ///   }
    /// }.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class StackdriverFormatter : ExpressionTemplate
    {
        /// <summary>
        /// Default rules to map to Stackdriver Logging.
        /// https://cloud.google.com/logging/docs/agent/configuration#special-fields.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once SA1401
        #pragma warning disable SA1401 // FieldsMustBePrivate. I want consumers to have access.
        public static readonly string[] MessageRules = new[]
        {
            "message: if @x is null then @m else @x",
            "severity: MapSeverity(@l)", // custom resolver - StackdriverSeverityMapper
            "@m", // @m - rendered message, @mt - message template
            "time: @t",
            "@x", // exception details
            "requestUrl: RequestPath", // stackdriver might like it
            "..@p" // all the properties
        };

        /// <summary>
        /// Instantiate formatter with custom rules, defaulting to <see cref="MessageRules"/>.
        /// </summary>
        /// <param name="rules">A list of <see cref="MessageRules"/>.</param>
        public StackdriverFormatter(IEnumerable<string> rules = null!)
            : this(ToExpressionTemplate(rules ?? MessageRules))
        {
        }

        /// <summary>
        /// Instantiate formatter with a templateString, eg. "{ {@t, @mt, Severity: @l, @x, ..@p} }\n".
        /// </summary>
        /// <param name="template">A string template in the format "{ { rule1, rule2, rule3, ... } }\n".</param>
        /// <remarks>
        ///   <b>You probably want the other overload "StackdriverFormatter(IEnumerable&lt;string&gt;)"</b>
        ///   https://github.com/serilog/serilog-expressions.
        /// </remarks>
        public StackdriverFormatter(string template)
            : base(template, null, new StaticMemberNameResolver(typeof(StackdriverSeverityMapper)))
        {
        }

        /// <summary>
        /// Convert a list of <see cref="MessageRules"/> to a string expected by ExpressionTemplate ctor.
        /// </summary>
        /// <param name="rules">A collection of strings to be converted.</param>
        /// <returns>A string template in the format "{ { rule1, rule2, rule3, ... } }\n".</returns>
        public static string ToExpressionTemplate(IEnumerable<string> rules) =>
            $"{{ {{ {string.Join(", ", MessageRules)} }} }}\n";
    }
}