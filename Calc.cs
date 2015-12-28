using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace WindowsFormsApplication1
{
    class Calc : LogicProgram
    {
        static Calc instance;
        
        private Calc()
        { enabled = false; isReading = false; initPhrase = "calc"; outputCount = 0; inputStream = new Queue<string>(); outputStream = new Queue<string>(); }
        
        public static Calc getInstance()
        {
            if (instance == null)
                instance = new Calc();
            return instance;
        }

        protected override void think()
        {
            enabled = true;
            //println("Calc enabled");
            String expression = read();
            try
            {
                expression = expression + "=" + calc(expression).ToString();
            }
            catch
            {
                expression = "Error :C";
            }
            println(expression);         enabled = false;
        }


        Stack<Double> operands = new Stack<Double>();
        Stack<Char> operators = new Stack<Char>();
        bool isOperator(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '^' || c == '!');
        }
        int priority(char c)
        {
            if (c > 150) return 8;
            return (c == '+' || c == '-') ? 1 : ((c == '*' || c == '/' || c == '%' || c == '^') ?
                    2 : -1);
        }
        bool whiteSpace(char c)
        {
            return (c == ' ' || c == '	');
        }
        bool isScobe(char c)
        {
            return (c == '(' || c == ')');
        }
        bool isOperand(char c)
        {
            return (!whiteSpace(c) && !isOperator(c) && !isScobe(c));
        }
        bool isUnary(char c)
        {
            return (c == '+' || c == '-');
        }
        bool right_assoc(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '%' || c == '^');
        }
        double factorial(double x)
        {
            double s = 1;
            for (int i = 1; i <= x; i++)
                s *= i;
            return s;
        }
        void doOperation(char op)
        {
            if (op > 150)
            {
                double l = operands.Pop();

                switch (op - 150)
                {
                    case '+': operands.Push(l); break;
                    case '-': operands.Push(-l); break;
                    case '!': operands.Push(factorial(l)); break;
                }

            }
            else
            {

                double r = operands.Pop();
                double l = operands.Pop();
                switch (op)
                {
                    case '+': operands.Push(l + r); break;
                    case '-': operands.Push(l - r); break;
                    case '*': operands.Push(l * r); break;
                    case '/': operands.Push((double)l / r); break;
                    case '%': operands.Push(l - r * (double)l / r); break;
                    case '^': operands.Push(Math.Pow(l, r)); break;

                }
            }
        }
        public double calc(String s)
        {
            operands.Clear();
            operators.Clear();

            bool unary = true;
            for (int i = 0; i < s.Length; i++)
                if (!whiteSpace(s[i]))
                {
                    //cout<<i<<endl;
                    if (s[i] == '(')
                    {
                        operators.Push('(');
                        unary = true;
                    }
                    else if (s[i] == ')')
                    {
                        while (operators.Peek() != '(')
                            doOperation(operators.Pop());
                        operators.Pop();
                        unary = false;
                    }
                    else if (isOperator(s[i]))
                    {
                        char current = s[i];
                        if (unary && isUnary(current)) current += (char)150;
                        if (current == '!') current += (char)150;

                        while (!(operators.Count()==0) &&
                               (
                                (
                                    (!right_assoc(current) && priority(operators.Peek()) >= priority(current) && current > 0)
                                        ||
                                    (right_assoc(current) && priority(operators.Peek()) > priority(current) && current > 0)
                                )
                                ||
                                    (current < 0 && priority(operators.Peek()) > priority(current))
                                )
                               )
                            doOperation(operators.Pop());
                        operators.Push(current);
                        unary = false;
                    }
                    else
                    {
                        String operand = "";
                        while (i < s.Length && isOperand(s[i]))
                            operand += s[i++];

                        --i;
                        double d = Double.Parse(operand);
                        operands.Push(
                               d);
                        unary = false;
                    }
                }
            while (!(operators.Count()==0))
                doOperation(operators.Pop());
            return operands.Peek();
        }

    }
}
