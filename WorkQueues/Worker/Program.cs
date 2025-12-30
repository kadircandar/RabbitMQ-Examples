using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
// arguments: null: Ekstra parametreler yok.

// Bu kod, işçi uygulamasının aynı anda yalnızca bir mesaj işlemesini sağlar.

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // Bu kod, RabbitMQ'daki "Adil Dağıtım" (Fair Dispatch) ayarıdır.
// prefetchSize: 0: Boyut sınırlaması yok.
// prefetchCount: 1: Aynı anda yalnızca bir mesaj alınabilir. Bir işçiye (worker), elindeki işi bitirip onay (ACK) göndermeden ikinci bir mesajı verme demektir
// global: false: Ayar sadece bu kanal için geçerlidir.


Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel); // Asenkron tüketici oluşturur.
consumer.ReceivedAsync += async (model, ea) => // Mesaj alındığında çalışacak olay işleyicisi.
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000);

    Console.WriteLine(" [x] Done");

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false); // Mesajın başarıyla işlendiğini RabbitMQ'ya bildirir. Bu kod, RabbitMQ'ya gönderilen "İş bitti, mesajı silebilirsin" onay (Acknowledgement) sinyalidir.
    // deliveryTag: ea.DeliveryTag: İşlenen mesajın benzersiz tanımlayıcısı.
    // multiple: false: Sadece bu mesaj için onay gönderilir.
    // Bu onay, mesajın kuyruğundan silinmesini sağlar.
};

await channel.BasicConsumeAsync("task_queue", autoAck: false, consumer: consumer); // Kuyruğu dinlemeye başlar.
// "task_queue": Dinlenecek kuyruğun adı.
// autoAck: false: Mesajlar otomatik olarak onaylanmaz, işlendikten sonra manuel onay gerekir.
// consumer: consumer: Mesajları işlemek için kullanılan tüketici nesnesi.

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();