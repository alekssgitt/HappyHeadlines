using System.Diagnostics;
using System.Text;

namespace Common.Shared.Monitoring;

public static class MessagingTraceContext
{
    public static readonly ActivitySource ActivitySource = new("HappyHeadlines.Messaging");

    public static void InjectTraceHeaders(IDictionary<string, object?> headers)
    {
        var current = Activity.Current;
        if (current is null) return;

        headers["traceparent"] = Encoding.UTF8.GetBytes(current.Id ?? string.Empty);
        headers["tracestate"] = Encoding.UTF8.GetBytes(current.TraceStateString ?? string.Empty);
    }

    public static ActivityContext ExtractActivityContext(IDictionary<string, object?>? headers)
    {
        if (headers is null) return default;

        var traceParent = ReadHeaderAsString(headers, "traceparent");
        var traceState = ReadHeaderAsString(headers, "tracestate");

        if (string.IsNullOrWhiteSpace(traceParent)) return default;

        return ActivityContext.TryParse(traceParent, traceState, out var context)
            ? context
            : default;
    }

    private static string? ReadHeaderAsString(IDictionary<string, object?> headers, string key)
    {
        if (!headers.TryGetValue(key, out var value) || value is null) return null;

        return value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string text => text,
            _ => value.ToString()
        };
    }
}
