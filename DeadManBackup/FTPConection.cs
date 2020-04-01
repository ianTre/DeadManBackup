using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ComprobacionBackUp
{
    class FTPConection
    {

        

        public List<string> listFiles(string directorioRaiz)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directorioRaiz);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential("maxi", "123456");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
