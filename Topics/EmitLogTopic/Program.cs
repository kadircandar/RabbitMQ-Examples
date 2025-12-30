using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic); // RabbitMQ’da topic_logs adlı bir Topic Exchange oluşturur (yoksa oluşturur, varsa kullanır).
// Topic exchange, routing key’leri nokta ayrımlı konular olarak yorumlar ve queue’lara pattern kurallarıyla mesaj dağıtır. (* -> Tek kelime,  # -> 0 veya daha fazla kelime )
// Routing key’e bakar, ama birebir eşleşme değil, pattern (kural) ile eşleşir. Routing key bu kurala uyuyorsa mesajı gönder.
// exchange: Exchange'in adı
// type: ExchangeType.Topic  exchange tipi Topic

var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: routingKey, body: body); // topic_logs exchange'ine mesaj gönderir.
// routingKey: Mesajın yönlendirileceği anahtardır. Bu anahtar, mesajın hangi kuyruklara yönlendirileceğini belirler.
// body: Gönderilecek mesajın içeriğidir.


Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");