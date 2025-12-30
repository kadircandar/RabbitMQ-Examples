using RabbitMQ.Client; //RabbitMQ sunucusu ile iletişim kurmak için gereken temel sınıfları ve arayüzleri içerir.
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" }; //ConnectionFactory: Bağlantı ayarlarının yapıldığı yerdir. HostName = "localhost", RabbitMQ sunucusunun kendi bilgisayarında çalıştığını belirtir.
using var connection = await factory.CreateConnectionAsync(); // RabbitMQ sunucusu ile fiziksel bir TCP bağlantısı kurar.
using var channel = await connection.CreateChannelAsync(); // CreateChannelAsync(): TCP bağlantısı içinde sanal bir Kanal (Channel) açar. Tüm API işlemleri (kuyruk oluşturma, mesaj gönderme) bu kanal üzerinden yapılır.

await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null); // QueueDeclareAsync(): "hello" adlı bir kuyruk oluşturur. Kuyruk zaten varsa, bu işlem mevcut kuyruğu kullanır.

// Parametreler :
// queue: "hello": Kuyruğun adı.
// durable: false: Kuyruğun kalıcı olup olmadığını belirtir. false ise, RabbitMQ yeniden başlatıldığında kuyruk silinir.
// exclusive: false: Kuyruğun yalnızca bu bağlantı tarafından kullanılabileceğini belirtir. false ise, diğer bağlantılar da kuyruğa erişebilir.
// autoDelete: false: Kuyruğun otomatik olarak silinip silinmeyeceğini belirtir. false ise, kuyruk manuel olarak silinene kadar kalır.

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel); // AsyncEventingBasicConsumer: Asenkron mesaj tüketimi için kullanılır. Kanal ile ilişkilendirilir.
consumer.ReceivedAsync += (model, ea) => // ReceivedAsync: Mesaj alındığında tetiklenen olaydır.
{
    var body = ea.Body.ToArray(); // ea.Body.ToArray(): Alınan mesajın bayt dizisini elde eder.
    var message = Encoding.UTF8.GetString(body); // Encoding.UTF8.GetString(body): Bayt dizisini UTF-8 formatında stringe dönüştürür.
    Console.WriteLine($" [x] Received {message}"); // Konsola alınan mesajı yazdırır.
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer); // BasicConsumeAsync(): Belirtilen kuyruktan mesajları tüketmeye başlar.

// Parametreler :
// "hello": Mesajların alınacağı kuyruk adı.
// autoAck: true: Mesajların otomatik olarak onaylanıp onaylanmayacağını belirtir. true ise, mesaj alındığında otomatik olarak onaylanır.
// consumer: consumer: Mesajları işlemek için kullanılan tüketici nesnesi.

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();