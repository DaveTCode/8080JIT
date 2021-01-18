using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JIT8080._8080;

namespace JIT8080.Generator
{
    public static class Emulator
    {
        /// <summary>
        /// Generates methods which return the 16 bit value from two 8 bit 
        /// register pairs (BC, DE, HL)
        /// </summary>
        /// <param name="typeBuilder">Access to the type to add the method</param>
        /// <param name="name">The name of the function</param>
        /// <param name="regHighByte">The high byte (H in HL)</param>
        /// <param name="regLowByte">The low byte (L in HL)</param>
        /// <returns>A method info which constitutes the simple function</returns>
        private static MethodBuilder CreateRegisterPairAccess(TypeBuilder typeBuilder, string name,
            FieldInfo regHighByte, FieldInfo regLowByte)
        {
            var method = typeBuilder.DefineMethod(name, MethodAttributes.Public, CallingConventions.Standard,
                typeof(ushort), Array.Empty<Type>());
            method.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.AggressiveInlining);
            var methodIL = method.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, regHighByte);
            methodIL.Emit(OpCodes.Ldc_I4_8);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, regLowByte);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodBuilder CreateRegisterPairSet(TypeBuilder typeBuilder, string name, FieldInfo regHighByte,
            FieldInfo regLowByte)
        {
            var method = typeBuilder.DefineMethod(name, MethodAttributes.Public, CallingConventions.Standard,
                null, new[] {typeof(ushort)});
            method.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.AggressiveInlining);
            var methodIL = method.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, regLowByte);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Ldc_I4_8);
            methodIL.Emit(OpCodes.Shr);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, regHighByte);
            methodIL.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodBuilder CreateFlagRegister(TypeBuilder typeBuilder, CpuInternalBuilders internals)
        {
            var method = typeBuilder.DefineMethod("GetFlagRegister", MethodAttributes.Public,
                CallingConventions.Standard,
                typeof(byte), Type.EmptyTypes);
            method.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.AggressiveInlining);
            var methodIL = method.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.SignFlag);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.ZeroFlag);
            methodIL.Emit(OpCodes.Ldc_I4_6);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.AuxCarryFlag);
            methodIL.Emit(OpCodes.Ldc_I4_4);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.ParityFlag);
            methodIL.Emit(OpCodes.Ldc_I4_2);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Ldc_I4_2);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodBuilder CreateSetFlagRegister(TypeBuilder typeBuilder, CpuInternalBuilders internals)
        {
            var method = typeBuilder.DefineMethod("SetFlagRegister", MethodAttributes.Public,
                CallingConventions.Standard, null, new[] {typeof(byte)});
            method.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.AggressiveInlining);

            var methodIL = method.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Ldc_I4_4);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_2);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Stfld, internals.ParityFlag);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.EmitLd8Immediate( 0b0001_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_4);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Stfld, internals.AuxCarryFlag);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.EmitLd8Immediate( 0b0100_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_6);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Stfld, internals.ZeroFlag);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Ldc_I4, 0b1000_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Stfld, internals.SignFlag);

            methodIL.Emit(OpCodes.Ret);

            return method;
        }

        /// <summary>
        /// For any given offset into the program we can generate the requisite
        /// IL based upon the opcode (first byte of the span) and any operands 
        /// (further bytes from the span)
        /// 
        /// This returns the opcodes to emit (with args) and the number of bytes 
        /// consumed
        /// </summary>
        /// <param name="program">
        /// The full ROM (not just the portion we're looking at)
        /// </param>
        /// <param name="programCounter">
        /// Indicates what index into the ROM we are generating an instruction for
        /// </param>
        /// <param name="internals">
        /// Provides references to all internal fields/methods on this class
        /// for e.g. register access
        /// </param>
        /// <param name="jumpLabels">
        /// Provides labels for every address in the ROM for jump/call commands
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// 1. The emitter which can be called to put instructions on an IL stream
        /// 2. The number of bytes consumed by this operation
        /// 3. The number of cycles taken by the operation (non-branching)
        /// </returns>
        private static (IEmitter, byte, long) GenerateILForOpcode(Span<byte> program, int programCounter,
            CpuInternalBuilders internals, Label[] jumpLabels)
        {
            var opcodeByte = program[programCounter];
            var operand1 = program[(programCounter + 1) % program.Length];
            var operand2 = program[(programCounter + 2) % program.Length];
            var operandWord = (ushort) ((operand2 << 8) + operand1);
            var opcode = Opcodes8080Decoder.Decode(opcodeByte);

            IEmitter emitter = opcode switch
            {
                Opcodes8080.NOP => new NOPEmitter(),
                Opcodes8080.LXI => new LXIEmitter(opcodeByte, operand1, operand2, operandWord),
                Opcodes8080.STAX => new STAXEmitter(opcodeByte),
                Opcodes8080.INX => new INXDCXEmitter(opcodeByte),
                Opcodes8080.INR => new INRDCREmitter(opcodeByte),
                Opcodes8080.DCR => new INRDCREmitter(opcodeByte),
                Opcodes8080.MVI => new MVIEmitter(opcodeByte, operand1),
                Opcodes8080.RLC => new RLCEmitter(),
                Opcodes8080.DAD => new DADEmitter(opcodeByte),
                Opcodes8080.LDAX => new LDAXEmitter(opcodeByte),
                Opcodes8080.DCX => new INXDCXEmitter(opcodeByte),
                Opcodes8080.RRC => new RRCEmitter(),
                Opcodes8080.RAL => new RALEmitter(),
                Opcodes8080.RAR => new RAREmitter(),
                Opcodes8080.SHLD => new SHLDEmitter(operandWord),
                Opcodes8080.DAA => new NOPEmitter(), // TODO - Actually implement DAA after handling aux carry flag
                Opcodes8080.LHLD => new LHLDEmitter(operandWord),
                Opcodes8080.CMA => new CMAEmitter(),
                Opcodes8080.STA => new STAEmitter(operandWord),
                Opcodes8080.STC => new STCEmitter(),
                Opcodes8080.LDA => new LDAEmitter(operandWord),
                Opcodes8080.CMC => new CMCEmitter(),
                Opcodes8080.MOV => new MOVEmitter(opcodeByte),
                Opcodes8080.HLT => new HLTEmitter(),
                Opcodes8080.ADD => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.ADC => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.SUB => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.SBB => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.ANA => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.XRA => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.ORA => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.CMP => new General8BitALUEmitter(opcodeByte, opcode),
                Opcodes8080.RNZ => new RetEmitter(opcodeByte, internals.ZeroFlag, false),
                Opcodes8080.POP => new POPEmitter(opcodeByte),
                Opcodes8080.JNZ => new JumpOnFlagEmitter(opcodeByte, internals.ZeroFlag, false,
                    jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.JMP => new JumpOnFlagEmitter(opcodeByte, jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.CNZ => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.ZeroFlag, false, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.PUSH => new PUSHEmitter(opcodeByte),
                Opcodes8080.ADI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RST => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    jumpLabels[opcodeByte & 0b0011_1000], (ushort) (opcodeByte & 0b0011_1000)),
                Opcodes8080.RZ => new RetEmitter(opcodeByte, internals.ZeroFlag, true),
                Opcodes8080.RET => new RetEmitter(opcodeByte),
                Opcodes8080.JZ => new JumpOnFlagEmitter(opcodeByte, internals.ZeroFlag, true,
                    jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.CZ => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.ZeroFlag, true, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.CALL => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.ACI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RNC => new RetEmitter(opcodeByte, internals.CarryFlag, false),
                Opcodes8080.JNC => new JumpOnFlagEmitter(opcodeByte, internals.CarryFlag, false,
                    jumpLabels[operandWord % program.Length], operandWord),
                Opcodes8080.OUT => new OutEmitter(operand1),
                Opcodes8080.CNC => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.CarryFlag, false, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.SUI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RC => new RetEmitter(opcodeByte, internals.CarryFlag, true),
                Opcodes8080.JC => new JumpOnFlagEmitter(opcodeByte, internals.CarryFlag, true,
                    jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.IN => new InEmitter(operand1),
                Opcodes8080.CC => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.CarryFlag, true, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.SBI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RPO => new RetEmitter(opcodeByte, internals.ParityFlag, false),
                Opcodes8080.JPO => new JumpOnFlagEmitter(opcodeByte, internals.ParityFlag, false,
                    jumpLabels[operandWord % program.Length], operandWord),
                Opcodes8080.XTHL => new XTHLEmitter(),
                Opcodes8080.CPO => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.ParityFlag, false, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.ANI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RPE => new RetEmitter(opcodeByte, internals.ParityFlag, true),
                Opcodes8080.PCHL => new PCHLEmitter(),
                Opcodes8080.JPE => new JumpOnFlagEmitter(opcodeByte, internals.ParityFlag, true,
                    jumpLabels[operandWord % program.Length], operandWord),
                Opcodes8080.XCHG => new XCHGEmitter(),
                Opcodes8080.CPE => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.ParityFlag, true, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.XRI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RP => new RetEmitter(opcodeByte, internals.SignFlag, false),
                Opcodes8080.JP => new JumpOnFlagEmitter(opcodeByte, internals.SignFlag, false,
                    jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.DI => new UpdateInterruptEnableEmitter(false),
                Opcodes8080.CP => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.SignFlag, false, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.ORI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                Opcodes8080.RM => new RetEmitter(opcodeByte, internals.SignFlag, true),
                Opcodes8080.SPHL => new SPHLEmitter(),
                Opcodes8080.JM => new JumpOnFlagEmitter(opcodeByte, internals.SignFlag, true,
                    jumpLabels[operandWord % program.Length],
                    operandWord),
                Opcodes8080.EI => new UpdateInterruptEnableEmitter(true),
                Opcodes8080.CM => new CallEmitter(opcodeByte, (ushort) (programCounter + opcode.Length()),
                    internals.SignFlag, true, jumpLabels[operandWord % program.Length], (ushort) (operandWord % program.Length)),
                Opcodes8080.CPI => new General8BitALUEmitter(opcodeByte, opcode, operand1),
                _ => throw new ArgumentOutOfRangeException(nameof(opcode), opcode, "Invalid 8080 opcode")
            };

            return (emitter, opcode.Length(), opcode.Cycles(opcodeByte));
        }

        private static void CreateConstructor(TypeBuilder typeBuilder, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField, FieldBuilder rendererField)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                new[] {typeof(IMemoryBus8080), typeof(IIOHandler), typeof(IRenderer)});
            var methodIL = constructorBuilder.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Stfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_2);
            methodIL.Emit(OpCodes.Stfld, ioHandlerField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_2);
            methodIL.Emit(OpCodes.Stfld, rendererField);
            methodIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Given a ROM loaded as an array of bytes this constructs an emulator with
        /// a "Run" function and all relevant flags & registers.
        /// 
        /// TODO - More explanation about how we turn the program into IL
        /// </summary>
        /// 
        /// <param name="program">
        /// The loaded ROM
        /// </param>
        /// 
        /// <param name="memoryBus">
        /// This will provide ReadByte & WriteByte functionality for the
        /// entire 16 bit address space.
        /// </param>
        /// 
        /// <param name="ioHandler">
        /// This will provide IO/OUT function for the whole 8 bit port space
        /// </param>
        /// 
        /// <param name="renderer">
        /// This will get called during VBlank interrupts to trigger a redraw
        /// </param>
        /// 
        /// <param name="interruptUtils">
        /// Allowance for the owning computer to register code which runs at
        /// various points during execution and can fire interrupts.
        /// </param>
        ///
        /// <param name="initialProgramCounter">
        /// Defines the first instruction which will be executed (e.g. on CP/M
        /// machines 0x100 is the first operation executed)
        /// </param>
        /// 
        /// <returns>
        /// An object which contains references to all the field info and 
        /// method info required to make use of (and inspect the running 
        /// state) of the emulator.
        /// </returns>
        public static Cpu8080 CreateEmulator(Span<byte> program, IMemoryBus8080 memoryBus, IIOHandler ioHandler,
            IRenderer renderer, IInterruptUtils interruptUtils, ushort initialProgramCounter = 0x0)
        {
            var asmName = new AssemblyName("Emulator");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Execute");
            var typeBuilder = moduleBuilder.DefineType("Emulator", TypeAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod("Run", MethodAttributes.Public, CallingConventions.Standard);
            var methodIL = methodBuilder.GetILGenerator();

            var cpuInternal = new CpuInternalBuilders
            {
                A = typeBuilder.DefineField("A", typeof(byte), FieldAttributes.Public),
                B = typeBuilder.DefineField("B", typeof(byte), FieldAttributes.Public),
                C = typeBuilder.DefineField("C", typeof(byte), FieldAttributes.Public),
                D = typeBuilder.DefineField("D", typeof(byte), FieldAttributes.Public),
                E = typeBuilder.DefineField("E", typeof(byte), FieldAttributes.Public),
                H = typeBuilder.DefineField("H", typeof(byte), FieldAttributes.Public),
                L = typeBuilder.DefineField("L", typeof(byte), FieldAttributes.Public),
                SignFlag = typeBuilder.DefineField("SignFlag", typeof(bool), FieldAttributes.Public),
                ZeroFlag = typeBuilder.DefineField("ZeroFlag", typeof(bool), FieldAttributes.Public),
                AuxCarryFlag = typeBuilder.DefineField("AuxCarryFlag", typeof(bool), FieldAttributes.Public),
                CarryFlag = typeBuilder.DefineField("CarryFlag", typeof(bool), FieldAttributes.Public),
                ParityFlag = typeBuilder.DefineField("ParityFlag", typeof(bool), FieldAttributes.Public),
                StackPointer = typeBuilder.DefineField("StackPointer", typeof(ushort), FieldAttributes.Public),
                InterruptEnable = typeBuilder.DefineField("InterruptEnable", typeof(bool), FieldAttributes.Public),
                CycleCounter = typeBuilder.DefineField("CycleCounter", typeof(long), FieldAttributes.Public),
                MemoryBusField = typeBuilder.DefineField("_memoryBus", typeof(IMemoryBus8080), FieldAttributes.Private),
                IOHandlerField = typeBuilder.DefineField("_ioHandler", typeof(IIOHandler), FieldAttributes.Private),
                RendererField = typeBuilder.DefineField("_renderer", typeof(IRenderer), FieldAttributes.Private),
                ProgramLabels = Enumerable.Range(0, program.Length).Select(_ => methodIL.DefineLabel()).ToArray(),
                DestinationAddress = methodIL.DeclareLocal(typeof(ushort)),
                JumpTableStart = methodIL.DefineLabel()
            };
            cpuInternal.HL = CreateRegisterPairAccess(typeBuilder, "HL", cpuInternal.H, cpuInternal.L);
            cpuInternal.BC = CreateRegisterPairAccess(typeBuilder, "BC", cpuInternal.B, cpuInternal.C);
            cpuInternal.DE = CreateRegisterPairAccess(typeBuilder, "DE", cpuInternal.D, cpuInternal.E);
            cpuInternal.SetHL = CreateRegisterPairSet(typeBuilder, "SetHL", cpuInternal.H, cpuInternal.L);
            cpuInternal.SetBC = CreateRegisterPairSet(typeBuilder, "SetHL", cpuInternal.B, cpuInternal.C);
            cpuInternal.SetDE = CreateRegisterPairSet(typeBuilder, "SetHL", cpuInternal.D, cpuInternal.E);
            cpuInternal.GetFlagRegister = CreateFlagRegister(typeBuilder, cpuInternal);
            cpuInternal.SetFlagRegister = CreateSetFlagRegister(typeBuilder, cpuInternal);
            
            CreateConstructor(typeBuilder, cpuInternal.MemoryBusField, cpuInternal.IOHandlerField, cpuInternal.RendererField);

            // Then we create the IL for every position in the program, whether
            // that position will ever be considered code or not
            var operationsAtIndex = new (IEmitter, byte, long)[program.Length];
            for (var pc = 0; pc < program.Length; pc++)
            {
                operationsAtIndex[pc] = GenerateILForOpcode(program, pc, cpuInternal, cpuInternal.ProgramLabels);
            }
            var seenRomIndexes = new HashSet<int>(program.Length);
            
            // Allow for definition of locals and other emits by an interrupt util handler
            interruptUtils.PreProgramEmit(methodIL);

            // Skip the jump table and start at the defined initial PC (defaults to 0)
            methodIL.Emit(OpCodes.Br, cpuInternal.ProgramLabels[initialProgramCounter]);

            // Place the jump table
            GenerateDynamicJumpTable(methodIL, cpuInternal);

            // Finally we actually generate a program, this amounts to the following steps:
            // 1. Find the next program counter position which has not yet been processed
            // 2. Emit a label for that program counter position
            // 3. Emit the CLR IL commands for that operation
            // 4. Increment the program counter to the next opcode
            // 5. Continue that process until a previously processed opcode is found and then emit a branch to that label (looping the program)
            // 6. Repeat 1-5 until there are no program counter values left to process
            var programCounter = 0;
            while (seenRomIndexes.Count != program.Length)
            {
                var hasLooped = false;
                while (!hasLooped)
                {
                    var (instructions, instructionLength, cyclesTaken) = operationsAtIndex[programCounter];
                    methodIL.MarkLabel(cpuInternal.ProgramLabels[programCounter]);
                    
                    // Check for interrupts
                    interruptUtils.PostInstructionEmit(methodIL, cpuInternal, (ushort)programCounter);
                    
                    // Clear the cycle counter so that we know the exact number of cycles that
                    // the operation took
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I8, 0L);
                    methodIL.Emit(OpCodes.Stfld, cpuInternal.CycleCounter);

#if DEBUG
                    methodIL.EmitWriteLine($"{programCounter:X4} - {instructions}");
                    methodIL.EmitDebugString(cpuInternal);
#endif
                    instructions.Emit(methodIL, cpuInternal);
                    
                    // Store off the number of cycles in the last instruction
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, cpuInternal.CycleCounter);
                    methodIL.Emit(OpCodes.Ldc_I8, cyclesTaken);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Stfld, cpuInternal.CycleCounter);

                    seenRomIndexes.Add(programCounter);
                    programCounter = (programCounter + instructionLength) % program.Length;

                    // If this segment of the program rejoins the main program
                    // then it should branch back into it and we can start on
                    // the next segment
                    if (seenRomIndexes.Contains(programCounter))
                    {
                        methodIL.Emit(OpCodes.Br, cpuInternal.ProgramLabels[programCounter]);
                        hasLooped = true;
                    }
                }

                // Find the next segment which has not yet been processed
                programCounter = Enumerable.Range(0, program.Length).FirstOrDefault(ix => !seenRomIndexes.Contains(ix));
            }

            var t = typeBuilder.CreateType();
            return new Cpu8080
            {
                Internals = new Cpu8080Internals
                {
                    A = t.GetField("A"),
                    B = t.GetField("B"),
                    C = t.GetField("C"),
                    D = t.GetField("D"),
                    E = t.GetField("E"),
                    H = t.GetField("H"),
                    L = t.GetField("L"),
                    BC = t.GetMethod("BC"),
                    DE = t.GetMethod("DE"),
                    HL = t.GetMethod("HL"),
                    StackPointer = t.GetField("StackPointer"),
                    SignFlag = t.GetField("SignFlag"),
                    ZeroFlag = t.GetField("ZeroFlag"),
                    AuxCarryFlag = t.GetField("AuxCarryFlag"),
                    ParityFlag = t.GetField("ParityFlag"),
                    CarryFlag = t.GetField("CarryFlag"),
                    GetFlagRegister = t.GetMethod("GetFlagRegister"),
                    SetFlagRegister = t.GetMethod("SetFlagRegister"),
                },
                Emulator = Activator.CreateInstance(t, memoryBus, ioHandler, renderer),
                Run = t.GetMethod("Run")
            };
        }

        /// <summary>
        /// Various operations in an 8080 allow for jumping to an arbitrary
        /// address (which may be in ROM, RAM, VRAM, MMIO etc).
        ///
        /// This function supports that with a very naive linear search
        /// through a jump table.
        /// </summary>
        /// 
        /// <param name="methodIL">
        /// The IL generator in which to place this macro
        /// </param>
        ///
        /// <param name="cpuInternal">
        /// Contains references to the various locals and program labels
        /// required to build the table.
        /// </param>
        private static void GenerateDynamicJumpTable(ILGenerator methodIL, CpuInternalBuilders cpuInternal)
        {
            methodIL.MarkLabel(cpuInternal.JumpTableStart);
            foreach (var (ix, programLabel) in cpuInternal.ProgramLabels.Select((l, ix) => (ix, l)))
            {
                methodIL.Emit(OpCodes.Ldc_I4, ix);
                methodIL.Emit(OpCodes.Ldloc, cpuInternal.DestinationAddress);
                methodIL.Emit(OpCodes.Beq, programLabel);
            }

            methodIL.EmitWriteLine("Failed to perform dynamic jump");
            methodIL.Emit(OpCodes.Ret);
        }
    }
}