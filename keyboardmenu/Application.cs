using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using static System.Console;

namespace keyboardmenu
{
    internal class Application
    {

        public void Start()
        {
            RunMainMenu();
        }

        public void RunMainMenu()
        {
            string prompt = @"

                                ▄████▄   ██░ ██  ▄▄▄       ██▀███   ▒█████   ███▄    █ 
                               ▒██▀ ▀█  ▓██░ ██▒▒████▄    ▓██ ▒ ██▒▒██▒  ██▒ ██ ▀█   █ 
                               ▒▓█    ▄ ▒██▀▀██░▒██  ▀█▄  ▓██ ░▄█ ▒▒██░  ██▒▓██  ▀█ ██▒
                               ▒▓▓▄ ▄██▒░▓█ ░██ ░██▄▄▄▄██ ▒██▀▀█▄  ▒██   ██░▓██▒  ▐▌██▒
                               ▒ ▓███▀ ░░▓█▒░██▓ ▓█   ▓██▒░██▓ ▒██▒░ ████▓▒░▒██░   ▓██░
                               ░ ░▒ ▒  ░ ▒ ░░▒░▒ ▒▒   ▓▒█░░ ▒▓ ░▒▓░░ ▒░▒░▒░ ░ ▒░   ▒ ▒ 
                               ░  ▒    ▒ ░▒░ ░  ▒   ▒▒ ░  ░▒ ░ ▒░  ░ ▒ ▒░ ░ ░░   ░ ▒░
                               ░         ░  ░░ ░  ░   ▒     ░░   ░ ░ ░ ░ ▒     ░   ░ ░ 
                               ░ ░       ░  ░  ░      ░  ░   ░         ░ ░           ░ 
                               ░                                                       
                                      charon - encrypted communication interface 
                                      (use the arrow keys to navigate options.)

";
            string[] options = { "connect", "verify security", "about", "exit" };
            Menu mainMenu = new Menu(prompt, options);
            int selectedIndex = mainMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunFirstChoice();
                    break;
                case 1:
                    VerifySec();
                    break;
                case 2:
                    DisplayAboutInfo();
                    break;
                case 3:
                    Exit();
                    break;
            }
        }

        private void Exit()
        {
            WriteLine("\npress any key to exit...");
            ReadKey(true);
            Environment.Exit(0);
        }

        private void DisplayAboutInfo()
        {
            Clear();
            WriteLine(@"  






");
            WriteLine("                      encrypted communication interface for secret societies and individuals alike.");
            WriteLine("                                                     version 1");
            WriteLine("                                       made by alp eraslan & serhat ekinci");
            WriteLine(" ");
            WriteLine("                                     press any key to return to the main menu.");
            ReadKey(true);
            RunMainMenu();
        }

        private void RunFirstChoice()
        {
            Beep(3000, 1000);
            Clear();

            StartSQLConnection();

            //Exit();
        }

        private void VerifySec()
        {
            Beep(3000, 1000);
            Clear();

            displayNetworkInfo(NetworkInterfaceType.Wireless80211);
            printmac();
            getOperatingSystemInfo();
            spoofMAC();
            Exit();
        }

        private static string displayNetworkInfo(NetworkInterfaceType _type)
        {
            string hostname = Dns.GetHostName();
            string output = "";
            WriteLine("hostname:  {0}", hostname);
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                WriteLine("local ipv4 address:  {0}", ip.Address.ToString());
                            }
                        }
                    }
                }

                if (output != "") break;
            }

            return output;
        }

        private static void enable_LocalAreaConection(string adapterId, bool enable = true)
        {
            string interfaceName = "Wi-Fi";
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Id == adapterId)
                {
                    WriteLine("interface found");
                    interfaceName = i.Name;
                    break;
                }
            }

            string control;
            if (enable)
                control = "enable";
            else
                control = "disable";

            WriteLine("changing interface");
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("netsh", $"interface set interface \"{interfaceName}\" {control}");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }

        private static string randomMac()
        {
            string chars = "ABCDEF0123456789";
            string windows = "26AE";
            string result = "";
            Random random = new Random();

            result += chars[random.Next(chars.Length)];
            result += windows[random.Next(windows.Length)];

            for (int i = 0; i < 5; i++)
            {
                result += "-";
                result += chars[random.Next(chars.Length)];
                result += chars[random.Next(chars.Length)];

            }

            return result;
        }

        private static bool spoofMAC()
        {
            WriteLine("attempting spoof");
            bool err = false;
            using RegistryKey NetworkAdapters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}");
            foreach (string adapter in NetworkAdapters.GetSubKeyNames())
            {
                if (adapter != "Properties")
                {
                    try
                    {
                        using RegistryKey NetworkAdapter = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{adapter}", true);
                        if (NetworkAdapter.GetValue("BusType") != null)
                        {
                            NetworkAdapter.SetValue("NetworkAddress", randomMac());
                            string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId").ToString();
                            enable_LocalAreaConection(adapterId, false);
                            enable_LocalAreaConection(adapterId, true);

                        }
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        WriteLine("\n start in administrator mode");
                        err = true;
                        break;
                    }
                }
            }

            return err;
        }


        private static string ExecuteCommand(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            Process process = new Process { StartInfo = startInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static void StartSQLConnection()
        {
            // login bilgilerini main menu acilmadan charon ascii art'i tam ortadayken altta istese nasil olur acaba
            WriteLine("enter nickname:");
            string nickname = ReadLine();

            Thread.Sleep(50);
            Clear();

            String sql = "";
            string connectionString;
            MySqlConnection cnn;
            MySqlCommand sqlCommand;
            MySqlDataReader dataReader;

            // sql pornosu burada basliyor
            connectionString = "Server=sql11.freesqldatabase.com;User=sql11679178;Database=sql11679178;Port=3306;Password=vdzQajqjuM;SSL Mode=None";
            cnn = new MySqlConnection(connectionString);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }

            // chat resfresh
            Thread.Sleep(50);
            Thread refreshFunction = new Thread(new ThreadStart(Application.loopChatRefresh));
            refreshFunction.Start();

            // buraya exit ve fileupload gibi kosullar eklenecek
            while (true)
            {
                SetCursorPosition(75, CursorTop);
                sql = "INSERT INTO messages (clientMessage) VALUES (@clientMessage)";
                sqlCommand = new MySqlCommand(sql, cnn);
                sqlCommand.Parameters.AddWithValue("@clientMessage", nickname + ": " + ReadLine());
                deleteReadLineOutput();
                sqlCommand.ExecuteNonQuery();
            }

        }


        static void loopChatRefresh()
        {
            string connectionString = "Server=sql11.freesqldatabase.com;User=sql11679178;Database=sql11679178;Port=3306;Password=vdzQajqjuM;SSL Mode=None";
            string output = "";
            string lastMessage = "";
            string checkLastMessage = "";
            string query;

            MySqlConnection cnn;
            MySqlCommand sqlCommand;
            MySqlDataReader dataReader;

            query = "SELECT clientMessage FROM messages ";
            cnn = new MySqlConnection(connectionString);
            cnn.Open();

            sqlCommand = new MySqlCommand(query, cnn);
            dataReader = sqlCommand.ExecuteReader();

            // chat gecmisi
            while (dataReader.Read())
            {
                output += dataReader.GetValue(0) + "\n";
            }
            // chat gecmisini yaz
            Write(output);
            dataReader.Close();


        writeLastMessage:
            // son mesaji dondur
            dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                lastMessage = dataReader.GetValue(0) + "\n";
            }

            dataReader.Close();
            if (checkLastMessage != "")
            {
                SetCursorPosition(0, CursorTop);
                Beep(2000, 100);
                Write(lastMessage);
                SetCursorPosition(75, CursorTop);
            }

            while (true)
            {
                dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    checkLastMessage = dataReader.GetValue(0) + "\n";
                }

                dataReader.Close();

                if (checkLastMessage != lastMessage)
                    goto writeLastMessage;
            }

        }

        static void deleteReadLineOutput()
        {
            if (CursorTop == 0) return;
            SetCursorPosition(0, CursorTop);
            Write(new string(' ', WindowWidth));
            SetCursorPosition(0, CursorTop - 1);
        }

        private void getOperatingSystemInfo()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get())
            {
                if (managementObject["Caption"] != null)
                {
                    WriteLine("os name: " + managementObject["Caption"].ToString());
                }
                if (managementObject["OSArchitecture"] != null)
                {
                    WriteLine("os architecture: " + managementObject["OSArchitecture"].ToString());
                }
                if (managementObject["CSDVersion"] != null)
                {
                    WriteLine("os system service pack: " + managementObject["CSDVersion"].ToString());
                }
            }
        }

        private void printmac()
        {
            try
            {
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
                    Console.WriteLine("mac addr (" + networkInterface.Name + "): " + physicalAddress.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error retrieving mac: " + ex.Message);
            }
        }

        // EXPERIMENTAL STUFF
        //private static void changeIP()
        //{
        //    System.Diagnostics.Process.Start("ipconfig", @"/release");
        //    System.Diagnostics.Process.Start("ipconfig", @"/renew");
        //}

        //private static void mac()
        //{
        //    string interfaceName = GetWiFiInterfaceName();
        //    if (string.IsNullOrEmpty(interfaceName))
        //    {
        //        WriteLine("Wi-Fi network interface not found.");
        //        return;
        //    }
        //    string newMacAddress = "AA-BB-CC-DD-EE-FF";
        //    string currentMacAddress = GetMAC(interfaceName);
        //    WriteLine("Current MAC address: {0}", currentMacAddress);
        //    changeRegMAC(interfaceName, newMacAddress);
        //    netInfChange(interfaceName);
        //    string newMacAddress2 = GetMAC(interfaceName);
        //    WriteLine("New MAC address: {0}", newMacAddress2);
        //    if (newMacAddress2 == newMacAddress)
        //    {
        //        WriteLine("MAC address changed successfully.");
        //    }
        //    else
        //    {
        //        WriteLine("Failed to change MAC address.");
        //    }
        //}

        //private static string GetWiFiInterfaceName()
        //{
        //    string output = ExecuteCommand("getmac -v");
        //    string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string line in lines)
        //    {
        //        if (line.Contains("Wi-Fi"))
        //        {
        //            return line.Split(' ')[0];
        //        }
        //    }

        //    return null;
        //}

        //private static string GetMAC(string interfaceName)
        //{
        //    string output = ExecuteCommand("getmac /fo csv /nh /v /all | findstr /i \"" + interfaceName + "\"");
        //    if (string.IsNullOrEmpty(output))
        //    {
        //        return null;
        //    }
        //    string[] values = output.Split(',');
        //    return values[1].Replace("\"", "");
        //}

        //private static void changeRegMAC(string interfaceName, string newMacAddress)
        //{
        //    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}", true);
        //    foreach (string subKeyName in key.GetSubKeyNames())
        //    {
        //        RegistryKey subKey = key.OpenSubKey(subKeyName, true);
        //        if (subKey.GetValue("NetEnabled").ToString() == "1")
        //        {
        //            string connectionName = subKey.GetValue("ConnectionName").ToString();
        //            if (connectionName.Contains(interfaceName))
        //            {
        //                subKey.SetValue("NetworkAddress", newMacAddress);
        //                break;
        //            }
        //        }

        //        subKey.Close();
        //    }
        //    key.Close();
        //}

        //private static void netInfChange(string interfaceName)
        //{
        //    ExecuteCommand("netsh interface set interface \"" + interfaceName + "\" disable");
        //    ExecuteCommand("netsh interface set interface \"" + interfaceName + "\" enable");
        //}
    }
}
