using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGateIDE
{
    class ApplicationRunner
    {
        public List<string> Errors { get; private set; }
        public string Path { get; private set; }

        public ApplicationRunner(string path)
        {
            Path = path;
        }

        public Result<bool> Run()
        {
            if (Path != String.Empty && Path != null)
            {
                Console.WriteLine("\nRunning the program!");
                Console.WriteLine("{0}", Path);

                ProcessStartInfo psi = new ProcessStartInfo("LogicGateSim.exe", Path); // LogicGateSim.exe has to be added to path for this to work.
                Process.Start(psi);
                return Result<bool>.Success(true);
            }
            else
            {
                Console.WriteLine("\nThe Program was empty!\n");
                return Result<bool>.Failure("You must select a path!");
            }
        }
    }
}
