using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {

        
        public Form2()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerAsync();
        }
        public void check()
        {
            while (!Settings1.Default.authenticated) ;
            
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //http://REDIRECT_URI#access_token= 533bacf01e11f55b536a565b57531ad114461ae8736d6506a3&expires_in=86400&user_id=8492 
            //http://REDIRECT_URI#error=access_denied&   error_description=The+user+or+authorization+server+denied+the+request. 
            try
            {
                string url = webBrowser1.Url.ToString();
                string l = url.Split('#')[1];
                if (l[0] == 'a')
                {
                    //success
                    string token = l.Split('&')[0].Split('=')[1];
                    string user_id = l.Split('=')[3];
                    //MessageBox.Show(token + "\n" +user_id);
                    Settings1.Default.token = token;
                    Settings1.Default.user_id = user_id;
                    Settings1.Default.authenticated = true;
                    
                }
                {
                    //fail
                }
            } catch
            {
 
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.authenticated) ;

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Visible = false;
            

        }
    }


}
