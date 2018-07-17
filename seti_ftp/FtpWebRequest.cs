using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;


namespace seti_ftp
{
    class Client
    {
        private string password;
        private string userName;
        public string uri;
        private string server;

        public bool Passive = true;
        public bool Binary = true;
        public bool EnableSsl = false;
        public bool Hash = false;

        public Client(string server, string userName, string password)
        {
            this.uri = server;
            this.server = server;
            this.userName = userName;
            this.password = password;
        }
        public void CWD(string path)
        {
            uri += path;
        }
        public void CDUP()
        {
            string path1 = uri.Remove(0, server.Length);
            int pos = 0;
            for (int i = path1.Length; i >= 1; i--)
            {
                if (path1[i-1] == '/')
                {
                    pos = i-1;
                    break;
                }
            }
            if (pos > 0)
            {
                string path2 = path1.Remove(pos);
                uri = server + path2;
            }
            else { uri = server; }

        }

        public long GetFileSize(string fileName)
        {
            try
            {
                var request = createRequest(fileName, WebRequestMethods.Ftp.GetFileSize);
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    return response.ContentLength;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public string[] ListDirectory()
        {
            var list = new List<string>();
            var request = createRequest("NLST");
            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                list.Add(reader.ReadLine());
                            }
                        }
                    }
                }
            }
            catch (Exception e) { list.Add(e.Message.ToString()); }

            return list.ToArray();
        }
        private FtpWebRequest createRequest(string method)
        {
            return createRequest(uri, method);
        }
        private FtpWebRequest createRequest(string uri, string method)
        {
            var r = (FtpWebRequest)WebRequest.Create(uri);

            r.Credentials = new NetworkCredential(userName, password);
            r.Method = method;
            r.UseBinary = Binary;
            r.EnableSsl = EnableSsl;
            r.UsePassive = Passive;
            return r;
        }
    }
}
