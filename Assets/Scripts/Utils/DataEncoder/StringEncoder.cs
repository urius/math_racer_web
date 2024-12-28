using System;
using System.Text;

namespace Utils.DataEncoder
{
    public class StringEncoder
    {
        public static string Encode(string input, string key)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var encodedBytes = new byte[inputBytes.Length];

            for (var i = 0; i < inputBytes.Length; i++)
            {
                encodedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encodedBytes);
        }

        public static string Decode(string encodedInput, string key)
        {
            var encodedBytes = Convert.FromBase64String(encodedInput);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var decodedBytes = new byte[encodedBytes.Length];

            for (var i = 0; i < encodedBytes.Length; i++)
            {
                decodedBytes[i] = (byte)(encodedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}