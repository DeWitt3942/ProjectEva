using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication1
{
    public class VKutils
    {
        private static VKutils instance;
        private static Utils utils;
        private VKutils() {
            utils = Utils.getInstance();
        }
        public static VKutils getInstance()
        {
            if (instance == null)
                instance = new VKutils();
            return instance;
        }
        public void getDialogue()
        {

        }
        

        public string vkReq(string method, string p)
        {
            return "https://api.vk.com/method/" + method + ".xml?" + p + "&v=5.37&access_token=" + Settings1.Default.token;
        }

        public class User
        {

            public String id;
            public String first_name, last_name;
            public static Dictionary<string, User> users = new Dictionary<string, User>();

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

                
                doc.LoadXml(utils.GET_http((VKutils.getInstance()).vkReq("users.get", "user_ids=" + id)));
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
            public string from_id;
            public string body;
            public long message_id;
            public Message(string user,  string text, string from, long id = 0)
            { user_id = user; body = text; message_id = id; from_id = from; }
            private static Dictionary<long, Message> messages = new Dictionary<long, Message>();
            public static Message EMPTY = new Message("", "","");
            private Message(XmlNode xml)
            {
                user_id = xml.SelectSingleNode("user_id").InnerText;
                from_id = xml.SelectSingleNode("from_id").InnerText;
                body = xml.SelectSingleNode("body").InnerText;
                message_id = long.Parse(xml.SelectSingleNode("date").InnerText);
            }
            public static Message addNewMessage(XmlNode xml)
            {
                Message m = new Message(xml);
                addQuestion(m);
                messages[m.message_id] = m;
                return m;
            }
            public static string send(string to, string text)
            {
                return utils.GET_http((VKutils.getInstance()).vkReq("messages.send", "user_id=" + to + "&message=" + text));
            }
            public override String ToString()
            {
                return user_id + ": " + body;
            }
            private static Dictionary<String, HashSet<Message>> messagesWithKeyWords = new Dictionary<string, HashSet<Message>>();
            public HashSet<String> getKeyWords()
            {
                HashSet<String> keywords = new HashSet<string>(this.body.Split(' '));
                
                    

                /*
                if (this.body.Contains("привіт"))
                {
                    keywords.Add("привіт");
                    Console.WriteLine("+1");
                }*/
                return keywords;
            }
            public static void addQuestion(Message m)
            {
                foreach (String key in m.getKeyWords())
                {
                    if (!messagesWithKeyWords.ContainsKey(key))
                        messagesWithKeyWords[key] = new HashSet<Message>();
                    messagesWithKeyWords[key].Add(m);
                }
            }
            public static HashSet<Message> getQuestions(HashSet<String> keywords)
            {
                bool first = true;
                HashSet<Message> result = new HashSet<Message>();
                foreach (String key in keywords)
                {

                    if (!messagesWithKeyWords.ContainsKey(key))
                    {
                        result.Clear();
                        return result;
                    }

                    if (first)
                    {
                        result = messagesWithKeyWords[key];
                        first ^= true;
                    }

                    //result = result.Intersect(messagesWithKeyWords[key]) as HashSet<Message>;
                    result = new HashSet<Message>(result.Intersect(messagesWithKeyWords[key]));
                }
                return result;
            }
        }
        public List<Message> getMessagesFromXMLString(string xmlString) 
        {
            try {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.SelectSingleNode("response").SelectSingleNode("items");
                XmlNodeList messages = node.SelectNodes("message");
                List<Message> result = new List<Message>();
                foreach (XmlNode messnode in messages)
                {

                    result.Add(Message.addNewMessage(messnode));
//                # result.Add(messnode.SelectSingleNode("from_id").InnerText + ": " + messnode.SelectSingleNode("body").InnerText);

                }
                return result; }
            catch (Exception e)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                if (doc.SelectSingleNode("error") != null)
                {
                    List<Message> result = new List<Message>();
                    result.Add(Message.EMPTY);
                    return result;
                }

                return new List<Message>();
            }
        }
        public List<Message> getCompleteDialogue(String user_id)
        {
            int offset = 0;
            List<Message> result = new List<Message>();
            

            while (true)
            {
                List<Message> tempresult = getMessagesFromXMLString(utils.GET_http(vkReq("messages.getHistory", "offset="+offset.ToString() + "&count=200&user_id="+ Settings1.Default.send_id)));
                // MessageBox.Show(tempresult.Count.ToString());
                if (result.Count > 1000)
                    break;
                if (tempresult.Count == 0)
                    break;
                if (tempresult.Count==1 && tempresult[0]==Message.EMPTY)
                {
                    Thread.Sleep(300);
                    continue;
                }
                foreach (Message s in tempresult)
                    result.Add(s);
                offset += tempresult.Count;
            }
            MessageBox.Show("FINISHED");
            return result;


        }
    }
}
