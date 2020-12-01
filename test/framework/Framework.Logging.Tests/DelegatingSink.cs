using System;
using Serilog.Core;
using Serilog.Events;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// An implementation of Serilog's <see cref="ILogEventSink"/> that allows one to register a callback for Serilog log output.
    /// </summary>
    public class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _write;

        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="write">Callback to receive Serilog log output.</param>
        /// <exception cref="ArgumentNullException">The callback write is not allowed to be null.</exception>
        public DelegatingSink(Action<LogEvent> write)
        {
            _write = write ?? throw new ArgumentNullException(nameof(write));
        }

        /// <summary>
        /// <see cref="ILogEventSink.Emit"/>.
        /// </summary>
        /// <param name="logEvent">The Serilog log event to write to the callback sink.</param>
        public void Emit(LogEvent logEvent)
        {
            _write(logEvent);
        }
    }
}