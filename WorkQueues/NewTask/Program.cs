using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };  
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);  
// Bu kod, RabbitMQ üzerinde sağlam ve kalıcı bir iletişim kanalı açar.
// task_queue: Kuyruğun adı.
// durable: true: RabbitMQ kapansa bile kuyruk silinmez, veriler korunur.
// exclusive: false: Bu kuyruğa herkes bağlanabilir (paylaşımlı).
// autoDelete: false: İçinde mesaj kalmasa veya kimse dinlemese bile kuyruk kapanmaz.

var message = GetMessage(args); // 
var body = Encoding.UTF8.GetBytes(message);

var properties = new BasicProperties // Persistent = true: Mesajın disk üzerine kaydedilmesini sağlar.
{
    Persistent = true
};

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "task_queue", mandatory: true,
    basicProperties: properties, body: body);

//Bu kod, hazırladığınız mesajı RabbitMQ üzerindeki kuyruğa gerçekten gönderdiğiniz (yayınladığınız) andır.
// exchange: string.Empty: Varsayılan değişim kullanılır.
// routingKey: "task_queue": Mesajın gönderileceği kuyruk adı.
// mandatory: true: Mesajın teslim edilememesi durumunda geri gönderilmesini sağlar.
// basicProperties: properties: Mesajın özellikleri (kalıcılık gibi).
// body: body: Gönderilecek mesajın içeriği (byte dizisi).

// Mesajın kuyruğa başarıyla gönderildiğini konsola yazdırır.

Console.WriteLine($" [x] Sent {message}");

static string GetMessage(string[] args) // Komut satırından gelen argümanları birleştirir veya varsayılan mesajı döner.
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}

// Bu kod, konsola gönderilen mesajın içeriğini yazdırır.
// örneğin: " [x] Sent Hello World!" ya da " [x] Sent Task 1..." gibi.