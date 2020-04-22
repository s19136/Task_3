using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Task_3.Services
{
    public static class PasswordEncryptionService
    {
        public static string[] encrypt (string pass)
        {
            var salt = CreateSalt();
            var encrypted = Create(pass, salt);
            string[] res = new string[2];
            res[0] = encrypted;
            res[1] = salt;
            return res;
        }
        public static string Create(string pass, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: pass,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using(var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static Boolean Validate(string pass, string salt, string encrypted)
            => Create(pass, salt) == encrypted;
    }
}
