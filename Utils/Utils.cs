using System.Security.Cryptography;
using System.Text;
using ModelsNamespace;
using Newtonsoft.Json;

namespace UtilsNamespace
{
    public static class Utils
    {
        public static string Hash(string value)
        {
            var md5 = MD5.Create();
            var valueBytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = md5.ComputeHash(valueBytes);
            var hashStringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                hashStringBuilder.Append(hashBytes[i].ToString("x2"));
            var hashString = hashStringBuilder.ToString();
            return hashString;
        }

        public static string AccountToJsonString(Account account)
        {
            return JsonConvert.SerializeObject(account);
        }

        public static Account JsonStringToAccount(string jsonString)
        {
            return JsonConvert.DeserializeObject<Account>(jsonString);
        }
        public static string MessageToJsonString(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public static Message JsonStringToMessage(string jsonString)
        {
            return JsonConvert.DeserializeObject<Message>(jsonString);
        }

        public static byte[] StringToUtf8(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

    }
}
