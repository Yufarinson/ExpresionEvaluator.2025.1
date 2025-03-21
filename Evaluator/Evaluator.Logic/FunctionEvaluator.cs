using System.Globalization;
using System.Text;

namespace Evaluator.Logic;

public class FunctionEvaluator
{
    public static double Evalute(string infix)
    {
        var postfix = ToPostfix(infix);
        return Calculate(postfix);
    }

    private static double Calculate(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix)
        {
            if (IsOperator(token))
            {
                var operator2 = stack.Pop();
                var operator1 = stack.Pop();
                stack.Push(Result(operator1, token[0], operator2));
            }
            else
            {
                // Se convierte el token a double (admite decimales y números de varios dígitos)
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static double Result(double operator1, char op, double operator2)
    {
        return op switch
        {
            '+' => operator1 + operator2,
            '-' => operator1 - operator2,
            '*' => operator1 * operator2,
            '/' => operator1 / operator2,
            '^' => Math.Pow(operator1, operator2),
            _ => throw new Exception("Invalid expression"),
        };
    }

    private static List<string> ToPostfix(string infix)
    {
        var output = new List<string>();
        var stack = new Stack<char>();
        var number = new StringBuilder();

        for (int i = 0; i < infix.Length; i++)
        {
            char ch = infix[i];

            if (char.IsDigit(ch) || ch == '.')
            {
                // Acumula dígitos y el punto decimal
                number.Append(ch);
            }
            else
            {
                if (number.Length > 0)
                {
                    // Agrega el número completo acumulado a la salida
                    output.Add(number.ToString());
                    number.Clear();
                }

                if (ch == '(')
                {
                    stack.Push(ch);
                }
                else if (ch == ')')
                {
                    // Desapila hasta encontrar '('
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        output.Add(stack.Pop().ToString());
                    }
                    if (stack.Count > 0 && stack.Peek() == '(')
                    {
                        stack.Pop();
                    }
                }
                else if (IsOperator(ch))
                {
                    while (stack.Count > 0 && stack.Peek() != '(' && PriorityExpression(ch) <= PriorityStack(stack.Peek()))
                    {
                        output.Add(stack.Pop().ToString());
                    }
                    stack.Push(ch);
                }
            }
        }
        if (number.Length > 0)
        {
            output.Add(number.ToString());
        }
        while (stack.Count > 0)
        {
            output.Add(stack.Pop().ToString());
        }
        return output;
    }

    private static int PriorityStack(char op)
    {
        return op switch
        {
            '^' => 3,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 0,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static int PriorityExpression(char op)
    {
        return op switch
        {
            '^' => 4,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 5,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static bool IsOperator(char ch) => "()^*/+-".IndexOf(ch) >= 0;
    private static bool IsOperator(string token) => token.Length == 1 && IsOperator(token[0]);

}