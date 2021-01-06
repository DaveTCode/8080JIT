using System;
using System.Reflection.Emit;
using SpaceInvadersJIT._8080;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace SpaceInvadersJIT.Generator
{
    internal interface IEmitter
    {
        internal void Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField);
    }

    internal class NOPEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField) =>
            methodIL.Emit(OpCodes.Nop);

        public override string ToString() => "NOP";
    }

    internal class LXIEmitter : IEmitter
    {
        private readonly byte _opcode;
        private readonly byte _operand1;
        private readonly byte _operand2;
        private readonly ushort _operandWord;

        internal LXIEmitter(byte opcode, byte operand1, byte operand2, ushort operandWord)
        {
            _opcode = opcode;
            _operand1 = operand1;
            _operand2 = operand2;
            _operandWord = operandWord;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            switch (_opcode)
            {
                case 0x01: // LXI BC
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.C);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand2);
                    methodIL.Emit(OpCodes.Stfld, internals.B);
                    break;
                case 0x11: // LXI DE
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.E);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand2);
                    methodIL.Emit(OpCodes.Stfld, internals.D);
                    break;
                case 0x21: // LXI HL
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.L);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand2);
                    methodIL.Emit(OpCodes.Stfld, internals.H);
                    break;
                case 0x31: // LXI SP
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldc_I4, _operandWord);
                    methodIL.Emit(OpCodes.Stfld, internals.StackPointer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_opcode),
                        $"LXI shouldn't be run for opcode byte {_opcode}");
            }
        }

        public override string ToString() =>
            _opcode switch
            {
                0x01 => $"LXI BC {_operand1:X2} {_operand2:X2}",
                0x11 => $"LXI DE {_operand1:X2} {_operand2:X2}",
                0x21 => $"LXI HL {_operand1:X2} {_operand2:X2}",
                0x31 => $"LXI SP {_operandWord:X4}",
                _ => throw new ArgumentOutOfRangeException(nameof(_opcode),
                    $"LXI shouldn't be run for opcode byte {_opcode}"),
            };
    }

    internal class MVIEmitter : IEmitter
    {
        private readonly byte _opcode;
        private readonly byte _operand1;

        internal MVIEmitter(byte opcode, byte operand1)
        {
            _opcode = opcode;
            _operand1 = operand1;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            switch (_opcode)
            {
                case 0x06: // MVI B
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.B);
                    break;
                case 0x0E: // MVI C
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.C);
                    break;
                case 0x16: // MVI D
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.D);
                    break;
                case 0x1E: // MVI E
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.E);
                    break;
                case 0x26: // MVI H
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.H);
                    break;
                case 0x2E: // MVI L
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.L);
                    break;
                case 0x36: // MVI M
                    methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
                    break;
                case 0x3E: // MVI A
                    methodIL.Emit(OpCodes.Ldc_I4_S, _operand1);
                    methodIL.Emit(OpCodes.Stfld, internals.A);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_opcode),
                        $"MVI shouldn't be run for opcode byte {_opcode}");
            }
        }

        public override string ToString() =>
            _opcode switch
            {
                0x06 => $"MVI B {_operand1:X2}",
                0x0E => $"MVI C {_operand1:X2}",
                0x16 => $"MVI D {_operand1:X2}",
                0x1E => $"MVI E {_operand1:X2}",
                0x26 => $"MVI H {_operand1:X2}",
                0x2E => $"MVI L {_operand1:X2}",
                0x36 => $"MVI M {_operand1:X2}",
                0x3E => $"MVI A {_operand1:X2}",
                _ => throw new ArgumentOutOfRangeException(nameof(_opcode), $"Invalid opcode {_opcode} for MVI"),
            };
    }

    internal class MOVEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal MOVEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        private static void EmitSourceInstructions(Register source, ILGenerator methodIL, CpuInternalBuilders internals,
            FieldBuilder memoryBusField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            switch (source)
            {
                case Register.A:
                    methodIL.Emit(OpCodes.Ldfld, internals.A);
                    break;
                case Register.B:
                    methodIL.Emit(OpCodes.Ldfld, internals.B);
                    break;
                case Register.C:
                    methodIL.Emit(OpCodes.Ldfld, internals.C);
                    break;
                case Register.D:
                    methodIL.Emit(OpCodes.Ldfld, internals.D);
                    break;
                case Register.E:
                    methodIL.Emit(OpCodes.Ldfld, internals.E);
                    break;
                case Register.H:
                    methodIL.Emit(OpCodes.Ldfld, internals.H);
                    break;
                case Register.L:
                    methodIL.Emit(OpCodes.Ldfld, internals.L);
                    break;
                case Register.M:
                    methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var (source, destination) = DecodeOpcode();

            if (destination == Register.M)
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                EmitSourceInstructions(source, methodIL, internals, memoryBusField);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Call, internals.HL);
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
            }
            else
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                EmitSourceInstructions(source, methodIL, internals, memoryBusField);
                methodIL.Emit(OpCodes.Stfld, destination switch
                {
                    Register.A => internals.A,
                    Register.B => internals.B,
                    Register.C => internals.C,
                    Register.D => internals.D,
                    Register.E => internals.E,
                    Register.H => internals.H,
                    Register.L => internals.L,
                    _ => throw new ArgumentOutOfRangeException(),
                });
            }
        }

        private (Register, Register) DecodeOpcode()
        {
            var sourceRegisterBits = _opcode & 0b0000_0111;
            var destinationRegisterBits = (_opcode & 0b0011_1000) >> 3;

            return ((Register) sourceRegisterBits, (Register) destinationRegisterBits);
        }

        public override string ToString()
        {
            var (source, destination) = DecodeOpcode();
            return $"MOV {destination},{source}";
        }
    }

    internal class LDAEmitter : IEmitter
    {
        private readonly ushort _operand;

        internal LDAEmitter(ushort operand)
        {
            _operand = operand;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4, _operand);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
            methodIL.Emit(OpCodes.Stfld, internals.A);
        }

        public override string ToString() => $"LDA {_operand:X4}";
    }

    internal class STAEmitter : IEmitter
    {
        private readonly ushort _operand;

        internal STAEmitter(ushort operand)
        {
            _operand = operand;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4, _operand);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
        }

        public override string ToString() => $"STA {_operand:X4}";
    }

    internal class CMAEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Not);
            methodIL.Emit(OpCodes.Stfld, internals.A);
        }

        public override string ToString() => "CMA";
    }

    internal class STCEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }

        public override string ToString() => "STC";
    }

    internal class CMCEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
            methodIL.Emit(OpCodes.Not);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }

        public override string ToString() => "CMC";
    }

    internal class JMPEmitter : IEmitter
    {
        private readonly ushort _address;
        private readonly Label _jumpLabel;

        internal JMPEmitter(ushort address, Label jumpLabel)
        {
            _address = address;
            _jumpLabel = jumpLabel;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField) =>
            methodIL.Emit(OpCodes.Br, _jumpLabel);

        public override string ToString() => $"JMP {_address:X4}";
    }

    internal class INXDCXEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal INXDCXEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);

            switch (_opcode)
            {
                case 0x03: // INX BC
                    methodIL.Emit(OpCodes.Call, internals.BC);
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetBC);
                    break;
                case 0x0B: // DCX BC
                    methodIL.Emit(OpCodes.Call, internals.BC);
                    methodIL.Emit(OpCodes.Ldc_I4_M1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetBC);
                    break;
                case 0x13: // INX DE
                    methodIL.Emit(OpCodes.Call, internals.DE);
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetDE);
                    break;
                case 0x1B: // DCX DE
                    methodIL.Emit(OpCodes.Call, internals.DE);
                    methodIL.Emit(OpCodes.Ldc_I4_M1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetDE);
                    break;
                case 0x23: // INX HL
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetHL);
                    break;
                case 0x2B: // DCX HL
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    methodIL.Emit(OpCodes.Ldc_I4_M1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Call, internals.SetHL);
                    break;
                case 0x33: // INX SP
                    methodIL.Emit(OpCodes.Ldfld, internals.StackPointer);
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Stfld, internals.StackPointer);
                    break;
                case 0x3B: // DCX SP
                    methodIL.Emit(OpCodes.Ldfld, internals.StackPointer);
                    methodIL.Emit(OpCodes.Ldc_I4_M1);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Conv_U2);
                    methodIL.Emit(OpCodes.Stfld, internals.StackPointer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString() => _opcode switch
        {
            0x03 => "INX BC",
            0x0B => "DCX BC",
            0x13 => "INX DE",
            0x1B => "DCX DE",
            0x23 => "INX HL",
            0x2B => "DCX HL",
            0x33 => "INX SP",
            0x3B => "DCX SP",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class STAXEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal STAXEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var regPairMethod = _opcode switch
            {
                0x02 => internals.BC,
                0x12 => internals.DE,
                _ => throw new ArgumentOutOfRangeException()
            };

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, regPairMethod);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
        }

        public override string ToString() => _opcode switch
        {
            0x02 => "STAX B",
            0x12 => "STAX D",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class LDAXEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal LDAXEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var regPairMethod = _opcode switch
            {
                0x0A => internals.BC,
                0x1A => internals.DE,
                _ => throw new ArgumentOutOfRangeException()
            };

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, regPairMethod);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
            methodIL.Emit(OpCodes.Stfld, internals.A);
        }

        public override string ToString() => _opcode switch
        {
            0x0A => "LDAX B",
            0x1A => "LDAX D",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class InEmitter : IEmitter
    {
        private readonly byte _port;

        internal InEmitter(byte port)
        {
            _port = port;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, ioHandlerField);
            methodIL.Emit(OpCodes.Ldc_I4_S, _port);
            methodIL.Emit(OpCodes.Callvirt, ioHandlerField.FieldType.GetMethod("In")!);
            methodIL.Emit(OpCodes.Stfld, internals.A);
        }

        public override string ToString() => $"IN {_port}";
    }

    internal class OutEmitter : IEmitter
    {
        private readonly byte _port;

        internal OutEmitter(byte port)
        {
            _port = port;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, ioHandlerField);
            methodIL.Emit(OpCodes.Ldc_I4_S, _port);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Callvirt, ioHandlerField.FieldType.GetMethod("Out")!);
        }

        public override string ToString() => $"IN {_port}";
    }

    internal class INRDCREmitter : IEmitter
    {
        private readonly Register _register;
        private readonly OpCode _incOrDec;

        internal INRDCREmitter(byte opcode)
        {
            (_register, _incOrDec) = opcode switch
            {
                0x04 => (Register.B, OpCodes.Ldc_I4_1),
                0x05 => (Register.B, OpCodes.Ldc_I4_M1),
                0x0C => (Register.C, OpCodes.Ldc_I4_1),
                0x0D => (Register.C, OpCodes.Ldc_I4_M1),
                0x14 => (Register.D, OpCodes.Ldc_I4_1),
                0x15 => (Register.D, OpCodes.Ldc_I4_M1),
                0x1C => (Register.E, OpCodes.Ldc_I4_1),
                0x1D => (Register.E, OpCodes.Ldc_I4_M1),
                0x24 => (Register.H, OpCodes.Ldc_I4_1),
                0x25 => (Register.H, OpCodes.Ldc_I4_M1),
                0x2C => (Register.L, OpCodes.Ldc_I4_1),
                0x2D => (Register.L, OpCodes.Ldc_I4_M1),
                0x34 => (Register.M, OpCodes.Ldc_I4_1),
                0x35 => (Register.M, OpCodes.Ldc_I4_M1),
                0x3C => (Register.A, OpCodes.Ldc_I4_1),
                0x3D => (Register.A, OpCodes.Ldc_I4_M1),
                _ => throw new ArgumentOutOfRangeException(nameof(opcode), opcode, "Invalid opcode for INR instruction")
            };
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var field = _register switch
            {
                Register.A => internals.A,
                Register.B => internals.B,
                Register.C => internals.C,
                Register.D => internals.D,
                Register.E => internals.E,
                Register.H => internals.H,
                Register.L => internals.L,
                Register.M => null,
                _ => throw new ArgumentOutOfRangeException()
            };

            
            if (field == null)
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Call, internals.HL);
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
            }
            else
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, field);
            }
            
            methodIL.Emit(_incOrDec);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Conv_U1);
            var result = methodIL.DeclareLocal(typeof(byte));
            methodIL.Emit(OpCodes.Stloc, result.LocalIndex);

            if (field == null)
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Call, internals.HL);
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
            }
            else
            {
                methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
                methodIL.Emit(OpCodes.Stfld, field);
            }

            // Handle zero flag based on value of accumulator
            FlagUtilities.SetZeroFlagFromLocal(methodIL, internals.ZeroFlag, result);

            // Handle sign flag based on value of accumulator
            FlagUtilities.SetSignFlagFromLocal(methodIL, internals.SignFlag, result);

            // TODO - Handle parity/aux carry flags on increment
        }

        public override string ToString() => $"{(_incOrDec == OpCodes.Ldc_I4_1 ? "INR" : "DEC")} {_register}";
    }

    internal class RLCEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4, 128);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shr_Un);
            var bit7 = methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Stloc, bit7.LocalIndex); // Store off bit 7 as local
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldloc, bit7.LocalIndex);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, internals.A);

            // Set the carry flag based on bit 7
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, bit7.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }

        public override string ToString() => "RLC";
    }

    internal class RRCEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.And);
            var bit1 = methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Stloc, bit1.LocalIndex); // Store off bit 1 as local
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Ldloc, bit1.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, internals.A);

            // Set the carry flag based on bit 1
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, bit1.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }

        public override string ToString() => "RRC";
    }

    internal class RALEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4, 128);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shr_Un);
            var bit7 = methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Stloc, bit7.LocalIndex); // Store off bit 7 as local
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, internals.A);

            // Set the carry flag based on bit 7
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, bit7.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }
    }

    internal class RAREmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.And);
            var bit1 = methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Stloc, bit1.LocalIndex); // Store off bit 1 as local
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
            methodIL.Emit(OpCodes.Ldc_I4_7);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stfld, internals.A);

            // Set the carry flag based on bit 1
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, bit1.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }
    }

    internal class DADEmitter : IEmitter
    {
        private readonly string _regPair;

        internal DADEmitter(byte opcode)
        {
            _regPair = opcode switch
            {
                0x09 => "B",
                0x19 => "D",
                0x29 => "H",
                0x39 => "SP",
                _ => throw new ArgumentOutOfRangeException(nameof(opcode), opcode, "Invalid opcode for DAD command")
            };
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, internals.HL);
            methodIL.Emit(OpCodes.Ldarg_0);
            switch (_regPair)
            {
                case "B":
                    methodIL.Emit(OpCodes.Call, internals.BC);
                    break;
                case "D":
                    methodIL.Emit(OpCodes.Call, internals.DE);
                    break;
                case "H":
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    break;
                case "SP":
                    methodIL.Emit(OpCodes.Ldfld, internals.StackPointer);
                    break;
            }

            methodIL.Emit(OpCodes.Add);
            var result = methodIL.DeclareLocal(typeof(int));
            methodIL.Emit(OpCodes.Stloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Conv_U2);
            methodIL.Emit(OpCodes.Call, internals.SetHL);

            // Retrieve the int32 version of the answer to determine if there was a carry
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4, 0b1_0000_0000_0000_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
        }

        public override string ToString() => $"DAD {_regPair}";
    }

    internal class General8BitALUImmediateEmitter : IEmitter
    {
        private readonly Opcodes8080 _opcode;
        private readonly byte _operand;
        private readonly OpCode _arithmeticOpCode;
        private readonly bool _useCarryInOperation;
        private readonly bool _discardResult;

        internal General8BitALUImmediateEmitter(Opcodes8080 opcode, byte opcodeByte, byte operand)
        {
            _opcode = opcode;
            _operand = operand;
            (_arithmeticOpCode, _useCarryInOperation, _discardResult) = opcodeByte switch
            {
                0xC6 => (OpCodes.Add, false, true),
                0xCE => (OpCodes.Add, true, true),
                0xD6 => (OpCodes.Sub, false, true),
                0xDE => (OpCodes.Sub, true, true),
                0xE6 => (OpCodes.And, false, true),
                0xEE => (OpCodes.Xor, false, true),
                0xF6 => (OpCodes.Or, false, true),
                0xFE => (OpCodes.Sub, false, false),
                _ => throw new ArgumentOutOfRangeException(nameof(opcode), opcode,
                    "Invalid opcode for immediate ALU op"),
            };
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            methodIL.Emit(OpCodes.Ldc_I4_S, _operand);
            methodIL.Emit(_arithmeticOpCode);
            if (_useCarryInOperation)
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
                methodIL.Emit(_arithmeticOpCode);
            }
            var result = methodIL.DeclareLocal(typeof(int));
            var byteResult = methodIL.DeclareLocal(typeof(byte));
            methodIL.Emit(OpCodes.Stloc, result.LocalIndex); // Cache off the result as a local variable to allow setting flags
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stloc, byteResult.LocalIndex);

            if (!_discardResult)
            {
                methodIL.Emit(OpCodes.Ldloc, byteResult.LocalIndex);
                methodIL.Emit(OpCodes.Stfld, internals.A);
            }
            else
            {
                methodIL.Emit(OpCodes.Pop);
            }

            // Handle flag based on result
            FlagUtilities.SetZeroFlagFromLocal(methodIL, internals.ZeroFlag, byteResult);
            FlagUtilities.SetSignFlagFromLocal(methodIL, internals.SignFlag, byteResult);
            FlagUtilities.SetCarryFlagFromLocal(methodIL, internals.CarryFlag, result);

            // TODO - Handle aux carry & parity flags
        }

        public override string ToString() => $"{_opcode} {_operand:X2}";
    }

    /// <summary>
    /// This emitter is used for all 8 bit ALU operations apart from those
    /// which act on immediate values.
    /// </summary>
    internal class General8BitALUEmitter : IEmitter
    {
        private readonly Register _register;
        private readonly OpCode _arithmeticOpCode;
        private readonly bool _useCarryInOperation;
        private readonly Opcodes8080 _opcode;

        internal General8BitALUEmitter(byte opcodeByte, Opcodes8080 opcode)
        {
            _opcode = opcode;
            (_register, _arithmeticOpCode, _useCarryInOperation) = opcodeByte switch
            {
                0x80 => (Register.B, OpCodes.Add, false),
                0x81 => (Register.C, OpCodes.Add, false),
                0x82 => (Register.D, OpCodes.Add, false),
                0x83 => (Register.E, OpCodes.Add, false),
                0x84 => (Register.H, OpCodes.Add, false),
                0x85 => (Register.L, OpCodes.Add, false),
                0x86 => (Register.M, OpCodes.Add, false),
                0x87 => (Register.A, OpCodes.Add, false),
                0x88 => (Register.B, OpCodes.Add, true),
                0x89 => (Register.C, OpCodes.Add, true),
                0x8A => (Register.D, OpCodes.Add, true),
                0x8B => (Register.E, OpCodes.Add, true),
                0x8C => (Register.H, OpCodes.Add, true),
                0x8D => (Register.L, OpCodes.Add, true),
                0x8E => (Register.M, OpCodes.Add, true),
                0x8F => (Register.A, OpCodes.Add, true),
                0x90 => (Register.B, OpCodes.Sub, false),
                0x91 => (Register.C, OpCodes.Sub, false),
                0x92 => (Register.D, OpCodes.Sub, false),
                0x93 => (Register.E, OpCodes.Sub, false),
                0x94 => (Register.H, OpCodes.Sub, false),
                0x95 => (Register.L, OpCodes.Sub, false),
                0x96 => (Register.M, OpCodes.Sub, false),
                0x97 => (Register.A, OpCodes.Sub, false),
                0x98 => (Register.B, OpCodes.Sub, true),
                0x99 => (Register.C, OpCodes.Sub, true),
                0x9A => (Register.D, OpCodes.Sub, true),
                0x9B => (Register.E, OpCodes.Sub, true),
                0x9C => (Register.H, OpCodes.Sub, true),
                0x9D => (Register.L, OpCodes.Sub, true),
                0x9E => (Register.M, OpCodes.Sub, true),
                0x9F => (Register.A, OpCodes.Sub, true),
                0xA0 => (Register.B, OpCodes.And, false),
                0xA1 => (Register.C, OpCodes.And, false),
                0xA2 => (Register.D, OpCodes.And, false),
                0xA3 => (Register.E, OpCodes.And, false),
                0xA4 => (Register.H, OpCodes.And, false),
                0xA5 => (Register.L, OpCodes.And, false),
                0xA6 => (Register.M, OpCodes.And, false),
                0xA7 => (Register.A, OpCodes.And, false),
                0xA8 => (Register.B, OpCodes.Xor, false),
                0xA9 => (Register.C, OpCodes.Xor, false),
                0xAA => (Register.D, OpCodes.Xor, false),
                0xAB => (Register.E, OpCodes.Xor, false),
                0xAC => (Register.H, OpCodes.Xor, false),
                0xAD => (Register.L, OpCodes.Xor, false),
                0xAE => (Register.M, OpCodes.Xor, false),
                0xAF => (Register.A, OpCodes.Xor, false),
                0xB0 => (Register.B, OpCodes.Or, false),
                0xB1 => (Register.C, OpCodes.Or, false),
                0xB2 => (Register.D, OpCodes.Or, false),
                0xB3 => (Register.E, OpCodes.Or, false),
                0xB4 => (Register.H, OpCodes.Or, false),
                0xB5 => (Register.L, OpCodes.Or, false),
                0xB6 => (Register.M, OpCodes.Or, false),
                0xB7 => (Register.A, OpCodes.Or, false),
                0xB8 => (Register.B, OpCodes.Sub, false),
                0xB9 => (Register.C, OpCodes.Sub, false),
                0xBA => (Register.D, OpCodes.Sub, false),
                0xBB => (Register.E, OpCodes.Sub, false),
                0xBC => (Register.H, OpCodes.Sub, false),
                0xBD => (Register.L, OpCodes.Sub, false),
                0xBE => (Register.M, OpCodes.Sub, false),
                0xBF => (Register.A, OpCodes.Sub, false),
                _ => throw new ArgumentOutOfRangeException(nameof(opcodeByte), opcodeByte, "Invalid opcode for 8 bit ALU")
            };
        }

        private void LoadSource(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField)
        {
            switch (_register)
            {
                case Register.A:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.A);
                    break;
                case Register.B:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.B);
                    break;
                case Register.C:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.C);
                    break;
                case Register.D:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.D);
                    break;
                case Register.E:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.E);
                    break;
                case Register.H:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.H);
                    break;
                case Register.L:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.L);
                    break;
                case Register.M:
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Call, internals.HL);
                    methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.A);
            LoadSource(methodIL, internals, memoryBusField);
            methodIL.Emit(_arithmeticOpCode);
            if (_useCarryInOperation)
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
                methodIL.Emit(_arithmeticOpCode);
            }
            var result = methodIL.DeclareLocal(typeof(int));
            var byteResult = methodIL.DeclareLocal(typeof(byte));
            methodIL.Emit(OpCodes.Stloc, result.LocalIndex); // Cache off the result as a local variable to allow setting flags
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Conv_U1);
            methodIL.Emit(OpCodes.Stloc, byteResult.LocalIndex);

            if (_opcode != Opcodes8080.CMP)
            {
                methodIL.Emit(OpCodes.Ldloc, byteResult.LocalIndex);
                methodIL.Emit(OpCodes.Stfld, internals.A);
            }
            else
            {
                methodIL.Emit(OpCodes.Pop);
            }

            // Handle flags based on value of accumulator/local
            FlagUtilities.SetZeroFlagFromLocal(methodIL, internals.ZeroFlag, byteResult);
            FlagUtilities.SetSignFlagFromLocal(methodIL, internals.SignFlag, byteResult);
            FlagUtilities.SetCarryFlagFromLocal(methodIL, internals.CarryFlag, result);

            // TODO - Handle Carry/AuxCarry & Parity flags
        }

        public override string ToString() => $"{_opcode} {_register}";
    }

    internal class SHLDEmitter : IEmitter
    {
        private readonly ushort _address;

        internal SHLDEmitter(ushort address)
        {
            _address = address;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldfld, internals.L);
            methodIL.Emit(OpCodes.Ldc_I4, _address);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldfld, internals.H);
            methodIL.Emit(OpCodes.Ldc_I4, _address + 1);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
        }

        public override string ToString() => $"SHLD {_address:X4}";
    }

    internal class LHLDEmitter : IEmitter
    {
        private readonly ushort _address;

        internal LHLDEmitter(ushort address)
        {
            _address = address;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4, _address);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
            methodIL.Emit(OpCodes.Stfld, internals.L);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4, (ushort) (_address + 1));
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
            methodIL.Emit(OpCodes.Stfld, internals.H);
        }

        public override string ToString() => $"LHLD {_address:X4}";
    }

    internal class JumpOnFlagEmitter : IEmitter
    {
        private readonly byte _opcode;
        private readonly FieldBuilder _flagField;
        private readonly bool _flagValue;
        private readonly Label _jumpLabel;
        private readonly ushort _address;

        internal JumpOnFlagEmitter(byte opcode, FieldBuilder flagField, bool flagValue, Label jumpLabel, ushort address)
        {
            _opcode = opcode;
            _flagField = flagField;
            _flagValue = flagValue;
            _jumpLabel = jumpLabel;
            _address = address;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, _flagField);
            methodIL.Emit(_flagValue ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Beq, _jumpLabel);
        }

        public override string ToString() => _opcode switch
        {
            0xC2 => $"JNZ {_address:X4}",
            0xCA => $"JZ {_address:X4}",
            0xD2 => $"JNC {_address:X4}",
            0xDA => $"JC {_address:X4}",
            0xE2 => $"JPO {_address:X4}",
            0xEA => $"JPE {_address:X4}",
            0xF2 => $"JP {_address:X4}",
            0xFA => $"JM {_address:X4}",
            _ => throw new ArgumentOutOfRangeException(nameof(_opcode), _opcode, "Invalid opcode for jump with flag"),
        };
    }

    internal class PUSHEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal PUSHEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            switch (_opcode)
            {
                case 0xC5: // PUSH B
                    StackUtilities.PushPairToStack(methodIL, internals.B, internals.C, internals.StackPointer, memoryBusField);
                    break;
                case 0xD5: // PUSH D
                    StackUtilities.PushPairToStack(methodIL, internals.D, internals.E, internals.StackPointer, memoryBusField);
                    break;
                case 0xE5: // PUSH H
                    StackUtilities.PushPairToStack(methodIL, internals.H, internals.L, internals.StackPointer, memoryBusField);
                    break;
                case 0xF5: // PUSH PSW
                    StackUtilities.PushPairToStack(methodIL, internals.A, internals.GetFlagRegister, internals.StackPointer, memoryBusField);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_opcode), _opcode,
                        "Invalid opcode for PUSH instruction");
            }
            
        }

        public override string ToString() => _opcode switch
        {
            0xC5 => "PUSH B",
            0xD5 => "PUSH D",
            0xE5 => "PUSH H",
            0xF5 => "PUSH PSW",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class POPEmitter : IEmitter
    {
        private readonly byte _opcode;

        internal POPEmitter(byte opcode)
        {
            _opcode = opcode;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            switch (_opcode)
            {
                case 0xC1: // POP B
                    StackUtilities.PopPairFromStack(methodIL, internals.B, internals.C, internals.StackPointer, memoryBusField);
                    break;
                case 0xD1: // POP D
                    StackUtilities.PopPairFromStack(methodIL, internals.D, internals.E, internals.StackPointer, memoryBusField);
                    break;
                case 0xE1: // POP H
                    StackUtilities.PopPairFromStack(methodIL, internals.H, internals.L, internals.StackPointer, memoryBusField);
                    break;
                case 0xF1: // POP PSW
                    StackUtilities.PopPairFromStack(methodIL, internals.A, internals.SetFlagRegister, internals.StackPointer, memoryBusField);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_opcode), _opcode,
                        "Invalid opcode for POP instruction");
            }
        }

        public override string ToString() => _opcode switch
        {
            0xC1 => "POP B",
            0xD1 => "POP D",
            0xE1 => "POP H",
            0xF1 => "POP PSW",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class CallEmitter : IEmitter
    {
        private readonly byte _opcode;
        private readonly ushort _returnAddress;
        private readonly FieldBuilder _flag;
        private readonly bool _flagValue;
        private readonly Label _destination;

        internal CallEmitter(byte opcode, ushort returnAddress, FieldBuilder flag, bool flagValue, Label destination)
        {
            _opcode = opcode;
            _returnAddress = returnAddress;
            _flag = flag;
            _flagValue = flagValue;
            _destination = destination;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            if (_flag == null)
            {
                // CALL instruction
                StackUtilities.PushWordToStack(methodIL, internals.StackPointer, _returnAddress, memoryBusField);
                methodIL.Emit(OpCodes.Br, _destination);
            }
            else
            {
                var skipCallLabel = methodIL.DefineLabel();
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, _flag);
                methodIL.Emit(_flagValue ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                methodIL.Emit(OpCodes.Bne_Un, skipCallLabel);
                StackUtilities.PushWordToStack(methodIL, internals.StackPointer, _returnAddress, memoryBusField);
                methodIL.Emit(OpCodes.Br, _destination);
                methodIL.MarkLabel(skipCallLabel);
            }
        }

        public override string ToString() => _opcode switch
        {
            0xC4 => "CNZ",
            0xC7 => "RST 0",
            0xCC => "CZ",
            var b when b == 0xCD || b == 0xDD || b == 0xED || b == 0xFD => "CALL",
            0xCF => "RST 1",
            0xD4 => "CNC",
            0xD7 => "RST 2",
            0xDC => "CC",
            0xDF => "RST 3",
            0xE4 => "CPO",
            0xE7 => "RST 4",
            0xEC => "CPE",
            0xEF => "RST 5",
            0xF4 => "CP",
            0xF7 => "RST 6",
            0xFC => "CM",
            0xFF => "RST 7",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class RetEmitter : IEmitter
    {
        private readonly byte _opcode;
        private readonly FieldBuilder _flag;
        private readonly bool _flagValue;

        internal RetEmitter(byte opcode, FieldBuilder flag, bool flagValue)
        {
            _opcode = opcode;
            _flag = flag;
            _flagValue = flagValue;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var returnAddress = methodIL.DeclareLocal(typeof(byte));
            if (_flag == null)
            {
                StackUtilities.PopPairFromStack(methodIL, returnAddress, internals.StackPointer, memoryBusField);
                //ProgramCounterUtilities.JumpToDynamicAddress(methodIL, internals.ProgramLabels, returnAddress);
            }
            else
            {
                var skipCallLabel = methodIL.DefineLabel();
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, _flag);
                methodIL.Emit(_flagValue ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                methodIL.Emit(OpCodes.Bne_Un, skipCallLabel);
                StackUtilities.PopPairFromStack(methodIL, returnAddress, internals.StackPointer, memoryBusField);
                ProgramCounterUtilities.JumpToDynamicAddress(methodIL, internals.ProgramLabels, returnAddress);
                methodIL.MarkLabel(skipCallLabel);
            }
        }

        public override string ToString() => _opcode switch
        {
            0xC0 => "RNZ",
            0xC8 => "RZ",
            0xC9 => "RET",
            0xD0 => "RNC",
            0xD8 => "RC",
            0xD9 => "RET",
            0xE0 => "RPO",
            0xE8 => "RPE",
            0xF0 => "RP",
            0xF8 => "RM",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal class XCHGEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var h = methodIL.DeclareLocal(typeof(byte));
            var l = methodIL.DeclareLocal(typeof(byte));
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.H);
            methodIL.Emit(OpCodes.Stloc, h);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.D);
            methodIL.Emit(OpCodes.Stfld, internals.H);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, h);
            methodIL.Emit(OpCodes.Stfld, internals.D);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.L);
            methodIL.Emit(OpCodes.Stloc, l);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.E);
            methodIL.Emit(OpCodes.Stfld, internals.L);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, l);
            methodIL.Emit(OpCodes.Stfld, internals.E);
        }

        public override string ToString() => "XCHG";
    }

    internal class XTHLEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var temp = methodIL.DeclareLocal(typeof(byte));

            foreach (var (register, spInc) in new [] { (internals.L, false), (internals.H, true) })
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, register);
                methodIL.Emit(OpCodes.Stloc, temp);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, internals.StackPointer);
                if (spInc) // H is stored at SP + 1
                {
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                }
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
                methodIL.Emit(OpCodes.Stfld, register);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                methodIL.Emit(OpCodes.Ldloc, temp);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, internals.StackPointer);
                if (spInc) // H is stored at SP + 1
                {
                    methodIL.Emit(OpCodes.Ldc_I4_1);
                    methodIL.Emit(OpCodes.Add);
                }
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
            }
        }

        public override string ToString() => "XTHL";
    }

    internal class UpdateInterruptEnableEmitter : IEmitter
    {
        private readonly bool _enable;

        internal UpdateInterruptEnableEmitter(bool enable)
        {
            _enable = enable;
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(_enable ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stfld, internals.InterruptEnable);
        }

        public override string ToString() => _enable switch
        {
            true => "EI",
            false => "DI",
        };
    }

    internal class HLTEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            // TODO - Probably worth noting what caused a return via an enum?
            methodIL.Emit(OpCodes.Ret);
        }

        public override string ToString() => "HLT";
    }

    internal class SPHLEmitter : IEmitter
    {
        public override string ToString() => "SPHL";

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.H);
            methodIL.Emit(OpCodes.Ldc_I4_8);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.L);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Stfld, internals.StackPointer);
        }
    }

    internal class PCHLEmitter : IEmitter
    {
        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var destinationAddress = methodIL.DeclareLocal(typeof(ushort));
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.H);
            methodIL.Emit(OpCodes.Ldc_I4_8);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.L);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Stloc, destinationAddress.LocalIndex);
            ProgramCounterUtilities.JumpToDynamicAddress(methodIL, internals.ProgramLabels, destinationAddress);
        }

        public override string ToString() => "PCHL";
    }
}