using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace XybLauncher
{
    public class AccountParser
    {

        public class Account
        {
            public int Index { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LauncherData
        {
            public int Selected { get; set; }
            public Account[] Accounts { get; set; }
        }

        private LauncherData _data;

        // Constructor with hardcoded file path
        public AccountParser()
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "XybLauncher",  // Hardcoded folder name
                "accounts.json"     // Hardcoded file name
            );

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file {filePath} was not found.");

            string json = File.ReadAllText(filePath);
            _data = JsonSerializer.Deserialize<LauncherData>(json)
                    ?? throw new InvalidOperationException("Failed to parse JSON data.");
        }

        // Method to get account information based on the argument ('email' or 'password')
        public string GetAccountInfo(string argument)
        {
            var selectedAccount = _data.Accounts.FirstOrDefault(a => a.Index == _data.Selected);
            if (selectedAccount == null)
                throw new InvalidOperationException("No account found with the selected index.");

            return argument.ToLower() switch
            {
                "email" => selectedAccount.Email,
                "password" => selectedAccount.Password,
                _ => throw new ArgumentException("Invalid argument. Use 'email' or 'password'.")
            };
        }



    }

}


