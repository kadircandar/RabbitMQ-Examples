using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

var factory = new ConnectionFactory { HostName = "localhost" }; // RabbitMQ sunucusunun ana bilgisayar adını belirtir.

using var connection = await factory.CreateConnectionAsync(); // RabbitMQ sunucusuna asenkron bir bağlantı oluşturur.
using var channel = await connection.CreateChannelAsync();  // Bağlantı üzerinden asenkron bir kanal (channel) oluşturur.

await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);// RabbitMQ’da topic_logs adlı bir Topic Exchange oluşturur (yoksa oluşturur, varsa kullanır).
// Topic exchange, routing key’leri nokta ayrımlı konular olarak yorumlar ve queue’lara pattern kurallarıyla mesaj dağıtır. (* -> Tek kelime,  # -> 0 veya daha fazla kelime )
// Routing key’e bakar, ama birebir eşleşme değil, pattern (kural) ile eşleşir. Routing key bu kurala uyuyorsa mesajı gönder.
// exchange: Exchange'in adı
// type: ExchangeType.Topic  exchange tipi Topic


QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(); // Geçici, rastgele isimli bir kuyruk oluşturur.
string queueName = queueDeclareResult.QueueName;

foreach (string? bindingKey in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: bindingKey); // Oluşturulan geçici kuyruğu topic_logs exchange'ine bağlar ve belirtilen binding key ile ilişkilendirir.
    // bir queue’nun Topic Exchange’te hangi routing key pattern’lerine uyan mesajları alacağını tanımlar.
    // queue: Bağlanacak kuyruk adı
    // exchange: Bağlanacak exchange adı
    // routingKey: Kuyruğun dinleyeceği routing key (binding key)
}

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

var consumer = new AsyncEventingBasicConsumer(channel); // Asenkron bir tüketici oluşturur.
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer); // Belirtilen kuyruğu dinlemeye başlar ve mesajlar geldiğinde tüketici tarafından işlenir.
// queue: Dinlenecek kuyruk adı
// autoAck: true ise, mesaj alındığında otomatik olarak onaylanır (acknowledged).
// consumer: Mesajları işlemek için kullanılan tüketici nesnesi.

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();