using System.Reflection.Emit;

namespace JIT8080.Generator
{
    internal static class ILGeneratorExtensions
    {
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