using System;

namespace SpaceInvadersJIT._8080
{
    /// <summary>
    /// The 8080 uses two instructions (OUT d8 & IN d8) to talk directly to up
    /// to 255 connected ports.
    ///
    /// This class allows definition of those ports in managed code where the
    /// dynamically generated CLR IL can call the In/Out functions directly.
    /// </summary>
    public class IOHandler
    {
        public void Out(byte port, byte value) =>
            // TODO
            Console.WriteLine($"OUT {port}={value}");

        public byte In(byte port)
        {
            // TODO
            Console.WriteLine($"IN {port}");
            return 0x0;
        }
    }
}
