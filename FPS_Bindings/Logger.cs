using System;

namespace FPS_Bindings
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
    }
    
    public static class Logger
    {
        private static string TimeStamp()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
        }
        
        public static void WriteLine(string message, LogLevel logLevel, params object[] arg)
        {
            string type = "[" + Enum.GetName(typeof(LogLevel), logLevel) + "]";

            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
            
            Console.WriteLine(TimeStamp() + type + " " + message, arg);
            Console.ResetColor();
        }
    }
}