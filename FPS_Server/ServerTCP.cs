using System;
using System.Net;
using System.Net.Sockets;
using FPS_Bindings;

namespace FPS_Server
{
    public class ServerTCP
    {
        private static TcpListener serverSocket = new TcpListener(IPAddress.Any, 5555);
        public static Client[] clients = new Client[Constants.MAX_PLAYERS];

        public static void InitServer()
        {
            Logger.WriteLine("Initializing server socket ...", LogLevel.DEBUG);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);
        }

        private static void ClientConnectCallback(IAsyncResult ar)
        {
            TcpClient tempClient = serverSocket.EndAcceptTcpClient(ar);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);
            
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (clients[i].socket == null)
                {
                    clients[i] = new Client(tempClient, i);
                    General.JoinMap(i);
                    return;
                }
            }
        }

        public static void SendDataTo(int connectionId, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            clients[connectionId].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static void SendDataToAll(byte[] data)
        {
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                // send to all connected players except us
                if (clients[i].socket != null && General.tempPlayers[i].isPlaying)
                {
                    SendDataTo(i, data);   
                }
            }
        }


        public static void SendDataToAllBut(int connectionId, byte[] data)
        {
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (connectionId == i)
                {
                    continue;    
                }
                
                // send to all connected players except us
                if (clients[i].socket != null && General.tempPlayers[i].isPlaying)
                {
                    SendDataTo(i, data);   
                }
            }
        }

        public static void SendIngame(int connectionId)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int) ServerPackets.SIngame);
            buffer.WriteInteger(connectionId);
            SendDataTo(connectionId, buffer.ToArray());
            buffer.Dispose();
        }

        public static byte[] PlayerData(int connectionId)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int) ServerPackets.SPlayerData);
            buffer.WriteInteger(connectionId);
            
            return buffer.ToArray();
        }

        public static void SendPlayerData(int connectionId)
        {
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                // send to all connected players except us
                if (clients[i] != null && General.tempPlayers[i].isPlaying && connectionId != i)
                {
                    SendDataTo(connectionId, PlayerData(i));   
                }
            }
            
            SendDataToAll(PlayerData(connectionId));
        }
    
        public static void SendPlayerDisconnect(int connectionId)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int) FPS_Bindings.ServerPackets.SPlayerDisconnect);
            buffer.WriteInteger(connectionId);
            SendDataToAllBut(connectionId, buffer.ToArray());
        }
    
        public static void SendPlayerMove(int connectionId, Vector3 position, Quaternion rotation)
        {
            Logger.WriteLine("Sending Player move", LogLevel.INFO);
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int) FPS_Bindings.ServerPackets.SPlayerMove);
            buffer.WriteInteger(connectionId);
            buffer.WriteVector3(position);
            buffer.WriteQuaternion(rotation);
            SendDataToAllBut(connectionId, buffer.ToArray());
        }
    }
}
