using System.Text.RegularExpressions;
namespace Proyecto3 {
    // Clase que encapsula el árbol de expresión
    class ExpressionTree
    {
        private ExpressionNode root;

        // Constructor que recibe la notación postfija y construye el árbol
        public ExpressionTree(string postfix)
        {
            root = BuildTree(postfix);
        }

        // Método público para evaluar el árbol
        public double Evaluate()
        {
            if (root == null)
                throw new InvalidOperationException("El árbol de expresión está vacío.");
            return root.Evaluate();
        }

        // Construir el árbol de expresión desde la notación postfija
        // Método actualizado para construir el árbol de expresión
        private ExpressionNode BuildTree(string postfix)
        {
            string[] tokens = postfix.Split(" ");
            Stack<ExpressionNode> stack = new Stack<ExpressionNode>();

            foreach (string token in tokens)
            {
                // Detectar números (incluyendo negativos como "-5")
                if (double.TryParse(token, out double number))
                {
                    stack.Push(new NumberNode(number));
                }
                else if (Regex.IsMatch(token, @"[+\-*/%&|^~**]")) // Operadores válidos
                {
                    if (token == "~") // NOT es un operador unario
                    {
                        if (stack.Count < 1)
                            throw new InvalidOperationException("Operando insuficiente para el operador '~'.");

                        ExpressionNode left = stack.Pop();
                        stack.Push(new LogicalNode(token, left, null));
                    }
                    else if (token == "&" || token == "|" || token == "^")
                    {
                        if (stack.Count < 2)
                            throw new InvalidOperationException($"Operando insuficiente para el operador '{token}'.");

                        ExpressionNode right = stack.Pop();
                        ExpressionNode left = stack.Pop();
                        stack.Push(new LogicalNode(token, left, right));
                    }
                    else // Operadores matemáticos
                    {
                        if (stack.Count < 2)
                            throw new InvalidOperationException($"Operando insuficiente para el operador '{token}'.");

                        ExpressionNode right = stack.Pop();
                        ExpressionNode left = stack.Pop();
                        stack.Push(new OperatorNode(token, left, right));
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Token desconocido: {token}");
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("La expresión postfija es inválida.");

            return stack.Pop(); // La raíz del árbol
        }
    }
}