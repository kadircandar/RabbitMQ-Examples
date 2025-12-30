using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct); // RabbitMQ’da direct_logs adlı bir Direct Exchange oluşturur (veya varsa onu kullanır).
// Direct exchange, mesajın routing key’i ile queue’nun binding key’ini birebir karşılaştırır; aynıysa gönderir, değilse hiç bakmaz bile.
// exchange: "direct_logs"  exchange adı 
// type: ExchangeType.Direct  exchange tipi direct


var queueDeclareResult = await channel.QueueDeclareAsync(); // RabbitMQ’da geçici, rastgele isimlendirilmiş bir kuyruk oluşturur.
string queueName = queueDeclareResult.QueueName;

foreach (string? severity in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "direct_logs", routingKey: severity); // Oluşturulan geçici kuyruğu direct_logs exchange’ine bağlar ve her bir severity (info, warning, error) için binding key olarak kullanır.
    // queue: queueName  bağlanacak kuyruk adı
    // exchange: "direct_logs"  bağlanacak exchange adı
    // routingKey: severity  binding key
}

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel); // AsyncEventingBasicConsumer, RabbitMQ’dan gelen mesajları asenkron olarak işlemek için kullanılır.
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer); // Belirtilen kuyruğu dinlemeye başlar ve mesajlar geldiğinde consumer’ın Received olayını tetikler.

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();