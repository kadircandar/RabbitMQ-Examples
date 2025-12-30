using RabbitMQ.Client; //RabbitMQ sunucusu ile iletişim kurmak için gereken temel sınıfları ve arayüzleri içerir.
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" }; //ConnectionFactory: Bağlantı ayarlarının yapıldığı yerdir. HostName = "localhost", RabbitMQ sunucusunun kendi bilgisayarında çalıştığını belirtir.
using var connection = await factory.CreateConnectionAsync(); //RabbitMQ sunucusu ile fiziksel bir TCP bağlantısı kurar.
using var channel = await connection.CreateChannelAsync(); // CreateChannelAsync(): TCP bağlantısı içinde sanal bir Kanal (Channel) açar. Tüm API işlemleri (kuyruk oluşturma, mesaj gönderme) bu kanal üzerinden yapılır.

await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null); // QueueDeclareAsync(): "hello" adlı bir kuyruk oluşturur. Kuyruk zaten varsa, bu işlem mevcut kuyruğu kullanır.

// Parametreler :
// queue: "hello": Kuyruğun adı.
// durable: false: Kuyruğun kalıcı olup olmadığını belirtir. false ise, RabbitMQ yeniden başlatıldığında kuyruk silinir.
// exclusive: false: Kuyruğun yalnızca bu bağlantı tarafından kullanılabileceğini belirtir. false ise, diğer bağlantılar da kuyruğa erişebilir.
// autoDelete: false: Kuyruğun otomatik olarak silinip silinmeyeceğini belirtir. false ise, kuyruk manuel olarak silinene kadar kalır.

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message); // Encoding.UTF8.GetBytes(message): Mesajı UTF-8 formatında bayt dizisine dönüştürür. RabbitMQ mesajları bayt dizisi olarak gönderir.

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body); // BasicPublishAsync(): Mesajı belirtilen kuyruğa gönderir.
// Parametreler :
// exchange: string.Empty: Mesajın gönderileceği değişim (exchange) adı. Boş string, varsayılan değişimi kullanır.
// routingKey: "hello": Mesajın yönlendirileceği kuyruk adı.
// body: body: Gönderilecek mesajın bayt dizisi.
// Mesaj gönderildikten sonra konsola bilgi yazdırılır.


Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();