using System;

namespace Aguia
{
    class EfeitoConsole
    {
        int Contador;
        public EfeitoConsole()
        {
            Contador = 0;
        }
        public void Girar()
        {
            Contador++;
            switch (Contador % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            Console.CursorVisible = false;
        }
    }
}
