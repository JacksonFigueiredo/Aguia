using System;
using System.Collections.Generic;

namespace Aguia
{
    class Veiculos
    {
        private const int DEFAULT_ID = 3;

        private Dictionary<string, int> Dados = new Dictionary<string, int>();

        public Veiculos()
        {
            // Jeep's
            Dados.Add("QUAD", 3);

            Dados.Add("SPM3", 3);
            Dados.Add("HMMWV", 3);
            Dados.Add("COUGAR", 3);
            Dados.Add("ZFB", 3);

            Dados.Add("GROWLER", 3);
            Dados.Add("LYT2021", 3);
            Dados.Add("DPV", 3);
            Dados.Add("VDV", 3);
            // BTR's
            Dados.Add("AME_BTR90", 4);
            Dados.Add("LAV2", 4);
            Dados.Add("BMP2", 4);
            Dados.Add("ZBD09", 4);
            Dados.Add("M3A3", 4);
            // AA
            Dados.Add("PGZ95", 5);
            Dados.Add("LAVAD", 5);
            Dados.Add("9K22", 5);
            // Boat
            Dados.Add("DV15", 6);
            Dados.Add("CB90", 6);
            Dados.Add("RHIB", 6);
            Dados.Add("DIRTBIKE", 6);

            // Artillery
            Dados.Add("HIMARS", 7);
            Dados.Add("PWC", 7);

            // Plane
            Dados.Add("AME_F35", 8);
            Dados.Add("J20", 8);
            Dados.Add("PAKFA", 8);
            Dados.Add("F22", 8);

            // Plane II
            Dados.Add("A10", 9);
            Dados.Add("SU25", 9);
            Dados.Add("Q5", 9);
            Dados.Add("FA18", 9);
            Dados.Add("SU39", 9);

            // Tank
            Dados.Add("M1ABRAMS", 10);
            Dados.Add("T90", 10);
            Dados.Add("T99", 10);
            Dados.Add("M1128", 10);
            Dados.Add("SPRUT", 10);

            // Helicopter
            Dados.Add("AH6", 11);
            Dados.Add("Z11", 11);

            // Attack Helicopter
            Dados.Add("AH1Z", 12);
            Dados.Add("Z10", 12);
            Dados.Add("AH64", 12);
            Dados.Add("MI28", 12);

            // Support Helicopter
            Dados.Add("UH1Y", 13);
            Dados.Add("VENOM", 13);
            Dados.Add("Z9", 13);
            Dados.Add("KA60", 13);

            // AC130
            Dados.Add("AC130", 14);
            Dados.Add("AC-130", 14);
        }

        public int getVehicleId(string name)
        {
            int index;
            if (!Dados.TryGetValue(name, out index))
            {
                index = DEFAULT_ID;
            }
            return index;
        }
    }
}
