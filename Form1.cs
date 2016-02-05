using System;

using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static Form1 thisForm;
        public Form1()
        {
            InitializeComponent();
            auth();
            mquery = new Stack<Tuple<string, string>>();
            //utils = Utils.getInstance();
            // testAuth();
            thisForm = this;
        }
        public void auth()
        {

            int appId = 5079358;
            String scope = "messages";
            String url = "https://oauth.vk.com/authorize?client_id=" + appId + "&display=popup&redirect_uri=http://api.vk.com/blank.html&scope=" + scope + "&response_type=token&v=5.37";
            Form2 f2 = new Form2();
            f2.Show();
            WebBrowser browser = (WebBrowser)f2.Controls["webBrowser1"];
            browser.Navigate(url);
            newThread = new Thread(testAuth);
            newThread.Start();
        }
        private Utils utils = Utils.getInstance();
        private VKutils vkutils = VKutils.getInstance();

        public Thread newThread;
        public Tuple<String, String> ans;
        public static String result = "";
        private long lastmessage_id = 0;
        private Logic logic;

        Stack<Tuple<string, string>> mquery;
        public void makeMagic(string resp)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(resp);
            mquery.Clear();
            long last = lastmessage_id;
            lastmessage_id = long.Parse(doc.SelectSingleNode("response").SelectSingleNode("items").SelectSingleNode("message").SelectSingleNode("id").InnerText);
            foreach (XmlNode mes in doc.SelectSingleNode("response").SelectSingleNode("items").SelectNodes("message"))
            {
                if (long.Parse(mes.SelectSingleNode("id").InnerText) <= last)
                    break;
                mquery.Push(new Tuple<string, string>(mes.SelectSingleNode("from_id").InnerText, mes.SelectSingleNode("body").InnerText));


            }


            while (mquery.Count > 0)
            {

                var msg = mquery.Pop();
                if (msg.Item1 != Settings1.Default.user_id)
                    if (msg.Item2 != "")
                        logic.push(msg.Item1, msg.Item2);



            }

            Thread.Sleep(100);
            ans = logic.getAns();
            if (ans.Item1 != "")
            {

                VKutils.Message.send(ans.Item1, ans.Item2);
                //  lastmessage_id++;
            }

        }

        public void testAuth()
        {
            while (!Settings1.Default.authenticated) ;
            logic = Logic.getInstance();
            //logic.init();
            while (Settings1.Default.send_id == "") ;
            utils.printToFile("dialog.txt", vkutils.getCompleteDialogue(Settings1.Default.send_id));
            //now , lets do it
            HashSet<String> helloKeywords = new HashSet<String>();
            helloKeywords.Add("привіт");
            var hellos = VKutils.Message.getQuestions(helloKeywords);
            utils.printToFile("hellos.txt", hellos.ToList());
            //process
            return;
            while (true)
            {
                if (textBox1.Text != "")
                {
                    try
                    {
                        string resp = utils.GET_http(vkutils.vkReq("messages.getHistory", "count=10&user_id=" + Settings1.Default.send_id));

                        makeMagic(resp);
                    }
                    catch { }
                }
                System.Threading.Thread.Sleep(2000);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                long id = long.Parse(textBox1.Text);
                Settings1.Default.send_id = id.ToString();
            }
            catch
            {
                MessageBox.Show("Error, fuck you");
            }
        }
        HashSet<String> keywords = new HashSet<String>();
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            keywords = new HashSet<String>(textBox2.Text.Split(' '));
            var res = VKutils.Message.getQuestions(keywords);
            listBox1.Items.Clear();
            foreach(VKutils.Message item in res)
                
                listBox1.Items.Add(item.ToString());
            
        }
    }
}
