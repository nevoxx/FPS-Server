using System;
using System.Collections.Generic;
using FPS_Bindings;

namespace FPS_Server
{
    public class ServerHandleData
    {
        public delegate void Packet_(int connectionId, byte[] data);
        public static Dictionary<int, Packet_> packets = new Dictionary<int, Packet_>();
        private static int pLength;

        public static void InitPackets()
        {
            Logger.WriteLine("Initializing network messages ...", LogLevel.DEBUG);
            
            packets.Add((int) FPS_Bindings.ClientPackets.CMovement, HandleMovement);
        }

        public static void HandleData(int connectionId, byte[] data)
        {
            byte[] buffer = (byte[]) data.Clone();

            if (ServerTCP.clients[connectionId].buffer == null)
            {
                ServerTCP.clients[connectionId].buffer = new ByteBuffer();
            }
            
            ServerTCP.clients[connectionId].buffer.WriteBytes(buffer);

            if (ServerTCP.clients[connectionId].buffer.Count() == 0)
            {
                ServerTCP.clients[connectionId].buffer.Clear();
                return;
            }

            if (ServerTCP.clients[connectionId].buffer.Length() >= 4)
            {
                pLength = ServerTCP.clients[connectionId].buffer.ReadInteger(false);

                if (pLength <= 0)
                {
                    ServerTCP.clients[connectionId].buffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength<= ServerTCP.clients[connectionId].buffer.Length() - 4)
            {
                if (pLength <= ServerTCP.clients[connectionId].buffer.Length() - 4)
                {
                    ServerTCP.clients[connectionId].buffer.ReadInteger();
                    data = ServerTCP.clients[connectionId].buffer.ReadBytes(pLength);
                    HandleDataPackets(connectionId, data);
                }

                pLength = 0;

                if (ServerTCP.clients[connectionId].buffer.Length() >= 4)
                {
                    pLength = ServerTCP.clients[connectionId].buffer.ReadInteger(false);

                    if (pLength < 0)
                    {
                        ServerTCP.clients[connectionId].buffer.Clear();
                        return;
                    }
                }

                if (pLength < 1)
                {
                    ServerTCP.clients[connectionId].buffer.Clear();
                }
            }
        }

        private static void HandleDataPackets(int connectionId, byte[] data)
        {
            int packetId;
            ByteBuffer buffer;
            
            buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            packetId = buffer.ReadInteger();
            buffer.Dispose();

            if (packets.TryGetValue(packetId, out Packet_ packet))
            {
                Logger.WriteLine("<Packet> " + Enum.GetName(typeof(FPS_Bindings.ClientPackets), packetId), LogLevel.DEBUG);
                packet.Invoke(connectionId, data);
            }
        }

        public static void HandleMovement(int connectionId, byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();

            Vector3 position = buffer.ReadVector3();
            Quaternion rotation = buffer.ReadQuaternion();
            
            ServerTCP.SendPlayerMove(connectionId, position, rotation);
        }
    }
}
