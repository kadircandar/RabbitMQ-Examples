Bu repository, RabbitMQ Publish/Subscribe (Pub/Sub) modelini Fanout Exchange kullanarak C# (.NET) ile örnekleyen iki basit konsol uygulamasını içerir.

# 📌 Proje Yapısı

🔹 EmitLog (Publisher)
- logs adlı **Fanout Exchange** oluşturur (varsa kullanır)
- Komut satırından veya varsayılan bir mesajı alır
- Mesajı Exchange’e publish eder
- Exchange’e bağlı tüm kuyruklara mesaj gönderilir

🔹 ReceiveLogs (Consumer)
- logs adlı **Fanout Exchange’e** abone olur
- Otomatik isimlendirilmiş geçici bir queue oluşturur
- Bu kuyruğu Exchange’e bind eder
- Yayınlanan tüm log mesajlarını dinler ve ekrana yazar


👉 Bu yapı, gerçek hayatta loglama, monitoring ve event broadcast senaryolarında sıkça kullanılır.

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


## ▶️ Test / Çalıştırma Adımları
1️⃣ ReceiveLogs (Consumer) Çalıştırma
```text
cd ReceiveLogs
dotnet run
```

Çıktı:
```text
[*] Waiting for logs.
```

2️⃣ EmitLog (Publisher) Çalıştırma
```text
cd EmitLog
dotnet run "info: Application started"
```

Çıktı:
```text
[x] Sent info: Application started
```

3️⃣ ReceiveLogs Çıktısı (Örnek)
```text
[x] info: Application started
```


## 🧠 Özet
- **EmitLog** → mesaj yayınlar (Publisher)
- **ReceiveLogs** → mesajları dinler (Subscriber)
- **Fanout Exchange** → mesajları herkese dağıtır
- Bu repo, RabbitMQ Pub/Sub mantığını öğrenmek için minimal ve net bir örnek sunar.
