using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProbabilisticEncryption
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var keyInfo = new TripleDESKeyInfo();
            using (var myTripleDES = new TripleDESCryptoServiceProvider())
            {
                keyInfo.Key = myTripleDES.Key.ToArray();
                keyInfo.IV = myTripleDES.IV.ToArray();
            }

            var p = new int[] {2, 5, 7, 23};
            var provider = new ProbabilisticEncryptionProvider(keyInfo, p);

            while (true)
            {
                var s = Encoding.UTF8.GetBytes(Console.ReadLine());
                provider.Decrypt(provider.Encrypt(s));
            }
        }
    }
}