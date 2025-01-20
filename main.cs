using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Proyecto3;

// Este código implementa el uso de la biblioteca Collection.Generics para el uso de pilas, colas,
// entre otras estructuras de datos que se requieren de forma genérica y su implementación es más
// sencilla de esta manera.
class Program
{   

    static void Main(string[] args)
    {
        // Iniciar el servidor automáticamente en un hilo separado
        Thread serverThread = new Thread(StartServer);
        serverThread.IsBackground = true;
        serverThread.Start();
        Console.WriteLine("Servidor iniciado automáticamente en segundo plano.");

        // Mostrar el menú al usuario
        while (true)
        {
            Console.WriteLine("\nSeleccione una opción:");
            Console.WriteLine("1. Iniciar Cliente");
            Console.WriteLine("2. Evaluar expresión localmente");
            Console.WriteLine("3. Salir");
            Console.Write("Opción: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Se inicia el cliente creando su instancia y llamando al método Start
                    Console.WriteLine("Iniciando cliente...");
                    Client client = new Client();
                    client.Start();
                    //CalculatorForm cal = new CalculatorForm(client);
                    // Se crea una nueva gui, donde la cual requiere del cliente para interactuar con el mismo.
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new CalculatorForm(client));
                    break;

                case "2":
                    Console.WriteLine("Ingrese la expresión en notación infija:");
                    string infix = Console.ReadLine() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(infix))
                    {
                        Console.WriteLine("Expresión inválida.");
                        break;
                    }

                    string logicalOperatorsPattern = @"[&|~^]";
                    string postfix = ConvertToPostfix(infix);
                    Console.WriteLine($"Postfija: {postfix}");

                    ExpressionTree tree = new ExpressionTree(postfix);

                    if (Regex.IsMatch(infix, logicalOperatorsPattern))
                    {
                        Console.WriteLine("Evaluando expresión lógica...");
                        double result = tree.Evaluate();
                        Console.WriteLine($"Resultado lógico: {result == 1}");
                    }
                    else
                    {
                        Console.WriteLine("Evaluando expresión matemática...");
                        double result = tree.Evaluate();
                        Console.WriteLine($"Resultado matemático: {result}");
                    }
                    break;

                case "3":
                    Console.WriteLine("Saliendo...");
                    return;

                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }
        }
    }

    private static void StartServer()
    {
        Server server = new Server();
        server.Start();
    }

    public static string ConvertToPostfix(string infix)
    {   
        string newchain = SepararEnEspacios(infix);
        string[] tokens = newchain.Split(" ");
        Stack<string> operators = new Stack<string>();
        List<string> output = new List<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                operators.Pop();
            }
            else
            {
                while (operators.Count > 0 && operators.Peek() != "(" &&
                       GetPrecedence(operators.Peek()) >= GetPrecedence(token) &&
                       !IsRightAssociative(token))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return string.Join(" ", output);
    }

    private static int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "%" => 2,
            "**" => 3,
            "&" => 1,
            "|" => 1,
            "^" => 2,
            "~" => 3,
            _ => 0
        };
    }

    private static bool IsRightAssociative(string op)
    {
        return op == "**";
    }

    private static string SepararEnEspacios(string cadena) 
    {
        if (string.IsNullOrWhiteSpace(cadena)) 
        {
            return string.Empty; // Manejar cadenas vacías o solo espacios.
        }

        string newChain = "";
        for (int i = 0; i < cadena.Length; i++) 
        {
            char actual = cadena[i];
            char siguiente = i < cadena.Length - 1 ? cadena[i + 1] : '\0';

            // Agregar el carácter actual a la nueva cadena.
            newChain += actual;

            // Condición para insertar un espacio:
            // 1. Si el carácter actual es un paréntesis de apertura o un operador,
            //    y el siguiente no es un espacio ni un paréntesis de cierre.
            if (actual == '(' && siguiente != ' ' && siguiente != ')')
            {
                newChain += ' ';
            }
            // 2. Si el carácter actual es un número y el siguiente no es un número o espacio.
            else if (char.IsDigit(actual) && siguiente != ' ' && !char.IsDigit(siguiente))
            {
                newChain += ' ';
            }
            // 3. Si el carácter actual no es un número ni un espacio, y el siguiente es un número.
            else if (!char.IsDigit(actual) && actual != ' ' && char.IsDigit(siguiente))
            {
                newChain += ' ';
            }
            // 4. Si el carácter actual es un operador o un paréntesis de cierre,
            //    y el siguiente no es un espacio ni un paréntesis de apertura.
            else if ((actual == ')' || "+-*/%&|^~".Contains(actual)) && siguiente != ' ' && siguiente == '(')
            {
                newChain += ' ';
            }
            // 5. Paréntesis seguido de otro paréntesis
            else if (actual == ')' && siguiente == ')')
            {
                newChain += ' ';
            }
             
        }
        Console.WriteLine(newChain);    
        // Eliminar espacios innecesarios al inicio y al final.
        return newChain.Trim();
    }
}







