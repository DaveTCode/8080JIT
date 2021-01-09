# Space Invaders Emulator - CLR IL AOT/JIT Implementation

This project contains an 8080 emulator implemented by AOT recompiling the 
provided rom directly to C# IL. It also plugs in appropriate IN/OUT/Memory 
map to play the space invaders rom (which is not provided as part of this 
project)

## TODO

- Aux Carry Flag & DAA instruction
- Cycle tracking
- Interrupts
- Rendering
- Audio

## Explanation



## Development

### Pre-requisites

- Dotnet 5.0 SDK - https://dotnet.microsoft.com/download
- Suitable text editor/IDE (VS2019 or vscode suggested)

### Testing

There is a growing suite of simple per-opcode tests verifying the functionality of all individual opcodes, these can be run with `dotnet test`