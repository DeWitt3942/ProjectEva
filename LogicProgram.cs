using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace WindowsFormsApplication1
{
    abstract class LogicProgram
    {
        private Thread newThread;
        protected Queue<string> inputStream, outputStream;
        public int outputCount;
        public String initPhrase;
        public bool enabled = false;
        public bool isReading;
        public virtual void init() { }
        public void enable() {
            newThread = new Thread(think);
            newThread.Start();
        }

        protected virtual void think() { }
        protected string read()
        {
            isReading = true;
            while (inputStream.Count == 0) ;
            isReading = false;
            return inputStream.Dequeue();
            
        }
        protected void println(string S)
        {
            outputCount++;
            outputStream.Enqueue(S);
        }
        public String getResult()
        {
            if (outputStream.Count == 0)
                return "";
            outputCount--;
            return outputStream.Dequeue();
        }
        public void pushResult(string s)
        {
            inputStream.Enqueue(s);

        }
    }
}
