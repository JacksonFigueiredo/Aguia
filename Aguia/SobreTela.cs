using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;

using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Runtime.InteropServices;

namespace Aguia
{
    public partial class DesenhaFormulario : Form
    {
        // Thread
        private Thread FluxoDeAtualizacao = null, FluxodeJanela = null;
        // ProcessId
        private int IDDoProcesso = Memoria.IDDeProcessoInvalida;

        // SharpDX
        private WindowRenderTarget Dispositivo;
        private HwndRenderTargetProperties PropriedadesRenderizacao;
        private Factory padrao;
        private Quadros tela;
        private bool BoolRedimensionavel = false;
        private bool BoolMinimizado = true;
        public int SlotAtivoGlobal;
        public bool DesenhaSomenteInimigo;
        public float DistanciaSuaAteOInimigo;


        public float Precisao;
        public float Disparostotais;
        public float Disparosacertados;
        public string NomeEspectador;

        // Font
        private TextFormat fonte, fontepequena;
        private FontFactory fontepadrao;
        private const string familiadafonte = "Calibri";
        private const float tamanhodafonte = 18.0f;
        private const float tamanhodafontepequena = 14.0f;

        // Screen Rect
        private Rectangle retangulo;
        // Handle
        private IntPtr alca;

        // UI
        private static TelaPainelPrincipal painel;

        // Sprite
        private Bitmap icones;

        // Keybord
        private Gerenciavel.LowLevelKeyboardProc _proc = ChamadaDeHook;

        // Game Data
        private List<Jogador> jogadores = null;
        private Jogador jogadorlocal = null;
        private Matrix verprojecao, vermatrixinversa;
        private int contagemdeespectadores = 0;

        // UI Color
        private Color cordainterface = new Color(66, 255, 25, 255);
        private Color cordainterface2 = new Color(66, 255, 25, 255);

        public DesenhaFormulario(int IDDoProcesso)

        {
            int estiloinicial = Gerenciavel.GetWindowLong(Handle, -20);
            Gerenciavel.SetWindowLong(Handle, -20, estiloinicial | 0x80000 | 0x20);

            IntPtr HWND_TOPMOST = new IntPtr(-1);
            const UInt32 SWP_NOSIZE = 0x0001;
            const UInt32 SWP_NOMOVE = 0x0002;
            const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

            Gerenciavel.SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            OnResize(null);

            // Update ProcessID
            this.IDDoProcesso = IDDoProcesso;
            this.alca = Handle;

            //Init def.
            InitializeComponent();
        }



        protected override void OnResize(EventArgs e)
        {
            int[] margins = new int[] { 0, 0, retangulo.Width, retangulo.Height };
            Gerenciavel.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        }

        // Open
        private void DrawForm_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            //this.Visible = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.Width = rect.Width;
            //this.Height = rect.Height;

            padrao = new Factory();
            fontepadrao = new FontFactory();

            // Render settings
            PropriedadesRenderizacao = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(Width, Height),
                PresentOptions = PresentOptions.None
            };

            // Init device
            Dispositivo = new WindowRenderTarget(padrao, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), PropriedadesRenderizacao);
            tela = new Quadros(Dispositivo);

            // Init Sprite
            icones = tela.CarregaBitmap(Resource1.Icons);

            // Init font's
            fonte = new TextFormat(fontepadrao, familiadafonte, tamanhodafonte);
            fontepequena = new TextFormat(fontepadrao, familiadafonte, tamanhodafontepequena);

            // Open process
            Memoria.AbreProcesso(IDDoProcesso);

            // Init player array
            jogadores = new List<Jogador>();
            jogadorlocal = new Jogador();

            painel = new TelaPainelPrincipal();
            TelaRadar radardeinformacoes = new TelaRadar(10, 480, 250, 25, fontepadrao, cordainterface);
            //radar.Padding.Left = 5;
            //radar.Padding.Top = 3;
            painel.Items.Add(radardeinformacoes);
            //panel.Items.Add(new GUIRadar(10, 35, 250, 25));
            painel.Items.Add(new TelaVida(10, 380, 250, 25, fontepadrao, cordainterface));


            MenuMae menuinicial = new MenuMae(10, 35, 250, 25, fontepadrao, cordainterface);


            menuinicial.Adicionar(Memoria.checaconfig("EXTRASENSORIAL"), "EXTRASENSORIAL");
            menuinicial.Adicionar(Memoria.checaconfig("BARRA DE VIDA"), "BARRA DE VIDA");
            menuinicial.Adicionar(Memoria.checaconfig("NOME INIMIGO"), "NOME INIMIGO");
            menuinicial.Adicionar(Memoria.checaconfig("OSSOS"), "OSSOS");
            menuinicial.Adicionar(Memoria.checaconfig("DISTANCIA"), "DISTANCIA");
            menuinicial.Adicionar(Memoria.checaconfig("M ELETRONICA"), "M ELETRONICA");
            menuinicial.Adicionar(2, 0, 4, "MODO DE TRAVA", new string[] { "Cabeça", "Pescoço", "Espinha 2", "Espinha 1", "Espinha 0" });
            menuinicial.Adicionar(Memoria.checaconfig("M ELETRONICA VEICULO"), "M ELETRONICA VEICULO");
            menuinicial.Adicionar(0, 0, 1, "LOCAL VEICULO", new string[] { "AABB Center", "Driver" });
            menuinicial.Adicionar(Memoria.checaconfig("RADAR"), "RADAR");
            menuinicial.Adicionar(Memoria.checaconfig("ESP SOMENTE INIMIGOS"), "ESP SOMENTE INIMIGOS");
            menuinicial.Adicionar(Memoria.checaconfig("SEM RECUO / DISPER"), "SEM RECUO / DISPER");
            menuinicial.Adicionar(Memoria.checaconfig("SEM RESPIRAÇÃO"), "SEM RESPIRAÇÃO");
            menuinicial.Adicionar(Memoria.checaconfig("ESP VEICULO"), "ESP VEICULO");
            menuinicial.Adicionar(Memoria.checaconfig("MINHA VIDA / MUN"), "MINHA VIDA / MUN");
            menuinicial.Adicionar(Memoria.checaconfig("DESBLOQUEAR TUDO"), "DESBLOQUEAR TUDO");
            menuinicial.Adicionar(Memoria.checaconfig("NASCER EM QUALQUER UM"), "NASCER EM QUALQUER UM");
            menuinicial.Adicionar(Memoria.checaconfig("AUTO ESPOTAR"), "AUTO ESPOTAR");
            menuinicial.Adicionar(Memoria.checaconfig("ESP UTIL"), "ESP UTIL");
            menuinicial.Adicionar(Memoria.checaconfig("SUPER DISPARO"), "SUPER DISPARO");
            menuinicial.Adicionar(Memoria.checaconfig("TRAVA VEICULO"), "TRAVA VEICULO");

            painel.Items.Add(menuinicial);

            // Init draw thread
            FluxoDeAtualizacao = new Thread(new ParameterizedThreadStart(Atualiza));
            FluxoDeAtualizacao.Start();

            // Init window thread (resize / position)
            FluxodeJanela = new Thread(new ParameterizedThreadStart(DefinirJanela));
            FluxodeJanela.Start();

            // Hook Keys
            Teclado.FazAlca(_proc);
        }

        // Close
        private void DrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Finish Thread's
            FluxoDeAtualizacao.Abort();
            FluxodeJanela.Abort();
            // Close Process
            Memoria.FechaProcesso();
            // UnHook
            Teclado.DesfazAlca();
            // Close main process
            Environment.Exit(0);
        }

        // Update (With V-SYNC)


        public void atualizashotstat()
        {

            var DisparosStat = Memoria.LerInt64(Enderecos.ENDERECO_ESTATISTICASDETIRO);
            if (Memoria.SeEValido(DisparosStat))
            {
                Disparostotais = Memoria.LerInt32(DisparosStat + 0x4C);
                Disparosacertados = Memoria.LerInt32(DisparosStat + 0x54);
                Precisao = 0.00f;
                if (Disparostotais > 0 && Disparosacertados > 0)
                {
                    Precisao = (float)Math.Round((float)(Disparosacertados / Disparostotais * 100), 2);
                }
            }
            
        }




        private void Atualiza(object sender)
        {
            while (Memoria.ProcessoEstaRodando("bf4"))
            {
                // Resize
                if (BoolRedimensionavel)
                {
                    Dispositivo.Resize(new Size2(retangulo.Width, retangulo.Height));

                    BoolRedimensionavel = false;
                }

                AssistenciaMouse.TelaX = Left;
                AssistenciaMouse.TelaY = Top;
                AssistenciaMouse.Atualizar();

                // Begin Draw
                Dispositivo.BeginDraw();
                Dispositivo.Clear(new Color4(0.0f, 0.0f, 0.0f, 0.0f));

                // Check Window State
                if (!BoolMinimizado)
                {
                    // Get Menu
                    MenuMae menuopcoes = (MenuMae)painel.Items[2];





                    // Read & Draw Players
                    LerEntidades();

                    // AIM Bot
                    if (menuopcoes[Aguia.MenuMae.ItensDoMenu.AIBTON].Valor == 1)
                    {
                        FacaAMira();
                    }


                    if (menuopcoes[Aguia.MenuMae.ItensDoMenu.ESPUTIL].Valor == 1)
                    {

/*
                        Int64 pPrimeiroExplosivo = Memoria.LerInt64(Enderecos.OFFSET_CLIENTEXPLOSIONENTITY + 0x60);
                        Vector3 PosicaoExplosivo;
                        Vector3 PosicaoExplosivoNaTela;
                        while (Memoria.SeEValido(pPrimeiroExplosivo))
                        {
                            PosicaoExplosivo = Memoria.Ler<Vector3>(pPrimeiroExplosivo + 0x0250);
                            MundoParaTela(PosicaoExplosivo, out PosicaoExplosivoNaTela);
                            tela.DesenhaTexto((int)PosicaoExplosivoNaTela.X, (int)PosicaoExplosivoNaTela.Y, "Explosivo", Color.Red, true, fontepequena);
                            pPrimeiroExplosivo = Memoria.LerInt64(pPrimeiroExplosivo);

                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        Int64 pPrimeiraGranada = Memoria.LerInt64(Enderecos.OFFSET_CLIENTGRENADEENTITY + 0x60);
                        Vector3 PosicaoGranada;
                        Vector3 PosicaoGranadaNaTela;
                        while (Memoria.SeEValido(pPrimeiraGranada))
                        {

                            PosicaoGranada = Memoria.Ler<Vector3>(pPrimeiraGranada + 0x0250);
                            MundoParaTela(PosicaoGranada, out PosicaoGranadaNaTela);
                            tela.DesenhaTexto((int)PosicaoGranadaNaTela.X, (int)PosicaoGranadaNaTela.Y, "Granada", Color.Red, true, fontepequena);
                            pPrimeiraGranada = Memoria.LerInt64(pPrimeiraGranada);

                        }

    */



                    }



                    if (menuopcoes[Aguia.MenuMae.ItensDoMenu.SUPERDISPARO].Valor == 1)
                    {
                        Int64 PGameContext = Memoria.Ler<Int64>(ClassesDoJogo.ContextoDoJogoCliente.PegaInstancia());
                        Int64 pPlayerManager = Memoria.Ler<Int64>(PGameContext + ClassesDoJogo.ContextoDoJogoCliente.m_GerenciadorDoJogador);
                        Int64 pJogadorLocal = Memoria.Ler<Int64>(pPlayerManager + ClassesDoJogo.GerenciadorDoClienteDoJogo.m_JogadorLocal);
                        Int64 pSoldadoLocal = jogadorlocal.AtualizaEntidade(pJogadorLocal);
                        Int64 pComponentedeArmaDoCliente = Memoria.Ler<Int64>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.ComponentedeArmaDoSoldado);
                        Int64 pAlcaDaArma = Memoria.Ler<Int64>(pComponentedeArmaDoCliente + ClassesDoJogo.ComponenteDeVidaDoSoldado.alca);
                        Int32 EsloteAtivo = Memoria.Ler<Int32>(pComponentedeArmaDoCliente + ClassesDoJogo.ComponenteDeVidaDoSoldado.slotativo);
                        Int64 pSoldierWeapon = Memoria.Ler<Int64>(pAlcaDaArma + EsloteAtivo * sizeof(Int64));
                        var pWeaponFiring = Memoria.Ler<Int64>(pSoldierWeapon + 0x49C0);
                        Int64 pPrimaryFire = Memoria.LerInt64(pWeaponFiring + 0x0128);
                        Int64 pFiringFunctionData = Memoria.LerInt64(pPrimaryFire + 0x0010);


                        if (SlotAtivoGlobal < 1)
                        {
                            if (Memoria.LerFloat(pFiringFunctionData + 0x168 + 0x60) != 1200.000f)
                            {
                                Memoria.EscreveFlutuante(pFiringFunctionData + 0x0168 + 0x5C, 0.01f); // m_TriggerPullWeight --> 0.15 FAMAS
                                Memoria.EscreveFlutuante(pFiringFunctionData + 0x0168 + 0x60, 1200.000f); // m_RateOfFire
                                Memoria.EscreveFlutuante(pFiringFunctionData + 0x0168 + 0x64, 1200.000f); // m_RateOfFireForBurst
                                Memoria.EscreveFlutuante(pFiringFunctionData + 0x0168 + 0x68, 0.001f); //m_DelayBetweenBursts                                                                                                 

                            }


                        }

                    }


                    // UI
                    //if (c.GetType() == typeof(TForm))
                    painel.Atualiza();
                    painel.Desenha(tela);

                    TelaRadar radarinicial = (TelaRadar)painel.Items[0];
                    radarinicial.EVisivel = menuopcoes[Aguia.MenuMae.ItensDoMenu.RADAR].Valor == 1;
                    radarinicial.DesenhaEntidades(jogadorlocal, jogadores, padrao, icones, tela);

                    TelaVida vida = (TelaVida)painel.Items[1];
                    vida.EVisivel = menuopcoes[Aguia.MenuMae.ItensDoMenu.VIDAEMUNICAO].Valor == 1;

                    vida.Municao = jogadorlocal.Municao;
                    vida.MunicaoNoPente = jogadorlocal.MunicaoPente;
                    vida.Vida = (int)jogadorlocal.Vida;
                    vida.VidaMaxima = (int)jogadorlocal.VidaMaxima;
                    vida.Arma = jogadorlocal.NomeVeiculo;

                    atualizashotstat();


                    // Draw Spectator Count
                    //   tela.DesenhaTextoCentro(retangulo.Width / 2 - 125, retangulo.Height - (int)fonte.FontSize, 250, (int)fonte.FontSize, contagemdeespectadores + " ESPECTADORES NO SERVIDOR", new Color(255, 214, 0, 255), true, fontepadrao, fontepequena);
                    // FPS
                    tela.DesenhaTexto(10, 5, "FPS: " + TaxaDeQuadros.FPS + " Precisão Geral  : " + Precisao + "%" + "  Hora Agora : " + DateTime.Now.ToString("HH:mm:ss tt") + " Espectadores No Servidor : " + contagemdeespectadores, cordainterface2, true, fonte);

                    // Draw Spectator Warn
                    if (contagemdeespectadores > 0)
                    {
                        tela.DesenhaAvisoADM(icones, retangulo.Width / 2 - 125, 30, cordainterface, fontepequena, NomeEspectador);
                    }
                    //canvas.DrawText(300, 200, String.Format("Mesh: {0}", localPlayer.BulletSpeed), uiColor, fontSmall);
                }

                // End Draw
                Dispositivo.EndDraw();
                TaxaDeQuadros.CalculateFrameRate();
            }





            // Close Process
            Memoria.FechaProcesso();
            // UnHook
            Teclado.DesfazAlca();
            // Exit
            Environment.Exit(0);
        }

        // Read from Memorry
        private void LerEntidades()
        {
            // Reset Old Data
            jogadores.Clear();
            jogadorlocal = new Jogador();

            #region Get Local Player
            // Read Local
            Int64 PGameContext = Memoria.Ler<Int64>(ClassesDoJogo.ContextoDoJogoCliente.PegaInstancia());
            if (!Memoria.SeEValido(PGameContext))
                return;

            Int64 pPlayerManager = Memoria.Ler<Int64>(PGameContext + ClassesDoJogo.ContextoDoJogoCliente.m_GerenciadorDoJogador);
            if (!Memoria.SeEValido(pPlayerManager))
                return;

            Int64 pJogadorLocal = Memoria.Ler<Int64>(pPlayerManager + ClassesDoJogo.GerenciadorDoClienteDoJogo.m_JogadorLocal);
            if (!Memoria.SeEValido(pJogadorLocal))
                return;

            Int64 pSoldadoLocal = jogadorlocal.AtualizaEntidade(pJogadorLocal);
            if (!Memoria.SeEValido(pSoldadoLocal))
                return;

            Int64 pComponenteDeVida = Memoria.Ler<Int64>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.ComponenteDeVida);
            if (!Memoria.SeEValido(pComponenteDeVida))
                return;

            Int64 pPrediccaoDeControle = Memoria.Ler<Int64>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.ControleComPrediccao);
            if (!Memoria.SeEValido(pPrediccaoDeControle))
                return;

            Int64 m_pRenderizador = Memoria.Ler<Int64>(Enderecos.ENDERECO_GAMERENDER);
            Int64 p_pRenderizadorVisao = Memoria.Ler<Int64>(m_pRenderizador + ClassesDoJogo.RenderizadorDoJogo.RenderizaVisao);
            jogadorlocal.CampoDeVisao.X = Memoria.Ler<float>(p_pRenderizadorVisao + ClassesDoJogo.RenderizaVisao.CampoDeVisaoX);
            jogadorlocal.CampoDeVisao.Y = Memoria.Ler<float>(p_pRenderizadorVisao + ClassesDoJogo.RenderizaVisao.CampoDeVisaoY);

            // Health
            jogadorlocal.Vida = Memoria.Ler<float>(pComponenteDeVida + ClassesDoJogo.ComponenteDeSaude.Vida);
            jogadorlocal.VidaMaxima = Memoria.Ler<float>(pComponenteDeVida + ClassesDoJogo.ComponenteDeSaude.VidaMaxima);

            if (jogadorlocal.Vida <= 0.1f) // YOU DEAD :D
                return;

            jogadorlocal.Posicao = Memoria.Ler<Vector3>(pPrediccaoDeControle + ClassesDoJogo.PredicaoDoClienteSoldado.Posicao);
            if (!jogadorlocal.DentroDoVeiculo)
            {
                // Player Velocity
                jogadorlocal.VelocidadeVeiculo = Memoria.Ler<Vector3>(pPrediccaoDeControle + ClassesDoJogo.PredicaoDoClienteSoldado.Velocidade);
            }

            // Other
            jogadorlocal.Time = Memoria.Ler<Int32>(pJogadorLocal + ClassesDoJogo.JogadorCliente.IDdoTime);

           
            jogadorlocal.Estancia = (EstanciaJogador)Memoria.Ler<Int32>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.TipoDeEstanciaDoSoldado);
            jogadorlocal.Guinada = Memoria.Ler<float>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.VisaoAutorativa);
            jogadorlocal.EstaOcluido = Memoria.Ler<byte>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.EstaOcluido);

          
            if (jogadorlocal.DentroDoVeiculo)
            {
                Int64 pArmaAtualDisparando = Memoria.Ler<Int64>(Enderecos.ENDERECO_ARMA_ATUAL_ATIRANDO);
                if (Memoria.SeEValido(pArmaAtualDisparando))
                {
                
                    jogadorlocal.Municao = Memoria.Ler<Int32>(pArmaAtualDisparando + ClassesDoJogo.ArmaDisparando.ProjeteisCarregados);
                    jogadorlocal.MunicaoPente = Memoria.Ler<Int32>(pArmaAtualDisparando + ClassesDoJogo.ArmaDisparando.ProjeteisNoPente);

                    Int64 pDadosDeArmaDisparando = Memoria.Ler<Int64>(pArmaAtualDisparando + ClassesDoJogo.ArmaDisparando.FogoPrimario);
                    if (Memoria.SeEValido(pDadosDeArmaDisparando))
                    {
                        Int64 pDadosDisparo = Memoria.Ler<Int64>(pDadosDeArmaDisparando + ClassesDoJogo.FogoPrimario.ConfiguracaoDadosDisparo);
                        if (Memoria.SeEValido(pDadosDisparo))
                        {
                            Int64 pDadosDeProjetil = Memoria.Ler<Int64>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.DadosDoProjetil);
                            if (Memoria.SeEValido(pDadosDeProjetil))
                            {
                                jogadorlocal.GravidadeBala = Memoria.Ler<float>(pDadosDeProjetil + ClassesDoJogo.DadosEntidadeDeBala.Gravidade);
                                jogadorlocal.VelocidadeBala = Memoria.Ler<Vector3>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.VelocidadeInicial);
                                jogadorlocal.PosicaoDeDeslocamento = Memoria.Ler<Vector3>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.PosicaoDeDeslocamento);
                            }
                        }
                    }
                }

                // Read Vehicle Camera Matrix
                Int32 idEntrada = Memoria.Ler<Int32>(pJogadorLocal + ClassesDoJogo.JogadorCliente.IDEntradaControlavel);
                Int64 pControleControlavel = Memoria.Ler<Int64>(pJogadorLocal + ClassesDoJogo.JogadorCliente.ControleControlavel);
                Int64 ppComponentedeEntrada = Memoria.Ler<Int64>(pControleControlavel + 0x110);
                Int64 pComponenteDeEntradaCliente = Memoria.Ler<Int64>(ppComponentedeEntrada + (idEntrada * sizeof(Int64)));

                if (Memoria.SeEValido(pComponenteDeEntradaCliente))
                {
                    Int64 PEstadoComponentedeEntrada = Memoria.Ler<Int64>(pComponenteDeEntradaCliente + ClassesDoJogo.ComponenteEntradaCliente.EstadoAtual);
                    if (Memoria.SeEValido(PEstadoComponentedeEntrada))
                    {
                        Int32 IndexesDeCameras = Memoria.Ler<Int32>(PEstadoComponentedeEntrada + ClassesDoJogo.ComponenteDeEstado.Estado.IndexDeCameraAtual);
                        if (IndexesDeCameras >= 0)
                        {
                            Int64 ppComponenteDeCamera = Memoria.Ler<Int64>(pComponenteDeEntradaCliente + ClassesDoJogo.ComponenteEntradaCliente.ComponenteDeCamera);
                            if (Memoria.SeEValido(ppComponenteDeCamera))
                            {
                                Int64 ComponenteDeCameraDeCliente = Memoria.Ler<Int64>(ppComponenteDeCamera + (IndexesDeCameras * sizeof(Int64)));
                                if (Memoria.SeEValido(ComponenteDeCameraDeCliente))
                                {
                                    jogadorlocal.TransformaCaoMesh = Memoria.Ler<Matrix>(ComponenteDeCameraDeCliente + 0x0050);
                                    jogadorlocal.EMeshTransformavel = true;

                                    //Vector3 m_vehicleCrosshairForwardVector = new Vector3( localPlayer.MeshTransform.M31, localPlayer.MeshTransform.M32, localPlayer.MeshTransform.M33);
                                    //Vector3 m_vehicleCrosshairPosition = new Vector3(localPlayer.MeshTransform.M41, localPlayer.MeshTransform.M42, localPlayer.MeshTransform.M43);
                                    //m_vehicleCrosshairPosition = m_vehicleCrosshairPosition + m_vehicleCrosshairForwardVector * 100.0f;

                                    //localPlayer.WorldToScreen(rect, viewProj, m_vehicleCrosshairPosition, out m_vehicleCrosshairPosition);
                                    //canvas.DrawCircle((int)m_vehicleCrosshairPosition.X, (int)m_vehicleCrosshairPosition.Y, 5, Color.Red);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Int64 pComponentedeArmaDoCliente = Memoria.Ler<Int64>(pSoldadoLocal + ClassesDoJogo.EntidadeDoClienteSoldado.ComponentedeArmaDoSoldado);
                if (Memoria.SeEValido(pComponentedeArmaDoCliente))
                {
                    Int64 pAlcaDaArma = Memoria.Ler<Int64>(pComponentedeArmaDoCliente + ClassesDoJogo.ComponenteDeVidaDoSoldado.alca);
                    Int32 EsloteAtivo = Memoria.Ler<Int32>(pComponentedeArmaDoCliente + ClassesDoJogo.ComponenteDeVidaDoSoldado.slotativo);
                    SlotAtivoGlobal = EsloteAtivo;


                    if (Memoria.SeEValido(pAlcaDaArma))
                    {
                        Int64 pArmaDoSoldado = Memoria.Ler<Int64>(pAlcaDaArma + EsloteAtivo * sizeof(Int64));
                        if (Memoria.SeEValido(pArmaDoSoldado))
                        {
                            Int64 pSimulacaoDeMira = Memoria.Ler<Int64>(pArmaDoSoldado + ClassesDoJogo.ArmaClienteSoldado.MiraAutorativa);
                            if (Memoria.SeEValido(pSimulacaoDeMira))
                            {
                                // Sway
                                jogadorlocal.EstabilizacaoSWAY = Memoria.Ler<Vector2>(pSimulacaoDeMira + ClassesDoJogo.SimulacaoDeMiraSoldadoCliente.mswaayy);
                            }
                            Int64 pCorrecaoDeTiro = Memoria.Ler<Int64>(pArmaDoSoldado + ClassesDoJogo.ArmaClienteSoldado.primaria);
                            if (Memoria.SeEValido(pCorrecaoDeTiro))
                            {
                                // Ammo
                                jogadorlocal.Municao = Memoria.Ler<Int32>(pCorrecaoDeTiro + ClassesDoJogo.ArmaDisparando.ProjeteisCarregados);
                                jogadorlocal.MunicaoPente = Memoria.Ler<Int32>(pCorrecaoDeTiro + ClassesDoJogo.ArmaDisparando.ProjeteisNoPente);

                                Int64 PDadosDeDisparoDeArma = Memoria.Ler<Int64>(pCorrecaoDeTiro + ClassesDoJogo.ArmaDisparando.FogoPrimario);
                                if (Memoria.SeEValido(PDadosDeDisparoDeArma))
                                {
                                    Int64 pDadosDisparo = Memoria.Ler<Int64>(PDadosDeDisparoDeArma + ClassesDoJogo.FogoPrimario.ConfiguracaoDadosDisparo);
                                    if (Memoria.SeEValido(pDadosDisparo))
                                    {
                                        Int64 pDadosProjeil = Memoria.Ler<Int64>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.DadosDoProjetil);
                                        if (Memoria.SeEValido(pDadosProjeil))
                                        {
                                            jogadorlocal.GravidadeBala = Memoria.Ler<float>(pDadosProjeil + ClassesDoJogo.DadosEntidadeDeBala.Gravidade);
                                            jogadorlocal.VelocidadeBala = Memoria.Ler<Vector3>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.VelocidadeInicial);
                                            jogadorlocal.PosicaoDeDeslocamento = Memoria.Ler<Vector3>(pDadosDisparo + ClassesDoJogo.ConfiguracaoDadosDeDisparos.PosicaoDeDeslocamento);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            // Render View
            Int64 pRenderizadorJogo = Memoria.Ler<Int64>(ClassesDoJogo.RenderizadorDoJogo.PegaInstancia());
            Int64 pRenderizadorVisao = Memoria.Ler<Int64>(pRenderizadorJogo + ClassesDoJogo.RenderizadorDoJogo.RenderizaVisao);

            // Read Screen Matrix
            verprojecao = Memoria.Ler<Matrix>(pRenderizadorVisao + ClassesDoJogo.RenderizaVisao.VerProjecao);
            vermatrixinversa = Memoria.Ler<Matrix>(pRenderizadorVisao + ClassesDoJogo.RenderizaVisao.VerMatrixInversa);

            // Pointer to Players Array
            Int64 ArrayDeJogadores = Memoria.Ler<Int64>(pPlayerManager + ClassesDoJogo.GerenciadorDoClienteDoJogo.m_ppJogador);
            if (!Memoria.SeEValido(ArrayDeJogadores))
                return;

            // Reset
            contagemdeespectadores = 0;



            // Get Player by Id
            #region Get Player by Id
            for (uint i = 0; i < 70; i++)
            {
                // Create new Player
                Jogador Jogadores = new Jogador();

                // Pointer to ClientPlayer class (Player Array + (Id * Size of Pointer))
                Int64 pJogadorInimigo = Memoria.Ler<Int64>(ArrayDeJogadores + (i * sizeof(Int64)));
                if (!Memoria.SeEValido(pJogadorInimigo))
                    continue;

                if (pJogadorInimigo == pJogadorLocal)
                    continue;

                Jogadores.EEspectador = Convert.ToBoolean(Memoria.Ler<byte>(pJogadorInimigo + ClassesDoJogo.JogadorCliente.EEspectador));

                // Name
                Jogadores.Nome = Memoria.LerString2(pJogadorInimigo + ClassesDoJogo.JogadorCliente.Nome, 25);


                if (Jogadores.EEspectador)

                {
                    contagemdeespectadores++;

                    NomeEspectador = Jogadores.Nome;
                }



                // RPM.ReadInt64(pEnemyPlayer + Offsets.ClientPlayer.m_pControlledControllable);
                Int64 pSoldadoInimigo = Jogadores.AtualizaEntidade(pJogadorInimigo);
                if (!Memoria.SeEValido(pSoldadoInimigo))
                    continue;

                Int64 ComponenteDeVidaInimigo = Memoria.Ler<Int64>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.ComponenteDeVida);
                if (!Memoria.SeEValido(ComponenteDeVidaInimigo))
                    continue;

                Int64 pControlePreditivoInimigo = Memoria.Ler<Int64>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.ControleComPrediccao);
                if (!Memoria.SeEValido(pControlePreditivoInimigo))
                    continue;

                // Health
                Jogadores.Vida = Memoria.Ler<float>(ComponenteDeVidaInimigo + ClassesDoJogo.ComponenteDeSaude.Vida);
                Jogadores.VidaMaxima = Memoria.Ler<float>(ComponenteDeVidaInimigo + ClassesDoJogo.ComponenteDeSaude.VidaMaxima);

                // Dead
                if (Jogadores.Vida <= 0.1f)
                    continue;

                #region ForwardVector
                if (Jogadores.DentroDoVeiculo)
                {
                    Jogadores.VetorAdiantamento = new Vector3(Jogadores.TransformacaoVeiculo.M31, Jogadores.TransformacaoVeiculo.M32, Jogadores.TransformacaoVeiculo.M33);
                }
                else
                {
                    Int64 ComponenteDeArmaDoClienteInimigo = Memoria.Ler<Int64>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.ComponentedeArmaDoSoldado);
                    if (Memoria.SeEValido(ComponenteDeArmaDoClienteInimigo))
                    {
                        Int64 pAlcaDeArmaInimigo = Memoria.Ler<Int64>(ComponenteDeArmaDoClienteInimigo + ClassesDoJogo.ComponenteDeVidaDoSoldado.alca);
                        Int32 SlotAtivoInimigo = Memoria.Ler<Int32>(ComponenteDeArmaDoClienteInimigo + ClassesDoJogo.ComponenteDeVidaDoSoldado.slotativo);

                        if (Memoria.SeEValido(pAlcaDeArmaInimigo))
                        {
                            Int64 pArmaSoldadoInimigo = Memoria.Ler<Int64>(pAlcaDeArmaInimigo + SlotAtivoInimigo * 0x8);
                            if (Memoria.SeEValido(pArmaSoldadoInimigo))
                            {
                                Int64 pArmaClienteInimigo = Memoria.Ler<Int64>(pArmaSoldadoInimigo + ClassesDoJogo.ArmaClienteSoldado.arma);
                                if (Memoria.SeEValido(pArmaClienteInimigo))
                                {
                                    Matrix EspacoDeTiro = Memoria.Ler<Matrix>(pArmaClienteInimigo + ClassesDoJogo.ArmaCliente.EspacoDeTiro);
                                    Jogadores.VetorAdiantamento = new Vector3(EspacoDeTiro.M31, EspacoDeTiro.M32, EspacoDeTiro.M33);
                                }
                            }
                        }
                    }
                }
                #endregion

                // Origin (Position in Game X, Y, Z)
                Jogadores.Posicao = Memoria.Ler<Vector3>(pControlePreditivoInimigo + ClassesDoJogo.PredicaoDoClienteSoldado.Posicao);
                if (!Jogadores.DentroDoVeiculo)
                {
                    // Player Velocity
                    Jogadores.VelocidadeVeiculo = Memoria.Ler<Vector3>(pControlePreditivoInimigo + ClassesDoJogo.PredicaoDoClienteSoldado.Velocidade);
                }

                // Other
                Jogadores.Time = Memoria.Ler<Int32>(pJogadorInimigo + ClassesDoJogo.JogadorCliente.IDdoTime);

                if (DesenhaSomenteInimigo == true)
                {
                    if (Jogadores.Time == jogadorlocal.Time)
                        continue;
                }

                else
                      if (DesenhaSomenteInimigo == false)
                {

                }


                Jogadores.Estancia = (EstanciaJogador)Memoria.Ler<Int32>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.TipoDeEstanciaDoSoldado);
                Jogadores.Guinada = Memoria.Ler<float>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.VisaoAutorativa);
                Jogadores.EstaOcluido = Memoria.Ler<byte>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.EstaOcluido);
                Jogadores.EAmigo = (Jogadores.Time == jogadorlocal.Time);

                // Distance to You
                Jogadores.Distancia = Vector3.Distance(jogadorlocal.Posicao, Jogadores.Posicao);

                // Check
                if (Jogadores.EPermitido)
                {
                    // Get Menu
                    MenuMae MenuDeOpcoes = (MenuMae)painel.Items[2];

                    // Player Bone    
                    if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTON].Valor == 1 || MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPOSSOS].Valor == 1)
                    {
                        if (Jogadores.AtualizaOssos(pSoldadoInimigo))
                        {
                            Jogadores.EPermitidoMirar = true;
                            if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPOSSOS].Valor == 1)
                            {
                                tela.DesenhaOsso(retangulo, verprojecao, Jogadores);
                            }
                        }
                    }



                    Vector3 Pe, Cabeca;
                    if (MundoParaTela(Jogadores.Posicao, out Pe) &&
                        Jogadores.MundoParaTela(retangulo, verprojecao, out Cabeca))
                    {
                        // Distance to Center Screen
                        // player.DistanceToCrosshair = Vector2.Distance(new Vector2(0, 0), new Vector2(0, 0));

                        float CabecaParaPe = Pe.Y - Cabeca.Y;
                        float TamanhoDaCaixa = CabecaParaPe / 2;
                        float X = Cabeca.X - (TamanhoDaCaixa) / 2;


                        // ESP Box
                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPCAIXA].Valor == 1)
                        {
                            tela.DesenhaAABB(retangulo, verprojecao, Jogadores);
                        }

                        // ESP Vehicle
                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPVEICULOS].Valor == 1)
                        {
                            tela.DesenhaVAABB(retangulo, verprojecao, Jogadores);
                        }

                        // ESP Distance
                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPDISTANCIA].Valor == 1)
                        {
                            tela.DesenhaTexto((int)X, (int)Pe.Y, (int)Jogadores.Distancia + "m", Color.White, true, fontepequena);
                        }

                        // ESP Health
                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPVIDAMUNICAO].Valor == 1)
                        {
                            tela.DesenhaVida((int)X, (int)Cabeca.Y - 6, (int)TamanhoDaCaixa, 3, (int)Jogadores.Vida, (int)Jogadores.VidaMaxima);
                            // Vehicle Health
                            if (Jogadores.DentroDoVeiculo && Jogadores.EMotorista)
                            {
                                tela.DesenhaVida((int)X, (int)Cabeca.Y - 10, (int)TamanhoDaCaixa, 3, (int)Jogadores.VidaVeiculo, (int)Jogadores.VidaMaximaVeiculo);
                            }
                        }

                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPNOME].Valor == 1)
                        {
                            tela.DesenhaTexto((int)X, (int)Pe.Y - 35, (string)Jogadores.Nome, Color.White, true, fontepequena);
                        }

                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.SEMRECUODISPER].Valor == 1)
                        {
                            if (SlotAtivoGlobal < 1)
                            {
                                var spreadrecoil0 = Memoria.LerInt64(Enderecos.ENDERECO_VERANGULOS); // OFFSETS ANGLES
                                var spreadrecoil1 = Memoria.LerInt64(spreadrecoil0 + 0x49c0);
                                var spreadrecoil2 = Memoria.LerInt64(spreadrecoil1 + 0x78);
                                var spreadrecoil3 = Memoria.LerInt64(spreadrecoil2 + 0x8);
                                if (Memoria.SeEValido(spreadrecoil3) && !Jogadores.DentroDoVeiculo)
                                {
                                    //  Memoria.EscreveFlutuante(spreadrecoil3 + 0x0430, 0.0f); // m_DeviationScaleFactorZoom;
                                    // Memoria.EscreveFlutuante(spreadrecoil3 + 0x0434, 0.0f); //m_GameplayDeviationScaleFactorZoom
                                    //  Memoria.EscreveFlutuante(spreadrecoil3 + 0x0438, 0.0f); // m_DeviationScaleFactorNoZoom
                                    //Memoria.EscreveFlutuante(spreadrecoil3 + 0x043C, 0.0f);
                                    Memoria.EscreveFlutuante(spreadrecoil3 + 0x0444, 0.0f); // Float32 m_FirstShotRecoilMultiplier; //0x0444
                                    Memoria.EscreveFlutuante(spreadrecoil3 + 0x0440, 100.0f); // Float32 m_ShootingRecoilDecreaseScale
                                                                                              //  Memoria.EscreveNOPS("bf4", 0x1409A5354, 4);
                                                                                              // Memoria.EscreveNOPS("bf4", 0x1409A535C, 2);
                                }
                            }
                        }





                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.DESTRAVATUDO].Valor == 1)
                        {
                            var pointer = Memoria.LerInt64(Enderecos.ENDERECO_CONFIGURACOESSINCRONIZADAS);
                            {

                                if (Memoria.SeEValido(pointer))
                                {
                                    Memoria.EscreveByte(pointer + 0x54, 1);
                                }

                            }

                        }


                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.SEMPRESPIRAR].Valor == 1)
                        {


                            if (SlotAtivoGlobal <= 1)
                            {
                                var pBreathControlHandler = Memoria.LerInt64(pSoldadoLocal + 0x0588);
                                if (Memoria.SeEValido(pBreathControlHandler) && !Jogadores.DentroDoVeiculo)
                                {
                                    float ValorRespiracao = Memoria.LerFloat(pBreathControlHandler + 0x0058);

                                    if (ValorRespiracao != 0.0f)
                                    {
                                        float NoBreathValue = 0.0f;
                                        Memoria.EscreveFlutuante(pBreathControlHandler + 0x0058, NoBreathValue);
                                    }
                                }
                            }
                        }



                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.NASCERQUALQUERUM].Valor == 1)
                        {
                            Memoria.EscreveByte(Enderecos.ENDERECO_CONFIGURACOESSINCRONIZADAS + 0x5a, 0);
                        }

                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPSOMENTEENEMY].Valor == 1)
                        {
                            DesenhaSomenteInimigo = true;
                        }

                        else
                              if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.ESPSOMENTEENEMY].Valor == 0)
                        {
                            DesenhaSomenteInimigo = false;
                        }

                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AUTOESPOTAR].Valor == 1)
                        {
                            var DadosSpotMapa = Memoria.LerInt32(pSoldadoInimigo + 0xBF0);
                            Memoria.EscreveInt32(DadosSpotMapa + 0x50, 1);
                        }

                        else
                            if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AUTOESPOTAR].Valor == 0)
                        {
                            var DadosSpotMapa = Memoria.LerInt32(pSoldadoInimigo + 0xBF0);
                            Memoria.EscreveInt32(DadosSpotMapa + 0x50, 0);
                        }



                        byte ocluidoo = Memoria.Ler<byte>(pSoldadoInimigo + ClassesDoJogo.EntidadeDoClienteSoldado.EstaOcluido);

                        if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.TRAVAVEICULO].Valor == 1)
                        {
                            if (Jogadores.DentroDoVeiculo)
                            {

                                Jogadores.InSight = ocluidoo != 0;



                            }

                        }
                        else
                        {
                            Jogadores.InSight = ocluidoo == 0;


                        }


                        #endregion
                    }





                    // Add
                    jogadores.Add(Jogadores);
                }
            }

        }

        // Get AIMBOT key
        private Keys PegaTeclaDeMira(int Valor)
        {
            switch (Valor)
            {
                case 0:
                    return Keys.LButton;
                case 1:
                    return Keys.RButton;
                case 2:
                    return Keys.LMenu;
                default:
                    return Keys.RButton;
            }
        }

        private Vector3 PegarVetorOssos(Boneco Osso, int Valor)
        {
            switch (Valor)
            {
                case 0:
                    return Osso.OSSO_CABECA;
                case 1:
                    return Osso.OSSO_PESCOCO;
                case 2:
                    return Osso.OSSO_ESPINHA2;
                case 3:
                    return Osso.OSSO_ESPINHA1;
                case 4:
                    return Osso.OSSO_ESPINHA0;
                default:
                    return Osso.OSSO_CABECA;
            }
        }

        // AIMBOT
        private void FacaAMira()
        {
            // Get Menu
            MenuMae MenuOpcoes = (MenuMae)painel.Items[2];

            // Check AIMBOT Key
            //  if (!Managed.IsKeyDown(getAimKey(menu[GUIMenu.MenuItems.AIM_Key].Value)))
            //    return;

            if (!Gerenciavel.TeclaDigitada(0x02) & !Gerenciavel.TeclaDigitada(0x12))
                return;

            // Check Local Player First
            if (jogadorlocal.EPermitido)
            {
                // Player in Vehicle
                if (jogadorlocal.DentroDoVeiculo)
                {
                    // Vehicle AIMBot activated
                    if (MenuOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTVEICULO].Valor == 1)
                    {
                        #region Get Crosshair Position
                        int TelaX = 0, TelaY = 0;
                        if (jogadorlocal.EMeshTransformavel)
                        {
                            Vector3 mVetorDeAdiantamentodaMira = new Vector3(jogadorlocal.TransformaCaoMesh.M31, jogadorlocal.TransformaCaoMesh.M32, jogadorlocal.TransformaCaoMesh.M33);
                            Vector3 mPosicaoDaMira = new Vector3(jogadorlocal.TransformaCaoMesh.M41, jogadorlocal.TransformaCaoMesh.M42, jogadorlocal.TransformaCaoMesh.M43);
                            mPosicaoDaMira = mPosicaoDaMira + mVetorDeAdiantamentodaMira * 100.0f;

                            if (mVetorDeAdiantamentodaMira.Z != 1 && MundoParaTela(mPosicaoDaMira, out mPosicaoDaMira))
                            {
                                TelaX = (int)mPosicaoDaMira.X;
                                TelaY = (int)mPosicaoDaMira.Y;
                            }
                            else
                            {
                                TelaX = (retangulo.Width / 2);
                                TelaY = (retangulo.Height / 2);
                            }
                        }
                        else
                        {
                            TelaX = (retangulo.Width / 2);
                            TelaY = (retangulo.Height / 2);
                        }
                        #endregion

                        Vector3 LocalDeAcerto;
                        if (JogadorMaisProximodaMira(out LocalDeAcerto, TelaX, TelaY))
                        {
                            // DoAIM
                            if (MundoParaTela(LocalDeAcerto, out LocalDeAcerto))
                            {
                                int MouseDeltaX = (int)LocalDeAcerto.X - TelaX;
                                int MouseDeltaY = (int)LocalDeAcerto.Y - TelaY;
                                MoverMouse(MouseDeltaX, MouseDeltaY);
                            }
                        }
                    }
                }
                else
                {
                    Vector3 LocalDeAcerto;
                    if (JogadorMaisProximo(out LocalDeAcerto, retangulo.Width / 2, retangulo.Height / 2))
                    {
                        // DoAIM
                        Vector2 Angulo = CorrecaoDeAngulo(LocalDeAcerto);

                        if (!jogadorlocal.DentroDoVeiculo)
                        {
                            if (DistanciaSuaAteOInimigo > 100f)
                            {
                                float zerodist = 100;
                                if (DistanciaSuaAteOInimigo > 500f)
                                {
                                    zerodist = 150f;
                                }
                                else if (DistanciaSuaAteOInimigo > 300f && DistanciaSuaAteOInimigo <= 500f)
                                {
                                    zerodist = 125f;
                                }
                                float tempo = zerodist / jogadorlocal.VelocidadeBala.Z;
                                float queda = -0.5f * jogadorlocal.GravidadeBala * tempo * tempo;
                                float theta = (float)Math.Atan2(queda, zerodist);
                                Angulo.Y -= theta;
                            }
                        }

                        Memoria.EscreveAngulo(Angulo.X, Angulo.Y);
                    }
                }
            }
        }

        private void MoverMouse(int X, int Y)
        {
            Int64 ModoDeBordaDeEntrada = Memoria.Ler<Int64>(Enderecos.ENDERECO_MODOENTRADABORDA);
            if (!Memoria.SeEValido(ModoDeBordaDeEntrada))
                return;

            Int64 pRatoMouse = Memoria.Ler<Int64>(ModoDeBordaDeEntrada + ClassesDoJogo.ModoDeEntradaDeBorda.Mouse);
            if (!Memoria.SeEValido(pRatoMouse))
                return;

            Int64 pDispositivoDeMouse = Memoria.Ler<Int64>(pRatoMouse + ClassesDoJogo.Mouse.Dispositivo);
            if (!Memoria.SeEValido(pDispositivoDeMouse))
                return;

            // X
            Memoria.EscreveInt32(pDispositivoDeMouse + ClassesDoJogo.DispositivoDoMouse.Buffer,
                Math.Abs(X) > 5 ? X / 2 : X);
            // Y
            Memoria.EscreveInt32(pDispositivoDeMouse + ClassesDoJogo.DispositivoDoMouse.Buffer + sizeof(Int32),
                Math.Abs(Y) > 5 ? Y / 2 : Y);
        }


        private bool JogadorMaisProximodaMira(out Vector3 Alvo, int CentroX, int CentroY)
        {
            float DistanciaMaxima = 400;
            float DistanciaMaisProxima = DistanciaMaxima;

            // Def Value
            Alvo = new Vector3();

            foreach (Jogador Entidade in jogadores)
            {
                if (Entidade.EPermitido)
                {
                    if (Entidade.Time != jogadorlocal.Time)
                    {
                        // Player in Vehicle and him Driver
                        if (Entidade.DentroDoVeiculo && Entidade.EMotorista)
                        {
                            DistanciaSuaAteOInimigo = Entidade.Distancia;
                            // Get Menu
                            MenuMae MenuDeOpcoes = (MenuMae)painel.Items[2];
                            // Get AIM Point
                            Vector3 PontoDeMira = new Vector3();

                            if (MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTLOCALVEICULO].Valor == 0)
                            {
                                // Center AABB
                                Vector3 Posicao = new Vector3(Entidade.TransformacaoVeiculo.M41, Entidade.TransformacaoVeiculo.M42, Entidade.TransformacaoVeiculo.M43);
                                Vector3 Multiplica = tela.Multiplica((Entidade.AABBVeiculo.Maximo + Entidade.AABBVeiculo.Minimo) * 0.5f, Entidade.TransformacaoVeiculo);
                                PontoDeMira = Posicao + Multiplica;
                            }
                            else
                            {
                                PontoDeMira = Entidade.Posicao;
                                PontoDeMira.Y += (1.75f / 2);
                            }

                            //  Correction
                            PontoDeMira = CorrecaoDeMiraParaVeiculo(PontoDeMira, Entidade.Distancia, Entidade.VelocidadeVeiculo);

                            Vector3 Tela;
                            if (MundoParaTela(PontoDeMira, out Tela))
                            {
                                float DistanciaDaTela = Vector2.Distance(new Vector2(Tela.X, Tela.Y),
                                        new Vector2(CentroX, CentroY));

                                if (DistanciaDaTela < DistanciaMaisProxima)
                                {
                                    DistanciaMaisProxima = DistanciaDaTela;
                                    Alvo = PontoDeMira;
                                }
                            }
                        }
                        else
                        {
                            if (Entidade.EPermitidoMirar && Entidade.EstaVisivel)
                            {
                                DistanciaSuaAteOInimigo = Entidade.Distancia;
                                // Get Menu
                                MenuMae MenuDeOpcoes = (MenuMae)painel.Items[2];
                                // Get AIM Point
                                Vector3 PontoDeMira = PegarVetorOssos(Entidade.Osso, MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTLOCAL].Valor);

                                //  Correction
                                PontoDeMira = CorrecaoDeMiraParaVeiculo(PontoDeMira, Entidade.Distancia, Entidade.VelocidadeVeiculo);

                                Vector3 Tela;
                                if (MundoParaTela(PontoDeMira, out Tela))
                                {
                                    float DistanciaDaTela = Vector2.Distance(new Vector2(Tela.X, Tela.Y),
                                        new Vector2(CentroX, CentroY));

                                    if (DistanciaDaTela < DistanciaMaisProxima)
                                    {
                                        DistanciaMaisProxima = DistanciaDaTela;
                                        Alvo = PontoDeMira;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Check
            return !Alvo.IsZero;
        }


        private bool JogadorMaisProximo(out Vector3 Alvo, int CentroX, int CentroY)           // SOLDADO A PE MIRANDO EM ALGUEM
        {
            float DistanciaMaxima = 400;
            float DistanciaMaisProxima = DistanciaMaxima;

            // Def Value
            Alvo = new Vector3();

            foreach (Jogador Entidades in jogadores)
            {
                if (Entidades.EPermitido && !Entidades.DentroDoVeiculo)// SE JOGADOR INIMIGO  NAO ESTA DENTRO DO VEICULO
                {
                    if (Entidades.Time != jogadorlocal.Time)
                    {


                        if (Entidades.EPermitidoMirar && Entidades.EstaVisivel)
                        {
                            // Get Menu


                            DistanciaSuaAteOInimigo = Entidades.Distancia;

                            MenuMae MenuDeOpcoes = (MenuMae)painel.Items[2];
                            // Get AIM Point
                            Vector3 PontoDeMira = PegarVetorOssos(Entidades.Osso, MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTLOCAL].Valor);

                            //  Correction
                            PontoDeMira = CorrecaoDeMira(PontoDeMira, Entidades.Distancia, Entidades.VelocidadeVeiculo);

                            Vector3 Tela;
                            if (MundoParaTela(PontoDeMira, out Tela))
                            {
                                float DistanciaParaTela = Vector2.Distance(new Vector2(Tela.X, Tela.Y),
                                    new Vector2(CentroX, CentroY));

                                if (DistanciaParaTela < DistanciaMaisProxima)
                                {
                                    DistanciaMaisProxima = DistanciaParaTela;
                                    Alvo = PontoDeMira;
                                }
                            }
                        }
                    }
                }
                else /// COMEÇA TRAVA VEICULO REPETI A LOGICA DE CIMA 
                {

                    if (Entidades.EPermitido && Entidades.DentroDoVeiculo) //MUNDAÇA AQUI, SE JOGADOR INIMIGO ESTA DENTRO DO VEICULO
                    {
                        if (Entidades.Time != jogadorlocal.Time)
                        {


                            if (Entidades.EPermitidoMirar && Entidades.InSight)// SE ESTA NA MIRA
                            {
                                // Get Menu


                                DistanciaSuaAteOInimigo = Entidades.Distancia;

                                MenuMae MenuDeOpcoes = (MenuMae)painel.Items[2];
                                // Get AIM Point
                                Vector3 PontoDeMira = PegarVetorOssos(Entidades.Osso, MenuDeOpcoes[Aguia.MenuMae.ItensDoMenu.AIBTLOCAL].Valor);

                                //  Correction
                                PontoDeMira = CorrecaoDeMira_TravaVEiculo(PontoDeMira, Entidades.Distancia, Entidades.VelocidadeVeiculo);

                                Vector3 Tela;
                                if (MundoParaTela(PontoDeMira, out Tela))
                                {
                                    float DistanciaParaTela = Vector2.Distance(new Vector2(Tela.X, Tela.Y),
                                        new Vector2(CentroX, CentroY));

                                    if (DistanciaParaTela < DistanciaMaisProxima)
                                    {
                                        DistanciaMaisProxima = DistanciaParaTela;
                                        Alvo = PontoDeMira;
                                    }
                                }
                            }
                        }
                    }


                }//TERMINA TRAVA VEICULO
            }

            //Check
            return !Alvo.IsZero;
        }


        private Vector3 CorrecaoDeMiraParaVeiculo(Vector3 LocalDeAcerto, float Distancia, Vector3 VelocidadeInimigo)
        {
            LocalDeAcerto += jogadorlocal.PosicaoDeDeslocamento;
            float m_tempo = Distancia / Math.Abs(jogadorlocal.VelocidadeBala.Z);
            float m_gravidade = Math.Abs(jogadorlocal.GravidadeBala);
            LocalDeAcerto = LocalDeAcerto + (VelocidadeInimigo * m_tempo);
           
            LocalDeAcerto.Y += (jogadorlocal.VelocidadeBala.Y * m_tempo) + (0.5f * m_gravidade * m_tempo * m_tempo);
            return LocalDeAcerto;
        }


        private Vector3 CorrecaoDeMira(Vector3 LocalAcertoDisparo, float Distancia, Vector3 VelocidadeInimigo)
        {
            LocalAcertoDisparo += jogadorlocal.PosicaoDeDeslocamento;
            float m_tempo = Distancia / Math.Abs(jogadorlocal.VelocidadeBala.Z);
            float m_gravidade = Math.Abs(jogadorlocal.GravidadeBala);
            Vector3 dist = Distancia + (VelocidadeInimigo * m_tempo);
            Vector3 timeFinal = dist / Math.Abs(jogadorlocal.VelocidadeBala.Z);
            Vector3 trail = timeFinal - m_tempo;
            LocalAcertoDisparo += trail * Math.Abs(jogadorlocal.VelocidadeBala.Z);
            LocalAcertoDisparo.Y += 0.5f * m_gravidade * m_tempo * m_tempo;
            return LocalAcertoDisparo;
        }
        private Vector3 CorrecaoDeMira_TravaVEiculo(Vector3 m_HitLocation, float Distance, Vector3 EnemyVelocity) // repeti a correcao exclusiva só pra ele pois assim ficou melhor, nao me pergunte porque hahaha
        {
            m_HitLocation += jogadorlocal.PosicaoDeDeslocamento;
            float m_time = Distance / Math.Abs(jogadorlocal.VelocidadeBala.Z);
            float m_grav = Math.Abs(jogadorlocal.GravidadeBala);
            Vector3 dist = Distance + (EnemyVelocity * m_time);
            Vector3 timeFinal = dist / Math.Abs(jogadorlocal.VelocidadeBala.Z);
            Vector3 trail = timeFinal - m_time;
            m_HitLocation += trail * Math.Abs(jogadorlocal.VelocidadeBala.Z);
            m_HitLocation.Y += 0.5f * m_grav * m_time * m_time;
            return m_HitLocation;
        }



        private Vector2 CorrecaoDeAngulo(Vector3 LocalDeAcerto)
        {
            Vector3 Espaco = new Vector3();
            Espaco.X = LocalDeAcerto.X - vermatrixinversa.M41;
            Espaco.Y = LocalDeAcerto.Y - vermatrixinversa.M42;
            Espaco.Z = LocalDeAcerto.Z - vermatrixinversa.M43;
            Espaco = Vector3.Normalize(Espaco);

            Vector2 Angulos = new Vector2();
            Angulos.X = (float)-Math.Atan2(Espaco.X, Espaco.Z);
            Angulos.Y = (float)Math.Atan2(Espaco.Y, Math.Sqrt((Espaco.X * Espaco.X) + (Espaco.Z * Espaco.Z)));
            Angulos.X -= jogadorlocal.EstabilizacaoSWAY.X;
            Angulos.Y -= jogadorlocal.EstabilizacaoSWAY.Y;
            return Angulos;
        }


        public bool MundoParaTela(Vector3 _Mundo, out Vector3 _Tela)
        {
            _Tela = new Vector3(0, 0, 0);
            float TelaW = (verprojecao.M14 * _Mundo.X) + (verprojecao.M24 * _Mundo.Y) + (verprojecao.M34 * _Mundo.Z + verprojecao.M44);

            if (TelaW < 0.0001f)
                return false;

            float CentroX = (verprojecao.M11 * _Mundo.X) + (verprojecao.M21 * _Mundo.Y) + (verprojecao.M31 * _Mundo.Z + verprojecao.M41);
            float CentroY = (verprojecao.M12 * _Mundo.X) + (verprojecao.M22 * _Mundo.Y) + (verprojecao.M32 * _Mundo.Z + verprojecao.M42);

            _Tela.X = (retangulo.Width / 2) + (retangulo.Width / 2) * CentroX / TelaW;
            _Tela.Y = (retangulo.Height / 2) - (retangulo.Height / 2) * CentroY / TelaW;
            _Tela.Z = TelaW;
            return true;
        }

        // Get Window Rect
        private void DefinirJanela(object sender)
        {
            while (true)
            {
                IntPtr HandlerAlvo = IntPtr.Zero;
                HandlerAlvo = Gerenciavel.FindWindow(null, "Battlefield 4");

                if (HandlerAlvo != IntPtr.Zero)
                {
                    Gerenciavel.DadosTela TamanhoDoAlvo = new Gerenciavel.DadosTela();
                    Gerenciavel.GetWindowRect(HandlerAlvo, out TamanhoDoAlvo);

                    // Game is Minimized
                    if (TamanhoDoAlvo.Esquerdo < 0 && TamanhoDoAlvo.Topo < 0 && TamanhoDoAlvo.Direito < 0 && TamanhoDoAlvo.Inferior < 0)
                    {
                        BoolMinimizado = true;
                        continue;
                    }

                    // Reset
                    BoolMinimizado = false;

                    Gerenciavel.DadosTela TamanhoDaBorda = new Gerenciavel.DadosTela();
                    Gerenciavel.GetClientRect(HandlerAlvo, out TamanhoDaBorda);

                    int EstiloDeDesenho = Gerenciavel.GetWindowLong(HandlerAlvo, Gerenciavel.GWL_ESTILO);

                    int JanelaHeigh;
                    int JanelaWidth;
                    int bordaHeight;
                    int BordaWidth;

                    if (retangulo.Width != (TamanhoDoAlvo.Inferior - TamanhoDoAlvo.Topo)
                        && retangulo.Width != (TamanhoDaBorda.Direito - TamanhoDaBorda.Esquerdo))
                        BoolRedimensionavel = true;

                    retangulo.Width = TamanhoDoAlvo.Direito - TamanhoDoAlvo.Esquerdo;
                    retangulo.Height = TamanhoDoAlvo.Inferior - TamanhoDoAlvo.Topo;

                    if ((EstiloDeDesenho & Gerenciavel.WS_BORDA) != 0)
                    {
                        JanelaHeigh = TamanhoDoAlvo.Inferior - TamanhoDoAlvo.Topo;
                        JanelaWidth = TamanhoDoAlvo.Direito - TamanhoDoAlvo.Esquerdo;

                        retangulo.Height = TamanhoDaBorda.Inferior - TamanhoDaBorda.Topo;
                        retangulo.Width = TamanhoDaBorda.Direito - TamanhoDaBorda.Esquerdo;

                        bordaHeight = (JanelaHeigh - TamanhoDaBorda.Inferior);
                        BordaWidth = (JanelaWidth - TamanhoDaBorda.Direito) / 2; //only want one side
                        bordaHeight -= BordaWidth; //remove bottom

                        TamanhoDoAlvo.Esquerdo += BordaWidth;
                        TamanhoDoAlvo.Topo += bordaHeight;

                        retangulo.Left = TamanhoDoAlvo.Esquerdo;
                        retangulo.Top = TamanhoDoAlvo.Topo;
                    }
                    Gerenciavel.MoveWindow(alca, TamanhoDoAlvo.Esquerdo, TamanhoDoAlvo.Topo, retangulo.Width, retangulo.Height, true);
                }
                Thread.Sleep(300);
            }
        }

        // Keybord
        private static IntPtr ChamadaDeHook(int Codigo, IntPtr Parametro, IntPtr Lparametro)
        {
            if (Codigo >= 0 && Parametro == (IntPtr)Teclado.WM_KEYDOWN)
            {
                int CodigoTecla = Marshal.ReadInt32(Lparametro);
                Keys key = (Keys)CodigoTecla;

                MenuMae MenuOpcoes = (MenuMae)painel.Items[2];

                switch (key)
                {
                    case Keys.Down:
                        MenuOpcoes.MoveAbaixo();
                        break;
                    case Keys.Up:
                        MenuOpcoes.MoveAcima();
                        break;
                    case Keys.Right:
                        MenuOpcoes.MoveADireita();
                        break;
                    case Keys.Left:
                        MenuOpcoes.MoveAEsquerda();
                        break;
                    case Keys.F1:
                        MenuOpcoes.EVisivel = !MenuOpcoes.EVisivel;
                        break;
                }
            }
            return Gerenciavel.CallNextHookEx(Teclado._IDHook, Codigo, Parametro, Lparametro);
        }
    }
}
