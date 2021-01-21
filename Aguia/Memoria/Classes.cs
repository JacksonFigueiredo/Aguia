using System;

namespace Aguia
{
    struct ClassesDoJogo
    {
        public struct ContextoDoJogoCliente
        {
            public static Int64 m_GerenciadorDeFisica = 0x28; // HavokPhysicsManager
            public static Int64 m_GerenciadorDoJogador = 0x60;  // ClientPlayerManager

            public static Int64 PegaInstancia()
            {
                return Enderecos.ENDERECO_CONTEXTODOJOGO;
            }
        }

        public struct GerenciadorDoClienteDoJogo
        {
            public static Int64 m_JogadorLocal = 0x540; // ClientPlayer
            public static Int64 m_ppJogador = 0x548;     // ClientPlayer
        }

        public struct VisualizadorDoJogadorCliente
        {
            public static Int64 m_Proprietario = 0x00F8; // ClientPlayer
        }

   

        public struct JogadorCliente
        {
            public static Int64 Nome = 0x40;            // 10 CHARS
            public static Int64 EEspectador = 0x13C9;   // BYTE
            public static Int64 IDdoTime = 0x13CC;        // INT32
            public static Int64 ClientSoldierEntity = 0x14B0;     // ClientSoldierEntity 
            public static Int64 Entrada = 0x1508;         // ClientEntryComponent
            public static Int64 PropriaVisualizacaoDoJogador = 0x1510; // ClientPlayerView
            public static Int64 VisualizacaoDoJogador = 0x1520;    // ClientPlayerView
            public static Int64 ControlavelAnexado = 0x14C0;   // ClientSoldierEntity (ClientVehicleEntity)
            public static Int64 ControleControlavel = 0x14D0; // ClientSoldierEntity
            public static Int64 IdDeEntradaAnexada = 0x14C8;   // INT32
            public static Int64 IDEntradaControlavel = 0x14C8; // INT32
        }

        public struct ComponenteEntradaCliente
        {
            public static Int64 ComponenteDeCamera = 0x200; // ClientCameraComponent
            public static Int64 CameraAnterior = 0x278;     // UINT32
            public static Int64 EstadoDePrediccao = 0x280;    // EntryComponent.State
            public static Int64 EstadoDeCorrecao = 0x298;    // EntryComponent.State
            public static Int64 EstadoAtual = 0x2B0;      // EntryComponent.State Ptr
            public static Int64 EstadoDeRePlicacao = 0x2B8;   // EntryComponent.State Ptr
        }

        public struct ComponenteDeEstado
        {
	        public struct Estado
	        {
                public static Int64 IndexDeCameraAtual = 0x00;  // UINT32
                public static Int64 IndexDeCameraAnterior = 0x04; // UINT32
                public static Int64 EstanciaAtiva = 0x08;        // INT32
                public static Int64 EstanciaAnterior = 0xC;       // INT32
                public static Int64 ResetDeEstanciaNaSaida = 0x10;   // BOOL
	        }
        }

        public struct EntidadeDeVeiculoDoCliente
        {
            public static Int64 dados = 0x0030;           // VehicleEntityData
            public static Int64 entidadedefisica = 0x0238; // DynamicPhysicsEntity
            public static Int64 velocidade = 0x0280;       // D3DXVECTOR3 
            public static Int64 velocidadeanterior = 0x0290;   // D3DXVECTOR3 
            public static Int64 chassisdoveiculo = 0x03E0;        // ClientChassisComponent
            public static Int64 aabbfilho = 0x0250;   // AxisAlignedBox
        }

        public struct EntidadeDinamicaDeFisica
        {
            public static Int64 transformacaodeentidade = 0xA0;  // PhysicsEntityTransform
        }

        public struct TransformacaoDeEntidadeDeFisica
        {
            public static Int64 Transformacao = 0x00;       // D3DXMATRIX
        }

        public struct EntidadeDeDadosDeVeiculo
        {
            public static Int64 VidaMaximaFrontal = 0x148; // FLOAT
            public static Int64 nomesid = 0x0248;       // char* ID_P_VNAME_9K22
        }

        public struct ComponenteDeChassisDoCliente
        {
            public static Int64 Velocidade = 0x01C0; // D3DXVECTOR4
        }

        public struct EntidadeDoClienteSoldado
        {
            public static Int64 Dados = 0x0030;         // VehicleEntityData
            public static Int64 Jogador = 0x01E0;          // ClientPlayer
            public static Int64 ComponenteDeVida = 0x0140; // HealthComponent
            public static Int64 VisaoAutorativa = 0x04D8;   // FLOAT
            public static Int64 AnguloAutorativo = 0x04DC; // FLOAT 
            public static Int64 TipoDeEstanciaDoSoldado = 0x04F0;         // INT32
            public static Int64 BandeirasdeRenderizacao = 0x04F4;      // INT32
            public static Int64 EntidadeDeFisica = 0x0238;   // VehicleDynamicPhysicsEntity
            public static Int64 ControleComPrediccao = 0x0490;    // ClientSoldierPrediction
            public static Int64 ComponentedeArmaDoSoldado = 0x0570; // ClientSoldierWeaponsComponent
            public static Int64 ComponenteRagDoll = 0x0580;        // ClientRagDollComponent 
            public static Int64 AlcaControladorRespiracao = 0x0588;    // BreathControlHandler 
            public static Int64 EstaCorrendo = 0x5B0;  // BYTE 
            public static Int64 EstaOcluido = 0x05B1;  // BYTE
        }

        public struct ComponenteDeSaude
        {
            public static Int64 Vida = 0x0020;        // FLOAT
            public static Int64 VidaMaxima = 0x0024;     // FLOAT
            public static Int64 VidaMaximaDoVeiculo = 0x0038; // FLOAT (pLocalSoldier + 0x1E0 + 0x14C0 + 0x140 + 0x38)
        }

        public struct PredicaoDoClienteSoldado
        {
            public static Int64 Posicao = 0x0030; // D3DXVECTOR3
            public static Int64 Velocidade = 0x0050; // D3DXVECTOR3
        }

        public struct ComponenteDeVidaDoSoldado
        {
            public static Int64 alca = 0x0890;      // m_handler + m_activeSlot * 0x8 = ClientSoldierWeapon
            public static Int64 slotativo = 0x0A98;   // INT32 (WeaponSlot)
            public static Int64 alcaativa = 0x08D0; // ClientActiveWeaponHandler 
        }

        public struct AtualizacaoDaEstanciaDoSoldado
        {
            public static Int64 TransformacaoDoMundoAtivo = 0x0028; // QuatTransform
            public static Int64 TransformacoesValidas = 0x0040;       // BYTE
        }

        public struct ComponenteRagDollSoldado
        {
            public static Int64 transformadoresragdoll = 0x0088; // UpdatePoseResultData
            public static Int64 transformador = 0x05D0;         // D3DXMATRIX
        }

        public struct TransformadorQuat
        {
            public static Int64 EscalaTransformacao = 0x0000; // D3DXVECTOR4
            public static Int64 Rotacao = 0x0010;      // D3DXVECTOR4
        }

        public struct ArmaClienteSoldado
        {
            public static Int64 Dados = 0x0030;              // WeaponEntityData
            public static Int64 MiraAutorativa = 0x4988; // ClientSoldierAimingSimulation
            public static Int64 arma = 0x49A8;           // ClientWeapon
            public static Int64 primaria = 0x49C0;          // WeaponFiring
        }

        public struct AlcaDaArmaAtivaDoCliente
        {
            public static Int64 armaativa = 0x038; // ClientSoldierWeapon
        }

        public struct DadosEntidadeDeArma
        {
            public static Int64 nome = 0x0130; // char*
        }

        public struct SimulacaoDeMiraSoldadoCliente
        {
            public static Int64 miradorfps = 0x0010;  // AimAssist
            public static Int64 yyyaw = 0x0018;       // FLOAT
            public static Int64 piiiitch = 0x001C;     // FLOAT
            public static Int64 mswaayy = 0x0028;      // D3DXVECTOR2
            public static Int64 niveldezoom = 0x0068; // FLOAT
        }

        public struct ArmaCliente
        {
            public static Int64 Modificador = 0x0020; // WeaponModifier
            public static Int64 EspacoDeTiro = 0x0040; // D3DXMATRIX
        }

        public struct ArmaDisparando
        {
            public static Int64 Swaay = 0x0078;                  // WeaponSway
            public static Int64 FogoPrimario = 0x0128;           // PrimaryFire 
            public static Int64 ProjeteisCarregados = 0x01A0;      // INT32 
            public static Int64 ProjeteisNoPente = 0x01A4; // INT32 
            public static Int64 TimerPorPenalidadeDeAquecimentoDaArma = 0x01B0;   // FLOAT
        }

        public struct SwayDaArma
        {
            public static Int64 DadosDeWay = 0x0008;      // GunSwayData
            public static Int64 DeviacaoDePitch = 0x0130; // FLOAT 
            public static Int64 DeviacaoDeYaw = 0x0134;   // FLOAT 
        }



        public struct FogoPrimario
        {
            public static Int64 ConfiguracaoDadosDisparo = 0x0010; // ShotConfigData
        }

        public struct ConfiguracaoDadosDeDisparos
        {
            public static Int64 PosicaoDeDeslocamento = 0x0060;    // VECTOR3
            public static Int64 VelocidadeInicial = 0x0080;      // VECTOR3
            public static Int64 DadosDoProjetil = 0x00B0;   // BulletEntityData
        }

        public struct DadosEntidadeDeBala
        {
            public static Int64 Gravidade = 0x0130;     // FLOAT
            public static Int64 DanoInicial = 0x0154; // FLOAT
            public static Int64 DanoFinal = 0x0158;   // FLOAT
        }

        public struct AssistenciaDeMira
        {
            public static Int64 yaaww = 0x0014;   // FLOAT
            public static Int64 piitch = 0x0018; // FLOAT
        }

        public struct ControladorRespiracao
        {
            public static Int64 TimeDeControleRespiracao = 0x0038; // FLOAT
            public static Int64 MultiplicadorDeControleDeRespiracao = 0x003C; // FLOAT  
            public static Int64 TimerDePenalidadeDeRespiracao = 0x0040; // FLOAT  
            public static Int64 MultiplicadorDeControladorDePenalizacaoDeRespiracao = 0x0044; // FLOAT  
            public static Int64 ControleDeRespiracaoAtivo = 0x0048; // FLOAT  
            public static Int64 EntradaDeControleDeRespiracao = 0x004C; // FLOAT  
            public static Int64 RespiracaoAtiva = 0x0050; // FLOAT  
            public static Int64 Ligado = 0x0058; // FLOAT  
        }

        public struct RenderizadorDoJogo
        {
            public static Int64 RenderizaVisao = 0x60; // RenderView

            public static Int64 PegaInstancia()
            {
                return Enderecos.ENDERECO_GAMERENDER;
            }
        }

        public struct RenderizaVisao
        {
            public static Int64 Transforma = 0x0040;         // D3DXMATRIX
            public static Int64 CampoDeVisaoY = 0x00B4;              // FLOAT
            public static Int64 CampoDeVisaoX = 0x0250;              // FLOAT
            public static Int64 VerProjecao = 0x0420;          // D3DXMATRIX
            public static Int64 VerMatrixInversa = 0x02E0; // D3DXMATRIX
            public static Int64 VerProjecaoInversa = 0x04A0;   // D3DXMATRIX
        }

        public struct ModoDeEntradaDeBorda
        {
            public static Int64 Mouse = 0x0058; // Mouse
            public static Int64 PegarInstancia()
            {
                return Enderecos.ENDERECO_MODOENTRADABORDA;
            }
        }

        public struct Mouse
        {
            public static Int64 Dispositivo = 0x0010; //  MouseDevice
        }

        public struct DispositivoDoMouse
        {
            public static Int64 Buffer = 0x0104; // D3DXVECTOR3
        }

        public struct ArmaDoVeiculo
        {
            public static Int64 ComponenteDeCameraDoCliente = 0x0010; // ClientCameraComponent
            public static Int64 PegaInstancia()
            {
                return Enderecos.ENDERECO_ARMA_ATUAL_ATIRANDO;
            }
        }

        public struct ComponenteDeCameraDoCliente
        {
            public static Int64 CameraEstaticaOuModoLivre = 0x00B8; // StaticCamera
        }

        public struct CameraEstatica
        {
            public static Int64 MatrixDePreCross = 0x0010;    // D3DXMATRIX
            public static Int64 CrossMatriz = 0x0050;       // D3DXMATRIX
            public static Int64 EnderecoDeAvanco = 0x01D0;     // D3DXVECTOR3
        }
    }
}
