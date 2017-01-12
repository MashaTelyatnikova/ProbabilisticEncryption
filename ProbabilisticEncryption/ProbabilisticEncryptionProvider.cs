using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProbabilisticEncryption
{
    public class ProbabilisticEncryptionProvider
    {
        private readonly TripleDESKeyInfo keyInfo;
        private readonly int[] p;

        public ProbabilisticEncryptionProvider(TripleDESKeyInfo keyInfo, int[] p)
        {
            this.keyInfo = keyInfo;
            this.p = p;
        }

        public byte[] Encrypt(byte[] openText)
        {
            Console.WriteLine($"Encryption of {Encoding.UTF8.GetString(openText)}");
            var random = new Random(Guid.NewGuid().GetHashCode());

            var min = p.Min();
            var max = p.Max();
            var a = random.Next(min + 1, max*max);
            Console.WriteLine($"a = {a}");
            var ai = new int[p.Length];

            for (var i = 0; i < p.Length; ++i)
            {
                ai[i] = a%p[i];
            }

            var added = string.Join(",", ai) + ",";
            Console.WriteLine($"random additive = {added}");
            var encryptedOpenTextByAES = AESHelper.Encrypt(openText, $"{a}");

            Console.WriteLine(
                $"Encrypted open text by AES with key ${a} = {Encoding.UTF8.GetString(encryptedOpenTextByAES)}");

            var newOpenText = Encoding.UTF8.GetBytes(added).Concat(encryptedOpenTextByAES).ToArray();

            Console.WriteLine($"New open text = {Encoding.UTF8.GetString(newOpenText)}");

            var result = TripleDESHelper.EncryptStringToBytes(newOpenText, keyInfo);
            Console.WriteLine($"Encrypted by TripleDES new open text = {Encoding.UTF8.GetString(result)}");
            return result;
        }

        public byte[] Decrypt(byte[] encrypted)
        {
            var decrypted = TripleDESHelper.DecryptStringFromBytes(encrypted, keyInfo);
            

            var str = Encoding.UTF8.GetString(decrypted);
            Console.WriteLine($"Decrypted by TripleDES text = {str}");
            var aNew = new List<int>();
            var cur = new StringBuilder();
            var length = 0;
            var i1 = 0;
            while (i1 < str.Length && aNew.Count != p.Length)
            {
                if (str[i1] == ',')
                {
                    aNew.Add(int.Parse(cur.ToString()));
                    cur.Clear();
                }
                else
                {
                    cur.Append(str[i1]);
                }
                length++;
                i1++;
            }

            var r = new int[p.Length - 1, p.Length];
            for (var i = 0; i < p.Length - 1; ++i)
            {
                for (var j = i + 1; j < p.Length; ++j)
                {
                    r[i, j] = (int) BigIntegerExtensions.Invert(p[i], p[j]);
                    Console.WriteLine($"r[i][j] = {r[i, j]}");
                }
            }

            var x = new int[p.Length];
            for (int i = 0; i < p.Length; ++i)
            {
                x[i] = aNew[i];
                for (int j = 0; j < i; ++j)
                {
                    x[i] = r[j, i]*(x[i] - x[j]);

                    x[i] = x[i]%p[i];
                    if (x[i] < 0) x[i] += p[i];
                }

                Console.WriteLine($"x[i] = {x[i]}");
            }
            var aS = x[0];
            for (var i = 1; i < p.Length; ++i)
            {
                var rS = x[i];
                for (var j = 0; j < i; ++j)
                {
                    rS *= p[j];
                }
                aS += rS;
            }

            Console.WriteLine($"a = {aS}");
            var bytes = decrypted.Skip(length).ToArray();
            var result = AESHelper.Decrypt(bytes, $"{aS}");
            
            Console.WriteLine($"decrypted by AES with key {aS} = {Encoding.UTF8.GetString(result)}");
            return result;
        }
    }
}