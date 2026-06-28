# CodeVault System

CodeVault; rol tabanlı yetkilendirme altyapısına sahip, geliştirici (Developer) ve yönetici (Admin) portallarını barındıran, premium koyu tema (dark mode) tasarımıyla zenginleştirilmiş modern bir kod ve iş yönetim portalıdır.

---

## 🏗️ Mimari Yapı ve Veritabanları

Proje, birbirleriyle `HttpClient` üzerinden haberleşen iki ana katmandan oluşur:

1. **CodeVaultAPI (Web API)**
   - **Veritabanı:** `CodeVaultSystemDB` (Yerel MS SQL veya LocalDB)
   - **Tablolar:** `Developers` (Geliştiriciler), `Projects` (Projeler), `Technologies` (Teknolojiler)
   - **Görevi:** Sistemdeki çekirdek iş verilerini yönetir ve MVC projesine REST API uç noktaları sağlar.

2. **CodeVaultMVC (ASP.NET Core MVC)**
   - **Veritabanı:** `CodeVaultSystem_MVC_DB` (ASP.NET Core Identity altyapısı)
   - **Tablolar:** `AspNetUsers` (Kullanıcılar), `Tasks` (Görevler), `Comments` (Yorumlar)
   - **Görevi:** Kullanıcı arayüzünü sunar, kimlik doğrulama, oturum ve roller ile görev/yorum yönetim süreçlerini yürütür.

---

## ✨ Öne Çıkan Özellikler

### 🛡️ Rol Tabanlı Giriş ve Parametre Tabanlı Taşıma (Cookie Olmadan)
* Rol seçimi ve oturum yönlendirme işlemlerinde kesinlikle özel çerez (cookie) yönetimi kullanılmamaktadır.
* Seçilen rol bilgisi (`Admin` veya `Developer`) sayfalara ve formlara tamamen **URL Parametreleri** (`?role=...`) ve form içi **gizli girdiler** (`hidden input`) aracılığıyla aktarılır.
* Giriş esnasında kullanıcının veritabanındaki gerçek rolü ile URL/Form üzerinden gelen rolün uyuşması zorunlu tutularak güvenlik sağlanır.

### 👥 Otomatik Yönetici Seeding ve Şifre Force-Reset
* Uygulama her başladığında `admin@admin.com` kullanıcısı sistemde yoksa otomatik olarak oluşturulur (Seed).
* Güvenlik ve test kolaylığı açısından, admin kullanıcısının parolası uygulama her ayağa kalktığında otomatik olarak `Admin123*` olacak şekilde sıfırlanır ve güncellenir.

### 🔗 Otomatik Geliştirici Profil Eşleşmesi ve Kısıtlamalar
* Yeni bir kullanıcı `Developer` rolüyle kayıt olduğunda, MVC katmanı arka planda API'ye istek atarak veritabanındaki `Developers` tablosunda otomatik olarak bir geliştirici profili oluşturur.
* Geliştiriciler görev eklerken "Geliştirici" seçmek zorunda kalmazlar (dropdown kaldırılmıştır); sistem arka planda aktif oturum açmış kullanıcının e-posta adresiyle API'den ilgili `DeveloperID`'yi bulur ve göreve otomatik atar.
* Geliştiriciler sadece kendi oluşturdukları görevleri ve yorumları görebilir, bunlar üzerinde CRUD (Ekleme/Düzenleme/Silme) işlemleri yapabilirler.

### 🖤 Premium Koyu Tema (Dark Mode) Tasarımı
* Tüm iç paneller, listeler ve formlar yüksek kaliteli koyu tema (`#0f172a` Slate-900 / `#1e293b` Slate-800) arayüzü ile donatılmıştır.
* **Glow & Watermark:** Yönetici Dashboard'unda her kartın arka planında transparan devasa ikonlar (fligran) ve kenarlarda neon renkli parlayan sınır çizgileri yer alır.
* **Okunabilir Placeholder'lar:** Koyu renkli form kontrollerinde placeholder (yer tutucu) metinlerinin rahatça okunabilmesi için özel yüksek kontrastlı (`#64748b`) stiller uygulanmıştır.

### 📱 Mobil Uyumlu Tasarım (Responsive Layout)
* Geliştirici portalındaki üst navbar, mobil cihazlarda hamburger menü şeklinde katlanarak açılır.
* Yönetici portalındaki sabit sol menü (Sidebar), dar ekranlarda otomatik olarak üste katlanıp dikey olarak hizalanır ve içeriğin taşması engellenir.

### 📊 Dinamik Raporlama ve Dışa Aktarma (PDF & Excel)
* Yönetici (Admin) panelinde sistem kayıtlarının tek bir tıklamayla PDF veya Excel formatında indirilebilmesi sağlandı.
* **Excel İndirme (EPPlus):** Projeler, Teknolojiler ve Geliştiriciler verilerini ayrı ayrı sekmelerde (Sheets) düzenli tablolar halinde raporlar. Başlık alanları belirginleştirilmiş ve sütun genişlikleri içeriğe göre otomatik (`AutoFit`) ayarlanmıştır.
* **PDF İndirme (QuestPDF):** Sistem verilerini A4 sayfa boyutunda, temiz ızgara (grid) yerleşimi, kurumsal temayla uyumlu renk kodlamaları ve otomatik sayfa numaralandırması ile profesyonel bir döküman formatında sunar.

---

## 🚀 Çalıştırma Talimatları

### 1. API Projesini Başlatın
Öncelikle veritabanı uç noktalarını sağlayan API projesini ayağa kaldırın:
```bash
cd CodeVaultSystem/CodeVaultAPI
dotnet run
```
*API varsayılan olarak `https://localhost:7000` portunda çalışacaktır.*

### 2. MVC Projesini Başlatın
Ardından kullanıcı portalını ve web arayüzünü içeren MVC projesini ayrı bir terminalde başlatın:
```bash
cd CodeVaultSystem/CodeVaultMVC
dotnet run
```
*MVC projesi çalıştırıldığında tarayıcınızda doğrudan `/Account/ChooseRole` rol seçim ekranına yönlendirileceksiniz.*

---

## 🔑 Varsayılan Giriş Bilgileri

* **Yönetici (Admin) Girişi:**
  - **E-posta:** `admin@admin.com`
  - **Parola:** `Admin123*`
* **Geliştirici (Developer) Girişi:**
  - Rol seçim ekranında **Geliştirici Girişi** -> **Kayıt Ol** adımlarını izleyerek saniyeler içinde yeni bir hesap oluşturup giriş yapabilirsiniz. Profiliniz otomatik olarak oluşturulup eşleştirilecektir.
