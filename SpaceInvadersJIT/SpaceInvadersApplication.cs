using System;
using JIT8080._8080;

namespace SpaceInvadersJIT
{
    internal class SpaceInvadersApplication : IIOHandler, IRenderer, IDisposable, IMemoryBus8080
    {
        private IntPtr _window;
        private IntPtr _renderer;
        private ShiftRegister _shiftRegister;
        private readonly byte[] _rom = new byte[0x2000];
        private readonly byte[] _ram = new byte[0x2000];

        internal SpaceInvadersApplication(byte[] program)
        {
            Array.Copy(program, _rom, Math.Min(0x2000, program.Length));
        }

        internal void InitialiseWindow()
        {
            _ = SDL2.SDL_Init(SDL2.SDL_INIT_VIDEO | SDL2.SDL_INIT_AUDIO);
            _ = SDL2.SDL_CreateWindowAndRenderer(
                224,
                256,
                0,
                out _window,
                out _renderer);
            _ = SDL2.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            _ = SDL2.SDL_RenderClear(_renderer);
            SDL2.SDL_SetWindowTitle(_window, "Space Invaders");
            _ = SDL2.SDL_SetHint(SDL2.SDL_HINT_RENDER_SCALE_QUALITY, "0");
        }

        public void Out(byte port, byte value)
        {
            switch (port)
            {
                case 2:
                    _shiftRegister.Offset = (byte) (value & 0b111);
                    break;
                case 3:
                    // TODO SOUND1 port
                    break;
                case 4:
                    _shiftRegister.Register = (ushort) ((_shiftRegister.Register >> 8) | (value << 8));
                    break;
                case 5:
                    // TODO SOUND2 port
                    break;
                case 6:
                    // TODO WATCHDOG PORT
                    break;
            }
        }

        public byte In(byte port)
        {
            return port switch
            {
                0 => 0x0, // TODO INP0
                1 => 0x0, // TODO INP1
                2 => 0x0, // TODO INP2
                3 => _shiftRegister.Value(),
                _ => 0x0 // TODO - Other ports
            };
        }

        public void Dispose()
        {
            SDL2.SDL_DestroyRenderer(_renderer);
            SDL2.SDL_DestroyWindow(_window);
            SDL2.SDL_Quit();
        }

        public void VBlank()
        {
            // TODO - Draw to screen on vblank
        }

        public byte ReadByte(ushort address) =>
            address switch
            {
                < 0x2000 => _rom[address],
                _ => _ram[(address - 0x2000) & 0x1FFF],
            };

        public void WriteByte(byte value, ushort address)
        {
            Console.WriteLine($"WRITE {address:X4}={value:X2}");
            if (address < 0x2000)
            {
                return;
            }

            _ram[(address - 0x2000) & 0x1FFF] = value;
        }
    }
}
