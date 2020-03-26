using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                List<String> refs = new List<string>();
                Console.WriteLine("enter frames' number / type 0 to exit");
                int frames;
                while (true)
                    try
                    {
                        frames = Convert.ToInt32(Console.ReadLine());
                        break;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("not an int");
                    }
                if (frames == 0) break;
                Console.WriteLine("enter file name");
                System.IO.StreamReader file;
                while (true)
                    try
                    {
                        string name = Console.ReadLine();
                        file = new System.IO.StreamReader(name);
                        break;
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Could not open file , try again");
                    }
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    refs.Add(line);
                }
                memoryManager mm = new memoryManager(refs, frames);

                mm.FIFOSim();
                mm.LRUSim();
                mm.OPTSim();
            }
        }
    }
}
