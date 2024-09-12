using System.Diagnostics;
using System.Runtime.InteropServices;

class TimeSync
{    
    private static void Sync()
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
        //registrar log na main
        // não é mais necessário declarar intervalos para o funcionamneto do programa, pois a condição será especificada na main
    }

    private static bool CheckNtpServer()
    {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
        bool specifiedServer;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
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
                if (!output.Contains("pool.ntp.br"))
                {
                    return specifiedServer = false;
                }
                else
                {
                    return specifiedServer = true;
                }
            }
        }
    }

    
}