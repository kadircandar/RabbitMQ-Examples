# RabbitMQ Examples 🚀
Bu repo, RabbitMQ mesaj kuyruğu sisteminin temel ve ileri düzey kullanım senaryolarını C# (.NET) dili ve Docker kullanarak uygulamalı olarak göstermektedir.

<img width="1270" height="889" alt="image" src="https://github.com/user-attachments/assets/9107ccae-7b08-4379-86d8-062551bcd53f" />


# Temel Bileşenler:

- **Producers (Üreticiler):** Mesaj gönderen uygulamalar.
- **Exchange:** Mesajları yönlendiren birim (direct, topic, fanout, headers tipleri).
- **Queues:** Mesajların saklandığı kuyruklar.
- **Bindings:** Exchange ile Queue arasındaki bağlantılar (Routing Key ile).
- **Consumers (Tüketiciler):** Kuyruktaki mesajları alıp işleyen uygulamadır.


## 📌 Desteklenen Mesajlaşma Modelleri
- **Simple Queue:** Tek bir üreticiden tek bir tüketiciye basit mesaj iletimi.
- **Work Queues:** Görevlerin birden fazla tüketici arasında paylaştırılması.
- **Publish/Subscribe (Fanout):** Mesajın tüm kuyruklara yayınlanması.
- **Routing (Direct):** Mesajın belirli bir anahtara (routing key) göre yönlendirilmesi.
- **Topics:** Mesajın desenlere (wildcards) göre filtrelenerek iletilmesi.
  
## 🛠 Kullanılan Teknolojiler
- **Dil:** .NET 10.0 / C#
- **Mesaj Yönlendirici:** RabbitMQ
- **Konteynerleştirme:** Docker
- **Kütüphane:** RabbitMQ.Client (Official .NET Client)

## 🚀 Başlarken
Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin.
### Gereksinimler
- .NET SDK
- Docker Desktop

## 📁 Proje Yapısı


- **HelloWorld** -> Temel Producer/Consumer örneği
- **WorkQueues** -> Görevlerin birden fazla tüketici arasında paylaştırılması.
- **PublishSubscribe** -> Mesajın tüm kuyruklara yayınlanması.
- **Routing** -> Mesajın belirli bir anahtara (routing key) göre yönlendirilmesi.
- **Topics** -> Mesajın desenlere (wildcards) göre filtrelenerek iletilmesi.
- **RemoteProcedureCall** -> RabbitMQ'yu bir istek-cevap (request-response) mekanizması olarak kullanmanızı sağlar. Standart mesaj kuyruğundan farkı, göndericinin (Client) bir cevap beklemesidir
- **PublisherConfirms** -> Mesajın RabbitMQ tarafına ulaştığından emin olmak için kullanılan bir güvenlik mekanizmasıdır.
