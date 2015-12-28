using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsFormsApplication1
{
    class LogicBase
    {
        class Morphem
        {
            public string word;
            public List<Morphem> synonyms;
            public List<MorphemList> classes;
            public Dictionary<string, int> chars;
            private static Dictionary<string, Morphem> morphems;
            private Morphem(string w)
            {
                chars = new Dictionary<string, int>();
                classes = new List<MorphemList>();
                w = word;

            }
            public Morphem valueOf(string word)
            {
                if (!morphems.ContainsKey(word))
                    morphems[word] = new Morphem(word);
                return morphems[word];
            }

        }
        class MorphemList{
            public List<Morphem> elements;
            public Dictionary<string, int> chars;
            public MorphemList(List<Morphem> l)
            {
                l = elements;
                chars = new Dictionary<string,int>();
            }
            public void p()
            {
//   elements.Contains
            }
        }
        static LogicBase instance;
        Logic logic;
        Random r;
        private LogicBase()
        { 
            logic = Logic.getInstance();
            r = new Random();
        }
        public static LogicBase getInstance()
        {
            if (instance != null)
                instance = new LogicBase();
            return instance;
        }
        
        string[] badanswers = { "Stop swearing!" };
        string[] swears = { "fuck", "shit" };
        string[] hellos = { "привіт", "привєт", "хай", "здрастє", "hello", "здоров" };

        bool isTypeOf(string[] words, string line)
        {
            foreach (string s in words)
                if (line.Contains(s))
                    return true;
            return false;
        }
        bool swear(string phrase)
        {
            phrase = phrase.ToLower();
            for (int i = 0; i < swears.Length; i++)
                if (phrase.Contains(swears[i]))
                    return true;
            return false;

        }
        int goodness(string phrase)
        {

            if (swear(phrase)) return -5;
            return 0;
        }
        bool isHello(string s)
        {
            s = s.ToLower();

            return isTypeOf(hellos, s);
        }
       
        public string getAnswer(string question)
        {

            if (question == "") return question;


            logic.angryness -= goodness(question);
            if (logic.angryness > 20)
                return "Я тебе по IP вичислю!!!адин111";
            if (goodness(question) < 0)
                return badanswers[r.Next() % 3];

            if (r.Next() % 3 < 1)
                return "хз, шо сказати";
            if (isHello(question))

                return "Привіт, " + Form1.User.getUser(logic.to).first_name;

            if (logic.angryness > 10)
                return "Ну що ти хочеш?";
            if (logic.angryness < -10)
                return "Авжеж С:";
            if (logic.angryness > 3)
                return "Ага";
            if (Math.Abs(logic.angryness) <= 3)
                return "Ммм";
            if (logic.angryness < -3)
                return "Ясненько)";
            return "1";

        }
    }
}
