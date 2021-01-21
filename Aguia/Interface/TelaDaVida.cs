using System;
using SharpDX;
using SharpDX.DirectWrite;
using System.Collections.Generic;

namespace Aguia
{
    class TelaVida : Objeto
    {
        // Текст
        public string Texto = "Munição / Vida";

        // Цвет текста
        public Color CorDoTexto { get; set; }

        // Шрифр
        public TextFormat Formato { get; set; }
        public TextFormat FormatoGrande { get; set; }
        private Factory FormatoPadrao { get; set; }

        // Отступ
        public AreaDePreenchimento Preenchimento = new AreaDePreenchimento();

        // Флаг
        public bool EdesenhaTexto = false;

        // Параметры
        public int Vida = 0;
        public int VidaMaxima = 100;

        public int Municao = 0;
        public int MunicaoNoPente = 0;

        // Имя оружия
        public string Arma = string.Empty;

        public TelaVida(int X, int Y, int Width, int Height, Factory FortadoDoTexto, Color CorDoTexto)
            : base(X, Y, Width, Height)
        {
            this.EClicavel = true;
            this.EArrastavel = true;
            this.EdesenhaTexto = true;
            this.CorDoTexto = CorDoTexto;
            this.FormatoPadrao = FortadoDoTexto;

            CorFundoTopoMenu = new Color(0, 0, 0, 125);
            CorDeFundoPassar = new Color(0, 0, 0, 150);
            CordeFundoClicar = new Color(0, 0, 0, 150);
            CordeFundoArrastar = new Color(0, 0, 0, 175);

            this.Formato = new TextFormat(FortadoDoTexto, "Calibri", 14);
            this.FormatoGrande = new TextFormat(FortadoDoTexto, "Calibri", 24);

            Preenchimento.Esquerda = 5;
            Preenchimento.Topo = 3;
        }

        public override void Desenhar(Quadros Quadros)
        {
            // Отрисовка фона и обработка нажатий
            base.Desenhar(Quadros);

            // Текущий цвет
            Color CorDeAcao = GetColor();

            // Обводка
            Quadros.DesenhaRetangulo(X - 1, Y - 1, W + 2, H + 65 + 1, CorDeAcao, 2);

            // Фон
            Quadros.DesenhaPreenchimentoRetangulo(X, Y + H, W, 65, new Color(100, 100, 100, 125));

            // Полоска
            Quadros.DesenhaVida(X + 5, Y + H + 55, W - 10, 5, Vida, VidaMaxima);

            // Заговолок
            if(EdesenhaTexto) 
            {
                Quadros.DesenhaTexto(X + Preenchimento.Esquerda, Y + Preenchimento.Topo, Texto, CorDoTexto, Formato);

                string StringMunicao = String.Format("{0}/{1}", Municao, MunicaoNoPente);
                if (Municao < 0)
                    StringMunicao = "-/-";

                //if (Ammo == 0 && AmmoClip != 0)
                //    ammoStr = "Reload";

                Quadros.DesenhaTexto(X + 5, Y + H + 5, StringMunicao, CorDoTexto, FormatoGrande);

                Quadros.DesenhaTextoAlinhamento(X + W - Preenchimento.Esquerda - 100, Y + H + 5, 100, 25, String.Format("+{0}", Vida), CorDoTexto, FormatoPadrao, FormatoGrande, TextAlignment.Trailing);
                //canvas.DrawText(X + Width - 55, Y + Height + 5, String.Format("+{0}", healthStr), TextColor, FormatBig);
                if (!string.IsNullOrEmpty(Arma))
                {
                    Quadros.DesenhaTextoAlinhamento(X + W - Preenchimento.Esquerda - 100, Y + Preenchimento.Topo, 100, 25, Arma, CorDoTexto, FormatoPadrao, Formato, TextAlignment.Trailing);
                }
            }
        }

    }
}
