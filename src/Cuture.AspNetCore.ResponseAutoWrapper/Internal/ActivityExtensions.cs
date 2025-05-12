#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

using System.Collections.Generic;

namespace System.Diagnostics;

internal static class ActivityExtensions
{
    #region Public 方法

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/release/9.0/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Activity.cs#L552
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="exception"></param>
    /// <param name="tags"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Activity AddException(this Activity activity, Exception exception, in TagList tags = default, DateTimeOffset timestamp = default)
    {
        ArgumentNullException.ThrowIfNull(exception);

        TagList exceptionTags = tags;

        const string ExceptionEventName = "exception";
        const string ExceptionMessageTag = "exception.message";
        const string ExceptionStackTraceTag = "exception.stacktrace";
        const string ExceptionTypeTag = "exception.type";

        bool hasMessage = false;
        bool hasStackTrace = false;
        bool hasType = false;

        for (int i = 0; i < exceptionTags.Count; i++)
        {
            if (exceptionTags[i].Key == ExceptionMessageTag)
            {
                hasMessage = true;
            }
            else if (exceptionTags[i].Key == ExceptionStackTraceTag)
            {
                hasStackTrace = true;
            }
            else if (exceptionTags[i].Key == ExceptionTypeTag)
            {
                hasType = true;
            }
        }

        if (!hasMessage)
        {
            exceptionTags.Add(new KeyValuePair<string, object?>(ExceptionMessageTag, exception.Message));
        }

        if (!hasStackTrace)
        {
            exceptionTags.Add(new KeyValuePair<string, object?>(ExceptionStackTraceTag, exception.ToString()));
        }

        if (!hasType)
        {
            exceptionTags.Add(new KeyValuePair<string, object?>(ExceptionTypeTag, exception.GetType().ToString()));
        }

        return activity.AddEvent(new ActivityEvent(ExceptionEventName, timestamp, [.. exceptionTags]));
    }

    public static Activity? RecordException(this Activity? activity, Exception exception, in TagList tags = default, DateTimeOffset timestamp = default)
    {
        if (activity is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error);
            activity.AddException(exception, tags, timestamp);
        }
        return activity;
    }

    #endregion Public 方法
}
