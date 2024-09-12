using System.Diagnostics;
using System.Runtime.InteropServices;

class TimeSync
{
    private string statusServer = "";

    public void SetStatusServer(statusServer)
    {
        this.statusServer = statusServer;
    }
    
    public static void TimeSync()
    {
        ProcessStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C w32tm /resync",
            Verb = "runas",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Maximized
        };

        using (Process process = new Process)
        {
            process.StartInfo = processInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Console.WriteLine(output);
            System.Threading.Thread.Sleep(1000);
        }
        //declarar classe de registro de log

        //declarar função para minimizar o cmd
    }

    private static bool CheckNtpServer()
    {
        bool specifiedServer;
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