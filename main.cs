using System.Diagnostics.Eventing.Reader;
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
                   // Ejecutar la interfaz gráfica en un nuevo hilo para evitar conflictos con la consola
                    Thread guiThread = new Thread(() =>
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Client client = new Client(); // Crear una instancia del cliente
                        Console.WriteLine($"Cliente creado con ID: {client.Id}");
                        Application.Run(new CalculatorForm(client)); // Iniciar la interfaz gráfica
                    });
                    guiThread.SetApartmentState(ApartmentState.STA); // Requerido para la compatibilidad con Windows Forms
                    guiThread.Start();
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

    // Método para convertir de notación infija a postfija
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
            char anterior = i > 0 ? cadena[i - 1] : '\0';

            // Agregar el carácter actual a la nueva cadena.
            newChain += actual;

            // Condición para insertar un espacio:
            // 1. Si el carácter actual es un paréntesis de apertura o un operador,
            //    y el siguiente no es un espacio ni un paréntesis de cierre.
            if (actual == '(' && siguiente != ' ' && siguiente != ')')
            {
                newChain += ' ';
            }
            // 2. Si el carácter actual es un número o coma y el siguiente no es un número, coma o espacio.
            else if ((char.IsDigit(actual) || actual == ',') && siguiente != ' ' && !char.IsDigit(siguiente) && siguiente != ',')
            {
                newChain += ' ';
            }
            // 3. Si el carácter actual no es un número, coma ni espacio, y el siguiente es un número o coma.
            else if (!char.IsDigit(actual) && actual != ',' && actual != ' ' && (char.IsDigit(siguiente) || siguiente == ','))
            {
                newChain += ' ';
            }
            // 4. Si el carácter actual es un operador o un paréntesis de cierre,
            //    y el siguiente no es un espacio ni un paréntesis de apertura.
            else if ((actual == ')' || "+*/%&|^~".Contains(actual)) && siguiente != ' ' && siguiente != '(')
            {
                newChain += ' ';
            }
            // 5. Manejar números decimales negativos: si el carácter actual es un signo menos (-),
            //    y es el primer carácter o viene después de un operador o un paréntesis de apertura,
            //    y el siguiente carácter es un dígito.
            else if (actual == '-' && (i == 0 || "(+-*/%&|^~".Contains(anterior)) && char.IsDigit(siguiente))
            {
                continue; // No insertar espacio aquí para números negativos.
            }
            // 6. Paréntesis seguido de otro paréntesis
            else if (actual == ')' && siguiente == '(')
            {
                newChain += ' ';
            }
        }
        Console.WriteLine(newChain);    
        // Eliminar espacios innecesarios al inicio y al final.
        return newChain.Trim();
    }
}







