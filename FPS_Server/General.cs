using System;
using FPS_Bindings;

namespace FPS_Server
{
    public static class General
    {
        public static PlayerRect[] players = new PlayerRect[Constants.MAX_PLAYERS];
        public static TempPlayerRect[] tempPlayers = new TempPlayerRect[Constants.MAX_PLAYERS];
        public static bool isRunning;
        
        public static int GetTickCount()
        {
            return Environment.TickCount;
        }
        
        public static void InitServer()
        {
            isRunning = true;
            
            Logger.WriteLine("Loading server ...", LogLevel.DEBUG);
            
            int start = GetTickCount();
            
            InitClients();
            ServerHandleData.InitPackets();
            ServerTCP.InitServer();

            int end = GetTickCount();
            
            Logger.WriteLine("Server has been loaded in {0}ms", LogLevel.DEBUG, end - start);
            
            // Start Loop here in a new thread
        }

        private static void InitClients()
        {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                ServerTCP.clients[i] = new Client(null, 0);
                players[i] = new PlayerRect();
                tempPlayers[i] = new TempPlayerRect();
            }
        }

        public static void JoinMap(int connectionId)
        {
            tempPlayers[connectionId].isPlaying = true;
            
            ServerTCP.SendIngame(connectionId);
            ServerTCP.SendPlayerData(connectionId);
        }
    }    
}