using System;
using SharpDX;
using SharpDX.DirectWrite;
using System.Collections.Generic;

namespace Aguia
{
    class MenuMae : Objeto
    {
        // Текст
        public string Texto = "Aguia [F1] Mostrar/Ocultar";

        // Цвет текста
        public Color CorDoTexto { get; set; }

        // Шрифр
        public TextFormat Formato { get; set; }

        private Factory FormatoPadrao { get; set; }

        // Отступ
        public AreaDePreenchimento Preenchimento = new AreaDePreenchimento();

        // Флаг
        public bool EDesenhaTexto = false;

        // Меню
        public List<ItemDoMenu> ItemsDoMenu = new List<ItemDoMenu>();

        // Переменные
        public int ItemSelecionado = 0;
        public const int PesoDoItemASerDesenhado = 25;

        public enum TipoDeMenu
        {
            Integer,
            Boolean,
            Array
        }

        public enum ItensDoMenu
        {
            ESPCAIXA,
            ESPVIDAMUNICAO,
            ESPNOME,
            ESPOSSOS,
            ESPDISTANCIA,
            AIBTON,
            AIBTLOCAL,
            AIBTVEICULO,
            AIBTLOCALVEICULO,
            RADAR,
            ESPSOMENTEENEMY,
            SEMRECUODISPER,
            SEMPRESPIRAR,
            ESPVEICULOS,
            VIDAEMUNICAO,
            DESTRAVATUDO,
            NASCERQUALQUERUM,
            AUTOESPOTAR,
            ESPUTIL,
            SUPERDISPARO,
            TRAVAVEICULO


        }

        public class ItemDoMenu
        {
            public string Nome;
            public string[] Nomes;
            public int Valor;
            public int Minimo, Maximo;

            private TipoDeMenu type;

            public ItemDoMenu(int Valor, string Nome)
            {
                this.Valor = Valor;
                this.Nome = Nome;
                this.type = TipoDeMenu.Boolean;
            }

            public ItemDoMenu(int Valor, int Minimo, int Maximo, string Nome)
            {
                this.Valor = Valor;
                this.Minimo = Minimo;
                this.Maximo = Maximo;
                this.Nome = Nome;
                this.type = TipoDeMenu.Integer;
            }

            public ItemDoMenu(int Valor, int Minimo, int Maximo, string Nome, string[] Nomes)
            {
                this.Valor = Valor;
                this.Minimo = Minimo;
                this.Maximo = Maximo;
                this.Nome = Nome;
                this.Nomes = Nomes;
                this.type = TipoDeMenu.Array;
            }

            public string ValorColuna
            {
                get
                {
                    switch (type)
                    {
                        case TipoDeMenu.Integer:
                            return Valor.ToString();
                        case TipoDeMenu.Boolean:
                            return Valor == 0 ? "Desligado" : "Ligado";
                        case TipoDeMenu.Array:
                            return Nomes[Valor];
                        default:
                            return Valor.ToString();
                    }
                }
            }

            public void MoverDireita()
            {
                switch (type)
                {
                    case TipoDeMenu.Array:
                    case TipoDeMenu.Integer:
                        if (Valor < Maximo)
                            Valor++;
                        else
                            Valor = Minimo;
                        break;
                    case TipoDeMenu.Boolean:
                        Valor = Valor == 0 ? 1 : 0;
                        break;
                }
            }

            public void MoverEsquerda()
            {
                switch (type)
                {
                    case TipoDeMenu.Array:
                    case TipoDeMenu.Integer:
                        if (Valor > Minimo)
                            Valor--;
                        else
                            Valor = Maximo;
                        break;
                    case TipoDeMenu.Boolean:
                        Valor = Valor == 0 ? 1 : 0;
                        break;
                }
            }
        }

        public void Adicionar(int Valor, string Nome)
        {
            ItemsDoMenu.Add(new ItemDoMenu(Valor, Nome));
        }

        public void Adicionar(int Valor, int Minimo, int Maximo, string Nome)
        {
            ItemsDoMenu.Add(new ItemDoMenu(Valor, Minimo, Maximo, Nome));
        }

        public void Adicionar(int Valor, int Minimo, int Maximo, string Nome, string[] Nomes)
        {
            ItemsDoMenu.Add(new ItemDoMenu(Valor, Minimo, Maximo, Nome, Nomes));
        }

        public ItemDoMenu this[ItensDoMenu item]
        {
            get
            {
                return ItemsDoMenu[(int)item];
            }
        }

        public MenuMae(int X, int Y, int Width, int Height)
            : base(X, Y, Width, Height)
        {
            EClicavel = true;
            EArrastavel = true;

            CorFundoTopoMenu = new Color(0, 0, 0, 125);
            CorDeFundoPassar = new Color(0, 0, 0, 150);
            CordeFundoClicar = new Color(0, 0, 0, 150);
            CordeFundoArrastar = new Color(0, 0, 0, 175);
        }

        public MenuMae(int X, int Y, int Width, int Height, Factory FormatFactory, Color TextColor)
            : base(X, Y, Width, Height)
        {
            EClicavel = true;
            EArrastavel = true;
            EDesenhaTexto = true;

            this.Formato = new TextFormat(FormatFactory, "Calibri", 14);
            this.CorDoTexto = TextColor;
            this.FormatoPadrao = FormatFactory;

            CorFundoTopoMenu = new Color(0, 0, 0, 125);
            CorDeFundoPassar = new Color(0, 0, 0, 150);
            CordeFundoClicar = new Color(0, 0, 0, 150);
            CordeFundoArrastar = new Color(0, 0, 0, 175);

            Preenchimento.Esquerda = 5;
            Preenchimento.Topo = 3;
        }

        public void MoveAcima()
        {
            if (!EVisivel)
                return;

            if (ItemSelecionado > 0)
            {
                ItemSelecionado--;
            }
            else
            {
                ItemSelecionado = ItemsDoMenu.Count - 1;
            }
        }

        public void MoveAbaixo()
        {
            if (!EVisivel)
                return;

            if (ItemSelecionado < ItemsDoMenu.Count - 1)
            {
                ItemSelecionado++;
            }
            else
            {
                ItemSelecionado = 0;
            }
        }

        public void MoveADireita()
        {
            if (!EVisivel)
                return;
            ItemsDoMenu[ItemSelecionado].MoverDireita();
            Memoria.Salvaconfig(ItemsDoMenu[ItemSelecionado].Nome, "1");
        }

        public void MoveAEsquerda()
        {
            if (!EVisivel)
                return;
            ItemsDoMenu[ItemSelecionado].MoverEsquerda();
            Memoria.Salvaconfig(ItemsDoMenu[ItemSelecionado].Nome, "0");
        }

        public override void Desenhar(Quadros Quadros)
        {
            // Отрисовка фона и обработка нажатий
            base.Desenhar(Quadros);

            // Текущий цвет
            Color CorDeAcao = GetColor();

            int Largura2 = ItemsDoMenu.Count * PesoDoItemASerDesenhado;

            // Обводка
            Quadros.DesenhaRetangulo(X - 1, Y - 1, W + 2, H + Largura2 + 1, CorDeAcao, 2);

            // Фон
            Quadros.DesenhaPreenchimentoRetangulo(X, Y + H, W, Largura2, new Color(0, 0, 0, 125)); // cor do fundo do menu principal

            // Заговолок
            if (EDesenhaTexto)
            {
                Quadros.DesenhaTexto(X + Preenchimento.Esquerda, Y + Preenchimento.Topo, Texto, CorDoTexto, Formato);
            }

            // Отрисовка элементов
            for (int i = 0; i < ItemsDoMenu.Count; i++)
            {
                if (i == ItemSelecionado)
                {
                    Quadros.DesenhaPreenchimentoRetangulo(X, (Y + H) + i * PesoDoItemASerDesenhado, W, PesoDoItemASerDesenhado, new Color(0, 122, 204));
                }
                Quadros.DesenhaLinha(X,
                    (Y + H) + i * PesoDoItemASerDesenhado + PesoDoItemASerDesenhado,
                    X + W,
                    (Y + H) + i * PesoDoItemASerDesenhado + PesoDoItemASerDesenhado,
                    CorDeAcao);

                // Name
                Quadros.DesenhaTexto(X + Preenchimento.Esquerda,
                    (Y + H) + i * PesoDoItemASerDesenhado + Preenchimento.Topo, ItemsDoMenu[i].Nome,
                    CorDoTexto,
                    Formato);

                // Value
                Quadros.DesenhaTextoAlinhamento(X + W - Preenchimento.Esquerda - 100,
                    (Y + H) + i * PesoDoItemASerDesenhado + Preenchimento.Topo,
                    100, 25,
                    ItemsDoMenu[i].ValorColuna,
                    CorDoTexto,
                    FormatoPadrao,
                    Formato,
                    TextAlignment.Trailing);
            }
        }
    }
}
