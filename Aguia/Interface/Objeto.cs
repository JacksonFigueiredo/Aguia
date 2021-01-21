using System;
using SharpDX;

namespace Aguia
{
    class Objeto
    {
        // Позиция X
        public int X { get; set; }
        // Позиция Y
        public int Y { get; set; }
        // Ширина
        public int W { get; set; }
        // Высота
        public int H { get; set; }
        // Центр
        public int CentroX { get { return X + W / 2; } }
        public int CenterY { get { return Y + H / 2; } }

        // Текущее сотояние
        private EstadosObjeto Estado, PreEstado;

        // Состояния
        public bool EstaPassandoPorCima { get { return Estado == EstadosObjeto.PassarPorCima; } }
        public bool EstaArrastando { get { return Estado == EstadosObjeto.Arrastar; } }

        // Флаги
        public bool EClicavel = false;
        public bool EArrastavel = false;
        public bool EVisivel = true;

        // Цвет фона
        public Color CorFundoTopoMenu { get; set; }
        public Color CorDeFundoPassar { get; set; }
        public Color CordeFundoClicar { get; set; }
        public Color CordeFundoArrastar { get; set; }

        // События
        public delegate void GUIHandler(Objeto ob);
        public event GUIHandler ItemPassavel;
        public event GUIHandler ItemClicavel;
        public event GUIHandler ItemDestacavel;

        // Mouse Hover
        protected void NoItemPassavel()
        {
            if (ItemPassavel != null)
            {
                ItemPassavel(this);
            }
        }

        // Mouse Leave
        protected void NoItemDestacavel()
        {
            if (ItemDestacavel != null)
            {
                ItemDestacavel(this);
            }
        }

        // Mouse Click
        protected void NoItemClicavel()
        {
            if (ItemClicavel != null)
            {
                ItemClicavel(this);
            }
        }

        // Конструктор
        public Objeto(int X, int Y, int Width, int Height)
            : this(X, Y, Width, Height, Color.White, Color.Green, Color.White, Color.White)
        {
            // Ets
        }

        public Objeto(int X, int Y, int Width, int Height, Color BackgroungColor,
            Color BackgroungColorHover, Color BackgroungColorClick, Color BackgroungColorDrag)
        {
            this.X = X;
            this.Y = Y;
            this.W = Width;
            this.H = Height;
            this.Estado = EstadosObjeto.Nenhum;
            this.PreEstado = EstadosObjeto.Nenhum;
            this.CorFundoTopoMenu = BackgroungColor;
            this.CorDeFundoPassar = BackgroungColorHover;
            this.CordeFundoClicar = BackgroungColorClick;
            this.CordeFundoArrastar = BackgroungColorDrag;
        }

        private long UltimoTemploDeClick = 0;
        private Point PosicaoDeClick;

        public bool PreDesenho()
        {
            // Проверка
            if (!EClicavel)
                return false;

            // Событие перетаскивания
            if (Estado == EstadosObjeto.Arrastar && AssistenciaMouse.EClicado() && EArrastavel)
            {
                X = AssistenciaMouse.X - AssistenciaMouse.TelaX - PosicaoDeClick.X;
                Y = AssistenciaMouse.Y - AssistenciaMouse.TelaY - PosicaoDeClick.Y;
            }
            else
            {
                PreEstado = Estado;
                if (Colisao(PontoParaTela(AssistenciaMouse.TelaX, AssistenciaMouse.TelaY), new Vector2(AssistenciaMouse.X, AssistenciaMouse.Y)))
                {
                    if (AssistenciaMouse.EClicado())
                    {
                        PreEstado = Estado;
                        Estado = EstadosObjeto.Clicar;

                        if (PreEstado == EstadosObjeto.Clicar)
                        {
                            if (Environment.TickCount - UltimoTemploDeClick >= 500 /*ms*/)
                            {
                                UltimoTemploDeClick = 0;
                                PreEstado = Estado;
                                Estado = EstadosObjeto.Arrastar;
                                PosicaoDeClick = new Point(AssistenciaMouse.X - AssistenciaMouse.TelaX - X,
                                    AssistenciaMouse.Y - AssistenciaMouse.TelaY - Y);
                            }
                        }
                        else
                        {
                            UltimoTemploDeClick = Environment.TickCount;
                            // Сообщаем о клике
                            NoItemClicavel();
                        }
                    }
                    else
                    {
                        PreEstado = Estado;
                        Estado = EstadosObjeto.PassarPorCima;

                        if (PreEstado != EstadosObjeto.PassarPorCima)
                        {
                            // Сообщаем о наведении
                            NoItemPassavel();
                        }
                    }
                }
                else
                {
                    PreEstado = Estado;
                    Estado = EstadosObjeto.Nenhum;

                    if (PreEstado == EstadosObjeto.PassarPorCima)
                    {
                        // Сообщаем о удалении
                        NoItemDestacavel();
                    }
                }
            }
            // Сообщяем что объект получил фокус
            return Estado != EstadosObjeto.Nenhum;
        }

        // Цвет фона
        public Color GetColor()
        {
            switch (Estado)
            {
                case EstadosObjeto.Clicar:
                    return CordeFundoClicar;
                case EstadosObjeto.Arrastar:
                    return CordeFundoArrastar;
                case EstadosObjeto.PassarPorCima:
                    return CorDeFundoPassar;
                default:
                    return CorFundoTopoMenu;
            }
        }

        // Отрисовка
        public virtual void Desenhar(Quadros canvas)
        {
            //PreDraw();
            canvas.DesenhaPreenchimentoRetangulo(X, Y, W, H, GetColor());
            //canvas.DrawFillRoundRect(X, Y, Width, Height, 2, GetColor());
        }

        public bool Colisao(Objeto B)
        {
            return Colisao(this, B);
        }

        public Vector2 PontoParaTela(int TelaX, int TelaY)
        {
            return new Vector2(AssistenciaMouse.TelaX + X, AssistenciaMouse.TelaY + Y);
        }

        public bool Colisao(Vector2 A, Vector2 B)
        {
            float MaximoX = A.X + W;
            float MaximoY = A.Y + H;
            return MaximoX >= B.X && B.X >= A.X && MaximoY >= B.Y && B.Y >= A.Y;
        }

        public bool Colisao(Objeto A, Objeto B)
        {
            float W = 0.5f * (A.W + B.W);
            float H = 0.5f * (A.H + B.H);
            float DX = A.CentroX - B.CentroX;
            float DY = A.CenterY - B.CenterY;
            return Math.Abs(DX) <= W && Math.Abs(DY) <= H;
        }
    }
}
