using System;
using System.Security;
using Microsoft.Exchange.WebServices.Data;

namespace Impeto.Framework.Exchange.Authentication
{
    public interface IUserData
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        SecureString Password { get; }
        Uri AutodiscoverUrl { get; set; }
    }

    public class UserDataFromConsole : IUserData
    {
        public static UserDataFromConsole UserData;

        public static IUserData GetUserData(string email, string senha, string autodiscoverUrl)
        {
            if (string.IsNullOrEmpty(autodiscoverUrl))
            {
                autodiscoverUrl = "https://outlook.office365.com/EWS/Exchange.asmx";
            }

            UserData = new UserDataFromConsole();

            var config = new ConfiguracaoBS().obterConfiguracao();

            UserData.EmailAddress = email != null ? email : config.EmailAddress;            
            UserData.Password = new SecureString();
            string senhaconfig = senha != null ? senha : config.Password;
            for (var x = 0; x <= senhaconfig.Length - 1; x++)
            {
                UserData.Password.AppendChar(senhaconfig[x]);
            }

            UserData.AutodiscoverUrl = new Uri(autodiscoverUrl /* != null ? autodiscoverUrl : config.AutoDiscover*/); 

            if (UserData == null)
            {
                GetUserDataFromConsole();
            }

            return UserData;
        }

        private static void GetUserDataFromConsole()
        {
            UserData = new UserDataFromConsole();

            Console.Write("Enter email address: ");
            UserData.EmailAddress = Console.ReadLine();

            UserData.Password = new SecureString();

            Console.Write("Enter password: ");

            while (true)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (userInput.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (userInput.Key == ConsoleKey.Backspace)
                {
                    if (UserData.Password.Length != 0)
                    {
                        UserData.Password.RemoveAt(UserData.Password.Length - 1);
                    }
                }
                else
                {
                    UserData.Password.AppendChar(userInput.KeyChar);
                    Console.Write("*");
                }
            }

            Console.WriteLine();

            UserData.Password.MakeReadOnly();
        }

        /* Se o enum "ExchangeVersion" não possuir o elemento Exchange2013_SP1, precisa rever o ambiente de desenvolvimento.
         *
         * [ Windows 10 + Visual Studio 2015 atualizado ]
         * 
         * Assembly Microsoft.Exchange.WebServices, Version=15.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
         * C:\Windows\assembly\GAC_MSIL\Microsoft.Exchange.WebServices\15.0.0.0__31bf3856ad364e35\Microsoft.Exchange.WebServices.dll
         *
         */
        public ExchangeVersion Version { get { return ExchangeVersion.Exchange2013_SP1; } }        

        public string EmailAddress
        {
            get;
            private set;
        }

        public SecureString Password
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
    }
}
