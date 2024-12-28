using System;
using System.Text;

namespace Utils.CryptoValue
{
    public class CryptoInt
    {
        public CryptoInt(int initialValue = default)
        {
            Value = initialValue;
        }
        
        public CryptoInt(string initialValueEncoded)
        {
            EncodedValue = initialValueEncoded;
        }

        public int Value
        {
            get => Decode(EncodedValue);
            set => EncodedValue = Encode(value);
        }

        public string EncodedValue { get; private set; }

        private static string Encode(int input)
        {
            var bytesToEncode = Encoding.UTF8.GetBytes(input.ToString());
            var encodedText = Convert.ToBase64String(bytesToEncode);
            
            return encodedText;
        }

        private static int Decode(string base64Str)
        {
            var decodedBytes = Convert.FromBase64String(base64Str);
            var decodedText = Encoding.UTF8.GetString(decodedBytes);
            
            return int.Parse(decodedText);
        }
    }
}