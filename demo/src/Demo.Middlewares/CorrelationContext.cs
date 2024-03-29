using System;
using System.Threading;

namespace Demo.Middlewares
{
    public class CorrelationContext
    {
        private static AsyncLocal<CorrelationContext> AsyncInstance = new AsyncLocal<CorrelationContext>();

        public static CorrelationContext Instance
        {
            get => AsyncInstance.Value;
            set => AsyncInstance.Value = value;
        }

        public string CorrelationId { get; set; } = Guid.NewGuid().ToString("D");
    }
}