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
                return utils.GET_http((VKutils.getInstance()).vkReq("messages.send", "user_id=" + to + "&message=" + text));
            }
            public override String ToString()
            {
                return user_id + ": " + body;
            }
        }

        public List<String> getMessagesFromXMLString(string xmlString) 
        {
            try {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.SelectSingleNode("response").SelectSingleNode("items");
                XmlNodeList messages = node.SelectNodes("message");
                List<String> result = new List<string>();
                foreach (XmlNode messnode in messages)
                {
                    result.Add(messnode.SelectSingleNode("user_id").InnerText + ": " + messnode.SelectSingleNode("body"));

                }
                return result; }
            catch (Exception e)
            {
                return new List<String>();
            }
        }
        public List<String> getCompleteDialogue(String user_id)
        {
            int offset = 0;
            List<String> result = new List<string>();
            

            while (true)
            {
                List<String> tempresult = getMessagesFromXMLString(utils.GET_http(vkReq("messages.getHistory", "offset="+offset.ToString() + "&count=10&user_id="+ Settings1.Default.send_id)));
                // MessageBox.Show(tempresult.Count.ToString());
                if (tempresult.Count == 0)
                    break;
                foreach (String s in tempresult)
                    result.Add(s);
                offset += tempresult.Count;
            }
            MessageBox.Show("FINISHED");
            return result;


        }
    }
}
