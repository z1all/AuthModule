using Microsoft.Extensions.DependencyInjection;
using AuthModule.Server;

var services = new ServiceCollection();
services.AddApplicationServices();

var serviceProvider = services.BuildServiceProvider();
serviceProvider.UseApplicationServices();


/*
TcpListener server = null;
try
{
    // Указываем IP адрес и порт, на котором будет работать сервер
    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    int port = 7000;

    // Создаем TcpListener для прослушивания указанного IP адреса и порта
    server = new TcpListener(ipAddress, port);

    // Начинаем прослушивание клиентов
    server.Start();

    Console.WriteLine("Сервер запущен...");

    // Бесконечный цикл для прослушивания клиентов
    while (true)
    {
        // Принимаем подключение от клиента
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Подключен новый клиент.");

        // Создаем новый поток для обработки подключившегося клиента
        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
        Console.WriteLine(1);
        clientThread.Start(client);
        Console.WriteLine(2);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    // Останавливаем TcpListener
    server.Stop();
}

static void HandleClient(object obj)
{
    Console.WriteLine(1222);
    TcpClient client = (TcpClient)obj;

    // Получаем поток для чтения и записи данных от клиента
    NetworkStream stream = client.GetStream();

    // Буфер для хранения полученных от клиента данных
    byte[] data = new byte[256];

    // Читаем данные от клиента
    while (true)
    {
        // Читаем данные от клиента в буфер
        int bytesRead = stream.Read(data, 0, data.Length);

        // Преобразуем данные в строку и выводим на консоль
        string message = System.Text.Encoding.UTF8.GetString(data, 0, bytesRead);
        Console.WriteLine("Получено сообщение: {0}", message);

        // Отправляем ответ клиенту
        byte[] response = System.Text.Encoding.UTF8.GetBytes("Сообщение получено");
        stream.Write(response, 0, response.Length);
    }

    // Закрываем соединение с клиентом
    client.Close();
}
*/
//Console.WriteLine("Hello, World!");

//TcpListener serverSocket = new(IPAddress.Any, 7000);
//serverSocket.Start();

//serverSocket.BeginAcceptTcpClient((a) =>
//{
//    Console.WriteLine(1);
//    Console.ReadKey();
//}, serverSocket);

////TcpClient clientSocket = await serverSocket.AcceptTcpClientAsync();



////NetworkStream stream = clientSocket.GetStream();

//////byte[] bytes = new byte[256];
////byte[] bytes = { 32, 21, 123, 123, 1, 4, 78, 222 };


////await stream.WriteAsync(bytes, 0, bytes.Length);
////await stream.FlushAsync();

////clientSocket.Close();
////serverSocket.Stop();

//Console.WriteLine("ServerStop");
//Console.ReadKey();

