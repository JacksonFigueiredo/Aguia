using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Aguia
{
    class Memoria : Gerenciavel
    {
        public static readonly int IDDeProcessoInvalida = -1;

        public static IntPtr palca {get; private set;}


        public static bool EscreveByte(Int64 _lpBaseAddress, byte _Valor)
        {
            var Buffer = BitConverter.GetBytes(_Valor);
            return EscreveMemoria(_lpBaseAddress, Buffer);
        }
        public static void EscreveBytes(Int64 Address, byte[] Bytes)
        {
            IntPtr Zero = IntPtr.Zero;
            WriteProcessMemory(palca, Address, Bytes, (uint)Bytes.Length, out Zero);
        }

        public static float LerFloat(Int64 __lpBase)
        {
            var Buffer = new byte[sizeof(float)];
            IntPtr ByteRead;
            ReadProcessMemory(palca, __lpBase, Buffer, sizeof(float), out ByteRead);
            return BitConverter.ToSingle(Buffer, 0);
        }

        public static byte LeByte(Int64 _lpBaseAddress)
        {
            var Buffer = new byte[sizeof(byte)];
            IntPtr ByteRead;
            ReadProcessMemory(palca, _lpBaseAddress, Buffer, sizeof(byte), out ByteRead);
            return Buffer[0];
        }

        public static IntPtr AbreProcesso(int processId)
        {
            palca = OpenProcess(PROCESSO_VM_LEITURA | PROCESSO_VM_ESCRITA | PROCESSO_VM_OPERACAO, false, processId);
            return palca;
        }

        public static void FechaProcesso()
        {
            if (palca != IntPtr.Zero)
            {
                Gerenciavel.CloseHandle(palca);
            }
        }


        #region Salvo As Configurações do Registro
        public static void Salvaconfig(string funcao, string valor)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Multi3");
            key.SetValue(funcao, valor);
            key.Close();
        }
        #endregion


        #region Leio as configurações do registro
        public static int checaconfig(string funcao)
        {

            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Multi3");


                return Convert.ToInt16(key.GetValue(funcao).ToString());
            }
            catch
            {
                return 0;
            }
                   
        }
        #endregion

        public static bool PegaProcessoPorNome(string NomeProcesso, out int IDProcesso)
        {
            Process[] mProcess = Process.GetProcessesByName(NomeProcesso);
            IDProcesso = mProcess != null && mProcess.Length > 0 ? mProcess[0].Id : IDDeProcessoInvalida;
            return IDProcesso != IDDeProcessoInvalida;
        }

        public static bool ProcessoEstaRodando(string processName)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == processName)
                    return true;
            }
            return false;
        }

        public static T Ler<T>(Int64 Endereco)
        {
            // Read in to Buffer
            byte[] Buffer = new byte[Marshal.SizeOf(typeof(T))];
            IntPtr ByteRead;
            Gerenciavel.ReadProcessMemory(palca, Endereco, Buffer, (uint)Buffer.Length, out ByteRead);

            // Get Struct from Buffer
            GCHandle handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)); 
            handle.Free();
            // Return
            return stuff;
        }

        public static string LerString(Int64 Endereco, UInt64 _Tamanho)
        {
            byte[] buffer = new byte[_Tamanho];
            IntPtr BytesRead;

            Gerenciavel.ReadProcessMemory(palca, Endereco, buffer, _Tamanho, out BytesRead);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0)
                {
                    byte[] _buffer = new byte[i];
                    Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                    return Encoding.ASCII.GetString(_buffer);
                }
            }
            return Encoding.ASCII.GetString(buffer);
        }

        public static string LerString2(Int64 Endereco, UInt64 _Tamanho)
        {
            byte[] buffer = new byte[_Tamanho];
            IntPtr BytesRead;

            Gerenciavel.ReadProcessMemory(palca, Endereco, buffer, _Tamanho, out BytesRead);
            return Encoding.ASCII.GetString(buffer);
        }

        public static bool SeEValido(Int64 address)
        {
            return (address >= 0x10000 && address < 0x000F000000000000);
        }



        public static Int32 LerInt32(Int64 _lpBa)
        {
            var Buffer = new byte[4];
            IntPtr ByteRead;
            ReadProcessMemory(palca, _lpBa, Buffer, 4, out ByteRead);
            return BitConverter.ToInt32(Buffer, 0);
        }



        public static bool EscreveMemoria(Int64 EnderecoMemoria, byte[] Buffer)
        {
            uint VelhoProtect;
            VirtualProtectEx(palca, (IntPtr)EnderecoMemoria, (uint)Buffer.Length, PAGINACAO_LEITURAESCRITA, out VelhoProtect);

            IntPtr ptrBytesWritten;
            return WriteProcessMemory(palca, EnderecoMemoria, Buffer, (uint)Buffer.Length, out ptrBytesWritten);
        }

        public static bool EscreveFlutuante(Int64 address, float _Value)
        {
            byte[] Buffer = BitConverter.GetBytes(_Value);
            return EscreveMemoria(address, Buffer);
        }

        public static void EscreveAngulo(float _Yaw, float _Pitch)
        {
            Int64 pBase = Ler<Int64>(Enderecos.ENDERECO_VERANGULOS);
            if (!SeEValido(pBase))
                return;

            Int64 pAuthorativeAiming = Ler<Int64>(pBase + ClassesDoJogo.ArmaClienteSoldado.MiraAutorativa);
            if (!SeEValido(pAuthorativeAiming))
                return;

            Int64 pFpsAimer = Ler<Int64>(pAuthorativeAiming + ClassesDoJogo.SimulacaoDeMiraSoldadoCliente.miradorfps);
            if (!SeEValido(pFpsAimer))
                return;

            EscreveFlutuante(pFpsAimer + ClassesDoJogo.AssistenciaDeMira.yaaww, _Yaw);
            EscreveFlutuante(pFpsAimer + ClassesDoJogo.AssistenciaDeMira.piitch, _Pitch);
        }

        public static bool EscreveInt32(Int64 Endereco, int _Valor)
        {
            byte[] Buffer = BitConverter.GetBytes(_Valor);
            return EscreveMemoria(Endereco, Buffer);
        }


        public static Int64 LerInt64(Int64 _lpBase)
        {
            var Buffer = new byte[8];
            IntPtr ByteRead;
            ReadProcessMemory(palca, _lpBase, Buffer, 8, out ByteRead);
            return BitConverter.ToInt64(Buffer, 0);
        }


             public static void EscreveNOPS(string NomeProcesso, long Endereco, long NumeroDeNops)
        {
            if (NomeProcesso.EndsWith(".exe"))
            {
                NomeProcesso = NomeProcesso.Replace(".exe", "");
            }
            Process[] processesByName = Process.GetProcessesByName(NomeProcesso);
            if (checked((int)processesByName.Length) != 0)
            {
                IntPtr intPtr = (IntPtr)OpenProcess((uint)127231, false, (int)processesByName[0].Id);
              
                if (intPtr != IntPtr.Zero)
                {
                    long nuuumero = (long)0;
                    long numerodenops = NumeroDeNops;
                    for (long i = (long)1; i <= numerodenops; i = checked(i + (long)1))
                    {
                        long nuumero1 = (long)intPtr;
                        long enderecoooo = checked(Endereco + nuuumero);
                        long nummero2 = (long)144;
                        long numero3 = (long)1;
                        long numero4 = (long)0;
                        WriteProcessMemory1(nuumero1, enderecoooo, ref nummero2, numero3, ref numero4);
                        nuuumero = checked(nuuumero + (long)1);
                    }
                }
            }

   

    }
    }



    }


