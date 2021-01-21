using System;
using SharpDX;

namespace Aguia
{
    class Jogador
    {
        // Names
        public string Nome;
        public string NomeVeiculo;

        // Flags
        public bool DentroDoVeiculo;
        public bool EMotorista;
        public bool EEspectador;
        public bool EAmigo;
        public bool InSight;

        // Weapon Data
        public int Municao, MunicaoPente;
        public Vector2 EstabilizacaoSWAY;
        public Vector3 PosicaoDeDeslocamento;
        public float GravidadeBala;
        public Vector3 VelocidadeBala;

        // Base
        public Vector3 Posicao;
        public Vector3 VelocidadeVeiculo;
        public Vector3 VetorAdiantamento;
        public Boneco Osso;
        public Vector2 CampoDeVisao;
        public int Time;
        public EstanciaJogador Estancia;
        public int EstaOcluido;

        public float Guinada;
        public float Distancia;

        // Soldier
        public float Vida;
        public float VidaMaxima;

        // Vehicle
        public float VidaVeiculo;
        public float VidaMaximaVeiculo;
        public OssosAlinhados AABBVeiculo;
        public Matrix TransformaCaoMesh;
        public bool EMeshTransformavel = false;
        public Matrix TransformacaoVeiculo;

        public bool EPermitido
        {
            get
            {
                return Vida > 0.1f && Vida <= 100 && !Posicao.IsZero;
            }
        }
        public bool EPermitidoMirar = false;

        public bool EstaVisivel
        {
            get
            {
                return (EstaOcluido == 0);
            }
        }

        public OssosAlinhados PegarAABB()
        {
            OssosAlinhados VARAABB = new OssosAlinhados();
            switch (Estancia)
            {
                case EstanciaJogador.DePe:
                    VARAABB.Minimo = new Vector4(-0.350000f, 0.000000f, -0.350000f, 0);
                    VARAABB.Maximo = new Vector4(0.350000f, 1.700000f, 0.350000f, 0);
                    break;
                case EstanciaJogador.Abaixado:
                    VARAABB.Minimo = new Vector4(-0.350000f, 0.000000f, -0.350000f, 0);
                    VARAABB.Maximo = new Vector4(0.350000f, 1.150000f, 0.350000f, 0);
                    break;
                case EstanciaJogador.Deitado:
                    VARAABB.Minimo = new Vector4(-0.350000f, 0.000000f, -0.350000f, 0);
                    VARAABB.Maximo = new Vector4(0.350000f, 0.400000f, 0.350000f, 0);
                    break;
            }
            return VARAABB;
        }

        public Int64 AtualizaEntidade(Int64 PonteiroEntidade)
        {
            Int64 ControlavelAnexado = Memoria.Ler<Int64>(PonteiroEntidade + ClassesDoJogo.JogadorCliente.ControlavelAnexado);
            if (Memoria.SeEValido(ControlavelAnexado))
            {
                Int64 PClienteSoldado = Memoria.Ler<Int64>(Memoria.Ler<Int64>(PonteiroEntidade + ClassesDoJogo.JogadorCliente.ClientSoldierEntity)) - sizeof(Int64);
                if (Memoria.SeEValido(PClienteSoldado))
                {
                    DentroDoVeiculo = true;

                    // Velocity
                    VelocidadeVeiculo = Memoria.Ler<Vector3>(ControlavelAnexado + ClassesDoJogo.EntidadeDeVeiculoDoCliente.velocidade);

                    // Driver
                    if (Memoria.Ler<Int32>(PonteiroEntidade + ClassesDoJogo.JogadorCliente.IdDeEntradaAnexada) == (int)TipoDeAssentoVeiculo.Motorista)
                    {
                        // Vehicle AABB
                        Int64 EntidadeDeFisicaDinamica = Memoria.Ler<Int64>(ControlavelAnexado + ClassesDoJogo.EntidadeDeVeiculoDoCliente.entidadedefisica);
                        if (Memoria.SeEValido(EntidadeDeFisicaDinamica))
                        {
                            Int64 EntidadeDeFisica = Memoria.Ler<Int64>(EntidadeDeFisicaDinamica + ClassesDoJogo.EntidadeDinamicaDeFisica.transformacaodeentidade);
                            TransformacaoVeiculo = Memoria.Ler<Matrix>(EntidadeDeFisica + ClassesDoJogo.TransformacaoDeEntidadeDeFisica.Transformacao);
                            AABBVeiculo = Memoria.Ler<OssosAlinhados>(ControlavelAnexado + ClassesDoJogo.EntidadeDeVeiculoDoCliente.aabbfilho);
                        }
                        Int64 _DadosDeEntidade = Memoria.Ler<Int64>(ControlavelAnexado + ClassesDoJogo.EntidadeDoClienteSoldado.Dados);
                        if (Memoria.SeEValido(_DadosDeEntidade))
                        {
                            Int64 _NomeSID = Memoria.Ler<Int64>(_DadosDeEntidade + ClassesDoJogo.EntidadeDeDadosDeVeiculo.nomesid);

                            string NomeStrng = Memoria.LerString(_NomeSID, 20);
                            if (NomeStrng.Length > 11)
                            {
                                Int64 ClienteAnexado = Memoria.Ler<Int64>(PClienteSoldado + ClassesDoJogo.EntidadeDoClienteSoldado.Jogador);
                                // AttachedControllable Max Health
                                Int64 p = Memoria.Ler<Int64>(ClienteAnexado + ClassesDoJogo.JogadorCliente.ControlavelAnexado);
                                Int64 p2 = Memoria.Ler<Int64>(p + ClassesDoJogo.EntidadeDoClienteSoldado.ComponenteDeVida);
                                VidaVeiculo = Memoria.Ler<float>(p2 + ClassesDoJogo.ComponenteDeSaude.VidaMaximaDoVeiculo);

                                // AttachedControllable Health
                                VidaMaximaVeiculo = Memoria.Ler<float>(_DadosDeEntidade + ClassesDoJogo.EntidadeDeDadosDeVeiculo.VidaMaximaFrontal);

                                // AttachedControllable Name
                                NomeVeiculo = NomeStrng.Remove(0, 11);
                                EMotorista = true;
                            }
                        }
                    }
                }
                return PClienteSoldado;
            }
            return Memoria.Ler<Int64>(PonteiroEntidade + ClassesDoJogo.JogadorCliente.ControleControlavel);
        }

        public bool AtualizaOssos(Int64 pEnemySoldier)
        {
            Int64 ComponenteRagdoll = Memoria.Ler<Int64>(pEnemySoldier + ClassesDoJogo.EntidadeDoClienteSoldado.ComponenteRagDoll);
            if (!Memoria.SeEValido(ComponenteRagdoll))
                return false;

            byte TransformacoesValidas = Memoria.Ler<byte>(ComponenteRagdoll + (ClassesDoJogo.ComponenteRagDollSoldado.transformadoresragdoll + ClassesDoJogo.AtualizacaoDaEstanciaDoSoldado.TransformacoesValidas));
            if (TransformacoesValidas != 1)
                return false;

            Int64 TransformadorQuat = Memoria.Ler<Int64>(ComponenteRagdoll + (ClassesDoJogo.ComponenteRagDollSoldado.transformadoresragdoll + ClassesDoJogo.AtualizacaoDaEstanciaDoSoldado.TransformacaoDoMundoAtivo));
            if (!Memoria.SeEValido(TransformadorQuat))
                return false;

            Osso.OSSO_CABECA =           Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_CABECA * 0x20);
            Osso.OSSO_ARCO_ESQUERDO_DO_CORPO =  Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_ARCO_ESQUERDO_DO_CORPO * 0x20);
            Osso.OSSO_PE_ESQUERDO =       Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_PE_ESQUERDO * 0x20);
            Osso.OSSO_MAO_ESQUERDA =       Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_MAO_ESQUERDA * 0x20);
            Osso.OSSO_JOELHO_ESQUERDO =   Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_JOELHO_ESQUERDO * 0x20);
            Osso.OSSO_OMBRO_ESQUERDO =   Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_OMBRO_ESQUERDO * 0x20);
            Osso.OSSO_PESCOCO =           Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_PESCOCO * 0x20);
            Osso.OSSO_ARCO_DIREITO_DO_CORPO = Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_ARCO_DIREITO_DO_CORPO * 0x20);
            Osso.OSSO_PE_DIREITO =      Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_PE_DIREITO * 0x20);
            Osso.OSSO_MAO_DIREITA =      Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_MAO_DIREITA * 0x20);
            Osso.OSSO_JOELHO_DIREITO =  Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_JOELHO_DIREITO * 0x20);
            Osso.OSSO_OMBRO_DIREITO =  Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_OMBRO_DIREITO * 0x20);
            Osso.OSSO_ESPINHA0 =          Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_ESPINHA0 * 0x20);
            Osso.OSSO_ESPINHA1 =         Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_ESPINHA1 * 0x20);
            Osso.OSSO_ESPINHA2 =         Memoria.Ler<Vector3>(TransformadorQuat + (int)Ossos.OSSO_ESPINHA2 * 0x20);
            return true;
        }

        public bool MundoParaTela(Rectangle RetanguloTela, Matrix  VerProjecao, out Vector3 _Tela)
        {
            _Tela = new Vector3(0, 0, 0);
            float HeadHeight = Posicao.Y;

            #region HeadHeight
            if (Estancia == EstanciaJogador.DePe)
            {
                HeadHeight += 1.7f;
            }
            if (Estancia == EstanciaJogador.Abaixado)
            {
                HeadHeight += 1.15f;
            }
            if (Estancia == EstanciaJogador.Deitado)
            {
                HeadHeight += 0.4f;
            }
            #endregion

            float TelaW = (VerProjecao.M14 * Posicao.X) + (VerProjecao.M24 * HeadHeight) + (VerProjecao.M34 * Posicao.Z + VerProjecao.M44);

            if (TelaW < 0.0001f)
                return false;

            float TelaX = (VerProjecao.M11 * Posicao.X) + (VerProjecao.M21 * HeadHeight) + (VerProjecao.M31 * Posicao.Z + VerProjecao.M41);
            float TelaY = (VerProjecao.M12 * Posicao.X) + (VerProjecao.M22 * HeadHeight) + (VerProjecao.M32 * Posicao.Z + VerProjecao.M42);

            _Tela.X = (RetanguloTela.Width / 2) + (RetanguloTela.Width / 2) * TelaX / TelaW;
            _Tela.Y = (RetanguloTela.Height / 2) - (RetanguloTela.Height / 2) * TelaY / TelaW;
            _Tela.Z = TelaW;
            return true;
        }      
    }
}
