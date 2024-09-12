using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    private const int SW_MAXIMIZE = 3;


    private static bool CheckNetwork()
    {
        try
        {
            using (var client = new System.Net.WebClient())
            {
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }
    }

    private static void TimeSync()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C w32tm /resync",
            Verb = "runas",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Maximized
        };

        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Console.WriteLine(output);
            System.Threading.Thread.Sleep(10000);
        }

        registerLog();


        ShowWindow(Process.GetCurrentProcess().MainWindowHandle, SW_HIDE);

        TimeSpan sleepDuration = TimeSpan.FromHours(48);
        Thread.Sleep(sleepDuration);
    }

    private static bool CheckNtpServer()
    {
        bool servidorCorreto = false;
        while (true)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C w32tm /query /peers",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Maximized
            };

            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
                if (!output.Contains("ntp.obspm.fr"))
                {
                    ExibirAviso();
                    Main();
                }
                else
                {
                    Console.WriteLine("Servidor Autenticado!");
                    servidorCorreto = true;
                    return servidorCorreto;
                }
            }
        }
    }

    public static void ChangeNtpServer()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/C w32tm /config /manualpeerlist:ntp.obspm.fr /syncfromflags:manual /reliable:yes /update",
            Verb = "runas",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Maximized
        };

        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            process.Close();

            Console.WriteLine(output);
        }
    }

    public static void StartService()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/C net start w32time",
            Verb = "runas",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Maximized
        };

        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            process.Close();

            Console.WriteLine(output);
        }
    }

    public static void ChangeTimeZone()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/C tzutil /s \"W. Europe Standard Time_dstoff\"",
            Verb = "runas",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Maximized
        };

        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            process.Close();

            Console.WriteLine(output);
        }
    }

    public static void ExibirAviso()
    {
        KillProcess();
        while (true)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C msg * \"Favor acionar Engenharia de Teste!\"",
                Verb = "runas",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Maximized
            };

            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                process.Close();

                Console.WriteLine(output);
            }
        }
    }

    public static void registerLog()
    {
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        string fileName = $"{currentDate}.txt";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        string fileContent = DateTime.Now.ToString();

        File.WriteAllText(filePath, fileContent);
    }

    public static DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }

    static void Main()
    {

        Console.WriteLine("Developed by: Pedro Goncalves \n Test Engineering Intern \n Jabil MAN Site");
        while (true)
        {
            while (GetCurrentTime().Hour == 6)
            {
                while (CheckNetwork() == true)
                {
                    Console.WriteLine("Iniciando o serviço de sincronização de horário...");
                    System.Threading.Thread.Sleep(3000);
                    StartService();
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("Matando testador...");
                    Console.WriteLine("Alterando o servidor NTP...");
                    ChangeNtpServer();
                    Console.WriteLine("Alterando fuso para UTC+1");
                    ChangeTimeZone();
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("Sincronizando o horário...");
                    if (CheckNtpServer() == true)
                    {
                        TimeSync();
                        registerLog();
                        System.Threading.Thread.Sleep(1000 * 86400);
                    }
                    else
                    {
                        ExibirAviso();
                        Main();
                    }

                }

                while (CheckNetwork() == false)
                {
                    ExibirAviso();
                    Console.WriteLine("Sem conexão com a internet, aguardando...");
                    System.Threading.Thread.Sleep(10000);
                    Main();
                }
            }
        }
    }
}