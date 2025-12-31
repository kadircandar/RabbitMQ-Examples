Bu proje, RabbitMQ Publisher Confirms mekanizmasını kullanarak mesaj yayınlama performansını ve güvenilirliğini farklı yaklaşımlarla karşılaştırmayı amaçlar.

<br>

Projede 50.000 mesaj yayınlanır ve mesajların broker tarafından alındığı şu üç farklı yöntemle doğrulanır:
- **Mesaj bazlı confirm**
- **Batch (toplu) confirm**
- **Asenkron confirm (event tabanlı)**

<br> 

## 🎯 Amaç
- Publisher Confirms kavramını pratikte göstermek
- Farklı confirm stratejilerinin performans farklarını ölçmek
- Yüksek hacimli mesaj yayınlamada en doğru yaklaşımı görmek

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
```text
docker run 
```

İstersen RabbitMQ host bilgisini parametre olarak geçebilirsin:

```text
dotnet run localhost
```

<br>


## 🧪 Test Edilen Senaryolar
1️⃣ Mesaj Bazlı Confirm
```text
await PublishMessagesIndividuallyAsync();
```
- Her mesaj tek tek publish edilir
- Her publish sonrası confirm beklenir
- ✅ En güvenli
- ❌ En yavaş yöntem

<br>

2️⃣ Batch (Toplu) Confirm
```text
await PublishMessagesIndividuallyAsync();
```

- Mesajlar batch halinde gönderilir
- Belirli sayıda publish sonrası confirm beklenir
- ⚖️ Güvenlik ve performans dengesi
  
<br>

3️⃣ Asenkron Confirm (Önerilen)
```text
await HandlePublishConfirmsAsynchronously();
```
- Confirm’ler event üzerinden yakalanır
- BasicAck, BasicNack, BasicReturn event’leri dinlenir
- En yüksek performans
- Yüksek throughput için idealdir 🚀

<br>

📊 Örnek Çıktı

```text
[INFO] publishing 50,000 messages and handling confirms per-message
[INFO] published 50,000 messages individually in 4200 ms

[INFO] publishing 50,000 messages and handling confirms in batches
[INFO] published 50,000 messages in batch in 1800 ms

[INFO] publishing 50,000 messages and handling confirms asynchronously
[INFO] published 50,000 messages and handled confirm asynchronously 650 ms
```
<br>

## 📌 Notlar
Bu proje Consumer içermemektedir. Amaç yalnızca Publisher tarafındaki confirm mekanizmasını incelemektir.
