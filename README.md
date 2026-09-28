# 🚀 Enterprise Distributed Notification Worker Service

Bu proje; mikroservis mimarilerinde ana API'yi yormadan, arka planda 7/24 asenkron olarak çalışan, yüksek performanslı bir **C# .NET Mesaj Kuyruğu (Message Queue) ve Arka Plan İşçi Servisidir**.

## 🎯 Proje Ne Yapıyor?
* **Yüksek Performanslı Bellek İçi Kuyruk (`System.Threading.Channels`):** API'den gelen binlerce bildirim isteğini (Email, SMS, Push) sistemi kilitlemeden thread-safe (iş parçacığı güvenli) bir bellek kuyruğuna alır.
* **Hata Toleranslı Arka Plan İşçisi (`BackgroundService`):** Kuyruğa düşen görevleri asenkron olarak eritirken, harici servis kesintilerine karşı kurumsal bir **Yeniden Deneme Mekanizması (Retry Policy)** çalıştırır.
* **Dead Letter Queue (DLQ) Yönetimi:** 3 denemede de başarılı olamayan hatalı görevleri sistem sağlığını korumak adına otomatik olarak DLQ çöplüğüne fırlatır ve loglar.

## 🛠️ Teknik Altyapı
* **Dil & Framework:** C# / .NET 9.0 (Worker Service Framework)
* **Kuyruk Mimarisi:** System.Threading.Channels (High-Throughput In-Memory Queue)
* **Tasarım Kalıpları:** Producer-Consumer Pattern, Asynchronous Background Tasks
