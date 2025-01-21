namespace Proyecto3 {
    // Clase base para nodos del árbol
    public abstract class ExpressionNode
    {
        public abstract double Evaluate();
    }

    // Nodo que representa un número (hoja)
    public class NumberNode : ExpressionNode
    {  
        private double value;

        public NumberNode(double value)
        {
            this.value = value;
        }

        public override double Evaluate()
        {
            return value;
        }
    }


    // Nodo que representa un operador (+, -, *, /...)
    public class OperatorNode : ExpressionNode
    {
        private readonly string operatorChar;
        private ExpressionNode left;
        private ExpressionNode right;

        public OperatorNode(string operatorChar, ExpressionNode left, ExpressionNode right)
        {
            this.operatorChar = operatorChar;
            this.left = left;
            this.right = right;
        }

        // La función Evaluate es Recursiva entre sus distintas clases comprobando el tipo de dato.
        // El caso base se alcanza en Number Node, donde se verifica que el nodo es un número.

        public override double Evaluate()
        {
            double leftValue = left.Evaluate();
            double rightValue = right.Evaluate();

            double? leftValueNullable = left?.Evaluate();
            double? rightValueNullable = right?.Evaluate();


            return operatorChar switch
            {
                "+" => leftValue + rightValue,
                "-" => leftValue - rightValue,
                "*" => leftValue * rightValue,
                "/" => leftValue / rightValue,
                "%" => leftValue * rightValue / 100,
                "**" => Math.Pow(leftValue, rightValue),
                _ => throw new InvalidOperationException($"Operador desconocido: {operatorChar}")
            };
        }
    }


    public class LogicalNode : ExpressionNode
    {
        private readonly string operatorChar;
        private readonly ExpressionNode left;
        private readonly ExpressionNode right;

        public LogicalNode(string operatorChar, ExpressionNode left, ExpressionNode right)
        {
            this.operatorChar = operatorChar;
            this.left = left;
            this.right = right;
        }

        public override double Evaluate()
        {
            bool leftValue = left.Evaluate() == 1;  // Convertir de 1/0 a true/false
            bool rightValue = right != null ? right.Evaluate() == 1 : false;

            return operatorChar switch
            {
                "&" => leftValue && rightValue ? 1 : 0,
                "|" => leftValue || rightValue ? 1 : 0,
                "^" => leftValue ^ rightValue ? 1 : 0,
                "~" => !leftValue ? 1 : 0,
                _ => throw new InvalidOperationException($"Operador lógico desconocido: {operatorChar}")
            };
        }
    }
}