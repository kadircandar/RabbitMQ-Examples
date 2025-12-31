Bu repository, RabbitMQ Publish/Subscribe modelini bu kez Direct Exchange kullanarak C# (.NET) ile örnekler. Mesajlar routing key (severity) değerine göre hedefli olarak tüketicilere iletilir.

# 📌 Proje Yapısı
🔹 EmitLogDirect (Publisher)
- direct_logs adlı Direct Exchange oluşturur (varsa kullanır)
- Mesajı bir severity değeri ile birlikte publish eder
  - **info**
  - **warning**
  - **error**
- Mesajlar **sadece ilgili severity’ye abone olan kuyruklara** gönderilir
  - **info mesajı** → sadece info dinleyen consumer
  - **error mesajı** → sadece error dinleyen consumer 

🔹 ReceiveLogsDirect (Consumer)
- direct_logs adlı **Direct Exchange’e** abone olur
- Geçici (temporary) bir queue oluşturur
- Komut satırından verilen **severity değerlerine göre** exchange’e bind olur
- Sadece abone olduğu severity’ye sahip mesajları alır ve ekrana yazar

## 🚀 Başlamadan Önce
Projeyi yerel makinenizde çalıştırmak için gereksinimler.

- .NET SDK
- Docker Desktop

 <br>

**Docker üzerinde RabbitMQ çalıştırmak için gerekli komut:**
```text
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```



## ▶️ Test / Çalıştırma Adımları
1️⃣ ReceiveLogsDirect (Consumer) Çalıştırma
```text
cd ReceiveLogsDirect
dotnet info warning
```
Bu consumer:
- info
- warning
seviyelerindeki mesajları dinler.

Çıktı:
```text
[*] Waiting for messages.
```

2️⃣ EmitLogDirect (Publisher) Çalıştırma
```text
cd EmitLogDirect
dotnet run error "Disk space is critically low"
```

Çıktı:
```text
[x] Sent 'error':'Disk space is critically low'
```

3️⃣ ReceiveLogsDirect Çıktısı (Örnek)
```text
[x] Received 'error':'Disk space is critically low'
```
**info** veya **warning** dinleyen consumer bu mesajı almaz.


## 🧠 Özet
- **EmitLogDirect** → severity bazlı mesaj yayınlar
- **ReceiveLogsDirect** → sadece ilgilendiği mesajları alır
- **Direct Exchange** → routing key eşleşmesine göre çalışır
- Bu repo, RabbitMQ’da filtrelenmiş mesajlaşma mantığını net ve sade şekilde gösterir.

