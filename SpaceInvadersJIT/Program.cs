using System;
using System.IO;
using System.Runtime.CompilerServices;
using SpaceInvadersJIT._8080;
using SpaceInvadersJIT.Generator;

[assembly:InternalsVisibleTo("SpaceInvadersJIT.Tests")]
namespace SpaceInvadersJIT
{
    internal class Program
    {
        private static void Main()
        {
            var rom = File.ReadAllBytes(Path.Combine("..", "..", "..", "..", "roms", "test", "TST8080.com"));
            var memoryBus = new MemoryBus8080(rom);
            var emulator = Emulator.CreateEmulator(rom, memoryBus, new IOHandler());

            Console.WriteLine("Emulator Created");

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());
        }
    }
}
