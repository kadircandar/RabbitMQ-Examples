using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout); // Bu kod RabbitMQ’da logs adlı bir Exchange oluşturur (ya da varsa onu kullanır).
// Exchange nedir? Exchange, mesajların ilk girdiği yerdir. Exchange de kurallara göre mesajı hangi queue(lar)a göndereceğine karar verir.
// Burada ExchangeType.Fanout kullanılması, mesajın tüm bağlı kuyruklara (queues) gönderileceği anlamına gelir.
// Yani, bu Exchange’e gönderilen her mesaj, bu Exchange’e bağlı tüm kuyruklara iletilir.
// routingKey: string.Empty kullanılması, bu Exchange’in routing key’i dikkate almadığını belirtir. Çünkü Fanout Exchange’ler routing key’i kullanmazlar.
// Bu, tüm mesajların tüm bağlı kuyruklara gönderileceği anlamına gelir.

var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);
await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body); // Bu kod, oluşturulan Exchange’e bir mesaj yayınlar (publish).
// routingKey: string.Empty kullanılması, bu Exchange’in routing key’i dikkate almadığını belirtir. Çünkü Fanout Exchange’ler routing key’i kullanmazlar.
// Yani, bu satırda, "logs" adlı Exchange’e, routing key kullanmadan (boş string) ve mesaj içeriği olarak body parametresi ile bir mesaj gönderilir.

Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
}