using System;
using System.Runtime.InteropServices;

namespace Aguia
{
    class AssistenciaMouse
    {
        // Позиция X
        public static int X { get; private set; }
        // Позиция Y
        public static int Y { get; private set; }

        // Позиция X
        public static int TelaX { get; set; }
        // Позиция Y
        public static int TelaY { get; set; }

        public static bool EClicado()
        {
            return Convert.ToBoolean(Gerenciavel.GetKeyState(0x01) & 0x8000);
        }

        public static void Atualizar()
        {
            Gerenciavel.Ponto pPonto;
            Gerenciavel.GetCursorPos(out pPonto);

            X = pPonto.X;
            Y = pPonto.Y;
        }
    }
}
