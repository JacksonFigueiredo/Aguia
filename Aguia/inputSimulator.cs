using System;
using System.Runtime.InteropServices;

internal class SimuladorDeEntrada
{
    private struct EntradaDeMouse
    {
        public int dx;

        public int dy;

        public uint mouseData;

        public uint dwFlags;

        public uint time;

        public IntPtr dwExtraInfo;
    }

    private struct EntradaDeTeclado
    {
        public ushort wVk;

        public ushort wScan;

        public uint dwFlags;

        public uint time;

        public IntPtr dwExtraInfo;
    }

    private struct EntradaDeHardware
    {
        public int uMsg;

        public short wParamL;

        public short wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct MouseKeybdHardwareInputUnion
    {
        [FieldOffset(0)]
        public SimuladorDeEntrada.EntradaDeMouse mi;

        [FieldOffset(0)]
        public SimuladorDeEntrada.EntradaDeTeclado ki;

        [FieldOffset(0)]
        public SimuladorDeEntrada.EntradaDeHardware hi;
    }

    private struct INPUT
    {
        public uint type;

        public SimuladorDeEntrada.MouseKeybdHardwareInputUnion mkhi;
    }

    [Flags]
    public enum InputType
    {
        MOUSE = 0,
        KEYBOARD = 1,
        HARDWARE = 2
    }

    [Flags]
    public enum MOUSEEVENTF
    {
        MOVE = 1,
        LEFTDOWN = 2,
        LEFTUP = 4,
        RIGHTDOWN = 8,
        RIGHTUP = 16,
        MIDDLEDOWN = 32,
        MIDDLEUP = 64,
        XDOWN = 128,
        XUP = 256,
        WHEEL = 2048,
        MOVE_NOCOALESCE = 8192,
        VIRTUALDESK = 16384,
        ABSOLUTE = 32768
    }

    [Flags]
    public enum KEYEVENTF
    {
        KEYDOWN = 0,
        EXTENDEDKEY = 1,
        KEYUP = 2,
        UNICODE = 4,
        SCANCODE = 8
    }

    [DllImport("user32.dll")]
    private static extern uint VkKeyScanA(uint c);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKeyA(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] SimuladorDeEntrada.INPUT[] pInputs, int cbSize);

    public static void KeyPress(TeclasVirtuais key)
    {
        SimuladorDeEntrada.KeyDown(key);
        SimuladorDeEntrada.KeyUp(key);
    }

    public static void KeyDown(TeclasVirtuais key)
    {
        SimuladorDeEntrada.INPUT[] array = new SimuladorDeEntrada.INPUT[1];
        array[0].type = 1u;
        array[0].mkhi.ki.wVk = 0;
        array[0].mkhi.ki.wScan = (ushort)SimuladorDeEntrada.MapVirtualKeyA((uint)key, 0u);
        array[0].mkhi.ki.dwFlags = 8u;
        array[0].mkhi.ki.time = 0u;
        array[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;
        SimuladorDeEntrada.SendInput(1u, array, Marshal.SizeOf(typeof(SimuladorDeEntrada.INPUT)));
    }

    public static void KeyUp(TeclasVirtuais key)
    {
        SimuladorDeEntrada.INPUT[] array = new SimuladorDeEntrada.INPUT[1];
        array[0].type = 1u;
        array[0].mkhi.ki.wVk = 0;
        array[0].mkhi.ki.wScan = (ushort)SimuladorDeEntrada.MapVirtualKeyA((uint)key, 0u);
        array[0].mkhi.ki.dwFlags = 10u;
        array[0].mkhi.ki.time = 0u;
        array[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;
        SimuladorDeEntrada.SendInput(1u, array, Marshal.SizeOf(typeof(SimuladorDeEntrada.INPUT)));
    }
}