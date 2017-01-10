using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace PreCompileEnvironmentVariablesWebhookCSharp
{
    public class CustomTraceWriter : TraceWriter
    {
        public CustomTraceWriter(TraceLevel level) : base(level)
        {
        }

        #region Overrides of TraceWriter

        public override void Trace(TraceEvent traceEvent)
        {
            Console.WriteLine($"{traceEvent.Timestamp} : {traceEvent.Message}");
        }

        #endregion
    }
}
