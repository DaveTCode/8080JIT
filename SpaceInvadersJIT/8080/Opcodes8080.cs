﻿using System;

namespace SpaceInvadersJIT._8080
{
    /// <summary>
    /// All the different opcodes that are used in an 8080 processor
    /// </summary>
    internal enum Opcodes8080
    {
        NOP,
        LXI,
        STAX,
        INX,
        INR,
        DCR,
        MVI,
        RLC,
        DAD,
        LDAX,
        DCX,
        RRC,
        RAL,
        RAR,
        SHLD,
        DAA,
        LHLD,
        CMA,
        STA,
        STC,
        LDA,
        CMC,
        MOV,
        HLT,
        ADD,
        ADC,
        SUB,
        SBB,
        ANA,
        XRA,
        ORA,
        CMP,
        RNZ,
        POP,
        JNZ,
        JMP,
        CNZ,
        PUSH,
        ADI,
        RST,
        RZ,
        RET,
        JZ,
        CZ,
        CALL,
        ACI,
        RNC,
        JNC,
        OUT,
        CNC,
        SUI,
        RC,
        JC,
        IN,
        CC,
        SBI,
        RPO,
        JPO,
        XTHL,
        CPO,
        ANI,
        RPE,
        PCHL,
        JPE,
        XCHG,
        CPE,
        XRI,
        RP,
        JP,
        DI,
        CP,
        ORI,
        RM,
        SPHL,
        JM,
        EI,
        CM,
        CPI,
    }

    internal static class Opcodes8080Extensions
    {
        /// <summary>
        /// Represents the number of bytes that this opcode takes up in the 
        /// program.
        /// </summary>
        /// 
        /// <param name="opcode">
        /// The decoded opcode
        /// </param>
        /// 
        /// <returns>
        /// The number of consecutive bytes in memory that correspond 
        /// to this opcode.
        /// </returns>
        internal static byte Length(this Opcodes8080 opcode) => opcode switch
        {
            Opcodes8080.NOP => 1,
            Opcodes8080.LXI => 3,
            Opcodes8080.STAX => 1,
            Opcodes8080.INX => 1,
            Opcodes8080.INR => 1,
            Opcodes8080.DCR => 1,
            Opcodes8080.MVI => 2,
            Opcodes8080.RLC => 1,
            Opcodes8080.DAD => 1,
            Opcodes8080.LDAX => 1,
            Opcodes8080.DCX => 1,
            Opcodes8080.RRC => 1,
            Opcodes8080.RAL => 1,
            Opcodes8080.RAR => 1,
            Opcodes8080.SHLD => 3,
            Opcodes8080.DAA => 1,
            Opcodes8080.LHLD => 3,
            Opcodes8080.CMA => 1,
            Opcodes8080.STA => 3,
            Opcodes8080.STC => 1,
            Opcodes8080.LDA => 3,
            Opcodes8080.CMC => 1,
            Opcodes8080.MOV => 1,
            Opcodes8080.HLT => 1,
            Opcodes8080.ADD => 1,
            Opcodes8080.ADC => 1,
            Opcodes8080.SUB => 1,
            Opcodes8080.SBB => 1,
            Opcodes8080.ANA => 1,
            Opcodes8080.XRA => 1,
            Opcodes8080.ORA => 1,
            Opcodes8080.CMP => 1,
            Opcodes8080.RNZ => 1,
            Opcodes8080.POP => 1,
            Opcodes8080.JNZ => 3,
            Opcodes8080.JMP => 3,
            Opcodes8080.CNZ => 3,
            Opcodes8080.PUSH => 1,
            Opcodes8080.ADI => 2,
            Opcodes8080.RST => 1,
            Opcodes8080.RZ => 1,
            Opcodes8080.RET => 1,
            Opcodes8080.JZ => 3,
            Opcodes8080.CZ => 3,
            Opcodes8080.CALL => 3,
            Opcodes8080.ACI => 2,
            Opcodes8080.RNC => 1,
            Opcodes8080.JNC => 3,
            Opcodes8080.OUT => 2,
            Opcodes8080.CNC => 3,
            Opcodes8080.SUI => 2,
            Opcodes8080.RC => 1,
            Opcodes8080.JC => 3,
            Opcodes8080.IN => 2,
            Opcodes8080.CC => 3,
            Opcodes8080.SBI => 2,
            Opcodes8080.RPO => 1,
            Opcodes8080.JPO => 3,
            Opcodes8080.XTHL => 1,
            Opcodes8080.CPO => 3,
            Opcodes8080.ANI => 2,
            Opcodes8080.RPE => 1,
            Opcodes8080.PCHL => 1,
            Opcodes8080.JPE => 3,
            Opcodes8080.XCHG => 1,
            Opcodes8080.CPE => 3,
            Opcodes8080.XRI => 2,
            Opcodes8080.RP => 1,
            Opcodes8080.JP => 3,
            Opcodes8080.DI => 1,
            Opcodes8080.CP => 3,
            Opcodes8080.ORI => 2,
            Opcodes8080.RM => 1,
            Opcodes8080.SPHL => 1,
            Opcodes8080.JM => 3,
            Opcodes8080.EI => 1,
            Opcodes8080.CM => 3,
            Opcodes8080.CPI => 2,
            _ => throw new NotImplementedException(),
        };
    }

    internal static class Opcodes8080Decoder
    {
        internal static Opcodes8080 Decode(byte opcode) => opcode switch
        {
            // 0x00-0x0F
            0x00 => Opcodes8080.NOP,
            0x01 => Opcodes8080.LXI,
            0x02 => Opcodes8080.STAX,
            0x03 => Opcodes8080.INX,
            0x04 => Opcodes8080.INR,
            0x05 => Opcodes8080.DCR,
            0x06 => Opcodes8080.MVI,
            0x07 => Opcodes8080.RLC,
            0x08 => Opcodes8080.NOP,
            0x09 => Opcodes8080.DAD,
            0x0A => Opcodes8080.LDAX,
            0x0B => Opcodes8080.DCX,
            0x0C => Opcodes8080.INR,
            0x0D => Opcodes8080.DCR,
            0x0E => Opcodes8080.MVI,
            0x0F => Opcodes8080.RRC,
            // 0x10-0x1F
            0x10 => Opcodes8080.NOP,
            0x11 => Opcodes8080.LXI,
            0x12 => Opcodes8080.STAX,
            0x13 => Opcodes8080.INX,
            0x14 => Opcodes8080.INR,
            0x15 => Opcodes8080.DCR,
            0x16 => Opcodes8080.MVI,
            0x17 => Opcodes8080.RAL,
            0x18 => Opcodes8080.NOP,
            0x19 => Opcodes8080.DAD,
            0x1A => Opcodes8080.LDAX,
            0x1B => Opcodes8080.DCX,
            0x1C => Opcodes8080.INR,
            0x1D => Opcodes8080.DCR,
            0x1E => Opcodes8080.MVI,
            0x1F => Opcodes8080.RAR,
            // 0x20-0x2F
            0x20 => Opcodes8080.NOP,
            0x21 => Opcodes8080.LXI,
            0x22 => Opcodes8080.SHLD,
            0x23 => Opcodes8080.INX,
            0x24 => Opcodes8080.INR,
            0x25 => Opcodes8080.DCR,
            0x26 => Opcodes8080.MVI,
            0x27 => Opcodes8080.DAA,
            0x28 => Opcodes8080.NOP,
            0x29 => Opcodes8080.DAD,
            0x2A => Opcodes8080.LHLD,
            0x2B => Opcodes8080.DCX,
            0x2C => Opcodes8080.INR,
            0x2D => Opcodes8080.DCR,
            0x2E => Opcodes8080.MVI,
            0x2F => Opcodes8080.CMA,
            // 0x30-0x3F
            0x30 => Opcodes8080.NOP,
            0x31 => Opcodes8080.LXI,
            0x32 => Opcodes8080.STA,
            0x33 => Opcodes8080.INX,
            0x34 => Opcodes8080.INR,
            0x35 => Opcodes8080.DCR,
            0x36 => Opcodes8080.MVI,
            0x37 => Opcodes8080.STC,
            0x38 => Opcodes8080.NOP,
            0x39 => Opcodes8080.DAD,
            0x3A => Opcodes8080.LDA,
            0x3B => Opcodes8080.DCX,
            0x3C => Opcodes8080.INR,
            0x3D => Opcodes8080.DCR,
            0x3E => Opcodes8080.MVI,
            0x3F => Opcodes8080.CMC,
            // 0x40-0x7F
            0x76 => Opcodes8080.HLT,
            < 0x80 => Opcodes8080.MOV,
            // 0x80-0xBF
            < 0x88 => Opcodes8080.ADD,
            < 0x90 => Opcodes8080.ADC,
            < 0x98 => Opcodes8080.SUB,
            < 0xA0 => Opcodes8080.SBB,
            < 0xA8 => Opcodes8080.ANA,
            < 0xB0 => Opcodes8080.XRA,
            < 0xB8 => Opcodes8080.ORA,
            < 0xC0 => Opcodes8080.CMP,
            // 0xC0-0xCF
            0xC0 => Opcodes8080.RNZ,
            0xC1 => Opcodes8080.POP,
            0xC2 => Opcodes8080.JNZ,
            0xC3 => Opcodes8080.JMP,
            0xC4 => Opcodes8080.CNZ,
            0xC5 => Opcodes8080.PUSH,
            0xC6 => Opcodes8080.ADI,
            0xC7 => Opcodes8080.RST,
            0xC8 => Opcodes8080.RZ,
            0xC9 => Opcodes8080.RET,
            0xCA => Opcodes8080.JZ,
            0xCB => Opcodes8080.JMP,
            0xCC => Opcodes8080.CZ,
            0xCD => Opcodes8080.CALL,
            0xCE => Opcodes8080.ACI,
            0xCF => Opcodes8080.RST,
            // 0xD0-0xDF
            0xD0 => Opcodes8080.RNC,
            0xD1 => Opcodes8080.POP,
            0xD2 => Opcodes8080.JNC,
            0xD3 => Opcodes8080.OUT,
            0xD4 => Opcodes8080.CNC,
            0xD5 => Opcodes8080.PUSH,
            0xD6 => Opcodes8080.SUI,
            0xD7 => Opcodes8080.RST,
            0xD8 => Opcodes8080.RC,
            0xD9 => Opcodes8080.RET,
            0xDA => Opcodes8080.JC,
            0xDB => Opcodes8080.IN,
            0xDC => Opcodes8080.CC,
            0xDD => Opcodes8080.CALL,
            0xDE => Opcodes8080.SBI,
            0xDF => Opcodes8080.RST,
            // 0xE0-0xEF
            0xE0 => Opcodes8080.RPO,
            0xE1 => Opcodes8080.POP,
            0xE2 => Opcodes8080.JPO,
            0xE3 => Opcodes8080.XTHL,
            0xE4 => Opcodes8080.CPO,
            0xE5 => Opcodes8080.PUSH,
            0xE6 => Opcodes8080.ANI,
            0xE7 => Opcodes8080.RST,
            0xE8 => Opcodes8080.RPE,
            0xE9 => Opcodes8080.PCHL,
            0xEA => Opcodes8080.JPE,
            0xEB => Opcodes8080.XCHG,
            0xEC => Opcodes8080.CPE,
            0xED => Opcodes8080.CALL,
            0xEE => Opcodes8080.XRI,
            0xEF => Opcodes8080.RST,
            // 0xF0-0xFF
            0xF0 => Opcodes8080.RP,
            0xF1 => Opcodes8080.POP,
            0xF2 => Opcodes8080.JP,
            0xF3 => Opcodes8080.DI,
            0xF4 => Opcodes8080.CP,
            0xF5 => Opcodes8080.PUSH,
            0xF6 => Opcodes8080.ORI,
            0xF7 => Opcodes8080.RST,
            0xF8 => Opcodes8080.RM,
            0xF9 => Opcodes8080.SPHL,
            0xFA => Opcodes8080.JM,
            0xFB => Opcodes8080.EI,
            0xFC => Opcodes8080.CM,
            0xFD => Opcodes8080.CALL,
            0xFE => Opcodes8080.CPI,
            0xFF => Opcodes8080.RST,
        };
    }
}
