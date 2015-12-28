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
    class TicTacToe : LogicProgram
    {
        private static TicTacToe instance;

        private int state;
        Thread mythread;

        private TicTacToe()
        {
            enabled = false; isReading = false; initPhrase = "tic-tac-toe"; outputCount = 0;
            inputStream = new Queue<string>(); outputStream = new Queue<string>();



        }
        public static TicTacToe getInstance()
        {
            if (instance == null)
                instance = new TicTacToe();
            return instance;
        }
        public Field field;
        public class Field
        {
            private int[,] f;
            int left;
            public Field()
            {
                f = new int[3, 3];
                left = 9;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        f[i, j] = 0;
            }
            public Field(int[,] a, int l = 9)
            {
                f = a;
                left = l;
            }

            public string show()
            {
                string s = "-------\n";
                for (int i = 0; i < 3; i++)
                {
                    s += "|";
                    for (int j = 0; j < 3; j++)
                        if (f[i, j] == 0)
                            s += ".|";
                        else
                            if (f[i, j] == 1)
                                s += "X|";
                            else s += "O|";
                    s += "\n";
                }
                s += "-------\n";
                return s;
            }

            public int canWin(int who)
            {

                for (int i = 0; i < 3; i++)
                {
                    if (f[0, i] == who && f[1, i] == who && f[2, i] == 0)
                    {
                        return 2 * 3 + i;

                    }
                    if (f[0, i] == 0 && f[1, i] == who && f[2, i] == who)
                    {
                        return 0 * 3 + i;

                    }
                    if (f[0, i] == who && f[1, i] == 0 && f[2, i] == who)
                    {
                        return 1 * 3 + i;

                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (f[i, 0] == who && f[i, 1] == who && f[i, 2] == 0)
                    {
                        return i * 3 + 2;

                    }
                    if (f[i, 0] == 0 && f[i, 1] == who && f[i, 2] == who)
                    {
                        return i * 3 + 0;

                    }
                    if (f[i, 0] == who && f[i, 1] == 0 && f[i, 2] == who)
                    {
                        return i * 3 + 1;

                    }
                }
                if (f[0, 0] == who && f[1, 1] == who && f[2, 2] == 0)
                    return 2 * 3 + 2;
                if (f[0, 0] == who && f[1, 1] == 0 && f[2, 2] == who)
                    return 1 * 3 + 1;
                if (f[0, 0] == 0 && f[1, 1] == who && f[2, 2] == who)
                    return 0 * 3 + 0;
                if (f[2, 0] == 0 && f[1, 1] == who && f[0, 2] == who)
                    return 2 * 3 + 0;
                if (f[2, 0] == who && f[1, 1] == 0 && f[0, 2] == who)
                    return 1 * 3 + 1;
                if (f[2, 0] == who && f[1, 1] == who && f[0, 2] == 0)
                    return 0 * 3 + 2;


                return -1;
            }
            public bool putX(int x, int y)
            {
                if (!(x >= 0 && x < 3 && y >= 0 && y < 3))
                    return false;
                if (f[x, y] > 0)
                    return false;
                f[x, y] = 1;
                return true;

            }
            public bool putO(int x, int y)
            {
                if (!(x >= 0 && x < 3 && y >= 0 && y < 3))
                    return false;
                if (f[x, y] > 0)
                    return false;
                f[x, y] = 2;
                return true;
            }
            public int checkWinner()
            {
                for (int i = 0; i < 3; i++)
                {
                    if (f[0, i] == f[1, i] && f[1, i] == f[2, i])
                        return f[0, i];
                    if (f[i, 0] == f[i, 1] && f[i, 1] == f[i, 2])
                        return f[i, 0];

                }
                if (f[0, 0] == f[1, 1] && f[2, 2] == f[1, 1])
                    return f[0, 0];
                if (f[2, 0] == f[1, 1] && f[1, 1] == f[0, 2])
                    return f[1, 1];
                return 0;

            }
        }
        class Player : IComparable
        {
            private static Dictionary<string, Player> players = new Dictionary<string, Player>();

            public string name;
            public int score;
            private static int maxScores = 10;
            private static SortedSet<Player> topScore = new SortedSet<Player>();
            public int CompareTo(Object obj)
            {
                if (obj == null) return 1;
                Player p2 = obj as Player;
                if (p2 != null)
                    return -this.score.CompareTo(p2.score);
                else throw new ArgumentException("Invalid args");
            }
            private Player(string n)
            {
                name = n;
                score = 0;
                topScore.Add(this);
                //   top_scores.Add(new Tuple<int, Player>(0, this));
            }
            public static Player getValueOf(string name)
            {
                if (!players.ContainsKey(name))
                    players[name] = new Player(name);

                return players[name];

            }
            public void win(int moves)
            {
                //top_scores.First();
                score += 10 - moves;


            }
            public void lose()
            {
                score -= 10;
            }
            public void tie()
            {
                score -= 2;
            }
            public static string TopScoresGet()
            {
                int n = Math.Min(maxScores, topScore.Count);
                int q = 0;
                string s = "";
                foreach (Player pl in topScore)
                {
                    q++;
                    s += q.ToString() + ") " + pl.name + " " + pl.score + "\n";


                    if (q == n) break;
                }
                return s;
            }
            public static void Clear()
            {
                players.Clear();
                topScore.Clear();
            }

        }

        bool moving;
        Player player;
        private void makePlayerMove()
        {
            while (true)
            {
                int x, y;
                while (true)
                {
                    try
                    {
                        string s = read();
                        x = Int16.Parse(s.Split(' ')[0]);
                        y = Int16.Parse(s.Split(' ')[1]);
                        x--; y--;
                        break;
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                if (field.putX(x, y))
                    break;
            }
            moving = false;
        }
        private void makeBotMove()
        {
            int x = field.canWin(2);
            if (x != -1)
            {
                field.putO(x / 3, x % 3);
                moving = true;
                return;
            }
            x = field.canWin(1);
            if (x != -1)
            {
                field.putO(x / 3, x % 3);
                moving = true;
                return;
            }
            if (field.putO(1, 1))
            { moving = true; return; }
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (field.putO(i, j))
                    {
                        moving = true;
                        return;
                    }
        }
        protected override void think()
        {
            enabled = true;
            string s;
            getPlayer();
            while (enabled)
            {
                println("What do you want to do? (topscores show/ topscores clear / play / exit )");

                String[] args = read().Split(' ');
                if (args[0] == "exit")
                { exit(); return; }
                if (args[0] == "play")
                {
                    println("Good, now, let's play");
                    while (enabled)
                    {
                        playSingleGame();
                        println("Wanna play another match?(YES/NO)");
                        s = read().ToLower();
                        if (s != "yes")
                            break;
                    }
                }
                if (args[0] == "topscores")
                {
                    if (args[1] == "show")
                        println(Player.TopScoresGet());
                    if (args[1] == "clear")
                    {
                        {
                            Player.Clear();
                            getPlayer();
                        }
                    }
                }
            }
            println("Have a nice day!");

            enabled = false;

        }
        void playSingleGame()
        {
            println("Who will turn first? You? (YES/NO)");
            string s = read().ToLower();
            field = new Field();
            int left = 9;
            moving = (s == "yes");
            int winner;
            while (left > 0)
            {
                println(field.show());
                if (moving)
                    makePlayerMove();
                else
                    makeBotMove();
                left--;
                winner = field.checkWinner();
                if (winner > 0)
                {

                    if (winner == 1)
                    {
                        println("You win in " + (9 - left).ToString() + " moves");
                        player.win(9 - left);
                    }
                    else
                    {
                        println("You lose :C");
                        player.lose();
                    }
                    break;
                }
                if (left == 0)
                {
                    println("Tie");
                    player.tie();
                }

            }
            println(field.show());
        }
        void exit()
        {
            println("Have a nice day :)");
            enabled = false;
        }
        void getPlayer()
        {
            println("What's your name?");
            string s = read();
            player = Player.getValueOf(s);
        }
    }

}

