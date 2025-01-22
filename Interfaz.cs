// Código dedicado meramente a las clases de la interfaz, el punto será utilizarlo en el cliente.

namespace Proyecto3
{
    public class CalculatorForm : Form
    {
        private readonly Client client;
        private TextBox expressionInput;
        private Button calculateButton;
        private Button modeButton;
        private Button historyButton; // Botón para consultar el historial
        private Label resultLabel;
        private Label modeText;
        private Label modeText2;
        private bool ttModeValue = true;

        public CalculatorForm(Client clientInstance)
        {
            client = clientInstance;

            // Configuración básica de la ventana
            Text = $"Calculadora de Expresiones Matemáticas - Cliente {client.Id}";
            Size = new Size(500, 400);

            // Entrada de la expresión
            Label expressionLabel = new Label
            {
                Text = "Expression",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(expressionLabel);

            expressionInput = new TextBox
            {
                Location = new Point(120, 20),
                Width = 300
            };
            Controls.Add(expressionInput);

            // Labels con información del modo que está siendo utilizado
            modeText = new Label
            {
                Location = new Point(120, 60),
                Text = "Evaluando expresiones matemáticas",
                AutoSize = true
            };
            Controls.Add(modeText);

            modeText2 = new Label
            {
                Location = new Point(20, 130),
                Text = "Si desea evaluar expresiones lógicas, haga clic en el botón de modo",
                AutoSize = true
            };
            Controls.Add(modeText2);

            // Botón para calcular
            calculateButton = new Button
            {
                Text = "Calculate",
                Location = new Point(20, 60),
                Width = 100
            };
            calculateButton.Click += CalculateButton_Click;
            Controls.Add(calculateButton);

            // Botón para cambiar de modo
            modeButton = new Button
            {
                Text = "Logic",
                Location = new Point(20, 100),
                Width = 100
            };
            modeButton.Click += ModeButton_Click;
            Controls.Add(modeButton);

            // Botón para consultar el historial
            historyButton = new Button
            {
                Text = "History",
                Location = new Point(20, 150),
                Width = 100
            };
            historyButton.Click += async (sender, e) => await HistoryButton_Click(sender, e); // Operación asíncrona
            Controls.Add(historyButton);

            // Resultado
            resultLabel = new Label
            {
                Text = "",
                Location = new Point(20, 210),
                AutoSize = true
            };
            Controls.Add(resultLabel);

            // Configuración inicial para teclas de entrada
            expressionInput.KeyPress -= ExpressionInput_KeyPress_Logic;
            expressionInput.KeyPress += ExpressionInput_KeyPress_Math;
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                string input = expressionInput.Text;

                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Ingrese una expresión válida.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Enviar datos al servidor a través del cliente
                string response = client.SendExpression(input);

                // Mostrar resultado
                resultLabel.Text = $"Result: {response}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModeButton_Click(object sender, EventArgs e)
        {
            ttModeValue = !ttModeValue;
            expressionInput.Clear();

            try
            {
                if (ttModeValue)
                {
                    modeButton.Text = "Logic";
                    modeText.Text = "Evaluando expresiones matemáticas";
                    modeText2.Text = "Si desea evaluar expresiones lógicas, haga clic en el botón de modo";

                    expressionInput.KeyPress -= ExpressionInput_KeyPress_Logic;
                    expressionInput.KeyPress += ExpressionInput_KeyPress_Math;
                }
                else
                {
                    modeButton.Text = "Math";
                    modeText.Text = "Evaluando expresiones lógicas";
                    modeText2.Text = "Si desea evaluar expresiones matemáticas, haga clic en el botón de modo";

                    expressionInput.KeyPress -= ExpressionInput_KeyPress_Math;
                    expressionInput.KeyPress += ExpressionInput_KeyPress_Logic;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task HistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Realizar la operación de consulta en un hilo separado
                string history = await Task.Run(() => client.GetHistory());

                // Mostrar el historial en un cuadro de diálogo
                MessageBox.Show(history, $"Historial del Cliente {client.Id}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar el historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExpressionInput_KeyPress_Math(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && "+-*/().%, ".IndexOf(e.KeyChar) == -1 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        private void ExpressionInput_KeyPress_Logic(object sender, KeyPressEventArgs e)
        {
            if ("10&|^~ ".IndexOf(e.KeyChar) == -1 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }
    }
}