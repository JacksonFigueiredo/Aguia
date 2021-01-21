using System;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;

namespace Aguia
{
    class Quadros
    {
        private SolidColorBrush BrushDeCorSolida;
        private WindowRenderTarget Dispositivo;

        // Color
        public Color CorInimigo = new Color(255, 0, 0, 200),
            CorInimigoVisivel = new Color(255, 255, 0, 220),
            CorInimigoNoVeiculo = new Color(255, 129, 72, 200),
            CorEsqueletoInimigo = new Color(245, 114, 0, 255),
            CorDeAliados = new Color(0, 255, 0, 200),
            CorAliadosNoVeiculo = new Color(64, 154, 200, 255),
            CorEsqueletoAliados = new Color(46, 228, 213, 255);

        public Quadros(WindowRenderTarget dispositivo)
        {
            this.Dispositivo = dispositivo;
            this.BrushDeCorSolida = new SolidColorBrush(dispositivo, Color.White);
        }

        public void DesenhaRetangulo(int X, int Y, int W, int H, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawRectangle(new Rectangle(X, Y, W, H), BrushDeCorSolida);
        }

        public void DesenhaRetangulo(int X, int Y, int W, int H, Color color, float stroke)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawRectangle(new Rectangle(X, Y, W, H), BrushDeCorSolida, stroke);
        }

        public void DesenhaPreenchimentoRetangulo(int X, int Y, int W, int H, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.FillRectangle(new RectangleF(X, Y, W, H), BrushDeCorSolida);
        }

        public void DesenhaTexto(int X, int Y, string text, Color color, TextFormat font)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawText(text, font, new RectangleF(X, Y, font.FontSize * text.Length, font.FontSize), BrushDeCorSolida);
        }

        public void DesenhaTexto(int X, int Y, string text, Color color, bool outline, TextFormat font)
        {
            if (outline)
            {
                BrushDeCorSolida.Color = Color.Black;
                Dispositivo.DrawText(text, font, new RectangleF(X + 1, Y + 1, font.FontSize * text.Length, font.FontSize), BrushDeCorSolida);
            }

            BrushDeCorSolida.Color = color;
            Dispositivo.DrawText(text, font, new RectangleF(X, Y, font.FontSize * text.Length, font.FontSize), BrushDeCorSolida);
        }

        public void DesenhaTextoAlinhamento(int X, int Y, int W, int H, string text, Color color, FontFactory fontFactory, TextFormat font, TextAlignment alignment)
        {
            BrushDeCorSolida.Color = color;
            TextLayout layout = new TextLayout(fontFactory, text, font, W, H);
            layout.TextAlignment = alignment;
            Dispositivo.DrawTextLayout(new Vector2(X, Y), layout, BrushDeCorSolida);
            layout.Dispose();
        }

        public void DesenhaTextoCentro(int X, int Y, int W, int H, string text, Color color, FontFactory fontFactory, TextFormat font)
        {
            BrushDeCorSolida.Color = color;
            TextLayout layout = new TextLayout(fontFactory, text, font, W, H);
            layout.TextAlignment = TextAlignment.Center;
            Dispositivo.DrawTextLayout(new Vector2(X, Y), layout, BrushDeCorSolida);
            layout.Dispose();
        }

        public void DesenhaTextoCentro(int X, int Y, int W, int H, string text, Color color, bool outline, FontFactory fontFactory, TextFormat font)
        {
            TextLayout layout = new TextLayout(fontFactory, text, font, W, H);
            layout.TextAlignment = TextAlignment.Center;

            if (outline)
            {
                BrushDeCorSolida.Color = Color.Black;
                Dispositivo.DrawTextLayout(new Vector2(X + 1, Y + 1), layout, BrushDeCorSolida);
            }

            BrushDeCorSolida.Color = color;
            Dispositivo.DrawTextLayout(new Vector2(X, Y), layout, BrushDeCorSolida);
            layout.Dispose();
        }

        public void DesenhaLinha(int X, int Y, int XX, int YY, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawLine(new Vector2(X, Y), new Vector2(XX, YY), BrushDeCorSolida);
        }

        public void DesenhaLinha(int X, int Y, int XX, int YY, Color color, float stroke)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawLine(new Vector2(X, Y), new Vector2(XX, YY), BrushDeCorSolida, stroke);
        }

        public void DesenhaLinha(Vector3 w2s, Vector3 _w2s, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawLine(new Vector2(w2s.X, w2s.Y), new Vector2(_w2s.X, _w2s.Y), BrushDeCorSolida);
        }

        public void DesenhaLinha(Vector3 w2s, Vector3 _w2s, Color color, float stroke)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawLine(new Vector2(w2s.X, w2s.Y), new Vector2(_w2s.X, _w2s.Y), BrushDeCorSolida, stroke);
        }

        public void DesenhaRetanguloRound(int X, int Y, int W, int H, float radius, Color color)
        {
            BrushDeCorSolida.Color = color;
            RoundedRectangle rect = new RoundedRectangle();
            rect.RadiusX = radius;
            rect.RadiusY = radius;
            rect.Rect = new RectangleF(X, Y, W, H);
            Dispositivo.DrawRoundedRectangle(rect, BrushDeCorSolida);
        }

        public void DesenhaRetanguloRound(int X, int Y, int W, int H, float radius, Color color, float stroke)
        {
            BrushDeCorSolida.Color = color;
            RoundedRectangle rect = new RoundedRectangle();
            rect.RadiusX = radius;
            rect.RadiusY = radius;
            rect.Rect = new RectangleF(X, Y, W, H);
            Dispositivo.DrawRoundedRectangle(rect, BrushDeCorSolida, stroke);
        }

        public void DesenhaPreenchimentoDeRetangulo(int X, int Y, int W, int H, float radius, Color color)
        {
            BrushDeCorSolida.Color = color;
            RoundedRectangle rect = new RoundedRectangle();
            rect.RadiusX = radius;
            rect.RadiusY = radius;
            rect.Rect = new RectangleF(X, Y, W, H);
            Dispositivo.FillRoundedRectangle(rect, BrushDeCorSolida);
        }

        public void DesenhaCirculo(int X, int Y, int W, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.DrawEllipse(new Ellipse(new Vector2(X, Y), W, W), BrushDeCorSolida);
        }

        public void DesenhaCirculoCompleto(int X, int Y, int W, Color color)
        {
            BrushDeCorSolida.Color = color;
            Dispositivo.FillEllipse(new Ellipse(new Vector2(X, Y), W, W), BrushDeCorSolida);
        }

        public void DesenhaImagem(int X, int Y, int W, int H, Bitmap bitmap)
        {
            Dispositivo.DrawBitmap(bitmap, new RectangleF(X, Y, W, H), 1.0f, BitmapInterpolationMode.Linear);
        }

        public void DesenhaImagem(int X, int Y, int W, int H, Bitmap bitmap, float angle)
        {
            Dispositivo.Transform = Matrix3x2.Rotation(angle, new Vector2(X + (H / 2), Y + (H / 2)));
            Dispositivo.DrawBitmap(bitmap, new RectangleF(X, Y, W, H), 1.0f, BitmapInterpolationMode.Linear);
            Dispositivo.Transform = Matrix3x2.Rotation(0);
        }

        public void DesenhaSprite(RectangleF destinationRectangle, Bitmap bitmap, RectangleF sourceRectangle)
        {
            Dispositivo.DrawBitmap(bitmap, destinationRectangle, 1.0f, BitmapInterpolationMode.Linear, sourceRectangle);
        }

        public void DesenhaSprite(RectangleF destinationRectangle, Bitmap bitmap, RectangleF sourceRectangle, float angle)
        {
            Vector2 center = new Vector2();
            center.X = destinationRectangle.X + destinationRectangle.Width / 2;
            center.Y = destinationRectangle.Y + destinationRectangle.Height / 2;

            Dispositivo.Transform = Matrix3x2.Rotation(angle, center);
            Dispositivo.DrawBitmap(bitmap, destinationRectangle, 1.0f, BitmapInterpolationMode.Linear, sourceRectangle);
            Dispositivo.Transform = Matrix3x2.Rotation(0);
        }

        public void DesenhaVida(int X, int Y, int W, int H, int Vida, int VidaMaxima)
        {
            if (Vida <= 0)
                Vida = 1;

            if (VidaMaxima < Vida)
                VidaMaxima = 100;

            int Progresso = (int)((float)Vida / ((float)VidaMaxima / 100));
            int w = (int)((float)W / 100 * Progresso);

            if (w <= 2)
                w = 3;

            Color color = new Color(255, 0, 0, 200);
            if (Progresso >= 20) color = new Color(255, 77, 32, 200);
            if (Progresso >= 40) color = new Color(255, 162, 5, 200);
            if (Progresso >= 60) color = new Color(252, 207, 18, 200);
            if (Progresso >= 80) color = new Color(175, 251, 39, 200);

            DesenhaPreenchimentoRetangulo(X, Y - 1, W + 1, H + 2, new Color(0, 0, 0, 100));
            DesenhaRetangulo(X, Y - 1, W + 1, H + 2, new Color(0, 0, 0, 200));
            DesenhaPreenchimentoRetangulo(X + 1, Y, w - 1, H, color);

            //DrawRoundRect(X, Y - 1, W + 1, H + 2, 2, new Color(0,0,0,200));
            //DrawFillRoundRect(X + 1, Y, w - 1, H, 2, color);
        }

        public void DesenhaProgresso(int X, int Y, int W, int H, int Valor, int ValorMaximo)
        {
            int Progresso = (int)((float)Valor / ((float)ValorMaximo / 100));
            int w = (int)((float)W / 100 * Progresso);

            Color color = new Color(0, 255, 0, 255);
            if (Progresso >= 20) color = new Color(173, 255, 47, 255);
            if (Progresso >= 40) color = new Color(255, 255, 0, 255);
            if (Progresso >= 60) color = new Color(255, 165, 0, 255);
            if (Progresso >= 80) color = new Color(255, 0, 0, 255);

            DesenhaPreenchimentoRetangulo(X, Y - 1, W + 1, H + 2, Color.Black);
            if (w >= 2)
            {
                DesenhaPreenchimentoRetangulo(X + 1, Y, w - 1, H, color);
            }
        }

        public void DesenhaOsso(Rectangle retangulo, Matrix mat, Jogador Jogador)
        {
            /*   Vector3 BONE_HEAD,
                   BONE_NECK,
                   BONE_SPINE2,
                   BONE_SPINE1,
                   BONE_SPINE,
                   BONE_LEFTSHOULDER,
                   BONE_RIGHTSHOULDER,
                   BONE_LEFTELBOWROLL,
                   BONE_RIGHTELBOWROLL,
                   BONE_LEFTHAND,
                   BONE_RIGHTHAND,
                   BONE_LEFTKNEEROLL,
                   BONE_RIGHTKNEEROLL,
                   BONE_LEFTFOOT,
                   BONE_RIGHTFOOT;

       */

            Vector3 OSSO_CABECA,
             OSSO_PESCOCO,
             OSSO_ESPINHA2,
             OSSO_ESPINHA1,
             OSSO_ESPINHA,
             OSSO_OMBRO_ESQUERDO,
             OSSO_OMBRO_DIREITO,
             OSSO_ARCO_ESQUERDO_DO_CORPO,
             OSSO_ARCO_DIREITO_DO_CORPO,
             OSSO_MAO_ESQUERDA,
             OSSO_MAO_DIREITA,
             OSSO_JOELHO_ESQUERDO,
             OSSO_JOELHO_DIREITO,
             OSSO_PE_ESQUERDO,
             OSSO_PE_DIREITO;




            if (MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_CABECA, out OSSO_CABECA) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_PESCOCO, out OSSO_PESCOCO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_ESPINHA2, out OSSO_ESPINHA2) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_ESPINHA1, out OSSO_ESPINHA1) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_ESPINHA0, out OSSO_ESPINHA) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_OMBRO_ESQUERDO, out OSSO_OMBRO_ESQUERDO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_OMBRO_DIREITO, out OSSO_OMBRO_DIREITO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_ARCO_ESQUERDO_DO_CORPO, out OSSO_ARCO_ESQUERDO_DO_CORPO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_ARCO_DIREITO_DO_CORPO, out OSSO_ARCO_DIREITO_DO_CORPO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_MAO_ESQUERDA, out OSSO_MAO_ESQUERDA) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_MAO_DIREITA, out OSSO_MAO_DIREITA) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_JOELHO_ESQUERDO, out OSSO_JOELHO_ESQUERDO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_JOELHO_DIREITO, out OSSO_JOELHO_DIREITO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_PE_ESQUERDO, out OSSO_PE_ESQUERDO) &&
                MundoParaTela(retangulo, mat, Jogador.Osso.OSSO_PE_DIREITO, out OSSO_PE_DIREITO))
            {
                int estroque = 3;
                int strokeW = estroque % 2 == 0 ? estroque / 2 : (estroque - 1) / 2;

                // Color
                Color coresqueleto = Jogador.EAmigo ? CorEsqueletoAliados : CorEsqueletoInimigo;

                // RECT's
                DesenhaPreenchimentoRetangulo((int)OSSO_CABECA.X - strokeW, (int)OSSO_CABECA.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_PESCOCO.X - strokeW, (int)OSSO_PESCOCO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_OMBRO_ESQUERDO.X - strokeW, (int)OSSO_OMBRO_ESQUERDO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_ARCO_ESQUERDO_DO_CORPO.X - strokeW, (int)OSSO_ARCO_ESQUERDO_DO_CORPO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_MAO_ESQUERDA.X - strokeW, (int)OSSO_MAO_ESQUERDA.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_OMBRO_DIREITO.X - strokeW, (int)OSSO_OMBRO_DIREITO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_ARCO_DIREITO_DO_CORPO.X - strokeW, (int)OSSO_ARCO_DIREITO_DO_CORPO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_MAO_DIREITA.X - strokeW, (int)OSSO_MAO_DIREITA.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_ESPINHA2.X - strokeW, (int)OSSO_ESPINHA2.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_ESPINHA1.X - strokeW, (int)OSSO_ESPINHA1.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_ESPINHA.X - strokeW, (int)OSSO_ESPINHA.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_JOELHO_ESQUERDO.X - strokeW, (int)OSSO_JOELHO_ESQUERDO.Y - strokeW, estroque, estroque, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_JOELHO_DIREITO.X - strokeW, (int)OSSO_JOELHO_DIREITO.Y - strokeW, 2, 2, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_PE_ESQUERDO.X - strokeW, (int)OSSO_PE_ESQUERDO.Y - strokeW, 2, 2, coresqueleto);
                DesenhaPreenchimentoRetangulo((int)OSSO_PE_DIREITO.X - strokeW, (int)OSSO_PE_DIREITO.Y - strokeW, 2, 2, coresqueleto);

                // Head -> Neck
                DesenhaLinha((int)OSSO_CABECA.X, (int)OSSO_CABECA.Y, (int)OSSO_PESCOCO.X, (int)OSSO_PESCOCO.Y, coresqueleto);

                // Neck -> Left
                DesenhaLinha((int)OSSO_PESCOCO.X, (int)OSSO_PESCOCO.Y, (int)OSSO_OMBRO_ESQUERDO.X, (int)OSSO_OMBRO_ESQUERDO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_OMBRO_ESQUERDO.X, (int)OSSO_OMBRO_ESQUERDO.Y, (int)OSSO_ARCO_ESQUERDO_DO_CORPO.X, (int)OSSO_ARCO_ESQUERDO_DO_CORPO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_ARCO_ESQUERDO_DO_CORPO.X, (int)OSSO_ARCO_ESQUERDO_DO_CORPO.Y, (int)OSSO_MAO_ESQUERDA.X, (int)OSSO_MAO_ESQUERDA.Y, coresqueleto);

                // Neck -> Right
                DesenhaLinha((int)OSSO_PESCOCO.X, (int)OSSO_PESCOCO.Y, (int)OSSO_OMBRO_DIREITO.X, (int)OSSO_OMBRO_DIREITO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_OMBRO_DIREITO.X, (int)OSSO_OMBRO_DIREITO.Y, (int)OSSO_ARCO_DIREITO_DO_CORPO.X, (int)OSSO_ARCO_DIREITO_DO_CORPO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_ARCO_DIREITO_DO_CORPO.X, (int)OSSO_ARCO_DIREITO_DO_CORPO.Y, (int)OSSO_MAO_DIREITA.X, (int)OSSO_MAO_DIREITA.Y, coresqueleto);

                // Neck -> Center
                DesenhaLinha((int)OSSO_PESCOCO.X, (int)OSSO_PESCOCO.Y, (int)OSSO_ESPINHA2.X, (int)OSSO_ESPINHA2.Y, coresqueleto);
                DesenhaLinha((int)OSSO_ESPINHA2.X, (int)OSSO_ESPINHA2.Y, (int)OSSO_ESPINHA1.X, (int)OSSO_ESPINHA1.Y, coresqueleto);
                DesenhaLinha((int)OSSO_ESPINHA1.X, (int)OSSO_ESPINHA1.Y, (int)OSSO_ESPINHA.X, (int)OSSO_ESPINHA.Y, coresqueleto);

                // Spine -> Left
                DesenhaLinha((int)OSSO_ESPINHA.X, (int)OSSO_ESPINHA.Y, (int)OSSO_JOELHO_ESQUERDO.X, (int)OSSO_JOELHO_ESQUERDO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_JOELHO_ESQUERDO.X, (int)OSSO_JOELHO_ESQUERDO.Y, (int)OSSO_PE_ESQUERDO.X, (int)OSSO_PE_ESQUERDO.Y, coresqueleto);

                // Spine -> Right
                DesenhaLinha((int)OSSO_ESPINHA.X, (int)OSSO_ESPINHA.Y, (int)OSSO_JOELHO_DIREITO.X, (int)OSSO_JOELHO_DIREITO.Y, coresqueleto);
                DesenhaLinha((int)OSSO_JOELHO_DIREITO.X, (int)OSSO_JOELHO_DIREITO.Y, (int)OSSO_PE_DIREITO.X, (int)OSSO_PE_DIREITO.Y, coresqueleto);
            }
        }

        public void DesenhaAABB(Rectangle Retangulo, Matrix mat, Jogador Jogador)
        {
            // Color
            Color color = Jogador.EAmigo ?
                CorDeAliados : Jogador.EstaVisivel ?
                CorInimigoVisivel : CorInimigo;

            float CosenoY = (float)Math.Cos(Jogador.Guinada);
            float sINY = (float)Math.Sin(Jogador.Guinada);

            OssosAlinhados AAbb = Jogador.PegarAABB();
            Vector3 FLD = new Vector3(AAbb.Minimo.Z * CosenoY - AAbb.Minimo.X * sINY, AAbb.Minimo.Y, AAbb.Minimo.X * CosenoY + AAbb.Minimo.Z * sINY) + Jogador.Posicao; // 0
            Vector3 BRT = new Vector3(AAbb.Minimo.Z * CosenoY - AAbb.Maximo.X * sINY, AAbb.Minimo.Y, AAbb.Maximo.X * CosenoY + AAbb.Minimo.Z * sINY) + Jogador.Posicao; // 1
            Vector3 BLD = new Vector3(AAbb.Maximo.Z * CosenoY - AAbb.Maximo.X * sINY, AAbb.Minimo.Y, AAbb.Maximo.X * CosenoY + AAbb.Maximo.Z * sINY) + Jogador.Posicao; // 2
            Vector3 FRT = new Vector3(AAbb.Maximo.Z * CosenoY - AAbb.Minimo.X * sINY, AAbb.Minimo.Y, AAbb.Minimo.X * CosenoY + AAbb.Maximo.Z * sINY) + Jogador.Posicao; // 3
            Vector3 FRD = new Vector3(AAbb.Maximo.Z * CosenoY - AAbb.Minimo.X * sINY, AAbb.Maximo.Y, AAbb.Minimo.X * CosenoY + AAbb.Maximo.Z * sINY) + Jogador.Posicao; // 4
            Vector3 BRB = new Vector3(AAbb.Minimo.Z * CosenoY - AAbb.Minimo.X * sINY, AAbb.Maximo.Y, AAbb.Minimo.X * CosenoY + AAbb.Minimo.Z * sINY) + Jogador.Posicao; // 5
            Vector3 BLT = new Vector3(AAbb.Minimo.Z * CosenoY - AAbb.Maximo.X * sINY, AAbb.Maximo.Y, AAbb.Maximo.X * CosenoY + AAbb.Minimo.Z * sINY) + Jogador.Posicao; // 6
            Vector3 FLT = new Vector3(AAbb.Maximo.Z * CosenoY - AAbb.Maximo.X * sINY, AAbb.Maximo.Y, AAbb.Maximo.X * CosenoY + AAbb.Maximo.Z * sINY) + Jogador.Posicao; // 7

            #region WorldToScreen
            if (!MundoParaTela(Retangulo, mat, FLD, out FLD) || !MundoParaTela(Retangulo, mat, BRT, out BRT)
                || !MundoParaTela(Retangulo, mat, BLD, out BLD) || !MundoParaTela(Retangulo, mat, FRT, out FRT)
                || !MundoParaTela(Retangulo, mat, FRD, out FRD) || !MundoParaTela(Retangulo, mat, BRB, out BRB)
                || !MundoParaTela(Retangulo, mat, BLT, out BLT) || !MundoParaTela(Retangulo, mat, FLT, out FLT))
                return;
            #endregion

            #region DrawLines
            DesenhaLinha(FLD, BRT, color);
            DesenhaLinha(BRB, BLT, color);
            DesenhaLinha(FLD, BRB, color);
            DesenhaLinha(BRT, BLT, color);

            DesenhaLinha(FRT, BLD, color);
            DesenhaLinha(FRD, FLT, color);
            DesenhaLinha(FRT, FRD, color);
            DesenhaLinha(BLD, FLT, color);

            DesenhaLinha(FRT, FLD, color);
            DesenhaLinha(FRD, BRB, color);
            DesenhaLinha(BRT, BLD, color);
            DesenhaLinha(BLT, FLT, color);
            #endregion
        }

        public void DesenhaVAABB(Rectangle Retangulo, Matrix mat, Jogador Jogador)
        {
            Color color = Jogador.EAmigo ? CorAliadosNoVeiculo : CorInimigoNoVeiculo;

            OssosAlinhados aabb = Jogador.AABBVeiculo;
            Vector3 m_Position = new Vector3(Jogador.TransformacaoVeiculo.M41, Jogador.TransformacaoVeiculo.M42, Jogador.TransformacaoVeiculo.M43);
            Vector3 FLD = Multiplicador(new Vector3(aabb.Minimo.X, aabb.Minimo.Y, aabb.Minimo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 BRT = Multiplicador(new Vector3(aabb.Maximo.X, aabb.Maximo.Y, aabb.Maximo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 BLD = Multiplicador(new Vector3(aabb.Minimo.X, aabb.Minimo.Y, aabb.Maximo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 FRT = Multiplicador(new Vector3(aabb.Maximo.X, aabb.Maximo.Y, aabb.Minimo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 FRD = Multiplicador(new Vector3(aabb.Maximo.X, aabb.Minimo.Y, aabb.Minimo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 BRB = Multiplicador(new Vector3(aabb.Maximo.X, aabb.Minimo.Y, aabb.Maximo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 BLT = Multiplicador(new Vector3(aabb.Minimo.X, aabb.Maximo.Y, aabb.Maximo.Z), Jogador.TransformacaoVeiculo) + m_Position;
            Vector3 FLT = Multiplicador(new Vector3(aabb.Minimo.X, aabb.Maximo.Y, aabb.Minimo.Z), Jogador.TransformacaoVeiculo) + m_Position;

            #region WorldToScreen
            if (!MundoParaTela(Retangulo, mat, FLD, out FLD) || !MundoParaTela(Retangulo, mat, BRT, out BRT)
                || !MundoParaTela(Retangulo, mat, BLD, out BLD) || !MundoParaTela(Retangulo, mat, FRT, out FRT)
                || !MundoParaTela(Retangulo, mat, FRD, out FRD) || !MundoParaTela(Retangulo, mat, BRB, out BRB)
                || !MundoParaTela(Retangulo, mat, BLT, out BLT) || !MundoParaTela(Retangulo, mat, FLT, out FLT))
                return;
            #endregion

            #region DrawLines
            DesenhaLinha(FLD, FLT, color);
            DesenhaLinha(FLT, FRT, color);
            DesenhaLinha(FRT, FRD, color);
            DesenhaLinha(FRD, FLD, color);
            DesenhaLinha(BLD, BLT, color);
            DesenhaLinha(BLT, BRT, color);
            DesenhaLinha(BRT, BRB, color);
            DesenhaLinha(BRB, BLD, color);
            DesenhaLinha(FLD, BLD, color);
            DesenhaLinha(FRD, BRB, color);
            DesenhaLinha(FLT, BLT, color);
            DesenhaLinha(FRT, BRT, color);
            #endregion
        }

        private PathGeometry Geometria;
        private GeometrySink sinkdegeometria;

        public void DesenhaTriangulo(Triangle Triangulo, Factory padraodefabrica, Color cor)
        {
            Geometria = new PathGeometry(padraodefabrica);
            sinkdegeometria = Geometria.Open();
            sinkdegeometria.BeginFigure(Triangulo.Point1, new FigureBegin());
            sinkdegeometria.AddLines(new SharpDX.Vector2[] { Triangulo.Point1, Triangulo.Point2, Triangulo.Point3 });
            sinkdegeometria.EndFigure(new FigureEnd());
            sinkdegeometria.Close();

            BrushDeCorSolida.Color = cor;
            Dispositivo.DrawGeometry(Geometria, BrushDeCorSolida);
            Dispositivo.FillGeometry(Geometria, BrushDeCorSolida);
            Geometria.Dispose();
            sinkdegeometria.Dispose();
        }

        public Vector3 Multiplicador(Vector3 vector, Matrix mat)
        {
            return new Vector3(mat.M11 * vector.X + mat.M21 * vector.Y + mat.M31 * vector.Z,
                                   mat.M12 * vector.X + mat.M22 * vector.Y + mat.M32 * vector.Z,
                                   mat.M13 * vector.X + mat.M23 * vector.Y + mat.M33 * vector.Z);
        }

        public Vector3 Multiplica(Vector4 vector, Matrix mat)
        {
            return new Vector3(mat.M11 * vector.X + mat.M21 * vector.Y + mat.M31 * vector.Z,
                                   mat.M12 * vector.X + mat.M22 * vector.Y + mat.M32 * vector.Z,
                                   mat.M13 * vector.X + mat.M23 * vector.Y + mat.M33 * vector.Z);
        }

        public bool MundoParaTela(Rectangle Retangulo, Matrix verprojecao, Vector3 _Mundo, out Vector3 _Tela)
        {
            _Tela = new Vector3(0, 0, 0);
            float TelaW = (verprojecao.M14 * _Mundo.X) + (verprojecao.M24 * _Mundo.Y) + (verprojecao.M34 * _Mundo.Z + verprojecao.M44);

            if (TelaW < 0.0001f)
                return false;

            float TelaX = (verprojecao.M11 * _Mundo.X) + (verprojecao.M21 * _Mundo.Y) + (verprojecao.M31 * _Mundo.Z + verprojecao.M41);
            float TelaY = (verprojecao.M12 * _Mundo.X) + (verprojecao.M22 * _Mundo.Y) + (verprojecao.M32 * _Mundo.Z + verprojecao.M42);

            _Tela.X = (Retangulo.Width / 2) + (Retangulo.Width / 2) * TelaX / TelaW;
            _Tela.Y = (Retangulo.Height / 2) - (Retangulo.Height / 2) * TelaY / TelaW;
            _Tela.Z = TelaW;
            return true;
        }

        public void DesenhaAvisoADM(Bitmap Foto, int X, int Y, Color cor, TextFormat fonte, string nome)
        {
            int Width = 400;
            int Height = 120;

            // Рисовка фона
            //DrawRect(X - 1, Y - 1, Width + 1, Height + 1, new Color(80, 80, 80, 255));
            //DrawFillRect(X, Y, Width, Height, new Color(51, 51, 51, 255));

            DesenhaPreenchimentoDeRetangulo(X, Y, Width, Height, 3, new Color(100, 100, 100, 125));
            DesenhaRetanguloRound(X, Y, Width, Height, 3, new Color(0, 0, 0, 200));

            DesenhaSprite(new RectangleF(X + 5, Y, 60, 60), Foto, new RectangleF(0, 30, 60, 60));
            DesenhaTexto(X + 65, Y + 10, "Atenção: " , cor, true, fonte);
            
            DesenhaTexto(X + 65, Y + 30, "Spectador : " + nome + " no servidor "  , cor, true, fonte);
        }

        public Bitmap CarregaBitmap(System.Drawing.Bitmap drawingBitmap)
        {
            Bitmap Resultado = null;

            //Lock the gdi resource
            System.Drawing.Imaging.BitmapData drawingBitmapData = drawingBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, drawingBitmap.Width, drawingBitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            //Prepare loading the image from gdi resource
            DataStream StreamDeDados = new DataStream(
                drawingBitmapData.Scan0,
                drawingBitmapData.Stride * drawingBitmapData.Height,
                true, false);
            BitmapProperties Propriedades = new BitmapProperties();
            Propriedades.PixelFormat = new PixelFormat(
                Format.B8G8R8A8_UNorm,
                AlphaMode.Premultiplied);

            //Load the image from the gdi resource
            Resultado = new Bitmap(
                Dispositivo,
                new Size2(drawingBitmap.Width, drawingBitmap.Height),
                StreamDeDados, drawingBitmapData.Stride,
                Propriedades);

            //Unlock the gdi resource
            drawingBitmap.UnlockBits(drawingBitmapData);

            return Resultado;
        }
    }
}
