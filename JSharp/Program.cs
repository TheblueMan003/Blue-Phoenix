using BluePhoenix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool compiling = false;
            string path="";
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-o")
                    {
                        compiling = true;
                        path = args[i + 1];
                        i++;
                    }
                }
            }
            if (compiling)
            {
                try
                {
                    CMD_Compile compiler = new CMD_Compile(args[0], path);
                    compiler.Export();
                }
                catch (Exception e)
                {
                    CMD_Compile.SafeWriteFile(path+"/console.log", CMD_Compile.consoleText.ToString()+"\n\n\n"+e.ToString());
                    return 1;
                }
            }
            else
            {
                if (args == null || args.Length == 0)
                    Application.Run(new Form1());
                else
                {
                    try
                    {
                        Application.Run(new Form1(args[0]));
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}
