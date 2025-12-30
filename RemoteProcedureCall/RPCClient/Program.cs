using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

public class RpcClient : IAsyncDisposable
{
    private const string QUEUE_NAME = "rpc_queue";

    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new(); //Gönderilen her RPC isteğini, gelecek cevabıyla eşleştirmek için kullanılır.

    private IConnection? _connection;
    private IChannel? _channel;
    private string? _replyQueueName;

    public RpcClient()
    {
        _connectionFactory = new ConnectionFactory { HostName = "localhost" };
    }

    public async Task StartAsync()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

 
        QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync(); // Rastgele isimlendirilmiş, geçici bir kuyruk oluşturur.
        _replyQueueName = queueDeclareResult.QueueName;
        var consumer = new AsyncEventingBasicConsumer(_channel); // Cevapları dinlemek için bir tüketici oluşturur.

        consumer.ReceivedAsync += (model, ea) =>
        {
            string? correlationId = ea.BasicProperties.CorrelationId; // Gelen mesajın correlationId'sini alır.

            if (false == string.IsNullOrEmpty(correlationId))
            {
                if (_callbackMapper.TryRemove(correlationId, out var tcs))
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    tcs.TrySetResult(response); // İlgili TaskCompletionSource'u tamamlar ve cevabı iletir.
                }
            }

            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_replyQueueName, true, consumer); // _replyQueueName kuyruğunu dinlemeye başlar ve gelen mesajları consumer’a teslim eder.
        // Bu sayede, gelen cevaplar ilgili RPC çağrılarına yönlendirilir.
        // true parametresi, mesajların otomatik olarak onaylanmasını sağlar.
    }

    public async Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException();
        }

        string correlationId = Guid.NewGuid().ToString();
        var props = new BasicProperties // Mesajın özelliklerini ayarlar.
        {
            CorrelationId = correlationId, // Bu istek için benzersiz bir kimlik belirler.
            ReplyTo = _replyQueueName  // Cevabın gönderileceği kuyruk adı.
        };

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously); // Cevabı beklemek için bir TaskCompletionSource oluşturur. RunContinuationsAsynchronously, TaskCompletionSource tamamlandığında await eden kodun consumer thread’ini bloklamadan, güvenli şekilde çalışmasını sağlar.
        _callbackMapper.TryAdd(correlationId, tcs); // correlationId ile tcs'yi eşleştirir.

        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: QUEUE_NAME, mandatory: true, basicProperties: props, body: messageBytes); // RPC isteğini kuyruğa gönderir.
        // exchange: boş bırakılır çünkü default exchange kullanılıyor.
        // routingKey: hedef kuyruk adı.
        // mandatory: true, mesajın teslim edilememesi durumunda iade edilmesini sağlar.
        // basicProperties: mesajın özellikleri.
        // body: mesajın içeriği.


        using CancellationTokenRegistration ctr = // İptal durumunda TaskCompletionSource'u iptal eder ve callback mapper'dan kaldırır.
            cancellationToken.Register(() =>
            {
                _callbackMapper.TryRemove(correlationId, out _); // İlgili tcs'yi kaldırır.
                tcs.SetCanceled();  // TaskCompletionSource'u iptal eder.
            });

        return await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }
}

public class Rpc
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("RPC Client");
        string n = args.Length > 0 ? args[0] : "30";
        await InvokeAsync(n);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static async Task InvokeAsync(string n) // RabbitMQ üzerinden RPC çağrısı yaparak, server’daki fib(n) işlemini çalıştır ve sonucu bekle.
    {
        var rpcClient = new RpcClient();
        await rpcClient.StartAsync();

        Console.WriteLine(" [x] Requesting fib({0})", n);
        var response = await rpcClient.CallAsync(n);
        Console.WriteLine(" [.] Got '{0}'", response);
    }
}