using System;
using System.Threading;
using FPS_Bindings;

namespace FPS_Server
{
    internal class Program
    {
        static Thread mainThread = new Thread(MainThread);

        public static void Main()
        {
            mainThread.Name = "MainThread";

            Logger.WriteLine("Initializing '" + mainThread.Name + "' thread.", LogLevel.DEBUG, mainThread.Name);
            mainThread.Start();
        }

        static void MainThread()
        {
            General.InitServer();

            while (General.isRunning)
            {
                // main loop
            }
        }
    }
}