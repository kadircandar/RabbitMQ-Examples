Bu repo, RabbitMQ kullanarak C# ile RPC (Remote Procedure Call) pattern’inin nasıl uygulanacağını gösterir.
Client bir isteği kuyruk üzerinden gönderir, Server isteği işler ve sonucu tekrar Client’a döner. 
RabbitMQ üzerinde RPC (Remote Procedure Call), bir istemcinin (client) başka bir bilgisayardaki (server) bir fonksiyonu çalıştırması ve sonucunu beklemesi sürecidir.


## 🧩 Temel Akış Mantığı
1. **Client**
- Bir mesaj gönderir (örneğin: “5 + 7 kaç?”)
- Mesajın içine:
  - replyTo → cevabın geleceği kuyruk
  - correlationId → isteği cevaba bağlamak için benzersiz ID koyar
    
2. **RPC Queue**
- Server bu kuyruğu dinler

3. **Server**
- Mesajı alır
- İşlemi yapar
- Sonucu replyTo kuyruğuna gönderir
- Aynı correlationId ile gönderir

4. **Client**
- Cevap kuyruğunu dinler
- correlationId eşleşirse cevabı kabul eder
  
<br> 

**Neden correlationId Gerekli?**

Client aynı anda birden fazla RPC isteği gönderebilir. Cevaplar karışabilir. Client, ID’ye bakarak doğru cevabı doğru isteğe bağlar.

<br>


## 🧠 RPC Mimarisi Nasıl Çalışır?

1. **RPCClient**
- `rpc_queue` kuyruğuna mesaj gönderir
- Her istek için:
  - `CorrelationId` üretir
  - `ReplyTo` alanını geçici kuyruğa ayarlar
- Cevabı bekler ve doğru istekle eşleştirir

2. **RPCServer**
- `rpc_queue` kuyruğunu dinler
- Gelen mesajı işler (`fib(n)`)
- Sonucu `ReplyTo` kuyruğuna, aynı `CorrelationId` ile gönderir
- Mesajı manuel olarak ACK eder


<br>

## 🚀 Başlamadan Önce
Projeyi yerel makinenizde çalıştırmak için gereksinimler.

- .NET SDK
- Docker Desktop

  
<br>

**Docker üzerinde RabbitMQ çalıştırmak için gerekli komut:**
```text
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

<br>

## ▶️ Çalıştırma
1️⃣ RPC Server’ı başlat
```text
cd RPCServer
dotnet run
```

Beklenen çıktı:
```text
 [x] Awaiting RPC requests
```

<br> 

2️⃣ RPC Client’ı çalıştır
```text
cd RPCClient
dotnet run 30
```

Çıktı:
```text
RPC Client
 [x] Requesting fib(30)
 [.] Got '832040'
```

<br>

🧪 Test Senaryosu

🔹Farklı Fibonacci Değerleri
```text
dotnet run 5
dotnet run 10
dotnet run 20
```
<br>

Beklenen sonuçlar:
| n | Sonuç |
| :--- | :----: | 
| 5 | 5 | 
| 10 | 55 | 
| 20 | 6765 | 

<br><br>

📌 Not
Bu örnek **eğitim amaçlıdır.**

Gerçek projelerde:

- Timeout
- Retry
- Circuit breaker
- Idempotency

gibi ek mekanizmalar düşünülmelidir.
