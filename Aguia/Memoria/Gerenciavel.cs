using System;
using System.Runtime.InteropServices;

namespace Aguia
{
    class Gerenciavel
    {
        // WINDOW FLAGS
        public static uint WS_MAXIMIZA = 0x1000000;
        public static uint WS_TITULO_JANELA = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        public static uint WS_BORDA = 0x800000;
        public static uint WS_VISIVEL = 0x10000000;
        public static int GWL_ESTILO = (-16);

        // READ FLAGS
        public static uint PROCESSO_VM_LEITURA = 0x0010;
        public static uint PROCESSO_VM_ESCRITA = 0x0020;
        public static uint PROCESSO_VM_OPERACAO = 0x0008;
        public static uint PAGINACAO_LEITURAESCRITA = 0x0004;



        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out DadosTela lpRect);


        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out DadosTela lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);



        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        [DllImport("User32.dll")]
        public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);



        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwAccess, bool inherit, int pid);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);


        private const int KEY_PRESSED = 0x8000;
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Ponto lpPoint);

   
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        public static extern long WriteProcessMemory1(long hProcess, long lpBaseAddress, ref long lpBuffer, long nSize, ref long lpNumberOfBytesWritten);

        public delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        public struct DadosTela
        {
            public int Esquerdo;        // x position of upper-left corner
            public int Topo;         // y position of upper-left corner
            public int Direito;       // x position of lower-right corner
            public int Inferior;      // y position of lower-right corner
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Ponto
        {
            public int X;
            public int Y;
        }

        [Flags]
        private enum EstadosDasTeclas
        {
            Nenhum = 0,
            Segurado = 1,
            Pressionado = 2
        }

        public static bool TeclaDigitada(int Tecla)
        {
            return Convert.ToBoolean(GetKeyState(Tecla) & KEY_PRESSED);
        }


        private static EstadosDasTeclas GetKeyState(System.Windows.Forms.Keys key)
        {
            EstadosDasTeclas Estado = EstadosDasTeclas.Nenhum;

            short retValor = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retValor & 0x8000) == 0x8000)
                Estado |= EstadosDasTeclas.Segurado;

            //If the low-order bit is 1, the key is toggled.
            if ((retValor & 1) == 1)
                Estado |= EstadosDasTeclas.Pressionado;

            return Estado;
        }

        public static bool TeclaFoiPressionada(System.Windows.Forms.Keys key)
        {
            return EstadosDasTeclas.Segurado == (GetKeyState(key) & EstadosDasTeclas.Segurado);
        }
    }
}
