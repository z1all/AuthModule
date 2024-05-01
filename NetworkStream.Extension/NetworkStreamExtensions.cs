using System.Text;

namespace System.Net.Sockets.Extension
{
    public static class NetworkStreamExtensions
    {
        public static string ReadString(this NetworkStream stream, int reserveByteCount)
        {
            byte[] data = new byte[reserveByteCount];
            int dataLength = stream.Read(data, 0, data.Length);
            return Encoding.UTF8.GetString(data, 0, dataLength);
        }

        public static byte[] ReadBytes(this NetworkStream stream, int reserveByteCount)
        {
            byte[] data = new byte[reserveByteCount];
            int dataLength = stream.Read(data, 0, data.Length);

            byte[] trimmedData = new byte[dataLength];
            Array.Copy(data, trimmedData, trimmedData.Length);

            return trimmedData;
        }

        public static void WriteString(this NetworkStream stream, string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            stream.Write(strBytes, 0, strBytes.Length);
        }

        public static void WriteBytes(this NetworkStream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }


        public static bool SendSuccess(this NetworkStream stream) { stream.WriteByte(0); return true; }

        public static bool SendFail(this NetworkStream stream) { stream.WriteByte(1); return false; }
    }
}
