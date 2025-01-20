using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Proyecto3
{
    public class Server
    {
        private readonly int port = 5000;
        private static readonly string LogFilePath = "operations_log.csv";

        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            //Console.WriteLine($"Servidor iniciado en el puerto {port}...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Cliente conectado...");
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private void HandleClient(object? clientObj)
        {
            if (clientObj is not TcpClient client) return;

            using NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                while (true)
                {
                    string? infixExpression = reader.ReadLine();
                    if (string.IsNullOrEmpty(infixExpression)) break;

                    Console.WriteLine($"Expresión recibida: {infixExpression}");

                    string postfixExpression = Program.ConvertToPostfix(infixExpression);
                    ExpressionTree tree = new ExpressionTree(postfixExpression);
                    double result = tree.Evaluate();

                    LogOperation(infixExpression, result);
                    writer.WriteLine($"Resultado: {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
            }
            finally
            {
                client.Close();
                // Console.WriteLine("Cliente desconectado.");
            }
        }

        private void LogOperation(string expression, double result)
        {
            string logEntry = $"{DateTime.Now},{expression},{result}";
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            Console.WriteLine($"Operación registrada: {logEntry}");
        }
    }
}
