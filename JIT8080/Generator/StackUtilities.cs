using System.Reflection.Emit;

namespace JIT8080.Generator
{
    internal static class StackUtilities
    {
        private static void IncDecStackPointer(ILGenerator methodIL, FieldBuilder stackPointer, OpCode incOrDecOpCode)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(incOrDecOpCode);
            methodIL.Emit(OpCodes.Stfld, stackPointer);
        }

        private static void ReadByte(ILGenerator methodIL, FieldBuilder stackPointer, FieldBuilder memoryBusField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("ReadByte")!);
        }

        internal static void PopPairFromStack(ILGenerator methodIL, LocalBuilder returnLocal, FieldBuilder stackPointer,
            FieldBuilder memoryBusField)
        {
            ReadByte(methodIL, stackPointer, memoryBusField);
            methodIL.Emit(OpCodes.Stloc, returnLocal.LocalIndex);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Add);

            ReadByte(methodIL, stackPointer, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4_8);
            methodIL.Emit(OpCodes.Shl);
            methodIL.Emit(OpCodes.Ldloc, returnLocal.LocalIndex);
            methodIL.Emit(OpCodes.Or);
            methodIL.Emit(OpCodes.Stloc, returnLocal.LocalIndex);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Add);
        }

        internal static void PopPairFromStack(ILGenerator methodIL, FieldBuilder msb, FieldBuilder lsb,
            FieldBuilder stackPointer, FieldBuilder memoryBusField)
        {
            foreach (var b in new[] {lsb, msb})
            {
                methodIL.Emit(OpCodes.Ldarg_0);
                ReadByte(methodIL, stackPointer, memoryBusField);
                methodIL.Emit(OpCodes.Stfld, b);

                IncDecStackPointer(methodIL, stackPointer, OpCodes.Add);
            }
        }

        /// <summary>
        /// Used for popping PSW (A, Flags) from the stack
        /// </summary>
        internal static void PopPairFromStack(ILGenerator methodIL, FieldBuilder msb, MethodBuilder lsb,
            FieldBuilder stackPointer, FieldBuilder memoryBusField)
        {
            methodIL.Emit(OpCodes.Ldarg_0);
            ReadByte(methodIL, stackPointer, memoryBusField);
            methodIL.Emit(OpCodes.Call, lsb);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Add);

            methodIL.Emit(OpCodes.Ldarg_0);
            ReadByte(methodIL, stackPointer, memoryBusField);
            methodIL.Emit(OpCodes.Stfld, msb);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Add);
        }

        internal static void PushPairToStack(ILGenerator methodIL, FieldBuilder msb, FieldBuilder lsb,
            FieldBuilder stackPointer, FieldBuilder memoryBusField)
        {
            foreach (var b in new[] {msb, lsb})
            {
                IncDecStackPointer(methodIL, stackPointer, OpCodes.Sub);

                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, memoryBusField);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, b);
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, stackPointer);
                methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
            }
        }

        /// <summary>
        /// Used for pushing PSW (A, Flags) to the stack
        /// </summary>
        internal static void PushPairToStack(ILGenerator methodIL, FieldBuilder msb, MethodBuilder lsb,
            FieldBuilder stackPointer, FieldBuilder memoryBusField)
        {
            IncDecStackPointer(methodIL, stackPointer, OpCodes.Sub);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, msb);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Sub);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, lsb);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
        }

        /// <summary>
        /// Push a word defined at compile time to the stack rather than values
        /// derived from register values.
        /// </summary>
        /// 
        /// <param name="methodIL">
        /// The IL generator for the main method
        /// </param>
        ///
        /// <param name="stackPointer">
        /// The current stack pointer
        /// </param>
        ///
        /// <param name="word">
        /// The value to push
        /// </param>
        ///
        /// <param name="memoryBusField"
        /// >A field on the main class providing WriteByte functionality
        /// </param>
        internal static void PushWordToStack(ILGenerator methodIL, FieldBuilder stackPointer, ushort word,
            FieldBuilder memoryBusField)
        {
            var msb = (byte) (word >> 8);
            var lsb = (byte) (word & 0b1111_1111);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Sub);
            
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4_S, msb);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);

            IncDecStackPointer(methodIL, stackPointer, OpCodes.Sub);

            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, memoryBusField);
            methodIL.Emit(OpCodes.Ldc_I4_S, lsb);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, stackPointer);
            methodIL.Emit(OpCodes.Callvirt, memoryBusField.FieldType.GetMethod("WriteByte")!);
        }
    }
}