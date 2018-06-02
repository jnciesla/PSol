using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    class PacketBuffer : IDisposable
    {
        List<byte> buff;
        byte[] readBuff;
        int readPos;
        bool buffUpdate = false;

        public PacketBuffer()
        {
            buff = new List<byte>();
            readPos = 0;
        }

        public int GetReadPos()
        {
            return readPos;
        }

        public byte[] ToArray()
        {
            return buff.ToArray();
        }

        public int Count()
        {
            return buff.Count;
        }

        public int Length()
        {
            return Count() - readPos;
        }

        public void Clear()
        {
            buff.Clear();
            readPos = 0;
        }

        // Write data
        public void AddInteger(int Input)
        {
            buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void AddFloat(float Input)
        {
            buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void AddString(string Input)
        {
            buff.AddRange(BitConverter.GetBytes(Input.Length));
            buff.AddRange(Encoding.ASCII.GetBytes(Input));
            buffUpdate = true;
        }

        public void AddByte(byte Input)
        {
            buff.Add(Input);
            buffUpdate = true;
        }

        public void AddBytes(byte[] Input)
        {
            buff.AddRange(Input);
            buffUpdate = true;
        }

        public void AddShort(short Input)
        {
            buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        // Read data
        public int GetInteger(bool Peek = true)
        {
            if(buff.Count > readPos)
            {
                if (buffUpdate)
                {
                    readBuff = buff.ToArray();
                    buffUpdate = false;
                }

                int ret = BitConverter.ToInt32(readBuff, readPos);
                if(Peek & buff.Count > readPos)
                {
                    readPos += 4;
                }
                return ret;
            } else {
                throw new Exception("Packet buffer is past its limit");
            }

        }

        public float GetFloat(bool Peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdate)
                {
                    readBuff = buff.ToArray();
                    buffUpdate = false;
                }

                float ret = BitConverter.ToSingle(readBuff, readPos);
                if (Peek & buff.Count > readPos)
                {
                    readPos += 4;
                }
                return ret;
            }
            else
            {
                throw new Exception("Packet buffer is past its limit");
            }
        }

        public string GetString(bool Peek = true)
        {
            int length = GetInteger(true);
                if (buffUpdate)
                {
                    readBuff = buff.ToArray();
                    buffUpdate = false;
                }

                string ret = Encoding.ASCII.GetString(readBuff, readPos, length);
                if (Peek & buff.Count > readPos)
                {
                    if(ret.Length > 0)
                {
                    readPos += length;
                }
                }
                return ret;
        }

        public byte GetByte(bool Peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdate)
                {
                    readBuff = buff.ToArray();
                    buffUpdate = false;
                }

                byte ret = readBuff[readPos];
                if (Peek & buff.Count > readPos)
                {
                    readPos += 1;
                }
                return ret;
            }
            else
            {
                throw new Exception("Packet buffer is past its limit");
            }
        }

        public byte[] GetBytes(int Length, bool Peek = true)
        {
            if (buffUpdate)
            {
                readBuff = buff.ToArray();
                buffUpdate = false;
            }

            byte[] ret = buff.GetRange(readPos, Length).ToArray();
            if (Peek)
            {
                readPos += Length;
            }
            return ret;
        }

        public int GetShort(bool Peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdate)
                {
                    readBuff = buff.ToArray();
                    buffUpdate = false;
                }

                short ret = BitConverter.ToInt16(readBuff, readPos);
                if (Peek & buff.Count > readPos)
                {
                    readPos += 4;
                }
                return ret;
            }
            else
            {
                throw new Exception("Packet buffer is past its limit");
            }

        }

        // IDisposable
        private bool disposedValue = false;

        protected virtual void Dispose(bool Disposing)
        {
            if (!this.disposedValue)
            {
                if (Disposing)
                {
                    buff.Clear();
                }

                readPos = 0;
            }
            this.disposedValue = true;
        }

        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
