using System;
using System.Reflection.Emit;
using JIT8080._8080;

namespace JIT8080.Generator
{
    /// <summary>
    /// This emitter is used for all 8 bit ALU operations and is within it's
    /// own file due to it's higher complexity
    /// </summary>
    internal class General8BitALUEmitter : IEmitter
    {
        private readonly Register _register;
        private readonly OpCode _arithmeticOpCode;
        private readonly bool _useCarryInOperation;
        private readonly byte _opcodeByte;
        private readonly Opcodes8080 _opcode;
        private readonly byte _operand;

        internal General8BitALUEmitter(byte opcodeByte, Opcodes8080 opcode)
        {
            _opcodeByte = opcodeByte;
            _opcode = opcode;

            _register = DetermineRegister(opcodeByte);
            _arithmeticOpCode = ArithmeticOpcode(_opcode);
            _useCarryInOperation = UseCarryInOperation(_opcode);
        }
        
        internal General8BitALUEmitter(byte opcodeByte, Opcodes8080 opcode, byte operand)
            : this(opcodeByte, opcode)
        {
            _operand = operand;
        }

        private static Register DetermineRegister(byte opcodeByte)
        {
            var register = (opcodeByte > 0xC0) ? ((opcodeByte & 0b0011_1000) >> 3) : (opcodeByte & 0b0000_0111);
            return register switch
            {
                0b000 => Register.B,
                0b001 => Register.C,
                0b010 => Register.D,
                0b011 => Register.E,
                0b100 => Register.H,
                0b101 => Register.L,
                0b110 => Register.M,
                0b111 => Register.A,
                _ => throw new ArgumentOutOfRangeException(nameof(opcodeByte), opcodeByte, "Invalid program")
            };
        }

        private static OpCode ArithmeticOpcode(Opcodes8080 opcode) => opcode switch
        {
            Opcodes8080.ADD => OpCodes.Add,
            Opcodes8080.ADC => OpCodes.Add,
            Opcodes8080.ADI => OpCodes.Add,
            Opcodes8080.ACI => OpCodes.Add,
            Opcodes8080.SUB => OpCodes.Sub,
            Opcodes8080.SBB => OpCodes.Sub,
            Opcodes8080.SBI => OpCodes.Sub,
            Opcodes8080.SUI => OpCodes.Sub,
            Opcodes8080.ANA => OpCodes.And,
            Opcodes8080.ANI => OpCodes.And,
            Opcodes8080.XRA => OpCodes.Xor,
            Opcodes8080.XRI => OpCodes.Xor,
            Opcodes8080.ORA => OpCodes.Or,
            Opcodes8080.ORI => OpCodes.Or,
            Opcodes8080.CMP => OpCodes.Sub,
            Opcodes8080.CPI => OpCodes.Sub,
            _ => throw new ArgumentOutOfRangeException(nameof(opcode), opcode, "Invalid opcode for 8 bit arithmetic"),
        };

        private static bool UseCarryInOperation(Opcodes8080 opcode) => opcode switch
        {
            Opcodes8080.ADC => true,
            Opcodes8080.SBB => true,
            Opcodes8080.ACI => true,
            Opcodes8080.SBI => true,
            _ => false,
        };

        private static bool WriteResult(Opcodes8080 opcode) => opcode != Opcodes8080.CMP && opcode != Opcodes8080.CPI; 

        private void LoadSource(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField)
        {
            if (_opcodeByte > 0xC0) // Immediate 8 bit arithmetic
            {
                methodIL.EmitLd8Immediate( _operand);
            }
            else
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
        }

        void IEmitter.Emit(ILGenerator methodIL, CpuInternalBuilders internals, FieldBuilder memoryBusField,
            FieldBuilder ioHandlerField)
        {
            var result = methodIL.DeclareLocal(typeof(int));

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
            methodIL.Emit(OpCodes.Stloc, result.LocalIndex);  // Cache off the result as a local variable to allow setting flags
            
            // Set the carry flag (different for different operations)
            methodIL.Emit(OpCodes.Ldarg_0);
            switch (_opcode)
            {
                case Opcodes8080.ADD:
                case Opcodes8080.ADC:
                case Opcodes8080.ACI:
                case Opcodes8080.ADI:
                    methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
                    methodIL.Emit(OpCodes.Ldc_I4, 255);
                    methodIL.Emit(OpCodes.Cgt);
                    methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
                    break;
                case Opcodes8080.SUB:
                case Opcodes8080.CMP:
                case Opcodes8080.CPI:
                case Opcodes8080.SUI:
                    LoadSource(methodIL, internals, memoryBusField);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.A);
                    methodIL.Emit(OpCodes.Cgt);
                    methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
                    break;
                case Opcodes8080.SBB:
                case Opcodes8080.SBI:
                    LoadSource(methodIL, internals, memoryBusField);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.CarryFlag);
                    methodIL.Emit(OpCodes.Add);
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, internals.A);
                    methodIL.Emit(OpCodes.Cgt);
                    methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
                    break;
                case Opcodes8080.ANA:
                case Opcodes8080.ANI:
                case Opcodes8080.XRA:
                case Opcodes8080.XRI:
                case Opcodes8080.ORA:
                case Opcodes8080.ORI:
                    methodIL.Emit(OpCodes.Ldc_I4_0);
                    methodIL.Emit(OpCodes.Stfld, internals.CarryFlag);
                    break;
                default:
                    throw new ArgumentException(nameof(_opcode));
            }

            if (WriteResult(_opcode))
            {
                methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
                methodIL.Emit(OpCodes.Stfld, internals.A);
            }
            else
            {
                methodIL.Emit(OpCodes.Pop);
            }

            // Handle flags based on value of accumulator/local
            FlagUtilities.SetZeroFlagFromLocal(methodIL, internals.ZeroFlag, result);
            FlagUtilities.SetSignFlagFromLocal(methodIL, internals.SignFlag, result);
            FlagUtilities.SetParityFlagFromLocal(methodIL, internals.ParityFlag, result);

            // TODO - Handle AuxCarry flag
        }

        public override string ToString() => _opcode switch
        {
            Opcodes8080.ADD => $"{_opcode} {_register}",
            Opcodes8080.ADC => $"{_opcode} {_register}",
            Opcodes8080.ADI => $"{_opcode} {_operand:X2}",
            Opcodes8080.ACI => $"{_opcode} {_operand:X2}",
            Opcodes8080.SUB => $"{_opcode} {_register}",
            Opcodes8080.SBB => $"{_opcode} {_register}",
            Opcodes8080.SBI => $"{_opcode} {_operand:X2}",
            Opcodes8080.SUI => $"{_opcode} {_operand:X2}",
            Opcodes8080.ANA => $"{_opcode} {_register}",
            Opcodes8080.ANI => $"{_opcode} {_operand:X2}",
            Opcodes8080.XRA => $"{_opcode} {_register}",
            Opcodes8080.XRI => $"{_opcode} {_operand:X2}",
            Opcodes8080.ORA => $"{_opcode} {_register}",
            Opcodes8080.ORI => $"{_opcode} {_operand:X2}",
            Opcodes8080.CMP => $"{_opcode} {_register}",
            Opcodes8080.CPI => $"{_opcode} {_operand:X2}",
            _ => throw new ArgumentOutOfRangeException(nameof(_opcode), _opcode, "Invalid 8 bit ALU opcode"),
        };
    }
}