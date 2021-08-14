using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComLibrary
{
    class Program
    {
        private static bool isExit = false;
        static ComClient client = new ComClient();
        static void Main(string[] args)
        {
            client.iniCom();
            client.SetComM("COM1");
            if (client.OpenCom())
            {
                Thread th = new Thread(() => Recive());
                th.IsBackground = true;
                th.Start();
                th = new Thread(() => Send());
                th.IsBackground = true;
                th.Start();
            }
            while (true)
            {
                if (isExit)
                {
                    client.Act_CloseCommExecute();
                    break;
                }
            }


        }
        /// <summary>
        /// 循环接受
        /// </summary>
        private static void Recive()
        {
            while (true)
            {
                string strOutput = "";
                while (true)
                {
                    ComClient.SystemWait(500);
                    string msg = client.FunReadMessage();
                    if (msg == "")
                    {
                        break;
                    }
                    strOutput += msg;
                }
                if (strOutput=="")
                {
                    continue;
                }
                Console.WriteLine(strOutput);
                ComClient.SystemWait(3000);
            }
        }
        /// <summary>
        /// 循环发送
        /// </summary>
        private static void Send()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "EXIT")
                {
                    isExit = true;
                    break;
                }
                client.FunSendMessage(msg);
            }
        }
    }
}
