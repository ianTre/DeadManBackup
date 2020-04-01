using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ComprobacionBackUp
{
    public class Establecimiento
    {
        public string Nombre { get; set; }
        public List<string> Archivos { get; set; }
        public string errores = string.Empty;
        private bool falloFull = false;
        private bool falloDiff = false;

        public Establecimiento(string directorio)
        {
            this.Nombre = directorio;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://172.16.5.170/" + directorio);
                ftpRequest.Credentials = new NetworkCredential("maxi", "123456");
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                List<string> archivos = new List<string>();

                using (Stream sw = ftpRequest.GetResponse().GetResponseStream())  //getting the response stream
                {
                    StreamReader sr = new StreamReader(sw);   //reading from the stream
                    string cadena = sr.ReadToEnd();
                    var arrayArchivos = cadena.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    this.Archivos = arrayArchivos;

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        internal bool ComprobarBackupsAlDia(string directorio)
        {
            return true;
        }



        public bool FalloBakcup()
        {
            ComprobarArchivos();
            if (falloDiff || falloFull)
                return true;
            return false;
        }


        public string ExplicarErrores()
        {
            string retornador = string.Empty;

            retornador += "Establecimiento : " + this.Nombre  + "\n";
            retornador += "Estado de BackUp :" + errores;

            return retornador;
        }

        private string ComprobarArchivos()
        {
            try
            {


                if (!ComprobarBackUpDiferencial())
                {
                    errores += "No se cargo el backup diferencial \n";
                }

                if (!ComprobarBackUpFull())
                {
                    errores += "No se cargo el Full Backup \n";
                }
                return errores;
            }
            catch (Exception ex)
            {
                errores += "Problemas al comprobar Estado de backups";
                return errores;
            }

        }

        private bool ComprobarBackUpFull()
        {
            try
            {
                bool retornador = false;
                string archivo = Archivos.FirstOrDefault(x => x.Contains("Klinicos-FullBackup.bak"));
                if (!String.IsNullOrEmpty(archivo))
                {

                    var propiedades = archivo.Split(new string[] { "   " } ,StringSplitOptions.None);
                    var sFechaCreacion = propiedades[0].Split(' ');
                    DateTime fechaCreacion;
                    //DateTime.TryParse(propiedades[0], out fechaCreacion ,);
                    DateTime.TryParseExact(sFechaCreacion[0], "MM-dd-yy", CultureInfo.CurrentCulture, DateTimeStyles.None, out fechaCreacion);

                    errores += "Fecha Archivo : " + fechaCreacion.ToString() + " ";

                    TimeSpan ts = DateTime.Now - fechaCreacion;
                    int diferenciaDias = ts.Days;

                    if (diferenciaDias < 8)
                    {
                        return true;
                    }
                }
                falloFull = true;
                return retornador;
            }
            catch (Exception ex)
            {
                errores += "No se pudo comprobar la fecha correctamente";
                errores += ex.Message;
                throw ex;
            }
        }

        private bool ComprobarBackUpDiferencial()
        {
            try
            {
                bool retornador = false;

                string archivo = Archivos.FirstOrDefault(x => x.Contains("KlinicoBkpDiff.bak"));

                if(!String.IsNullOrEmpty(archivo))
                {
                    var propiedades = archivo.Split(new string[] { "   " }, StringSplitOptions.None);
                    var sFechaCreacion = propiedades[0].Split(' ');
                    DateTime fechaCreacion;
                    //DateTime.TryParse(propiedades[0], out fechaCreacion ,);
                    DateTime.TryParseExact(sFechaCreacion[0], "MM-dd-yy", CultureInfo.CurrentCulture, DateTimeStyles.None, out fechaCreacion);

                    errores += "Fecha Archivo : " + fechaCreacion.ToString() + " "; 
                    
                    TimeSpan ts = DateTime.Now - fechaCreacion;
                    int diferenciaDias = ts.Days;

                    if (diferenciaDias < 2)
                    {
                        
                        return true;
                    }
                }
                falloDiff = true;
                return retornador;
            }
            catch (Exception ex)
            {
                errores += "No se pudo comprobar la fecha correctamente";
                errores += ex.Message;
                throw ex;
            }
        }


    }
}
