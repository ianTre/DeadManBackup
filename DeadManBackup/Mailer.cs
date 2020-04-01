using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ComprobacionBackUp
{
    public class Mailer
    {
        public string myDireccion = "soportetecnicosalud@gmail.com";
        public string destinatario = "soportetecnicosalud@gmail.com";
        SmtpClient SmtpServer;


        public Mailer()
        {
            SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("soportetecnicosalud@gmail.com", "SoporteTecnico2018");
            SmtpServer.EnableSsl = true;
        }


        public void SendMail(List<Establecimiento> lista)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(myDireccion);
                mail.To.Add(destinatario);
                mail.Subject = "Estado de BackUps";

                string body = string.Empty;

                foreach (Establecimiento establecimiento in lista)
                {
                    if(establecimiento.FalloBakcup())
                    {
                        body += establecimiento.ExplicarErrores();
                        body += "\n \n";
                    }
                }

                if (body.Length > 0)
                {
                    body += "Fecha del sistema : " + DateTime.Now + " " ;
                    mail.Body = body;
                    SmtpServer.Send(mail);
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }



    }
}
