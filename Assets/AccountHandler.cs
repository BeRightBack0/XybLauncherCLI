using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SharpCompress.Compressors.Filters;
using Spectre.Console;
using static XybLauncher.AccountHandler;

namespace XybLauncher
{
    internal class AccountHandler
    {
        // Define the file path using Local Application Data folder
        static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XybLauncher", "accounts.json");




        public static void ShowSelectedAccount()
        {
            var accountData = LoadAccountData();

            // Find the selected account based on the selected index
            var selectedAccount = accountData.Accounts.Find(a => a.Index == accountData.Selected);

            // Display the selected account's display name, or a message if no account is selected
            if (selectedAccount != null)
            {
                AnsiConsole.MarkupLine($"Selected Account: [blue]{selectedAccount.DisplayName}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]No account selected.[/]");
            }
        }

        public static async void StartAccountManager()
        {
            EnsureDirectoryExists();

            var accountData = LoadAccountData();

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option:")
                        .AddChoices("Add new account", "Select an account", "View selected account", "Exit"));

                switch (choice)
                {
                    case "Add new account":
                        AddNewAccount(accountData);
                        break;
                    case "Select an account":
                        SelectAccount(accountData);
                        break;
                    case "View selected account":
                        ViewSelectedAccount(accountData);
                        break;
                    case "Exit":
                       await Program.MainMenu();
                        break;
                }

                // Save the updated data to a file
                SaveAccountData(accountData);
            }
        }

        static void AddNewAccount(AccountData accountData)
        {
            // Determine the next index automatically
            int newIndex = accountData.Accounts.Count > 0 ? accountData.Accounts[^1].Index + 1 : 1;

            // Prompt the user for new account information
            var displayName = AnsiConsole.Ask<string>("Enter the [green]display name[/] of the new account:");
            var email = AnsiConsole.Ask<string>("Enter the [green]email[/] of the new account:");
            var password = AnsiConsole.Ask<string>("Enter the [green]password[/] of the new account:");

            // Create and add the new account
            accountData.Accounts.Add(new UserAccount
            {
                Index = newIndex,
                DisplayName = displayName,
                Email = email,
                Password = password
            });

            AnsiConsole.MarkupLine("[bold green]New account added.[/]");
        }

        static void SelectAccount(AccountData accountData)
        {
            // Ensure there are accounts to select
            if (accountData.Accounts.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No accounts available to select.[/]");
                return;
            }

            // Prompt the user to select an account by display name
            var selectedDisplayName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an account by [green]display name[/]:")
                    .AddChoices(accountData.Accounts.ConvertAll(a => a.DisplayName)));

            // Find the account by display name and set it as the selected account
            var selectedAccount = accountData.Accounts.Find(a => a.DisplayName == selectedDisplayName);

            if (selectedAccount != null)
            {
                accountData.Selected = selectedAccount.Index;
                AnsiConsole.MarkupLine($"[bold green]Account '{selectedAccount.DisplayName}' selected.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Account with the specified display name does not exist.[/]");
            }
        }

        static void ViewSelectedAccount(AccountData accountData)
        {
            var selectedAccount = accountData.Accounts.Find(a => a.Index == accountData.Selected);

            if (selectedAccount != null)
            {
                AnsiConsole.Write(new Table()
                    .AddColumn("Property")
                    .AddColumn("Value")
                    .AddRow("Index", selectedAccount.Index.ToString())
                    .AddRow("Display Name", selectedAccount.DisplayName)
                    .AddRow("Email", selectedAccount.Email));
                   // .AddRow("Password", selectedAccount.Password));
            }
            else
            {   
                AnsiConsole.MarkupLine("[bold red]No account is selected or the selected account does not exist.[/]");
            }
        }

        static void EnsureDirectoryExists()
        {
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(appDataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        static AccountData LoadAccountData()
        {
            if (File.Exists(appDataPath))
            {
                // Read the JSON file and deserialize it into an AccountData object
                string json = File.ReadAllText(appDataPath);
                return JsonConvert.DeserializeObject<AccountData>(json) ?? new AccountData { Accounts = new List<UserAccount>() };
            }
            else
            {
                // Return a new AccountData object if the file does not exist
                return new AccountData { Accounts = new List<UserAccount>() };
            }
        }

        static void SaveAccountData(AccountData accountData)
        {
            // Convert the AccountData object to JSON format
            string json = JsonConvert.SerializeObject(accountData, Formatting.Indented);

            // Save the JSON to a file
            File.WriteAllText(appDataPath, json);
        }

        // Define a class to represent individual user accounts
        public class UserAccount
        {
            public int Index { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // Define a class to represent the account data structure
        public class AccountData
        {
            public int Selected { get; set; }
            public List<UserAccount> Accounts { get; set; }
        }

    }
}
