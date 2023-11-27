// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System;
using UnityEngine;

using ConditionalAttribute = System.Diagnostics.ConditionalAttribute;

namespace Mediapipe.Unity
{
  public interface IExtendedLogger : ILogger
  {
    void Log(Logger.LogLevel logLevel, string tag, object message, UnityEngine.Object context);
    void Log(Logger.LogLevel logLevel, string tag, object message);
    void Log(Logger.LogLevel logLevel, object message, UnityEngine.Object context);
    void Log(Logger.LogLevel logLevel, object message);
  }

  public static class Logger
  {
    static IExtendedLogger internalLogger;
      
    public enum LogLevel
    {
      Fatal,
      Error,
      Warn,
      Info,
      Verbose,
      Debug,
    }

    public static LogLevel MinLogLevel { get; set; } = LogLevel.Info;
    public static IExtendedLogger InternalLogger
      => internalLogger ??= new LoggerWrapper(Debug.unityLogger);
    public static void SetLogger(IExtendedLogger newLogger)
      => internalLogger = newLogger;

    public static void SetLogger(ILogger newLogger)
      => internalLogger = new LoggerWrapper(newLogger);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogException(Exception exception, UnityEngine.Object context)
    {
      if (MinLogLevel >= LogLevel.Error)
        InternalLogger.LogException(exception, context);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogException(Exception exception)
    {
      if (MinLogLevel >= LogLevel.Error)
        InternalLogger.LogException(exception);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(string tag, object message, UnityEngine.Object context)
    {
      if (MinLogLevel >= LogLevel.Error)
        InternalLogger.LogError(tag, message, context);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(string tag, object message)
    {
      if (MinLogLevel >= LogLevel.Error)
        InternalLogger.LogError(tag, message);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object message) => LogError(null, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(string tag, object message, UnityEngine.Object context)
    {
      if (MinLogLevel >= LogLevel.Info)
        InternalLogger.LogWarning(tag, message, context);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(string tag, object message)
    {
      if (MinLogLevel >= LogLevel.Info)
        InternalLogger.LogWarning(tag, message);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message)
      => LogWarning(null, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogLevel logLevel, string tag, object message, UnityEngine.Object context)
    {
      if (MinLogLevel >= logLevel)
        InternalLogger.Log(logLevel, tag, message, context);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogLevel logLevel, string tag, object message)
    {
      if (MinLogLevel >= logLevel)
        InternalLogger.Log(logLevel, tag, message);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogLevel logLevel, object message, UnityEngine.Object context)
    {
      if (MinLogLevel >= logLevel)
        InternalLogger.Log(logLevel, message, context);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogLevel logLevel, object message)
    {
      if (MinLogLevel >= logLevel)
        InternalLogger.Log(logLevel, message);
    }

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string tag, object message)
      => Log(LogLevel.Info, tag, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message)
      => Log(LogLevel.Info, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(string tag, object message, UnityEngine.Object context)
      => Log(LogLevel.Info, tag, message, context);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(string tag, object message)
      => Log(LogLevel.Info, tag, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(object message)
      => Log(LogLevel.Info, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogVerbose(string tag, object message, UnityEngine.Object context)
      => Log(LogLevel.Verbose, tag, message, context);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogVerbose(string tag, object message)
      => Log(LogLevel.Verbose, tag, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogVerbose(object message)
      => Log(LogLevel.Verbose, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogDebug(string tag, object message, UnityEngine.Object context)
      => Log(LogLevel.Debug, tag, message, context);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogDebug(string tag, object message)
      => Log(LogLevel.Debug, tag, message);

    [Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogDebug(object message)
      => Log(LogLevel.Debug, message);

    class LoggerWrapper : IExtendedLogger
    {
      readonly ILogger logger;

      public LoggerWrapper(ILogger logger)
        => this.logger = logger;

      public LogType filterLogType
      {
        get => logger.filterLogType;
        set => logger.filterLogType = value;
      }

      public bool logEnabled
      {
        get => logger.logEnabled;
        set => logger.logEnabled = value;
      }

      public ILogHandler logHandler
      {
        get => logger.logHandler;
        set => logger.logHandler = value;
      }

      public bool IsLogTypeAllowed(LogType logType)
        => logger.IsLogTypeAllowed(logType);
      public void Log(LogType logType, object message) 
        => logger.Log(logType, message);
      public void Log(LogType logType, object message, UnityEngine.Object context)
        => logger.Log(logType, message, context);
      public void Log(LogType logType, string tag, object message)
        => logger.Log(logType, tag, message);
      public void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        => logger.Log(logType, tag, message, context);
      public void LogFormat(LogType logType, string format, params object[] args)
        => logger.LogFormat(logType, format, args);
      public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        => logger.LogFormat(logType, context, format, args);
      void ILogger.Log(object message)
        => logger.Log(message);
      void ILogger.Log(string tag, object message)
        => logger.Log(tag, message);
      void ILogger.Log(string tag, object message, UnityEngine.Object context)
        => logger.Log(tag, message, context);
      void IExtendedLogger.Log(LogLevel logLevel, string tag, object message, UnityEngine.Object context)
        => logger.Log(logLevel.GetLogType(), tag, message, context);
      void IExtendedLogger.Log(LogLevel logLevel, string tag, object message)
        => logger.Log(logLevel.GetLogType(), tag, message);
      void IExtendedLogger.Log(LogLevel logLevel, object message, UnityEngine.Object context)
        => logger.Log(logLevel.GetLogType(), message, context);
      void IExtendedLogger.Log(LogLevel logLevel, object message)
        => logger.Log(logLevel.GetLogType(), message);
      void ILogger.LogWarning(string tag, object message)
        => logger.LogWarning(tag, message);
      void ILogger.LogWarning(string tag, object message, UnityEngine.Object context)
        => logger.LogWarning(tag, message, context);
      void ILogger.LogError(string tag, object message)
        => logger.LogError(tag, message);
      void ILogger.LogError(string tag, object message, UnityEngine.Object context)
        => logger.LogError(tag, message, context);
      void ILogger.LogException(Exception exception)
        => logger.LogException(exception);
      void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
        => logger.LogException(exception, context);
    }
  }

  public static class LoggerLogLevelExtension
  {
    public static LogType GetLogType(this Logger.LogLevel logLevel)
    {
      switch (logLevel)
      {
        case Logger.LogLevel.Fatal:
        case Logger.LogLevel.Error: return LogType.Error;
        case Logger.LogLevel.Warn: return LogType.Warning;
        case Logger.LogLevel.Info:
        case Logger.LogLevel.Verbose:
        case Logger.LogLevel.Debug:
        default: return LogType.Log;
      }
    }
  }
}
