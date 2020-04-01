using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace ComprobacionBackUp
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                List<string> exepciones = new List<string>();
                if (args.Length != 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine(args[i]);
                        exepciones.Add(args[i]);
                    }
                }

                FTPConection ftp = new FTPConection();

                List<string> directorios = ftp.listFiles("ftp://172.16.5.170");
                List<string> carpetasBackup = directorios.FindAll(x => x.Contains("Backup-"));
                List<Establecimiento> establecimientos = new List<Establecimiento>();


                //Dictionary<string, bool> estados = new Dictionary<string, bool>();


                foreach (string directorio in carpetasBackup)
                {
                    if (directorio.Contains("Backup-"))
                    {
                        if (!exepciones.Exists(x => x.Contains(directorio)))
                        {
                            establecimientos.Add(new Establecimiento(directorio));
                        }
                    }
                }

                Mailer mailer = new Mailer();
                mailer.SendMail(establecimientos);



                Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
