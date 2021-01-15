using System;
using System.IO;
using System.Runtime.CompilerServices;
using JIT8080.Generator;

[assembly:InternalsVisibleTo("SpaceInvadersJIT.Tests")]
namespace SpaceInvadersJIT
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var rom = args.Length switch
            {
                > 0 => File.ReadAllBytes(args[0]),
                _ => new byte[] { 0x04, 0x00, 0x00, 0x76 }//File.ReadAllBytes(Path.Combine("..", "..", "..", "..", "roms", "test", "intro.bin")).AsSpan(0, 0x2000).ToArray(),
            };
            var application = new SpaceInvadersApplication(rom);
            var interruptUtils = new SpaceInvadersInterruptUtils();
            application.InitialiseWindow();
            var emulator = Emulator.CreateEmulator(rom, application, application, application, interruptUtils);

            Console.WriteLine("Emulator Created");
            var runDelegate = (Action) emulator.Run.CreateDelegate(typeof(Action), emulator.Emulator);
            
            Console.WriteLine("Delegate Created");

            runDelegate();
        }
    }
}
