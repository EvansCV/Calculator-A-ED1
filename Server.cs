using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Proyecto3
{
    public class Server
    {
        private readonly int port = 5000;

        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"Servidor iniciado en el puerto {port}...");

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
                // Leer el ID del cliente al inicio
                string? clientId = reader.ReadLine();
                if (string.IsNullOrEmpty(clientId))
                {
                    Console.WriteLine("Error: No se recibió un ID válido del cliente.");
                    return;
                }

                Console.WriteLine($"Cliente conectado con ID: {clientId}");

                while (true)
                {
                    string? request = reader.ReadLine();
                    if (string.IsNullOrEmpty(request)) break;

                    if (request.ToLower() == "historial")
                    {
                        // Consultar historial
                        string history = GetClientHistory(clientId);
                        Console.WriteLine($"Enviando historial al cliente {clientId}:");
                        Console.WriteLine(history);

                        // Enviar el historial línea por línea
                        foreach (string line in history.Split('\n'))
                        {
                            writer.WriteLine(line);
                        }
                        writer.WriteLine("END_OF_HISTORY"); // Marca el final del historial
                    }
                    else
                    {
                        // Procesar expresión
                        Console.WriteLine($"[Cliente {clientId}] Expresión recibida: {request}");

                        string postfixExpression = Program.ConvertToPostfix(request);
                        ExpressionTree tree = new ExpressionTree(postfixExpression);
                        double result = tree.Evaluate();

                        LogOperation(clientId, request, result);
                        writer.WriteLine($"Resultado: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Cliente desconectado.");
            }
        }


        private void LogOperation(string clientId, string expression, double result)
        {
            string filePath = $"{clientId}_operations.csv";

            // Crear el archivo con encabezados si no existe
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "FechaHora,Expresion,Resultado\n");
            }

            string logEntry = $"{DateTime.Now},{expression},{result}";
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
            Console.WriteLine($"[Cliente {clientId}] Operación registrada: {logEntry}");
        }

        private string GetClientHistory(string clientId)
        {
            string filePath = $"{clientId}_operations.csv";

            if (!File.Exists(filePath))
            {
                return "No hay operaciones registradas para este cliente.";
            }

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                return $"Error al leer el historial: {ex.Message}";
            }
        }

    }
}