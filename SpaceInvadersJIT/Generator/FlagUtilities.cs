using System.Reflection.Emit;

namespace SpaceInvadersJIT.Generator
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

        internal static void SetCarryFlagFromLocal(ILGenerator methodIL, FieldBuilder carryFlagBuilder, LocalBuilder result)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc, result.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I4, 0b1_0000_0000);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.Stfld, carryFlagBuilder);
        }
    }
}
