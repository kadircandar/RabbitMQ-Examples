using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);// Bu kod RabbitMQ’da logs adlı bir Exchange oluşturur (ya da varsa onu kullanır).
// Exchange nedir? Exchange, mesajların ilk girdiği yerdir. Exchange de kurallara göre mesajı hangi queue(lar)a göndereceğine karar verir.
// Burada ExchangeType.Fanout kullanılması, mesajın tüm bağlı kuyruklara (queues) gönderileceği anlamına gelir.
// Yani, bu Exchange’e gönderilen her mesaj, bu Exchange’e bağlı tüm kuyruklara iletilir.
// routingKey: string.Empty kullanılması, bu Exchange’in routing key’i dikkate almadığını belirtir. Çünkü Fanout Exchange’ler routing key’i kullanmazlar.
// Bu, tüm mesajların tüm bağlı kuyruklara gönderileceği anlamına gelir.


QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(); // Bu kod, RabbitMQ sunucusunda geçici (temporary) bir kuyruk (queue) oluşturur.
string queueName = queueDeclareResult.QueueName; 
await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty); // Bu kod, daha önce oluşturulan "logs" adlı Exchange ile geçici olarak oluşturulan kuyruk arasında bir bağlama (binding) yapar.
// routingKey: string.Empty kullanılması, bu Exchange’in routing key’i dikkate almadığını belirtir. Çünkü Fanout Exchange’ler routing key’i kullanmazlar.
// Yani, bu satırda, "logs" adlı Exchange’e gönderilen tüm mesajlar, bu geçici kuyrukta da alınacaktır.
// Bu sayede, bu kuyruk üzerinden "logs" Exchange’ine gönderilen tüm mesajlar dinlenebilir (consume edilebilir).
// Bu, publish/subscribe (yayınlama/abone olma) modelinin temelini oluşturur.


Console.WriteLine(" [*] Waiting for logs.");

var consumer = new AsyncEventingBasicConsumer(channel); // Bu kod, RabbitMQ sunucusundan gelen mesajları asenkron olarak işlemek için bir tüketici (consumer) oluşturur.
consumer.ReceivedAsync += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] {message}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer); // Bu kod, daha önce oluşturulan geçici kuyruk üzerinde mesaj tüketimini (consumption) başlatır.
// autoAck: true kullanılması, mesajların alındığında otomatik olarak onaylanacağı (acknowledged) anlamına gelir.
// Yani, mesaj alındığında RabbitMQ sunucusuna mesajın başarıyla işlendiği bildirilir.
// consumer parametresi, mesajları işlemek için oluşturulan tüketiciyi belirtir.

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();