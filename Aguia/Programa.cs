using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Aguia
{
    class Programa
    {


        [DllImport("user32.dll")] 
        private static extern int ShowWindow(int Handle, int showState); 
        [DllImport("kernel32.dll")] 
        public static extern int GetConsoleWindow(); 



        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.Write("Esperando");

            EfeitoConsole Gira = new EfeitoConsole();

            while (true)
            {
                int iddoprocesso;
                if (Memoria.PegaProcessoPorNome("bf4", out iddoprocesso))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    LimpaLinhaAtualDoConsole();

                    int win = GetConsoleWindow();
                    ShowWindow(win, 0);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(new string('-', Console.WindowWidth - 1));
                    Console.WriteLine("Status : Jogo Carregado{0}", new string(' ', 15));
                    Console.WriteLine("Status : ID De Processo - {0}", iddoprocesso);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(true);
                    Application.Run(new DesenhaFormulario(iddoprocesso));
                    break;
                }
                Gira.Girar();
                Thread.Sleep(100);
            }

            Console.ReadKey();
        }

        public static void LimpaLinhaAtualDoConsole()
        {
            int linhaatualdocursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);

            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write("");

            Console.SetCursorPosition(0, linhaatualdocursor);
        }
    }
}
