using System;
using System.Collections.Generic;
using System.Text;

namespace FPS_Bindings
{
    public class ByteBuffer : IDisposable
    {
        private List<byte> Buff;
        private byte[] readBuff;
        private int readPos;
        private bool buffUpdated = false;

        public ByteBuffer()
        {
            Buff = new List<byte>();
            readPos = 0;
        }

        #region Functions

        public int GetReadPos()
        {
            return readPos;
        }

        public byte[] ToArray()
        {
            return Buff.ToArray();
        }

        public int Count()
        {
            return Buff.Count;
        }

        public int Length()
        {
            return Count() - readPos;
        }

        public void Clear()
        {
            Buff.Clear();
            readPos = 0;
        }

        #endregion

        #region WriteData

        public void WriteBytes(byte[] data)
        {
            Buff.AddRange(data);
            buffUpdated = true;
        }

        public void WriteShort(short data)
        {
            Buff.AddRange(BitConverter.GetBytes(data));
            buffUpdated = true;
        }

        public void WriteInteger(int data)
        {
            Buff.AddRange(BitConverter.GetBytes(data));
            buffUpdated = true;
        }

        public void WriteLong(long data)
        {
            Buff.AddRange(BitConverter.GetBytes(data));
            buffUpdated = true;
        }

        public void WriteFloat(float data)
        {
            Buff.AddRange(BitConverter.GetBytes(data));
            buffUpdated = true;
        }

        public void WriteString(string data)
        {
            Buff.AddRange(BitConverter.GetBytes(data.Length));
            Buff.AddRange(Encoding.ASCII.GetBytes(data));
            buffUpdated = true;
        }

        public void WriteVector3(Vector3 data)
        {
            byte[] vectorArray = new byte[sizeof(float) * 3];

            Buffer.BlockCopy(BitConverter.GetBytes(data.x), 0, vectorArray, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.y), 0, vectorArray, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.z), 0, vectorArray, 2 * sizeof(float), sizeof(float));

            Buff.AddRange(vectorArray);
            buffUpdated = true;
        }

        public void WriteQuaternion(Quaternion data)
        {
            byte[] quaternionArray = new byte[sizeof(float) * 4];

            Buffer.BlockCopy(BitConverter.GetBytes(data.x), 0, quaternionArray, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.y), 0, quaternionArray, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.z), 0, quaternionArray, 2 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.w), 0, quaternionArray, 3 * sizeof(float), sizeof(float));

            Buff.AddRange(quaternionArray);
            buffUpdated = true;
        }

        #endregion

        #region ReadData

        public int ReadInteger(bool peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                int value = BitConverter.ToInt32(readBuff, readPos);

                if (peek & Buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("[INT32] You either read out incorrect values or the 'ByteBuffer' is empty");
            }
        }

        public short ReadShort(bool peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                short value = BitConverter.ToInt16(readBuff, readPos);

                if (peek & Buff.Count > readPos)
                {
                    readPos += 2;
                }

                return value;
            }
            else
            {
                throw new Exception("[SHORT] You either read out incorrect values or the 'ByteBuffer' is empty");
            }
        }

        public long ReadLong(bool peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                long value = BitConverter.ToInt64(readBuff, readPos);

                if (peek & Buff.Count > readPos)
                {
                    readPos += 8;
                }

                return value;
            }
            else
            {
                throw new Exception("[LONG] You either read out incorrect values or the 'ByteBuffer' is empty");
            }
        }

        public float ReadFloat(bool peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                float value = BitConverter.ToSingle(readBuff, readPos);

                if (peek & Buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("[FLOAT] You either read out incorrect values or the 'ByteBuffer' is empty");
            }
        }

        public byte[] ReadBytes(int length, bool peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                byte[] value = Buff.GetRange(readPos, length).ToArray();

                if (peek)
                {
                    readPos += length;
                }

                return value;
            }
            else
            {
                throw new Exception("[Byte[]] You either read out incorrect values or the 'ByteBuffer' is empty");
            }
        }

        public string ReadString(bool peek = true)
        {
            int length = ReadInteger(true);

            if (buffUpdated)
            {
                readBuff = Buff.ToArray();
                buffUpdated = false;
            }

            string value = Encoding.ASCII.GetString(readBuff, readPos, length);

            if (peek & Buff.Count > readPos)
            {
                if (value.Length > 0)
                {
                    readPos += length;
                }
            }

            return value;
        }

        public Vector3 ReadVector3(bool peek = true)
        {
            if (buffUpdated)
            {
                readBuff = Buff.ToArray();
                buffUpdated = false;
            }

            byte[] value = Buff.GetRange(readPos, sizeof(float) * 3).ToArray();

            Vector3 vector3;

            vector3.x = BitConverter.ToSingle(value, 0 * sizeof(float));
            vector3.y = BitConverter.ToSingle(value, 1 * sizeof(float));
            vector3.z = BitConverter.ToSingle(value, 2 * sizeof(float));

            if (peek)
            {
                readPos += sizeof(float) * 3;
            }

            return vector3;
        }

        public Quaternion ReadQuaternion(bool peek = true)
        {
            if (buffUpdated)
            {
                readBuff = Buff.ToArray();
                buffUpdated = false;
            }

            byte[] value = Buff.GetRange(readPos, sizeof(float) * 4).ToArray();

            Quaternion quaternion;

            quaternion.x = BitConverter.ToSingle(value, 0 * sizeof(float));
            quaternion.y = BitConverter.ToSingle(value, 1 * sizeof(float));
            quaternion.z = BitConverter.ToSingle(value, 2 * sizeof(float));
            quaternion.w = BitConverter.ToSingle(value, 3 * sizeof(float));

            if (peek)
            {
                readPos += sizeof(float) * 4;
            }

            return quaternion;
        }

        #endregion

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (disposing)
                {
                    Buff.Clear();
                    readPos = 0;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}