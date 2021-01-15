using System;

namespace SpaceInvadersJIT
{
    internal enum SpaceInvadersKey
    {
        Credit,
        P1Fire,
        P1Left,
        P1Right,
        P1Start,
        P2Fire,
        P2Left,
        P2Right,
        P2Start
    }

    internal static class SpaceInvadersKeyExtensions
    {
        internal static int PortIndex(this SpaceInvadersKey key) => key switch
        {
            SpaceInvadersKey.Credit => 0,
            SpaceInvadersKey.P1Fire => 0,
            SpaceInvadersKey.P1Left => 0,
            SpaceInvadersKey.P1Right => 0,
            SpaceInvadersKey.P1Start => 0,
            SpaceInvadersKey.P2Fire => 1,
            SpaceInvadersKey.P2Left => 1,
            SpaceInvadersKey.P2Right => 1,
            SpaceInvadersKey.P2Start => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };
        
        internal static byte KeyDownMask(this SpaceInvadersKey key) => key switch
        {
            SpaceInvadersKey.Credit => 0b0000_0001,
            SpaceInvadersKey.P1Fire => 0b0001_0000,
            SpaceInvadersKey.P1Left => 0b0010_0000,
            SpaceInvadersKey.P1Right => 0b0100_0000,
            SpaceInvadersKey.P1Start => 0b0000_0100,
            SpaceInvadersKey.P2Fire => 0b0001_0000,
            SpaceInvadersKey.P2Left => 0b0010_0000,
            SpaceInvadersKey.P2Right => 0b0100_0000,
            SpaceInvadersKey.P2Start => 0b0000_0010,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

        internal static byte KeyUpMask(this SpaceInvadersKey key) => (byte) ~key.KeyDownMask();
    }
}