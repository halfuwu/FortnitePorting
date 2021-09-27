using System;

namespace FortnitePorting.Utils
{
    public class SimpleLogger
    {
        private string _loggerName;

        public SimpleLogger(string name)
        {
            _loggerName = name;
        }

        public void Log<T>(T input, ELogLevel logLevel = ELogLevel.Info)
        {
            Console.Write($"{_loggerName}: ");
            Console.ForegroundColor = logLevel switch
            {
                ELogLevel.Debug => ConsoleColor.DarkMagenta,
                ELogLevel.Warn => ConsoleColor.Yellow,
                ELogLevel.Error => ConsoleColor.Red,
                ELogLevel.Info => ConsoleColor.DarkCyan,
                ELogLevel.Critical => ConsoleColor.DarkRed,
                ELogLevel.Variant => ConsoleColor.Magenta,
                _ => Console.ForegroundColor
            };
            Console.Write($"{logLevel}: ");
            Console.ResetColor();
            Console.WriteLine(input);
            
            if (logLevel == ELogLevel.Critical)
            {
                Program.PromptExit(1);
            }
        }

        public enum ELogLevel
        {
            Debug,
            Info,
            Variant,
            Warn,
            Error,
            Critical
        }
    }
}