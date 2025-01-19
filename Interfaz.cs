// Código dedicado meramente a las clases de la interfaz, el punto será utilizarlo en el cliente.

namespace Proyecto3 {
    public class CalculatorForm : Form
    {
        private readonly Client client;
        private TextBox expressionInput;
        private Button calculateButton;
        private Button modeButton;
        private Label resultLabel;
        private bool ttModeValue = true;
        public CalculatorForm(Client clientInstance)
        {
            client = clientInstance;
            // Configuración básica de la ventana
            Text = "Calculadora de Expresiones Matemáticas";
            Size = new Size(400, 300);

            // Entrada de la expresión
            Label expressionLabel = new Label();
            expressionLabel.Text = "Expression";
            expressionLabel.Location = new Point(20, 20);
            Controls.Add(expressionLabel);

            expressionInput = new TextBox();
            expressionInput.Location = new Point(120, 20);
            expressionInput.Width = 250;
            Controls.Add(expressionInput);

            // Botón para calcular
            calculateButton = new Button();
            calculateButton.Text = "Calculate";
            calculateButton.Location = new Point(20, 60);
            calculateButton.Click += CalculateButton_Click;
            Controls.Add(calculateButton);  

            // Botón para cambiar de modo
            modeButton = new Button();
            modeButton.Text = "Logic";
            modeButton.Location = new Point(20, 100);
            modeButton.Click += modeButton_Click;
            Controls.Add(modeButton);

            // Resultado
            resultLabel = new Label();
            resultLabel.Text = "Result: ";
            resultLabel.Location = new Point(20, 210);
            resultLabel.AutoSize = true;
            Controls.Add(resultLabel);

        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try {
                string input = expressionInput.Text;

                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Ingrese una expresión válida.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Formatear la entrada para enviarla al servidor
                string serverInput = "";
                for (int i = 0; i < input.Length; i++)
                {
                    serverInput += input[i];
                    // Agregar un espacio si no es el último carácter
                    if (i < input.Length - 1 && input[i + 1] != ' ')
                    {
                        serverInput += " ";
                    }
                }       

                // Enviar datos al servidor a través del cliente
                string response = client.SendExpression(serverInput);
                
                // Mostrar resultado
                resultLabel.Text = $"{response}";
            }
            catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void modeButton_Click (object sender, EventArgs e) {
            // Cambia el valor de la variable del modo
            ttModeValue = !ttModeValue;

            // Se limpia el textbox
            expressionInput.Clear(); 
            
            try {
                if (ttModeValue) {
                    Controls.Remove(modeButton);
                    modeButton.Text = "Logic";
                    Controls.Add(modeButton);
                    // Validación de entradas y control de fujo.
                    // Modo matemático
                    expressionInput.KeyPress -= expressionInput_KeyPress_Logic;
                    expressionInput.KeyPress += expressionInput_KeyPress_Math;        
                } else {
                    // Aquí más de lo mismo pero con expresiones matemáticas
                    Controls.Remove(modeButton);
                    modeButton.Text = "Math";
                    Controls.Add(modeButton);                    // Modo lógico
                    expressionInput.KeyPress -= expressionInput_KeyPress_Math;
                    expressionInput.KeyPress += expressionInput_KeyPress_Logic;
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void expressionInput_KeyPress_Math(object sender, KeyPressEventArgs e)
        {
            // Permitir números, operadores y teclas de control como retroceso
            if (!char.IsDigit(e.KeyChar) && "+-*/().% ".IndexOf(e.KeyChar) == -1 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        private void expressionInput_KeyPress_Logic(object sender, KeyPressEventArgs e)
        {
            // Permitir 1, 0, operadores lógicos y teclas de control como retroceso
            if ("10&|^~ ".IndexOf(e.KeyChar) == -1 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }
    }
}