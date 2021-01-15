using System;
using System.IO;
using JIT8080.Generator;

namespace CPMEmulator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var rom = args.Length switch
            {
                > 0 => File.ReadAllBytes(args[0]),
                _ => new byte[] { 0x04, 0x00, 0x00, 0x76 }
            };
            var application = new CPMApplication(rom);
            application.Emulator = Emulator.CreateEmulator(application.CompleteProgram(), application, application, application, application, 0x100);

            Console.WriteLine("Emulator Created");
            var runDelegate = (Action) application.Emulator.Run.CreateDelegate(typeof(Action), application.Emulator.Emulator);
            
            Console.WriteLine("Delegate Created");

            runDelegate();
        }
    }
}
