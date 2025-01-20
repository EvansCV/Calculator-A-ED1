using System.Net.Sockets;
using System.Text;

namespace Proyecto3
{
    public class Client
    {
        private readonly string serverIp = "127.0.0.1";
        private readonly int port = 5000;
        private static int _nextId = 1; // Contador estático para generar IDs únicos
        public int Id { get; private set; } // ID único del cliente

        // Constructor
        public Client()
        {
            Id = _nextId++; // Asigna un ID único al cliente y luego incrementa el contador
        }

        public void Start()
        {

            try
            {
                Console.WriteLine("Conectado al servidor. Escriba una expresión matemática (o 'salir' para terminar):");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public string SendExpression(string expression)
        {
            using TcpClient client = new TcpClient(serverIp, port);
            using NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            writer.WriteLine(expression);
            return reader.ReadLine() ?? "Sin respuesta del servidor.";
        }
    }
}