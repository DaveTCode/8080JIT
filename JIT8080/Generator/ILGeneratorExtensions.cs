using System;
using System.Reflection.Emit;

namespace JIT8080.Generator
{
    internal static class ILGeneratorExtensions
    {
        internal static void EmitDebugString(this ILGenerator ilGenerator, CpuInternalBuilders internals)
        {
            const string formatString = "A={0:X2} B={1:X2} C={2:X2} D={3:X2} E={4:X2} H={5:X2} L={6:X2} SP={7:X4} S={8} C={9} A={10} P={11} Z={12}";
            var wlParams = new[] 
            {
                typeof(string),
                typeof(object[])
            };

            var writeLineMethodInfo = typeof(Console).GetMethod("WriteLine", wlParams);
            ilGenerator.Emit(OpCodes.Ldstr, formatString);
            ilGenerator.Emit(OpCodes.Ldc_I4, 13);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));

            foreach (var (field, ix) in new[]
            {
                (internals.A, OpCodes.Ldc_I4_0), 
                (internals.B, OpCodes.Ldc_I4_1),
                (internals.C, OpCodes.Ldc_I4_2), 
                (internals.D, OpCodes.Ldc_I4_3), 
                (internals.E, OpCodes.Ldc_I4_4), 
                (internals.H, OpCodes.Ldc_I4_5), 
                (internals.L, OpCodes.Ldc_I4_6)
            })
            {
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(ix);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, field);
                ilGenerator.Emit(OpCodes.Box, typeof(byte));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
            
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Ldc_I4_7);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, internals.StackPointer);
            ilGenerator.Emit(OpCodes.Box, typeof(ushort));
            ilGenerator.Emit(OpCodes.Stelem_Ref);
            
            foreach (var (field, ix) in new[]
            {
                (internals.SignFlag, 8), 
                (internals.CarryFlag, 9),
                (internals.AuxCarryFlag, 10), 
                (internals.ParityFlag, 11), 
                (internals.ZeroFlag, 12)
            })
            {
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldc_I4, ix);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, field);
                ilGenerator.Emit(OpCodes.Box, typeof(bool));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }

            ilGenerator.EmitCall(OpCodes.Call, writeLineMethodInfo!, null);
        }
        
        internal static void EmitLd8Immediate(this ILGenerator ilGenerator, byte operand)
        {
            switch (operand)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                case { } x when x < 128:
                    ilGenerator.Emit(OpCodes.Ldc_I4_S, operand);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4, (int)operand);
                    break;
            }
        }
    }
}