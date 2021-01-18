# Space Invaders Emulator - CLR IL AOT/JIT Implementation

This project contains an 8080 emulator implemented by AOT recompiling the 
provided rom directly to C# IL. 

To validate the cpu implementation it provides two architectures which take advantage of it:

1. CP/M - The CP/M machine is implemented in it's most trivial form purely to validate the CPU using the test roms found on the internet 
2. Space Invaders - It also fully implements the space invaders arcade machine architecture (w/ SDL2 for rendering and shift register etc)

## Usage

_Note: This requires a space invaders rom file (which can be acquired on the internet)_

```
dotnet run -c Release --project SpaceInvadersJIT -- roms/real/invaders.rom
```

Likewise test roms can be executed with
```
dotnet run -c Release --project CPMEmulator -- roms/test/8080PRE.COM
```

## Explanation

c.f. https://blog.davetcode.co.uk/post/jit-8080/

## Development

### Pre-requisites

- Dotnet 5.0 SDK - https://dotnet.microsoft.com/download
- Suitable text editor/IDE (VS2019 or vscode suggested)

### Testing

There is a growing suite of simple per-opcode tests verifying the functionality of all individual opcodes, these can be run with `dotnet test`

Additionally there are a small set of validation roms for the 8080 which require a basic CP/M style machine. These can be run like this (note the correct response below the command):

```
dotnet run -c Release --project CPMEmulator -- roms/test/8080PRE.COM
8080 Preliminary tests complete
```

Note that the 8080TST.COM rom doesn't pass because DAA/AuxCarry is not implemented

## References

- [Space Invaders Architecture](https://computerarcheology.com/Arcade/SpaceInvaders/)
- [CP/M BDOS functions](https://www.seasip.info/Cpm/bdos.html)
- [8080 Programmers manual](https://altairclone.com/downloads/manuals/8080%20Programmers%20Manual.pdf)
- [CLR IL Reference](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes?view=net-5.0)

## Missing function

- Aux Carry Flag & DAA instruction (not implemented because it's not very important and DAA in JIT IL will be very tedious)
- Audio