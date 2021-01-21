using System;
using System.Diagnostics;

namespace Aguia
{
    class Teclado
    {
        public static int WH_KEYBOARD_LL = 13;
        public static int WM_KEYDOWN = 0x0100;
        public static IntPtr _IDHook = IntPtr.Zero;

        private static IntPtr DefinirHook(Gerenciavel.LowLevelKeyboardProc proc)
        {
            using (Process ProcessoAtual = Process.GetCurrentProcess())
            using (ProcessModule ModuloAtual = ProcessoAtual.MainModule)
            {
                return Gerenciavel.SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    Gerenciavel.GetModuleHandle(ModuloAtual.ModuleName), 0);
            }
        }

        public static void FazAlca(Gerenciavel.LowLevelKeyboardProc _proc)
        {
            _IDHook = DefinirHook(_proc);
        }

        public static void DesfazAlca()
        {
            Gerenciavel.UnhookWindowsHookEx(_IDHook);
        }
    }
}
