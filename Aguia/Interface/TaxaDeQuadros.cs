using System;
namespace Aguia
{
    class TaxaDeQuadros
    { 
        // FPS Stats
        private static int ultimapiscada;
        private static int ultimataxadequadro;
       private static int taxadequadros;

        // Update FPS
        public static int CalculateFrameRate()
        {
           if (Environment.TickCount - ultimapiscada >= 1000)
            {
              ultimataxadequadro = taxadequadros;
              taxadequadros = 0;
              ultimapiscada = Environment.TickCount;
           }
           taxadequadros++;
           if (ultimataxadequadro != 0)
          {
                   return ultimataxadequadro;
              
            }
          else
          {
                return 1;
           }
        }

        // Get FPS
        public static int FPS
        {
            get 
            {
                return ultimataxadequadro == 0 ? taxadequadros : ultimataxadequadro;
            }
        }
    }
}
