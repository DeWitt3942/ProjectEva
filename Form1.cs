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
        
        public Form1()
        {
            InitializeComponent();
            auth();
            mquery = new Stack<Tuple<string,string>>();
           // testAuth();
        }

        
        public class User
        {
            public String id;
            public String first_name, last_name;
            public static Dictionary<string, User> users = new Dictionary<string,  User>();
            
            public User(String iden, String first, String last)
            { id = iden; first_name = first; last_name = last; }
            public User(XmlNode node)
            {
                id = node.SelectSingleNode("id").InnerText;
                first_name = node.SelectSingleNode("first_name").InnerText;
                last_name = node.SelectSingleNode("last_name").InnerText;
            }
            public static void addUser(string id)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(GET_http(vkReq("users.get", "user_ids="+id)));
                users.Add(id, new User(doc.SelectSingleNode("response").SelectSingleNode("user")));
                
            }
            public static User getUser(string id)
            {
                if (users.ContainsKey(id))
                    return users[id];
                addUser(id);
                return users[id];

            }
            
        }
        public class Message
        {
            public string user_id;
            public string body;
            public long message_id;
            public Message(string user, string text, long id = 0)
            { user_id = user; body = text; message_id = id; }
            public Message(XmlNode xml)
            {
                user_id = xml.SelectSingleNode("user_id").InnerText;
                body = xml.SelectSingleNode("body").InnerText;
                message_id = long.Parse(xml.SelectSingleNode("date").InnerText);
            }
            public static string send(string to, string text)
            {
                return GET_http(vkReq("messages.send", "user_id=" + to + "&message=" + text));
            }
        }
        public Thread newThread;
        public Tuple<String, String> ans;
        public static String result = "";
        private long lastmessage_id = 0;
        private Logic logic;
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
            
            
            while (mquery.Count>0)
            {
            
                var msg = mquery.Pop();
                if (msg.Item1 != Settings1.Default.user_id)
                    if (msg.Item2!="")
                        logic.push(msg.Item1, msg.Item2);
                

                
            }
            
//            Thread.Sleep(100);
            ans = logic.getAns();
            if (ans.Item1 != "")
            {
                Message.send(ans.Item1, ans.Item2);
              //  lastmessage_id++;
            }
                
        }
        string getLastMessageId()
        {
            string res = GET_http(vkReq("messages.getHistory", "count=1&user_id=" + Settings1.Default.send_id));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(res);
            return doc.SelectSingleNode("response").SelectSingleNode("items").SelectSingleNode("message").SelectSingleNode("id").InnerText;
        }

        public void testAuth()
        {
            while (!Settings1.Default.authenticated);
            logic = Logic.getInstance();
            lastmessage_id = long.Parse(getLastMessageId());
            while (true)
            {
                if (textBox1.Text != "")
                {
                    try
                    {
                        string resp = GET_http(vkReq("messages.getHistory", "count=10&user_id=" + Settings1.Default.send_id));

                        makeMagic(resp);
                    }
                    catch { }
                }
                System.Threading.Thread.Sleep(1000);
            }
            

        }
        public static string vkReq(string method, string p)
        {
            return "https://api.vk.com/method/" + method + ".xml?" + p + "&v=5.37&access_token=" + Settings1.Default.token;
        }
        
        public static string GET_http(string url)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse res = req.GetResponse();
            System.IO.Stream stream = res.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            return sr.ReadToEnd();
            
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
                lastmessage_id = int.Parse(getLastMessageId());
            }catch
            {
                MessageBox.Show("Error, fuck you");
            }
        }
    }
}
