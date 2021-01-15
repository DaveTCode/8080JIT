using System;
using System.Collections.Generic;
using System.Diagnostics;
using JIT8080._8080;

namespace SpaceInvadersJIT
{
    internal class SpaceInvadersApplication : IIOHandler, IRenderer, IDisposable, IMemoryBus8080
    {
        internal const long HalfCyclesPerScreen = 17066L;
        internal const long CyclesPerScreen = HalfCyclesPerScreen * 2;
        private const int ScreenWidth = 224;
        private const int ScreenHeight = 256;
        private const double MsPerFrame = 1000.0 / 60.0;
        
        private IntPtr _window;
        private IntPtr _renderer;
        private IntPtr _texture;
        private ShiftRegister _shiftRegister;
        private readonly byte[] _rom = new byte[0x2000];
        private readonly byte[] _ram = new byte[0x2000];

        private readonly byte[,,] _screenBuffer = new byte[ScreenHeight,ScreenWidth,4];
        
        private readonly Stopwatch _stopwatch = new ();

        private byte[] _portStatus = { 0b0000_1000, 0b0000_0000 };
        
        private readonly Dictionary<SDL2.SDL_Keycode, SpaceInvadersKey> _keyMap = new()
        {
            {SDL2.SDL_Keycode.SDLK_RIGHT, SpaceInvadersKey.P1Right},
            {SDL2.SDL_Keycode.SDLK_LEFT, SpaceInvadersKey.P1Left},
            {SDL2.SDL_Keycode.SDLK_UP, SpaceInvadersKey.P1Fire},
            {SDL2.SDL_Keycode.SDLK_a, SpaceInvadersKey.P2Left},
            {SDL2.SDL_Keycode.SDLK_d, SpaceInvadersKey.P2Right},
            {SDL2.SDL_Keycode.SDLK_w, SpaceInvadersKey.P2Fire},
            {SDL2.SDL_Keycode.SDLK_RSHIFT, SpaceInvadersKey.P1Start},
            {SDL2.SDL_Keycode.SDLK_LSHIFT, SpaceInvadersKey.P2Start},
            {SDL2.SDL_Keycode.SDLK_RETURN, SpaceInvadersKey.Credit}
        };

        internal SpaceInvadersApplication(byte[] program)
        {
            Array.Copy(program, _rom, Math.Min(0x2000, program.Length));
        }

        internal void InitialiseWindow()
        {
            _ = SDL2.SDL_Init(SDL2.SDL_INIT_VIDEO);
            _ = SDL2.SDL_CreateWindowAndRenderer(
                224 * 2,
                256 * 2,
                0,
                out _window,
                out _renderer);
            _ = SDL2.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            _ = SDL2.SDL_RenderClear(_renderer);
            SDL2.SDL_SetWindowTitle(_window, "Space Invaders");
            _ = SDL2.SDL_SetHint(SDL2.SDL_HINT_RENDER_SCALE_QUALITY, "0");

            _texture = SDL2.SDL_CreateTexture(
                renderer: _renderer,
                format: SDL2.SDL_PIXELFORMAT_ARGB8888,
                access: (int)SDL2.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                w: ScreenWidth,
                h: ScreenHeight);
            
            _stopwatch.Start();
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
                1 => _portStatus[0],
                2 => _portStatus[1],
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
            for (var ii = 0; ii < 0x1C00; ii++)
            {
                var b = _ram[ii + 0x400]; // vram starts at 0x400 into RAM address space
                var y = ii * 8 / 256;
                var x = (ii * 8) % 256;
                for (var pixel = 0; pixel < 8; pixel++)
                {
                    var rotatedY = ScreenHeight - (x + pixel) - 1;
                    var rotatedX = y;
                    byte value = ((b >> pixel) & 1) == 1 ? 0xFF : 0x0;
                    _screenBuffer[rotatedY, rotatedX, 3] = 0xFF; // Alpha channel
                    _screenBuffer[rotatedY, rotatedX, 1] = value;
                    _screenBuffer[rotatedY, rotatedX, 2] = value;
                    _screenBuffer[rotatedY, rotatedX, 0] = value;
                }
            }

            unsafe
            {
                fixed (byte* p = _screenBuffer)
                {
                    _ = SDL2.SDL_UpdateTexture(_texture, IntPtr.Zero, (IntPtr)p, ScreenWidth * 4);
                }
            }

            _ = SDL2.SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, IntPtr.Zero);
            SDL2.SDL_RenderPresent(_renderer);

            CheckForInputs();
            
            // Pause until we're running at 60fps
            var msToSleep = MsPerFrame - (_stopwatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000;
            if (msToSleep > 0)
            {
                SDL2.SDL_Delay((uint)msToSleep);
            }
            _stopwatch.Restart();
        }

        private void CheckForInputs()
        {
            // TODO - Checking for input once per frame seems a bit sketchy
            while (SDL2.SDL_PollEvent(out var e) != 0)
            {
                switch (e.type)
                {
                    case SDL2.SDL_EventType.SDL_QUIT:
                        Environment.Exit(0);
                        break;
                    case SDL2.SDL_EventType.SDL_KEYUP:
                        if (e.key.keysym.sym == SDL2.SDL_Keycode.SDLK_ESCAPE)
                        {
                            Environment.Exit(0);
                        }
                        else if (_keyMap.ContainsKey(e.key.keysym.sym))
                        {
                            var key = _keyMap[e.key.keysym.sym];
                            _portStatus[key.PortIndex()] &= key.KeyUpMask();
                        }
                        break;
                    case SDL2.SDL_EventType.SDL_KEYDOWN:
                        if (_keyMap.ContainsKey(e.key.keysym.sym))
                        {
                            var key = _keyMap[e.key.keysym.sym];
                            _portStatus[key.PortIndex()] |= key.KeyDownMask();
                        }
                        break;
                }
            }
        }

        public byte ReadByte(ushort address) =>
            address switch
            {
                < 0x2000 => _rom[address],
                _ => _ram[(address - 0x2000) & 0x1FFF]
            };

        public void WriteByte(byte value, ushort address)
        {
#if DEBUG
            Console.WriteLine($"WRITE {address:X4}={value:X2}");
#endif
            if (address < 0x2000)
            {
                return;
            }

            _ram[(address - 0x2000) & 0x1FFF] = value;
        }
    }
}
