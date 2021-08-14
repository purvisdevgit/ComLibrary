using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComLibrary
{
    public class ComClient
    {
        [DllImport("kernel32")]
        private static extern uint GetTickCount();
        const string InterDll = "DllSerialPort";
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Open(string bpPorNamet);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Close(string bpPorNamet);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Read(string bpPorNamet, byte[] bpBufRead, int LenData);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Send(string bpPorNamet, byte[] bpBufRead, int LenData);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_ReadBufLen(string bpPorNamet);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Reset(string bpPorNamet);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_BoudRate(string bpPorNamet, int BoudRate);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fParity(string bpPorNamet, bool bfParity);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_ByteSize(string bpPorNamet, int ByteSize);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_StopBits(string bpPorNamet, int StopBits);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_Parity(string bpPorNamet, int Parity);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Alive(string bpPorNamet);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_XonLim(string bpPorNamet, int XonLim);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_XoffLim(string bpPorNamet, int XoffLim);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fDtrControl(string bpPorNamet, byte DtrControl);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fRtsControl(string bpPorNamet, byte fRtsControl);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fOutxCtsFlow(string bpPorNamet, bool bfOutxCtsFlow);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fOutxDsrFlow(string bpPorNamet, byte bfOutxDsrFlow);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Get_fDtrControl(string bpPorNamet, int fDtrControl);
        [DllImport(InterDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Comm_Set_fDsrSensitivity(string bpPorNamet, bool bfDsrSensitivity);

        const int Maxbuffer = 4080;
        private string asCom;
        private char[] RecBuffer = new char[Maxbuffer];
        private int RecLen;
        private int FCommBaudRate;
        //判断串口是否活动
        public bool IsOpen()
        {
            int i;
            i = Comm_Alive(asCom);
            if (i == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //打开串口
        public bool OpenCom()
        {
            int i;
            i = Comm_Open(asCom);
            if (i == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetComM(string sCom)
        {
            if (sCom.Length > 4)
            {
                sCom = @"\\.\" + sCom;  //2014-12-22 改进COM口大于10的问题
            }
            asCom = sCom;
            return true;
        }
        public bool SetCom(int iBoudRate, string sParity, int iByteSize, int iStopBits)
        {
            int i, iParity;
            if (sParity == "E")
            {
                iParity = 2;
            }
            else if (sParity == "O")
            {
                iParity = 1;
            }
            else
            {
                iParity = 0;
            }

            if (iStopBits == 1)
            {
                iStopBits = 0;
            }
            i = Comm_Alive(asCom);
            if (i == 0)
            {
                Comm_Reset(asCom);
                Comm_Set_BoudRate(asCom, iBoudRate);
                Comm_Set_ByteSize(asCom, iByteSize);
                Comm_Set_StopBits(asCom, iStopBits);
                Comm_Set_Parity(asCom, iParity);
                Comm_Set_fParity(asCom, true);
                SystemWait(100);
                FCommBaudRate = iBoudRate; //设置波特率
                return true;
            }
            else
            {
                Comm_Close(asCom);
                i = Comm_Open(asCom);
                Comm_Reset(asCom);
                if (i < 0)
                {
                    return false;
                }
                Comm_Set_BoudRate(asCom, iBoudRate);
                Comm_Set_ByteSize(asCom, iByteSize);
                Comm_Set_StopBits(asCom, iStopBits);
                Comm_Set_Parity(asCom, iParity);
                SystemWait(100);
                FCommBaudRate = iBoudRate; //设置波特率
                return true;
            }
        }
        public bool iniCom()
        {
            return SetCom(4800, "o", 8, 1);
        }
        //串口数据发送函数
        public int FunSendMessage(string strOrder)
        {
            FunReadMessage();
            int iLen;
            byte[] pOrder = new byte[Maxbuffer + 1];
            iLen = strOrder.Length / 2;
            ClearRecBuffer();
            pOrder = HextoByte(strOrder);
            iLen = pOrder.Length;
            if (Comm_Send(asCom, pOrder, iLen) == iLen)
                return 0;
            else
                return 1;
        }
        //串口数据接收函数
        public string FunReadMessage()
        {
            byte[] ReceiveInfo = new byte[Maxbuffer + 1];
            string sRec = "";
            int ReadLen = -1;
            //ReadLen = Comm_Read(asCom, ReceiveInfo, Maxbuffer + 1);
            ReadLen = Comm_ReadBufLen(asCom);
            if (ReadLen >= 0)
            {
                Comm_Read(asCom, ReceiveInfo, Maxbuffer + 1);
                sRec = BytetoHex(ReceiveInfo, ReadLen);
                return sRec;
            }
            else
            {
                return "";
            }
        }
        public bool Act_CloseCommExecute()
        {
            if (asCom == "")
            {
                return true;
            }
            int i;
            i = Comm_Close(asCom);
            if ((i == 0) || (i == -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int ClearRecBuffer()
        {
            RecLen = 0;
            return 0;
        }
        public bool ChangeParity(int btl)
        {
            if (Comm_Alive(asCom) == 0)
            {
                if (Comm_Set_Parity(asCom, btl) != 0)
                {
                    SystemWait(50);
                    return false;
                }
            }
            else
            {
                SetCom(btl, "E", 8, 1);
            }
            FCommBaudRate = btl;
            return true;
        }
        public bool ChangeParityIec(int btl)
        {
            int i;
            i = Comm_Set_BoudRate(asCom, btl);
            FCommBaudRate = btl;
            return true;
        }

        /// <summary>
        /// 系统等待
        /// </summary>
        /// <param name="ms"></param>
        public static void SystemWait(uint ms)
        {
            uint tickCount = GetTickCount();
            while (true)
            {
                if (((GetTickCount() - tickCount) < ms) )
                {
                    Application.DoEvents();
                    continue;
                }
                return;
            }
        }
        /// <summary>
        /// HEX转byte[]
        /// </summary>
        /// <param name="shex"></param>
        /// <returns></returns>
        public static byte[] HextoByte(string shex)
        {
            string str = shex.Replace(" ", "");
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                string str2 = str.Substring(i * 2, 2);
                buffer[i] = Convert.ToByte(str2, 0x10);
            }
            return buffer;
        }
        /// <summary>
        /// byte[]转HEX
        /// </summary>
        /// <param name="abyte"></param>
        /// <param name="iLen"></param>
        /// <returns></returns>
        public static string BytetoHex(byte[] abyte, int iLen)
        {
            string str = "";
            for (int i = 0; i < iLen; i++)
            {
                string str2 = sFormatstr(Convert.ToString(abyte[i], 0x10).ToUpper(), 2);
                str = str + str2;
            }
            return str;
        }
        /// <summary>
        /// 格式化补零字符串
        /// </summary>
        /// <param name="aStr"></param>
        /// <param name="iLen"></param>
        /// <returns></returns>
        public static string sFormatstr(string aStr, int iLen)
        {
            while (true)
            {
                if (aStr.Length >= iLen)
                {
                    if (aStr.Length > iLen)
                    {
                        aStr = aStr.Substring(aStr.Length - iLen, iLen);
                    }
                    return aStr;
                }
                aStr = "0" + aStr;
            }
        }
    }
}
