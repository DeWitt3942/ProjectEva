using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
namespace WindowsFormsApplication1
{
    class Logic
    {
        private static Logic instance;
        Queue<Tuple<String, String>> msgq, qq;
        int level  = 0;
        Tuple<string, string> emptyTuple;
        Calc logiccalc;
        TicTacToe logictictactoe;
        Random r = new Random();
        Boolean tictactoe = false;
        public int angryness = 0;
        public string to;
        LogicProgram[] programs;
        LogicProgram current;
        LogicBase logicBase;
        Thread thread;
        private Logic()
        {
            init();
        }
        public static Logic getInstance()
        {
            if (instance==null)
                instance = new Logic();
            return instance;
        }
        
        private void init()
        {
            
            thread = new Thread(think);
            msgq = new Queue<Tuple<string, string>>();
            qq = new Queue<Tuple<string, string>>();
            emptyTuple = new Tuple<string, string>("", "");
            programs = new LogicProgram[2];
            programs[0] = Calc.getInstance();
            programs[1] = TicTacToe.getInstance();
            current = null;
            thread.Start();
            logicBase = LogicBase.getInstance();


        }
        void think()
        {

            string ss = "";
            while (true)
            {
                //Logic flow cylcle
                //if (tictactoe.enabled)
                
                if (current == null)
                {
                    for (int i = 0; i < programs.Length; i++) //getLoadedProgram
                        if (programs[i].enabled) current = programs[i];
                }
                if (current !=null)
                {
                    if (!current.enabled)
                    { current = null; continue; }
                    while (current.outputCount>0)
                        msgq.Enqueue(new Tuple<string, string>(to, current.getResult()));

                    if (current.isReading)
                        if (qq.Count > 0)
                            current.pushResult(qq.Dequeue().Item2);
                    continue;
                }
                if (qq.Count > 0)
                {
                        to = qq.Peek().Item1;
                        
                        Console.WriteLine("Searching candidate:" + qq.Peek().Item2);
                        for (int i = 0; i < programs.Length; i++)
                            if (programs[i].initPhrase == qq.Peek().Item2)
                            { programs[i].enable(); qq.Dequeue(); current = programs[i];
                            break;
                            }
                        
                        if (current == null)
                        {
                            Console.WriteLine("not found candidate :C, trying ugly thing");
                            ss = logicBase.getAnswer(qq.Dequeue().Item2);
                            if (ss != "")
                                msgq.Enqueue(new Tuple<string, string>(to, ss));
                        }
                        
                    }


                    
            }

        }
   
    
        public Tuple<String,String> getAns()
        {
            if (msgq.Count == 0)
                return emptyTuple;
            Console.WriteLine(msgq.Peek());
            return msgq.Dequeue();
        }
        public void push(string from, string qu)
        {
            to = from;
            Console.WriteLine("Pushing:" + qu);
            qq.Enqueue(new Tuple<string, string>(from, qu));
        }
        
    }
}
