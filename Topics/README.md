Bu repository, RabbitMQ Topic Exchange kullanımını C# (.NET) ile örnekler. Topic Exchange, mesajları routing key pattern’lerine göre yönlendirerek esnek ve güçlü bir publish/subscribe modeli sunar.


# 📌 Proje Yapısı
🔹 EmitLogTopic (Publisher)
- topic_logs adlı **Topic Exchange** oluşturur (varsa kullanır)
- Mesajları **nokta ayrımlı routing key** ile publish eder
- Varsayılan routing key: **anonymous.info**

Örnek routing key’ler:
- `system.error`
- `app.backend.warning`
- `auth.login.info`

🔹 ReceiveLogsTopic (Consumer)
- topic_logs adlı **Topic Exchange’e** abone olur
- Geçici (temporary) bir queue oluşturur
- Komut satırından verilen **binding key (pattern)’lere** göre exchange’e bind olur
- Sadece pattern ile eşleşen mesajları alır ve ekrana yazar

🧩 Topic Exchange Mantığı
Topic Exchange, routing key’leri kelime grupları olarak yorumlar:

- `*` → **tek kelimeyi** temsil eder
- `#` → **0 veya daha fazla kelimeyi** temsil eder

Örnekler:
- `*.error` → system.error, app.error
- `app.#` → app.backend.error, app.frontend.info
- `#.warning` → system.cpu.warning

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
1️⃣ ReceiveLogsTopic (Consumer) Çalıştırma
```text
cd ReceiveLogsTopic
dotnet run "*.error" "app.#"
```

Bu consumer:
- `*.error`
- `app.#`
  
 pattern’lerine uyan mesajları alır.


Çıktı:
```text
[*] Waiting for messages.
```
<br><br>

2️⃣ EmitLogTopic (Publisher) Çalıştırma
```text
cd EmitLogTopic
dotnet run app.backend.error "Database connection failed"
```

Çıktı:
```text
[x] Sent 'app.backend.error':'Database connection failed'
```
<br><br>

3️⃣ ReceiveLogsTopic Çıktısı (Örnek)
```text
[x] Received 'app.backend.error':'Database connection failed'
```


## 🧠 Özet
- **EmitLogTopic** → routing key ile mesaj yayınlar
- **ReceiveLogsTopic** → pattern’a uyan mesajları alır
- **Topic Exchange** → wildcard destekli gelişmiş routing sağlar
- Bu örnek, RabbitMQ’da en güçlü routing mekanizmasını sade ve anlaşılır şekilde gösterir.
