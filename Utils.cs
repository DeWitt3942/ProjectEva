using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Utils
    {
        private static Utils instance;
        private Utils()
        {
            if (instance != null)
                throw (new Exception("Error while creating another utils"));

        }
        public static  Utils getInstance()
        {
            if (instance == null)
                instance = new Utils();
            return instance;
        }


        public void printToFile<T>(String filename, List<T> whatToType)
        {
            MessageBox.Show(whatToType.Count.ToString());
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
            {
                foreach (T each in whatToType)
                    file.WriteLine(each.ToString());
            }

        }


        public string GET_http(string url)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse res = req.GetResponse();
            System.IO.Stream stream = res.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            return sr.ReadToEnd();

        }

    }
}
