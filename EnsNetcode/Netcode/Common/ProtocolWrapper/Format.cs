using System;
using System.Text;
using Utils;

namespace ProtocolWrapper
{
    public static class Format
    {
        private const char DataSeparator = '*';

        private static StringBuilder sb=new();
        //Wrapper
        public static string Combine(CircularQueue<string> origin,int maxLength=1400)
        {
            if (origin.Empty())
            {
                Debug.Log("传入了空的原始数据");
                return null;
            }
            var result = sb;
            sb.Clear();
            bool isFirst = true;
            int length = 0;
            while (!origin.Empty())
            {
                if (!isFirst) result.Append(DataSeparator);
                else isFirst = false;

                origin.Read(out string item,false);
                length += item.Length;
                if (length <= maxLength)
                {
                    result.Append(item);
                    origin.RemoveNext();
                }
            }

            return result.ToString();
        }
        public static string[] Split(string origin)
        {
            return origin.Split(new char[]{DataSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }


        //Wrappper
        public static string EnFormat(string s)
        {
            return DataSeparator + s + DataSeparator;
        }
        public static string DeFormat(string s, out bool rightFormat)
        {
            rightFormat = false;
            if (string.IsNullOrEmpty(s)) return null;

            int firstSeparatorIndex = s.IndexOf(DataSeparator);
            int lastSeparatorIndex = s.LastIndexOf(DataSeparator);

            if (firstSeparatorIndex == -1 || lastSeparatorIndex == -1 || firstSeparatorIndex >= lastSeparatorIndex) return null;

            rightFormat = true;
            int length = lastSeparatorIndex - firstSeparatorIndex - 1;
            return s.Substring(firstSeparatorIndex + 1, length);
        }

        //Wrapper
        public static byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        public static string GetString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }
        public static string GetString(byte[] b, int start, int length)
        {
            return Encoding.UTF8.GetString(b, start, length);
        }
    }
}