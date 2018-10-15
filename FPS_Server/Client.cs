using System;
using System.Net.Sockets;
using FPS_Bindings;

namespace FPS_Server
{
    public class Client
    {
        public TcpClient socket;
        public NetworkStream myStream;
        public int connectionId;
        private byte[] recBuffer;

        public ByteBuffer buffer;

        public Client(TcpClient socket, int connectionId)
        {
            if (socket == null)
            {
                return;
            }
            
            this.socket = socket;
            this.connectionId = connectionId;

            this.socket.SendBufferSize = Constants.MAX_BUFFERSIZE;
            this.socket.ReceiveBufferSize = Constants.MAX_BUFFERSIZE;

            myStream = socket.GetStream();
            recBuffer = new byte[Constants.MAX_BUFFERSIZE];

            myStream.BeginRead(recBuffer, 0, this.socket.ReceiveBufferSize, ReceiverBufferCallback, null);
            
            Logger.WriteLine("Incoming connection from {0}", LogLevel.INFO, this.socket.Client.RemoteEndPoint.ToString());
            
        }

        private void ReceiverBufferCallback(IAsyncResult ar)
        {
            try
            {
                int readBytes = myStream.EndRead(ar);

                if (readBytes <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = new byte[readBytes];
                Buffer.BlockCopy(recBuffer, 0, newBytes, 0, readBytes);

                // Handle data here
                ServerHandleData.HandleData(connectionId, newBytes);

                myStream.BeginRead(recBuffer, 0, this.socket.ReceiveBufferSize, ReceiverBufferCallback, null);
            }
            catch
            {
                CloseConnection();
                return;
            }
        }
        
        private void CloseConnection()
        {
            Logger.WriteLine("Connection from {0} has been terminated.", LogLevel.INFO, this.socket.Client.RemoteEndPoint.ToString());

            ServerTCP.SendPlayerDisconnect(connectionId);
            
            socket.Close();
            socket = null;
        }
    }
}