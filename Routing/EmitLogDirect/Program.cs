using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);// RabbitMQ’da direct_logs adlı bir Direct Exchange oluşturur (veya varsa onu kullanır).
// Direct exchange, mesajın routing key’i ile queue’nun binding key’ini birebir karşılaştırır; aynıysa gönderir, değilse hiç bakmaz bile.
// exchange: "direct_logs"  exchange adı 
// type: ExchangeType.Direct  exchange tipi direct

var severity = (args.Length > 0) ? args[0] : "info"; // Mesajın routing key’ini belirliyor. Yani: “Bu mesaj hangi kategoriye ait?” Eğer hiç parametre verilmezse info olarak varsayılır.
// İşleri basitleştirmek için 'severity' i warning, info veya error olabileceğini varsayacağız.
// args[0]  komut satırından alınan ilk argüman (severity seviyesi)
// "info"  varsayılan severity seviyesi
// severity  mesajın routing key’i


var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);


await channel.BasicPublishAsync(exchange: "direct_logs", routingKey: severity, body: body); // direct_logs exchange’ine bir mesaj yayınlar (publish).
// exchange: "direct_logs"  mesajın gönderileceği exchange adı
// routingKey: severity  mesajın routing key’i (severity seviyesi)
// body: body  gönderilecek mesajın içeriği (byte dizisi)

Console.WriteLine($" [x] Sent '{severity}':'{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();