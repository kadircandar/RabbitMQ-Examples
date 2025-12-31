
Work Queues, işleri kuyruğa adil şekilde dağıtan, işi bitmeden yeni mesaj almayan ve işi tamamlayınca mesajı onaylayan bir RabbitMQ worker’dır.  

- Producer işleri kuyruğa gönderir
- Consumer’lar işleri sırayla alır ve işler
- Amaç: yükü dağıtmak ve sistemi hızlandırmak

# Proje Yapısı
**1. Producer (Mesaj Gönderen)** NewTask - RabbitMQ'ya mesaj gönderen uygulama
- RabbitMQ’ya bağlanır
- task_queue adlı kalıcı kuyruğu oluşturur
- Kuyruğa kalıcı (persistent) bir mesaj gönderir

👉 Amaç: İşleri güvenli şekilde kuyruğa bırakmak

**2. Consumer (Mesaj Alan)** Worker - RabbitMQ'dan mesaj alan uygulama
- task_queue kuyruğunu dinler
- Fair Dispatch ile aynı anda tek mesaj işler
- Mesajı işledikten sonra ACK gönderir

👉 Amaç: İşleri adil ve güvenli şekilde işlemek
<br><br>

### 🚀 Başlamadan Önce
Projeyi yerel makinenizde çalıştırmak için gereksinimler.
- .NET SDK
- Docker Desktop
<br>

**Docker üzerinde RabbitMQ çalıştırmak için gerekli komut:**
```text
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

**📥 NewTask Projesini Çalıştırma:**
```text
cd NewTask
dotnet run
```

**📥 Worker Projesini Çalıştırma:**
```text
cd Worker
dotnet run
```

<br>

### 🔄 Genel Akış
- Send, işi kuyruğa gönderir
- Receive, işi alır ve işler
- İş bitince ACK gönderilir
- RabbitMQ mesajı kuyruktan siler
- Bu yapı, yük dağıtımı, hata toleransı ve ölçeklenebilirlik sağlar.
<br>

Docker ile çalıştırılan RabbitMQ servisi ayağa kalktıktan sonra yönetim paneline aşağıdaki adres üzerinden erişilebilir:
```text
http://localhost:15672
```


## 🔁 Çoklu Worker Testi
Adım 1: Aynı anda iki Receive çalıştır: 
```text
cd Worker
dotnet run
dotnet run
```

Adım 2: Ardından birkaç mesaj gönder: (Farklı bir terminal'de)
```text
cd NewTask
dotnet run Task1..
dotnet run Task2....
dotnet run Task3.
```

Worker 1:
```text
 [x] Received Task1..
 [x] Done
```

Worker 2:
```text
 [x] Received Task2....
 [x] Done
```

