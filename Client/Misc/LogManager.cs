using BllEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; 

namespace Client
{
    public static class LogWriter
    {
        public static void WriteMessage(string operation, string exception, string source)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Log.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            try
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine("[" + DateTime.Now.ToString() + "] " + source + ": " + operation + " -- " + exception);
                    tw.Close();
                }
            }
            catch
            {
                
            }
        }
    }
}
