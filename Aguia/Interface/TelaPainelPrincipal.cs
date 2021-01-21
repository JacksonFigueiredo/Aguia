using System;
using System.Collections.Generic;

namespace Aguia
{
    class TelaPainelPrincipal
    {
        public bool NoFoco = false;

        private const int IDInvalida = -1;
        private int IDItemSelecionado = IDInvalida;

        public List<Objeto> Items = new List<Objeto>();

        public void Atualiza()
        {
            if (!NoFoco)
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    if (Items[i].EVisivel)
                    {
                        NoFoco = Items[i].PreDesenho();
                        IDItemSelecionado = i;

                        if (NoFoco)
                            break;
                    }
                }
            }
            else
            {
                if (IDItemSelecionado != IDInvalida)
                {
                    Objeto ob = Items[IDItemSelecionado];
                    if (ob.EVisivel)
                    {
                        NoFoco = ob.PreDesenho();
                    }
                    else
                    {
                        NoFoco = false;
                    }
                }
            }
        }

        public void Desenha(Quadros canvas)
        {
            foreach (Objeto Objetos in Items)
            {
                if (Objetos.EVisivel)
                {
                    Objetos.Desenhar(canvas);
                }
            }
        }
    }
}
