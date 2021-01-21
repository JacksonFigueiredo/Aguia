using System;
using SharpDX;
using SharpDX.DirectWrite;
using System.Collections.Generic;

namespace Aguia
{
    class TelaRadar : Objeto
    {
        // Текст
        public string Texto = "Radar";

        // Цвет текста
        public Color CorDoTexto { get; set; }

        // Шрифр
        public TextFormat Formato { get; set; }
        private Factory FormatoPadrao { get; set; }

        // Отступ
        public AreaDePreenchimento Preenchimento = new AreaDePreenchimento();

        // Флаг
        public bool EDesenhaTexto = false;

        // Увеличение растояния
        public int Escala = 1;
        private Veiculos Veiculos = null;

        public TelaRadar(int X, int Y, int W, int H)
            : base(X, Y, W, H)
        {
            EClicavel = true;
            EArrastavel = true;

            Veiculos = new Veiculos();

            CorFundoTopoMenu = new Color(0, 0, 0, 125);
            CorDeFundoPassar = new Color(0, 0, 0, 150);
            CordeFundoClicar = new Color(0, 0, 0, 150);
            CordeFundoArrastar = new Color(0, 0, 0, 175);
        }

        public TelaRadar(int X, int Y, int Width, int Height, Factory FormatFactory, Color TextColor)
            : base(X, Y, Width, Height)
        {
            EClicavel = true;
            EArrastavel = true;
            EDesenhaTexto = true;

            Veiculos = new Veiculos();

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

        public override void Desenhar(Quadros canvas)
        {
            // Отрисовка фона и обработка нажатий
            base.Desenhar(canvas);

            // Текущий цвет
            Color CorDeAcao = GetColor();

            // Обводка
            canvas.DesenhaRetangulo(X - 1, Y - 1, W + 2, H + W + 1, CorDeAcao, 2);
            //canvas.DrawRoundRect(X, Y + Height, Width, Width, 2, GetColor());

            // Фон радара
            canvas.DesenhaPreenchimentoRetangulo(X, Y + H, W, W, new Color(100, 100, 100, 125));
            //canvas.DrawFillRoundRect(X, Y + Height, Width, Width, 2, GetColor());

            // Заговолок
            if(EDesenhaTexto) 
            {
                canvas.DesenhaTexto(X + Preenchimento.Esquerda, Y + Preenchimento.Topo, Texto, CorDoTexto, Formato);
            }
        }

        private double GrausParaRadianos(double Angulo)
        {
            return Math.PI * Angulo / 180.0;
        }

        private double RadianoParaGraus(double Angulo)
        {
            return Angulo * (180.0 / Math.PI);
        }

        public void DesenhaEntidades(Jogador EntidadeLocal, List<Jogador> Entidades, SharpDX.Direct2D1.Factory factory, SharpDX.Direct2D1.Bitmap Icons, Quadros canvas)
        {
            if (!EVisivel)
                return;

            // Текущий цвет
            Color actionColor = GetColor();

            #region Draw FOV
            float CampoDeVisaoY = EntidadeLocal.CampoDeVisao.Y;
            CampoDeVisaoY = CampoDeVisaoY / 1.34f;
            CampoDeVisaoY -= (float)Math.PI / 2;

            float RadarCentroX = CentroX;
            float RadarCentroY = Y + (H + W / 2);

            int Ponto =
                (int)Math.Sqrt((RadarCentroX - RadarCentroX)
                * (RadarCentroX - RadarCentroX) + (RadarCentroY - RadarCentroY - W / 2)
                * (RadarCentroY - RadarCentroY - W / 2));

            int fov_x = (int)(Ponto * (float)Math.Cos(CampoDeVisaoY) + RadarCentroX);
            int fov_y = (int)(Ponto * (float)Math.Sin(CampoDeVisaoY) + RadarCentroY);
            CampoDeVisaoY = CampoDeVisaoY + (float)Math.PI;
            int fov_x1 = (int)(Ponto * (float)Math.Cos(-CampoDeVisaoY) + RadarCentroX);
            int fov_y1 = (int)(Ponto * (float)Math.Sin(-CampoDeVisaoY) + RadarCentroY);

            SharpDX.Direct2D1.Triangle Triangulo = new SharpDX.Direct2D1.Triangle();
            Triangulo.Point1 = new Vector2(RadarCentroX, Y + H + W / 2);
            Triangulo.Point2 = new Vector2(fov_x, Y + H + 1);
            Triangulo.Point3 = new Vector2(fov_x1, Y + H + 1);

            canvas.DesenhaTriangulo(Triangulo, factory, new Color(255,255,255, 50));
            #endregion

            // Перекрестие
            canvas.DesenhaLinha(CentroX, Y + H, CentroX, Y + H + W, actionColor, 2);
            canvas.DesenhaLinha(X, Y + H + W / 2, X + W, Y + H + W / 2, actionColor, 2);

            foreach(Jogador mEntidades in Entidades)
            {
                float DiferencaZ = EntidadeLocal.Posicao.Z - mEntidades.Posicao.Z;
                float DiferencaX = EntidadeLocal.Posicao.X - mEntidades.Posicao.X;

                float PontoX = DiferencaX * (float)Math.Cos(-EntidadeLocal.Guinada) - DiferencaZ * (float)Math.Sin(-EntidadeLocal.Guinada);
                float PontoY = DiferencaX * (float)Math.Sin(-EntidadeLocal.Guinada) + DiferencaZ * (float)Math.Cos(-EntidadeLocal.Guinada);

                PontoX *= Escala;
                PontoY *= Escala;

                PontoX += CentroX;
                PontoY += (Y + H + W / 2);

                // Check Max Distance
                if (Math.Abs(PontoX - CentroX) >= W / 2 - 15 || Math.Abs(PontoY - (Y + H + W / 2)) >= W / 2 - 15)
                    continue;

                // Save Normal Position
                Vector2 Posicao = new Vector2(PontoX, PontoY);

                // Get Forvard Position
                Vector3 PosicaoAvancada = mEntidades.Posicao + mEntidades.VetorAdiantamento * 10;

                DiferencaZ = EntidadeLocal.Posicao.Z - PosicaoAvancada.Z;
                DiferencaX = EntidadeLocal.Posicao.X - PosicaoAvancada.X;

                PontoX = DiferencaX * (float)Math.Cos(-EntidadeLocal.Guinada) - DiferencaZ * (float)Math.Sin(-EntidadeLocal.Guinada);
                PontoY = DiferencaX * (float)Math.Sin(-EntidadeLocal.Guinada) + DiferencaZ * (float)Math.Cos(-EntidadeLocal.Guinada);

                PontoX *= Escala;
                PontoY *= Escala;

                PontoX += CentroX;
                PontoY += (Y + H + W / 2);

                Vector2 VetorDeDiferenca = new Vector2(PontoX - Posicao.X, PontoY - Posicao.Y);
                VetorDeDiferenca = Vector2.Normalize(VetorDeDiferenca);
                //Vector2.Normalize(differenceVector);

                double AnguloParaRotacionar = Math.Atan2(0, 1) - Math.Atan2(VetorDeDiferenca.X, VetorDeDiferenca.Y);
                AnguloParaRotacionar = RadianoParaGraus(AnguloParaRotacionar);
                AnguloParaRotacionar = 180 + AnguloParaRotacionar;

                if (mEntidades.DentroDoVeiculo)
                {
                    if (mEntidades.EMotorista)
                    {
                        // Vehicle Id
                        int vID = Veiculos.getVehicleId(mEntidades.NomeVeiculo);
                        // Draw Vehilce
                        canvas.DesenhaSprite(new RectangleF(Posicao.X - 15, Posicao.Y - 15, 30, 30), 
                            Icons,
                            new RectangleF(vID * 30, mEntidades.EAmigo ? 30 : 60, 30, 30), 
                            (float)GrausParaRadianos(AnguloParaRotacionar));
                    }
                }
                else
                {
                    // Draw Player
                    canvas.DesenhaSprite(new RectangleF(Posicao.X - 15, Posicao.Y - 15, 30, 30), 
                        Icons, 
                        new RectangleF(mEntidades.EAmigo ? 30 : 60, 0, 30, 30), 
                        (float)GrausParaRadianos(AnguloParaRotacionar));
                }
            }  
          
            // Draw Local
            canvas.DesenhaSprite(new RectangleF(CentroX - 15, (Y + H + W / 2) - 15, 30, 30), Icons, new RectangleF(0, 0, 30, 30));
        }
    }
}
