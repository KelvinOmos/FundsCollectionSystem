using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CollectionSystem.WebApp
{
     
    public class Logger  
    {
        public bool CanUseFile(string path)
        {
            bool flag = true;
            int num = 0;
            while (true)
            {
                int num1 = 10;
                while (true)
                {
                    if ((!FileInUse(path) ? true : num1 == 0))
                    {
                        break;
                    }
                    Thread.Sleep(10);
                    num1--;
                    flag = false;
                }
                if ((!FileInUse(path) ? true : num1 != 0))
                {
                    flag = true;
                    break;
                }
                else if (num < 1)
                {
                    num++;
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            return flag;

        }

        public bool FileInUse(string path)
        {
            bool flag;
            try
            {
                bool canRead = true;
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    canRead = !fileStream.CanRead;
                }
                flag = canRead;
            }
            catch (Exception exception)
            {
                flag = true;
            }
            return flag;

        }

        public string Format(string msg)
        {
            object now = DateTime.Now;
            char[] chrArray = new char[] { '\r', '\n' };
            string str = string.Format("[{0}]: {1}\r\n", now, msg.Trim(chrArray));
            return str;
        }

        public void WriteErrorLog(Exception ex)
        {
            try
            {
                StreamWriter streamWriter = null;
                if (!Directory.Exists(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS")))
                {
                    Directory.CreateDirectory(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS"));
                }
                string str = string.Format("{0:ddMMyyyy}", DateTime.Now);
                string str1 = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS/", str, ".txt");
                streamWriter = new StreamWriter(str1, true);
                streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", ex.Source.ToString().Trim(), ":", ex.Message.ToString().Trim() }));
                if (ex.InnerException != null)
                {
                    streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", ex.InnerException.Message.ToString().Trim(), ":", ex.InnerException.StackTrace.ToString().Trim() }));
                }
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch(Exception exO)
            {

            }

        }

        public void WriteErrorLog(string Message, string tid = "", string maskedpan = "", string stan = "")
        {
            object obj = false;
            try
            {
                if (!Directory.Exists(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS")))
                {
                    Directory.CreateDirectory(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS"));
                }
                string str = string.Format("{0:ddMMyyyy}", DateTime.Now);
                string str1 = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS/", str, ".txt");

                try
                {
                    if (!Directory.Exists((new FileInfo(str1)).DirectoryName))
                    {
                        Directory.CreateDirectory((new FileInfo(str1)).DirectoryName);
                    }
                    string str2 = string.Concat(new string[] { tid, "|", maskedpan, "|", stan });
                    File.AppendAllText(str1, Format(string.Concat(new string[] { DateTime.Now.ToString(), ":", str2, ":", Message })));
                    obj = true;
                }
                catch (Exception exception)
                {
                    obj = false;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void WriteErrorLogX(string Message, string tid = "", string maskedpan = "", string stan = "")
        {
            try
            {
                if (!Directory.Exists(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS")))
                {
                    Directory.CreateDirectory(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS"));
                }
                string str = string.Format("{0:ddMMyyyy}", DateTime.Now);
                string str1 = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "LOGS/", str, ".txt");
                if (CanUseFile(str1))
                {
                    if (File.Exists(str1))
                    {
                        StreamWriter streamWriter = null;
                        string str2 = string.Concat(new string[] { tid, "|", maskedpan, "|", stan });
                        streamWriter = new StreamWriter(str1, true);
                        streamWriter.WriteLine(string.Concat(new string[] { DateTime.Now.ToString(), ":", str2, ":", Message }));
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    else
                    {
                        File.AppendAllText(str1, "STart");
                    }
                }
            }
            catch (Exception exception)
            {
            }

        }

        public void WriteIsoLog(string Message)
        {
            try
            {
                if (!Directory.Exists(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "ILOGS")))
                {
                    Directory.CreateDirectory(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "ILOGS"));
                }
                string str = string.Format("{0:ddMMyyyy}", DateTime.Now);
                string str1 = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "ILOGS/", str, ".txt");
                if (File.Exists(str1))
                {
                }
                StreamWriter streamWriter = null;
                streamWriter = new StreamWriter(str1, true);
                DateTime now = DateTime.Now;
                streamWriter.WriteLine(string.Concat(now.ToString(), ":", Message));
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
