using ModelsNamespace;
using System;
using System.Collections.Generic;
using System.Text;
using static UtilsNamespace.Utils;

namespace AccountGeneratorNamespace
{
    public static class AccountGenerator
    {
        private static readonly List<string> _websites = new List<string>
        {
            "wp.pl",
            "gmail.com",
            "interia.poczta.pl",
            "bing.com",
        };

        private const string _chars = "QqWwEeRrTtYuIpomasdasxvfdvtSDSFG123456789BVBCXZDSlkas";
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generuje instację Account. Adres oraz hash hasła zostają wygenerowane losowo.
        /// </summary>
        /// <returns></returns>
        public static Account GenerateRandomAccount()
        {
            var addressStringBuilder = new StringBuilder();
            var passwordStringBuilder = new StringBuilder();

            var websiteNumber = _random.Next(0, _websites.Count - 1);
            var addressLeftPartLength = _random.Next(4, 12);
            var passwordLength = _random.Next(4, 16);

            for (int i = 0; i < addressLeftPartLength; i++)
                addressStringBuilder.Append(_chars[_random.Next(0, _chars.Length - 1)]);

            for (int i = 0; i < passwordLength; i++)
                passwordStringBuilder.Append(_chars[_random.Next(0, _chars.Length - 1)]);

            addressStringBuilder.Append('@');
            addressStringBuilder.Append(_websites[websiteNumber]);

            return new Account()
            {
                EmailAddress = addressStringBuilder.ToString(),
                PasswordHash = Hash(passwordStringBuilder.ToString()),
                WantsNewsletter = _random.NextDouble()>0.3
            };
        }

    }
}
