using UnityEngine;
using System.Collections.Generic;

namespace Retro.Log
{
    // NOTE: The order is important.  Lower value = logging more.  Logging level stacks, meaning it will log the set LogLevel and any 
    //       levels below.  This means we must keep "None" last.
    //       Also, If you are modifying LogLevel, then you MUST update RetroLogCommand.cs. Yes, that includes RetroLogCommand::PrintHelp();
    //       ~PK
    public enum LogLevel : short { Verbose = 0x01, Error = 0x2, Warning = 0x4, Debug = 0x8, Exception = 0x10, None = 0x20 };

    public static class LogLevelExtensions
    {
        public static string ToShortString(this LogLevel ll)
        {
            switch (ll)
            {
                case LogLevel.Verbose:
                    return "[v]";
                case LogLevel.Error:
                    return "[e]";
                case LogLevel.Warning:
                    return "[w]";
                case LogLevel.Debug:
                    return "[d]";
                case LogLevel.Exception:
                    return "[x]";
                default:
                    return "[n]";
            }
        }

        public static LogType ToEngineLogType(this LogLevel ll)
        {
            switch (ll)
            {
                case LogLevel.Verbose:
                    return LogType.Log;
                case LogLevel.Error:
                    return LogType.Error;
                case LogLevel.Warning:
                    return LogType.Warning;
                case LogLevel.Debug:
                    return LogType.Log;
                case LogLevel.Exception:
                    return LogType.Exception;
                default:
                    return LogType.Log;
            }
        }
    }


    /// <summary>
    /// Logging class to enable/disable logging views in the console output of a build/editor.  Main usage is filtering logs to used with grep to trace runtime routes.
    /// </summary>
    public static class RetroLog
    {
        public static readonly string LOG_HEADER = "[RetroLog]";
        public static readonly string LOG_SEPARATOR = " - ";

        public delegate void LogEventDelegate(string szCategory, LogLevel logLevel, string szLogText);
        public static LogEventDelegate LogEvent;

        private static Dictionary<string, RetroLogger> _Category = new Dictionary<string, RetroLogger>();
        private static RetroLogger HackLogger = null;
        public static RetroLogger SystemLogger = null;

#if UNITY_EDITOR

        public static string[] GetCategoryList()
        {
            string[] cats = new string[_Category.Keys.Count];
            _Category.Keys.CopyTo(cats, 0);
            return cats;
        }

#endif

        static RetroLog()
        {
            // ensure we have the system log
            _Category.Add("System", new RetroLogger("System"));
            _Category.Add("hacks", new RetroLogger("hacks"));
            SystemLogger = RetrieveCatagory("System");
            HackLogger = RetrieveCatagory("hacks");

            // spit out all system logs
            SystemLogger.SetOutputLevel(LogLevel.Verbose);
            HackLogger.SetOutputLevel(LogLevel.Verbose);
        }

        private static void BroadcastLog(string szCategory, LogLevel logLevel, string szLogText)
        {
            if(LogEvent != null)
            {
                LogEvent(szCategory, logLevel, szLogText);
            }
        }

        public static RetroLogger RetrieveCatagory(string szCategory)
        {
            if(_Category.ContainsKey(szCategory))
            {
                return _Category[szCategory];
            }

            _Category.Add(szCategory, new RetroLogger(szCategory));
            SystemLogger.Log(LogLevel.Verbose, "Adding Category: " + szCategory);
            return _Category[szCategory];
        }

        public static void Log(string szCategory, LogLevel logLevel, string szText)
        {
            RetrieveCatagory(szCategory).Log(logLevel, szText);
        }

        public static void LogError(string szText)
        {
            SystemLogger.Log(LogLevel.Error, szText);
        }

        public static void LogWarning(string szText)
        {
            SystemLogger.Log(LogLevel.Warning, szText);
        }

        public static void LogException(string szText)
        {
            SystemLogger.Log(LogLevel.Exception, szText);
        }

        public static void Log(string szText)
        {
            SystemLogger.Log(LogLevel.Debug, szText);
        }

        public static void LogHack(string szText)
        {
            HackLogger.Log(LogLevel.Verbose, szText);
        }
    }

    /// <summary>
    /// Logging handler category.  You shouldnt need to create this directly.  Please use RetroLog.
    /// </summary>
    public class RetroLogger
    {
        public LogLevel ActiveOutputLevel { get; private set; }
        public string LogCategory { get; private set; }

        public RetroLogger(string szLogCategory)
        {
            // we want logging upto error when in the editor
            if(Debug.isDebugBuild || Application.isEditor)
                ActiveOutputLevel = LogLevel.Error;
            else
                ActiveOutputLevel = LogLevel.None;

            LogCategory = szLogCategory;
        }

        public void LogError(string szText)
        {
            Log(LogLevel.Error, szText);
        }

        public void LogWarning(string szText)
        {
            Log(LogLevel.Warning, szText);
        }

        public void LogException(string szText)
        {
            Log(LogLevel.Exception, szText);
        }

        public void Log(string szText)
        {
            Log(LogLevel.Debug, szText);
        }

        public void LogVerbose(string szText)
        {
            Log(LogLevel.Verbose, szText);
        }

        public void Log(LogLevel logLevel, string szText)
        {
            // ensure we are set to log to the desired level
            if (logLevel < ActiveOutputLevel)
                return;

            // log out
            switch(logLevel)
            {
                case LogLevel.Debug:
                    Debug.Log(RetroLog.LOG_HEADER + "[" + LogCategory + "]" + logLevel.ToShortString() + RetroLog.LOG_SEPARATOR + szText);
                    break;
                case LogLevel.Verbose:
                    Debug.Log(RetroLog.LOG_HEADER + "[" + LogCategory + "]" + logLevel.ToShortString() + RetroLog.LOG_SEPARATOR + szText);
                    break;
                case LogLevel.Error:
                    Debug.LogError(RetroLog.LOG_HEADER + "[" + LogCategory + "]" + logLevel.ToShortString() + RetroLog.LOG_SEPARATOR + szText);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(RetroLog.LOG_HEADER + "[" + LogCategory + "]" + logLevel.ToShortString() + RetroLog.LOG_SEPARATOR + szText);
                    break;
                case LogLevel.None:
                    // Dont log "None"; the dev is being stupid
                    break;
            }
        }
        
        public void SetOutputLevel(LogLevel eOutputLevel)
        {
            ActiveOutputLevel = eOutputLevel;
        }
    }

}