/*
Copyright 2019 - 2023 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace umi3d.common
{
    /// <summary>
    /// Enumeration flags for log type.
    /// </summary>
    [Flags]
    public enum UMI3DLogType
    {
        /// <summary>
        /// No logs.
        /// </summary>
        None = 0,

        /// <summary>
        /// Logs that are only visible if UNITY_ASSERTIONS is defined.
        /// 
        /// <para>
        /// They look like default logs.
        /// </para>
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Assert Logs that are only visible if UNITY_ASSERTIONS is defined.
        /// 
        /// <para>
        /// They look like assert logs.
        /// </para>
        /// </summary>
        DebugAssert = 2,
        /// <summary>
        /// TODO Logs that are only visible if UNITY_ASSERTIONS is defined.
        /// 
        /// <para>
        /// They look like todo logs.
        /// </para>
        /// </summary>
        DebugTodo = 4,
        /// <summary>
        /// HACK Logs that are only visible if UNITY_ASSERTIONS is defined.
        /// 
        /// <para>
        /// They look like hack logs.
        /// </para>
        /// </summary>
        DebugHack = 8,

        /// <summary>
        /// Default logs. You can used them in build.
        /// </summary>
        Default = 16,
        /// <summary>
        /// Warning logs. You can used them in build.
        /// </summary>
        Warning = 32,
        /// <summary>
        /// Assert Logs. You can used them in build.
        /// </summary>
        Assert = 64,
        /// <summary>
        /// Error Logs. You can used them in build.
        /// </summary>
        Error = 128,
        /// <summary>
        /// Exception Logs. You can used them in build.
        /// </summary>
        Exception = 256
    }

    /// <summary>
    /// The color of the log when displayed in the Unity Console.
    /// </summary>
    public enum UMI3DLogColor
    {
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Orange,
        Grey,
        Black
    }

    /// <summary>
    /// Alignment of a message in a formated string.
    /// </summary>
    public enum UMI3DStringAlignment
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Represent a cell in a tab log.
    /// </summary>
    public sealed class UMI3DLogCell
    {
        public string Header;
        public object Message;
        public int StringFormatSize;
        public UMI3DStringAlignment StringFormatAlignment;

        public UMI3DLogCell(string header, object message, int stringFormatSize, UMI3DStringAlignment alignment = UMI3DStringAlignment.Left)
        {
            this.Header = header;
            this.Message = message;
            this.StringFormatSize = stringFormatSize;
            this.StringFormatAlignment = alignment;
        }
    }

    /// <summary>
    /// A Logger for UMI3D client.
    /// 
    /// <para>
    /// To take advantage of this logger you can fallow this simple guide:
    /// <list type="bullet">
    /// <item>Right click on the console tab header and selection "Use Monospace font"</item>
    /// </list>
    /// </para>
    /// </summary>
    [Serializable]
    public class UMI3DClientLogger
    {
        /// <summary>
        /// All <see cref="UMI3DLogType"/> cases.
        /// </summary>
        public const UMI3DLogType LogTypeAllCases =
            UMI3DLogType.Debug
            | UMI3DLogType.DebugAssert
            | UMI3DLogType.DebugTodo
            | UMI3DLogType.DebugHack
            | UMI3DLogType.Default
            | UMI3DLogType.Warning
            | UMI3DLogType.Assert
            | UMI3DLogType.Error
            | UMI3DLogType.Exception;

        /// <summary>
        /// The static log type filter.
        /// </summary>
        public static UMI3DLogType GeneralLogType = LogTypeAllCases;

        /// <summary>
        /// The log handler that will handle logs.
        /// 
        /// <para>
        /// By default <see cref="UnityEngine.Debug.unityLogger"/>.logHandler.
        /// </para>
        /// </summary>
        public ILogHandler LogHandler;

        /// <summary>
        /// The log type filter of this logger.
        /// </summary>
        public UMI3DLogType LogType;
        /// <summary>
        /// The main tag of this logger.
        /// 
        /// <para>
        /// Usualy the type name of the logger's master.
        /// </para>
        /// </summary>
        public string MainTag;
        /// <summary>
        /// The main context of this logger.
        /// 
        /// <para>
        /// Usualy the instance of the logger's master if it is a <see cref="UnityEngine.Object"/>.
        /// </para>
        /// </summary>
        public UnityEngine.Object MainContext;
        /// <summary>
        /// Wherther or not the thread id has to be displayed.
        /// 
        /// <para>
        /// It is helpfull when debuging multi-threaded feature.
        /// </para>
        /// </summary>
        public bool IsThreadDisplayed;

        /// <summary>
        /// The list of table headers.
        /// </summary>
        Dictionary<string, (string[] headers, int cellIndex)> tabHeaders;

        /// <summary>
        /// Create a client logger by defining all of its properties.
        /// </summary>
        /// <param name="logHandler"></param>
        /// <param name="logType"></param>
        /// <param name="mainTag"></param>
        /// <param name="mainContext"></param>
        /// <param name="isThreadDisplayed"></param>
        public UMI3DClientLogger(ILogHandler logHandler, UMI3DLogType logType, string mainTag, UnityEngine.Object mainContext, bool isThreadDisplayed)
        {
            this.LogHandler = logHandler;
            this.LogType = logType;
            this.MainTag = mainTag;
            this.MainContext = mainContext;
            this.IsThreadDisplayed = isThreadDisplayed;
        }

        /// <summary>
        /// Create a client logger by defining some of its properties.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="mainTag"></param>
        /// <param name="mainContext"></param>
        /// <param name="isThreadDisplayed"></param>
        public UMI3DClientLogger(UMI3DLogType logType = LogTypeAllCases, string mainTag = null, UnityEngine.Object mainContext = null, bool isThreadDisplayed = false) : this(
            UnityEngine.Debug.unityLogger.logHandler,
            logType,
            mainTag,
            mainContext,
            isThreadDisplayed
        )
        {

        }

        private string FormatLog(UMI3DLogType logType, string tag, UMI3DLogColor color, object message, string headerMessage = null)
        {
            const string spacing = "     ";

            string result = "";

            if (color != UMI3DLogColor.White)
            {
                result += $"<color={color.ToString().ToLower()}>";
            }

            result += logType.FormatString(11, UMI3DStringAlignment.Left);

            if (!string.IsNullOrEmpty(MainTag))
            {
                result += $"{spacing}";
                if (MainTag.Length > 20 && MainTag.Length + tag.Length > 40)
                {
                    result += $"{MainTag}".FormatString(20, UMI3DStringAlignment.Left);
                    result += "-";
                    result += $"{tag}".FormatString(20, UMI3DStringAlignment.Left);
                }
                else
                {
                    result += $"{MainTag}-{tag}".FormatString(41, UMI3DStringAlignment.Left);
                }
            }
            else
            {
                result += $"{spacing}";
                result += $"{tag}".FormatString(41, UMI3DStringAlignment.Left);
            }

            if (IsThreadDisplayed)
            {
                result += $"{spacing}";
                result += $"{Thread.CurrentThread.ManagedThreadId}".FormatString(10, UMI3DStringAlignment.Left);
            }

            if (!string.IsNullOrEmpty(headerMessage))
            {
                result += $"{spacing}";
                result += $"{headerMessage} ";
                result += $"{message.GetString()}";
            }
            else
            {
                result += $"{spacing}";
                result += $"{message.GetString()}";
            }

            if (color != UMI3DLogColor.White)
            {
                result += "</color>";
            }

            return result;
        }

        /// <summary>
        /// A variant of Log that logs an Debug message.
        /// 
        /// <para>
        /// Debug logs:
        /// <list type="bullet">
        /// <item>are displayed in build only if the UNITY_ASSERTIONS macro has been added.</item>
        /// <item>have no stack trace in build.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public void Debug(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.White)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Debug) || !LogType.HasFlag(UMI3DLogType.Debug))
            {
                return;
            }

#if !UNITY_EDITOR
            var stackTraceType = Application.GetStackTraceLogType(UnityEngine.LogType.Log);
            Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
#endif

            LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.Debug, tag, color, message));

#if !UNITY_EDITOR
            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, stackTraceType);
#endif
        }

        /// <summary>
        /// A variant of Log that logs a DebugAssertion message.
        /// 
        /// <para>
        /// DebugAssertion logs are displayed in build only if the UNITY_ASSERTIONS macro has been added.
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public void DebugAssertion(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Orange)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.DebugAssert) || !LogType.HasFlag(UMI3DLogType.DebugAssert))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Assert, context ?? MainContext, FormatLog(UMI3DLogType.DebugAssert, tag, color, message));
        }

        /// <summary>
        /// A variant of Log that logs a DebugAssertion message if <paramref name="condition"/> is false.
        /// 
        /// <para>
        /// DebugAssert logs are displayed in build only if the UNITY_ASSERTIONS macro has been added.
        /// </para>
        /// </summary>
        /// <param name="condition">The condition has to be false to log the message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public void DebugAssert(bool condition, string tag, object message = null, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Orange)
        {
            if (condition) return;

            if (!GeneralLogType.HasFlag(UMI3DLogType.DebugAssert) || !LogType.HasFlag(UMI3DLogType.DebugAssert))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Assert, context ?? MainContext, FormatLog(UMI3DLogType.DebugAssert, tag, color, message));
        }

        /// <summary>
        /// A variant of Log that logs an DebugTodo message.
        /// 
        /// <para>
        /// DebugTodo logs are not displayed in build.
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS"), Conditional("UNITY_EDITOR")]
        public void DebugTodo(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Green)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.DebugTodo) || !LogType.HasFlag(UMI3DLogType.DebugTodo))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.DebugTodo, tag, color, message, "TODO:"));
        }

        /// <summary>
        /// A variant of Log that logs an DebugHack message.
        /// 
        /// <para>
        /// DebugHack logs are not displayed in build.
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS"), Conditional("UNITY_EDITOR")]
        public void DebugHack(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Orange)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.DebugHack) || !LogType.HasFlag(UMI3DLogType.DebugHack))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.DebugHack, tag, color, message, "HACK:"));
        }

        /// <summary>
        /// A variant of Log that logs a DebugTab message.
        /// 
        /// <para>
        /// Debug logs:
        /// <list type="bullet">
        /// <item>are displayed in build only if the UNITY_ASSERTIONS macro has been added.</item>
        /// <item>have no stack trace in build.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="tabName">A unique identifier for the tab.</param>
        /// <param name="cells">The collection of cells in a line.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public void DebugTab(string tabName, UMI3DLogCell[] cells, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.White)
        {
            UnityEngine.Debug.Assert(cells != null, $"Cells array is null when trying to {nameof(DebugTab)}");
            UnityEngine.Debug.Assert(cells.Length > 0, $"Cells array has 0 item when trying to {nameof(DebugTab)}");

            var headers = new string[cells.Length];
            var messages = new string[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                var size = cells[i].StringFormatSize;
                var alignment = cells[i].StringFormatAlignment;

                headers[i] = cells[i].Header.FormatString(size, UMI3DStringAlignment.Center);
                messages[i] = cells[i].Message.GetString().FormatString(size, alignment);
            }

            Predicate<string[]> predicate = _headers =>
            {
                if (_headers.Length != headers.Length)
                {
                    return false;
                }
                else
                {
                    return Enumerable.SequenceEqual(_headers, headers);
                }
            };

#if !UNITY_EDITOR
            var stackTraceType = Application.GetStackTraceLogType(UnityEngine.LogType.Log);
            Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
#endif

            var headerMessage = "Cell".FormatString(6) + " | " + string.Join(" | ", headers);
            var cellIndex = 0;
            if (tabHeaders == null)
            {
                tabHeaders = new Dictionary<string, (string[], int)>() { { tabName, (headers, cellIndex) } };
                LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.Debug, $"Tab: {tabName}", color, headerMessage));
            }
            else if (!tabHeaders.ContainsKey(tabName))
            {
                tabHeaders.Add(tabName, (headers, cellIndex));
                LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.Debug, $"Tab: {tabName}", color, headerMessage));
            }
            else
            {
                cellIndex = tabHeaders[tabName].cellIndex + 1;
                tabHeaders[tabName] = (headers, cellIndex);
            }

            var message = $"{cellIndex}".FormatString(6) + " | " + string.Join(" | ", messages);
            LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.Debug, $"Tab: {tabName}", color, message));

#if !UNITY_EDITOR
            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, stackTraceType);
#endif
        }

        /// <summary>
        /// A variant of Log that logs an Default message.
        /// 
        /// <para>
        /// Default logs are no stack trace in build.
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Default(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.White)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Default) || !LogType.HasFlag(UMI3DLogType.Default))
            {
                return;
            }

#if !UNITY_EDITOR
            var stackTraceType = Application.GetStackTraceLogType(UnityEngine.LogType.Log);
            Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
#endif

            LogHandler.LogFormat(UnityEngine.LogType.Log, context ?? MainContext, FormatLog(UMI3DLogType.Default, tag, color, message));

#if !UNITY_EDITOR
            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, stackTraceType);
#endif
        }

        /// <summary>
        /// A variant of Log that logs an Warning message.
        /// 
        /// <para>
        /// Warning have no stack trace.
        /// </para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Warning(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Yellow)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Warning) || !LogType.HasFlag(UMI3DLogType.Warning))
            {
                return;
            }

            var stackTraceType = Application.GetStackTraceLogType(UnityEngine.LogType.Warning);
            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, StackTraceLogType.None);

            LogHandler.LogFormat(UnityEngine.LogType.Warning, context ?? MainContext, FormatLog(UMI3DLogType.Warning, tag, color, message));

            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, stackTraceType);
        }

        /// <summary>
        /// A variant of Log that logs an Assertion message.
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Assertion(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Orange)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Assert) || !LogType.HasFlag(UMI3DLogType.Assert))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Assert, context ?? MainContext, FormatLog(UMI3DLogType.Assert, tag, color, message));
        }

        /// <summary>
        /// A variant of Log that logs an Assertion message if <paramref name="condition"/> is false.
        /// </summary>
        /// <param name="condition">The condition has to be false to log the message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Assert(bool condition, string tag, object message = null, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Orange)
        {
            if (condition) return;

            if (!GeneralLogType.HasFlag(UMI3DLogType.Assert) || !LogType.HasFlag(UMI3DLogType.Assert))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Assert, context ?? MainContext, FormatLog(UMI3DLogType.Assert, tag, color, message));
        }

        /// <summary>
        /// A variant of Log that logs an Error message.
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Error(string tag, object message, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Red)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Error) || !LogType.HasFlag(UMI3DLogType.Error))
            {
                return;
            }

            LogHandler.LogFormat(UnityEngine.LogType.Error, context ?? MainContext, FormatLog(UMI3DLogType.Error, tag, color, message));
        }

        /// <summary>
        /// A variant of Log that logs an Exception message.
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the method where the log call occurs.</param>
        /// <param name="exception">Runtime Exception.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="color">The color of the log in the Unity console.</param>
        public void Exception(string tag, Exception exception, UnityEngine.Object context = null, UMI3DLogColor color = UMI3DLogColor.Red)
        {
            if (!GeneralLogType.HasFlag(UMI3DLogType.Exception) || !LogType.HasFlag(UMI3DLogType.Exception))
            {
                return;
            }

            //LogHandler.LogException(exception, context ?? MainContext);
            LogHandler.LogFormat(UnityEngine.LogType.Exception, context ?? MainContext, FormatLog(UMI3DLogType.Exception, tag, color, exception.Message));
        }

        /// <summary>
        /// A simple method to let you preview the logs.
        /// 
        /// <para>
        /// <list type="bullet">
        /// <item>Create a client logger.</item>
        /// <item>Assign or not a main tag and a main context.</item>
        /// <item>Choose to display or not the thread id.</item>
        /// <item>Call this method.</item>
        /// </list>
        /// </para>
        /// </summary>
        public void Preview()
        {
#if UNITY_EDITOR
            Debug("Preview", "A debug message");
            DebugAssertion("Preview", "A debugAssertion message");
            DebugTodo("Preview", "A Todo message");
            DebugHack("Preview", "A Hack message");

            Default("Preview", "A default message");
            Warning("Preview", "A warning message");
            Assertion("Preview", "An assertion message");
            Error("Preview", "An error message");

            Exception("Preview", new Exception("An exception message"));
#endif
        }
    }

    /// <summary>
    /// Extensions toolkit.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Get a string from an object.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetString(this object message)
        {
            if (message == null)
            {
                return "Null";
            }

            IFormattable formattable = message as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            }

            return message.ToString();
        }

        /// <summary>
        /// Format a message to be of the size of <paramref name="size"/> and aligned by <paramref name="alignment"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="size"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static string FormatString(this object message, int size, UMI3DStringAlignment alignment = UMI3DStringAlignment.Left)
        {
            var s = message.GetString();

            if (s.Length > size)
            {
                s = s.Substring(0, size-3) + "...";
            }

            switch (alignment)
            {
                case UMI3DStringAlignment.Left:
                    return String.Format($"{{0, -{size}}}", s);
                case UMI3DStringAlignment.Center:
                    return String.Format(
                    $"{{0,-{size}}}",
                    String.Format($"{{0, {(size + s.Length) / 2}}}", s)
                    );
                case UMI3DStringAlignment.Right:
                    return String.Format($"{{0, {size}}}", s);
                default:
                    return s;
            }
        }
    }
}
