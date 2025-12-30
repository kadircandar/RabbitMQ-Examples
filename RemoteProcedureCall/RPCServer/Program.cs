using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

const string QUEUE_NAME = "rpc_queue";

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null); // RabbitMQ’da QUEUE_NAME adlı bir queue oluşturur (yoksa).Varsa aynı ayarlarla kullanır.
// queue: Kuyruk adı
// durable: Kuyruğun kalıcı olup olmayacağını belirtir. false ise, RabbitMQ yeniden başlatıldığında kuyruk silinir.
// exclusive: Kuyruğun yalnızca bu bağlantı tarafından kullanılabilir olup olmadığını belirtir. false ise, diğer bağlantılar da kuyruğa erişebilir.
// autoDelete: Kuyruğun son tüketici bağlantısı kapandığında otomatik olarak silinip silinmeyeceğini belirtir. false ise, kuyruk kalıcı olur.
// arguments: Kuyruğun özelliklerini belirten ek argümanlar (örneğin, TTL, maksimum uzunluk vb.) için kullanılır.

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // Kanalın aynı anda yalnızca bir mesaj işlemesini sağlar. Bu, iş yükünü dengelemeye yardımcı olur.
// prefetchSize: 0, mesaj boyutu sınırlaması yok.
// prefetchCount: 1, aynı anda yalnızca bir mesaj alınabilir.
// global: false, bu ayarın yalnızca bu kanal için geçerli olduğunu belirtir.

var consumer = new AsyncEventingBasicConsumer(channel); // Asenkron mesaj tüketimi için bir tüketici oluşturur.
consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs ea) =>
{
    AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender; // Tüketiciyi alır.
    IChannel ch = cons.Channel;
    string response = string.Empty;

    byte[] body = ea.Body.ToArray();
    IReadOnlyBasicProperties props = ea.BasicProperties; // Gelen mesajın özelliklerini alır (örneğin, CorrelationId, ReplyTo).
    var replyProps = new BasicProperties // Cevap mesajı için özellikler oluşturur.
    {
        CorrelationId = props.CorrelationId
    };

    try
    {
        var message = Encoding.UTF8.GetString(body);
        int n = int.Parse(message);
        Console.WriteLine($" [.] Fib({message})");
        response = Fib(n).ToString();
    }
    catch (Exception e)
    {
        Console.WriteLine($" [.] {e.Message}");
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        await ch.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!, mandatory: true, basicProperties: replyProps, body: responseBytes); // Cevap mesajını gönderir.
        // exchange: boş bırakılır çünkü default exchange kullanılıyor.
        // routingKey: Cevabın gönderileceği kuyruk adı (ReplyTo).
        // basicProperties: Cevap mesajının özellikleri (CorrelationId).
        // body: Cevap mesajının içeriği.
        // mandatory: true, mesajın teslim edilememesi durumunda iade edilmesini sağlar.
        
        await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false); // Mesajın işlendiğini onaylar.
        // deliveryTag: Onaylanacak mesajın teslim etiketi.
        // multiple: false, yalnızca belirtilen teslim etiketine sahip mesajı onaylar.
    }
};

await channel.BasicConsumeAsync(QUEUE_NAME, false, consumer); // QUEUE_NAME kuyruğunu dinlemeye başlar.
// autoAck: false, mesajların manuel olarak onaylanması gerektiğini belirtir.
// consumer: Mesajları işlemek için kullanılan tüketici.


Console.WriteLine(" [x] Awaiting RPC requests");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

// Yalnızca geçerli pozitif tamsayı girdileri kabul eder.
static int Fib(int n)
{
    if (n is 0 or 1)
    {
        return n;
    }

    return Fib(n - 1) + Fib(n - 2);
}