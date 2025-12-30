
Bu örnekte, C# dilinde iki program yazacağız: tek bir mesaj gönderen bir producer (üretici) ve mesajları alıp yazdırılan bir consumer (tüketici). Başlangıç için bu çok basit konuya odaklanacağız.

# Proje Yapısı
**1. Producer (Mesaj Gönderen)** 
Producer.cs - RabbitMQ'ya mesaj gönderen uygulama
- RabbitMQ sunucusuna bağlantı oluşturur (localhost)
- hello adında bir kuyruk tanımlar
- "Hello World!" mesajını bu kuyruğa gönderir
- Mesajı byte dizisine dönüştürüp yayınlar

**2. Consumer (Mesaj Alan)**
Consumer.cs - RabbitMQ'dan mesaj alan uygulama

- RabbitMQ sunucusuna bağlantı oluşturur
- hello kuyruğunu dinlemeye başlar
- Gelen mesajları okur ve konsola yazdırır
- Asenkron event handler kullanarak mesajları işler

### 🚀 Başlamadan Önce
Projeyi yerel makinenizde çalıştırmak için gereksinimler.
- .NET SDK
- Docker Desktop
<br>

**Docker üzerinde RabbitMQ çalıştırmak için gerekli komut:**
```text
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

<br>
Her ikisi de kuyruğu bildirdiği için istemcileri istediğiniz sırayla çalıştırabilirsiniz. Önce consumer (tüketiciyi) çalıştıracağız, böylece mesajı beklediğini ve ardından aldığını görebilirsiniz:<br><br>


**📥 Receive Projesini Çalıştırma:**
```text
cd Receive
dotnet run
```

**📥 Send Projesini Çalıştırma:**
```text
cd Send
dotnet run
```

<br><br>
Consumer (Tüketici), yayıncıdan (publisher) RabbitMQ aracılığıyla aldığı mesajı yazdıracaktır. Consumer (Tüketici) çalışmaya devam edecek ve mesajları bekleyecektir, bu nedenle yayıncıyı birkaç kez yeniden başlatmayı deneyin.


- **Consumer (Tüketici):** RabbitMQ üzerinden mesajları dinleyen ve işleyen **Receive** projesidir.
- **Publisher (Yayıncı):** RabbitMQ’ya mesaj gönderen **Send** projesidir.

<br>

Docker ile çalıştırılan RabbitMQ servisi ayağa kalktıktan sonra yönetim paneline aşağıdaki adres üzerinden erişilebilir:
```text
http://localhost:15672
```


Adım 1: Consumer'ı Çalıştığında


Çıktı:
```text
[*] Waiting for messages.
Press [enter] to exit.
```


Adım 2: Producer'ı Çalıştırın (Farklı bir terminal'de)

Çıktı:
```text
[x] Sent Hello World!
Press [enter] to exit.
```

### Adım 3: Consumer'da Mesajı Görün
Consumer terminalinde şunu göreceksiniz:
```
[x] Received Hello World!
