using System;
using System.IO;
using JIT8080.Generator;

namespace CPMEmulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rom = args.Length switch
            {
                > 0 => File.ReadAllBytes(args[0]),
                _ => new byte[] { 0x04, 0x00, 0x00, 0x76 }
            };
            var application = new CPMApplication(rom);
            var emulator = Emulator.CreateEmulator(application.CompleteProgram(), application, application, application, 0x100);

            Console.WriteLine("Emulator Created");
            var runDelegate = (Action) emulator.Run.CreateDelegate(typeof(Action), emulator.Emulator);
            
            Console.WriteLine("Delegate Created");

            runDelegate();
        }
    }
}
