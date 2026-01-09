using System;
using System.Security.Cryptography;
using System.Text;
using PsychedelicExperience.Common.Security;

namespace PsychedelicExperience.Membership.Security
{
    public class EncryptionKey
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public string DP { get; set; }
        public string DQ { get; set; }
        public string InverseQ { get; set; }
        public string D { get; set; }

        public EncryptionKey()
        {
        }

        private EncryptionKey(RSAParameters key)
        {
            Modulus = Convert.ToBase64String(key.Modulus);
            Exponent = Convert.ToBase64String(key.Exponent);
            P = Convert.ToBase64String(key.P);
            Q = Convert.ToBase64String(key.Q);
            DP = Convert.ToBase64String(key.DP);
            DQ = Convert.ToBase64String(key.DQ);
            InverseQ = Convert.ToBase64String(key.InverseQ);
            D = Convert.ToBase64String(key.D);
        }

        public static EncryptionKey New()
        {
            using (var algorithm = RSA.Create())
            {
                var key = algorithm.ExportParameters(true);

                return new EncryptionKey(key);
            }
        }

        public string Decrypt(EncryptedString value)
        {
            if (value == null) return null;

            using (var algorithm = RSA.Create())
            {
                SetKey(algorithm);

                var data = algorithm.Decrypt(value.GetBytes(), RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(data);
            }
        }

        public EncryptedString Encrypt(string value)
        {
            using (var algorithm = RSA.Create())
            {
                SetKey(algorithm);

                var data = Encoding.UTF8.GetBytes(value);
                var encrypted = algorithm.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

                return new EncryptedString(encrypted);
            }
        }

        private void SetKey(RSA algorithm)
        {
            var parameters = new RSAParameters
            {
                Modulus = Convert.FromBase64String(Modulus),
                Exponent = Convert.FromBase64String(Exponent),
                P = Convert.FromBase64String(P),
                Q = Convert.FromBase64String(Q),
                DP = Convert.FromBase64String(DP),
                DQ = Convert.FromBase64String(DQ),
                InverseQ = Convert.FromBase64String(InverseQ),
                D = Convert.FromBase64String(D),
            };

            algorithm.ImportParameters(parameters);
        }
    }
}