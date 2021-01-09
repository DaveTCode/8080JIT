using System.Reflection.Emit;

namespace JIT8080.Generator
{
    /// <summary>
    /// Contains static functions which provide macro like expansion of flag
    /// operations (e.g. Set Zero flag based on accumulator)
    /// </summary>
    internal static class FlagUtilities
    {
        internal static void SetZeroFlagFromLocal(ILGenerator methodIL, FieldBuilder zeroFlagField, LocalBuilder local)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, local);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Ceq);
            methodIL.Emit(OpCodes.Stfld, zeroFlagField);
        }

        internal static void SetSignFlagFromLocal(ILGenerator methodIL, FieldBuilder signFlagField, LocalBuilder local)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, local);
            methodIL.Emit(OpCodes.Ldc_I4, 0b1000_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, signFlagField);
        }

        internal static void SetCarryFlagFrom8BitLocal(ILGenerator methodIL, FieldBuilder carryFlagBuilder, LocalBuilder result)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4, 0b1111_1111);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, carryFlagBuilder);
        }

        /// <summary>
        /// This function performs the calculation as
        /// parity = result
        /// parity ^= (parity >> 4)
        /// parity ^= (parity >> 2)
        /// parity ^= (parity >> 1)
        /// </summary>
        internal static void SetParityFlagFromLocal(ILGenerator methodIL, FieldBuilder parityFlagBuilder, LocalBuilder result)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Dup);
            methodIL.Emit(OpCodes.Ldc_I4_4);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Xor);
            methodIL.Emit(OpCodes.Dup);
            methodIL.Emit(OpCodes.Ldc_I4_2);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Xor);
            methodIL.Emit(OpCodes.Dup);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Shr_Un);
            methodIL.Emit(OpCodes.Xor);
            methodIL.Emit(OpCodes.Not);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Stfld, parityFlagBuilder);
        }
    }
}
