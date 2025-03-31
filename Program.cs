using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;


namespace libraryApp
{

    public enum KitapKategorisi
    {
        Tarih,
        Bilim,
        Edebiyat,
        Felsefe
    }


    class Program {

        static void Main(string[] args) {

            string dosyaYolu = "kitaplar.json";
            KutuphaneYoneticisi kitapIslemleri = new KutuphaneYoneticisi(dosyaYolu);
            Random rnd = new Random();

            string? dosyaYolu2                 = "users.json";
            User   kullaniciIslemleri          = new User(dosyaYolu2); 

            User? currentUser = null;


            while (true) {

                if (currentUser == null) {
                    
                    Menu.GirisKontrolu();
                    
                    int secim2;
                    try {
                        secim2 = Convert.ToInt32(Console.ReadLine());

                        if ( (secim2 != 1) && (secim2 != 2)) {
                            Console.WriteLine("\nGeçersiz seçim! Devam etmek için bir tuşa basınız...");
                            Console.ReadKey();
                            continue;
                        }
                    } catch (FormatException) {
                        Console.WriteLine("Lütfen geçerli bir sayı giriniz!");
                        continue;
                    }

                    Console.WriteLine($"Seçiminiz : {secim2} \n");


                    if (secim2 == 1) { // --- KULLANICI GİRİŞ ALANI
                        bool girisBasarili = false;
                        

                        while (!girisBasarili) {
                            string? username;
                            string? password;

                            Console.Write("Kullanıcı Adı: ");
                            username = Console.ReadLine();

                            Console.Write("Şifre: ");
                            password = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                                Menu.ShowError("Kullanıcı adı ve şifre boş olamaz.");
                                continue;
                            }


                            try {
                                
                                if (kullaniciIslemleri.AuthenticateUser(username, password)) {
                                    currentUser = kullaniciIslemleri.GetUserByUsername(username); 
                                    // currentUser değişkeni, giriş yapan kullanıcının nesnesini tutuyor. Böylece sisteme giriş yaptıktan sonra 
                                    // bu nesneyi referans alarak kullanıcının rolünü kontrol edebiliyorsunuz.
                                    Menu.ShowSuccess("Başarıyla giriş yaptınız! 🎉");
                                    girisBasarili = true;
                                    
                                }    

                            } catch (Exception ex) {
                                Menu.ShowError(ex.Message);
                                Console.WriteLine("Tekrar denemek için [T], çıkmak için [Q] tuşlayın.");
                                var key = Console.ReadKey();
                                if (key.Key == ConsoleKey.Q) {
                                    break;
                                } else if (key.Key == ConsoleKey.T) {
                                continue;     
                                }
                            

                            }

                        }
                        // Giriş yapılmadıysa döngüden çıkıyoruz.
                        if (currentUser == null) {
                            continue;
                        }




                    } else if (secim2 == 2) { // --- KULLANICI KAYIT ALANI
                        Console.WriteLine("Yeni Kullanıcı Kayıt sayfasına hoş geldiniz! 🚀 Kayıt olmak için lütfen gerekli bilgileri doldurun. \n");
                        Console.WriteLine();

                        string? username;
                        string? password;

                        // Kullanıcı adı geçerli olana kadar döngü
                        while (true) {
                            Console.Write("Kullanıcı Adı: ");
                            username = Console.ReadLine();
                            
                            try {
                                new User().UsernameControl = username;
                                break;
                            } catch (Exception ex) {
                                Menu.ShowError(ex.Message);
                            }

                        }

                        while (true) {
                            Console.Write("Şifre: ");
                            password = Console.ReadLine();
                            
                            try {
                                new User().PasswordControl = password;
                                break;
                            } catch (Exception ex) {
                                Menu.ShowError(ex.Message);
                            }

                        }

                        Console.WriteLine("\nKaydınız gerçekleştiriliyor...");
                        Console.WriteLine("...");
                        Console.WriteLine("...");
                        Console.WriteLine("...");
                        Console.WriteLine();
                        Console.ReadKey();

                        int userID;
                        do {
                            userID = rnd.Next(1, 1000);
                        } while(kullaniciIslemleri.IdMevcutMu(userID));

                        
                        try {
                            User user = new User(username, password, userID);

                            if (kullaniciIslemleri.AddUser(user)) {
                                Menu.ShowSuccess("Başarıyla kaydoldunuz! 🎉");
                                Console.WriteLine("Devam etmek için lütfen bir tuşa basın.");
                                Console.ReadKey();// Başarı mesajını görmek için tuşa basmasını bekletiyoruz.
                            } else {
                                Menu.ShowError("Bu kullanıcı adı zaten alınmış.");
                            }

                        } catch {
                            // Hata zaten constructor içinde gösterildi (Menu.ShowError(ex.Message);), tekrar girişe yönlendir
                            continue;
                        }

                        // Menu.ShowMenu(); // düzenlemelisin. aynı aşağıda olduğu gibi kullanıcı rolüne göre yönlendirmeliyim.

                    }

                } 
                



                // Oturum açmış kullanıcıya göre menü seçeneklerini ayarlıyoruz.
                Console.Clear();
                Console.WriteLine(new string('-', 25));
                Console.WriteLine("   KÜTÜPHANE YÖNETİM SİSTEMİ   ");
                Console.WriteLine(new string('-', 25));
                Console.WriteLine();


                if (currentUser != null && currentUser.Role.ToLower() == "admin") {
                    // admin için menü seçenekleri
                    Menu.MenuForAdmins();
                } else {
                    // kullanıcı için menü seçenekleri
                    Menu.MenuForUsers(); // burada kullanıcıya yapabileceği işlemleri getirdim. kitap listelemeyi seçti. ancak bir işlem yaptıktan sonra hemen geri giriş yapma 
                    // sayfasına attı. her yerde aynı. kullanıcı bir işle yaptıktan sonra hemen çıkış yapıyor, düzeltmelisin.
                }

                Console.WriteLine();
                Console.Write("\nLütfen bir işlem seçiniz: ");




                int secim;
                try {
                    secim = Convert.ToInt32(Console.ReadLine());

                    if (secim < 1 || secim > 8) { // burayı düzenle

                        Console.WriteLine("\nGeçersiz seçim 🚫 Devam etmek için bir tuşa basınız...");
                        Console.ReadKey();
                        continue;
                    }

                }
                catch (FormatException) {
                    Console.WriteLine("Lütfen geçerli bir sayı giriniz 🚫");
                    continue;
                }

                Console.WriteLine($"Seçiminiz : {secim}");


                if (currentUser != null && currentUser.Role.ToLower() == "admin") { // eğer kullanıcının rolü admin ise adminin yapabileceği işler.

                    switch(secim) {

                        case 1: {   // KİTAP EKLE

                            Console.Clear(); // Yeni işlem için ekranı temizle
                            Console.WriteLine(new string('-', 25));
                            Console.WriteLine("   YENİ KİTAP EKLEME   ");
                            Console.WriteLine(new string('-', 25));
                            Console.WriteLine();


                            kitapIslemleri.ShowCategories();

                            Console.Write("Kitap Adı : ");
                            string? eklenecekKitap = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(eklenecekKitap))
                            {
                                Menu.ShowError("Kitap adı boş olamaz 🚫!");
                                continue;
                            }


                            Console.Write("Yazar : ");
                            string? eklenecekYazar = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(eklenecekYazar)) {
                                Menu.ShowError("Yazar adı boş olamaz 🚫!");
                                continue;
                            }


                            Console.WriteLine("Lütfen kitabınız için yukarıdaki kategorilerden birini ekleyin! \n");
                            Console.Write("Kategori Adı : ");
                            string? eklenecekKategori = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(eklenecekKategori)) {
                                Menu.ShowError("Kategori adı boş olamaz 🚫!");
                                continue;
                            }

                            eklenecekKategori = char.ToUpper(eklenecekKategori[0]) + eklenecekKategori.Substring(1).ToLower();

                            if (!Enum.GetNames(typeof(KitapKategorisi)).Contains(eklenecekKategori)) {
                                Menu.ShowError("Lütfen doğru bir kategori adı giriniz 🚫!");
                                continue;
                            }


                            Console.WriteLine("");
                            Console.WriteLine("");


                            int kitapID;
                            do {
                                kitapID = rnd.Next(1, 1000);
                            } while (kitapIslemleri.IdMevcutMu(kitapID));


                            var kitap = new Kitap() {
                                Id = kitapID,
                                KitapAdi = eklenecekKitap,
                                Yazar = eklenecekYazar,
                                Category = eklenecekKategori
                            };


                            if (kitapIslemleri.KitapEkle(kitap)) {
                                Menu.ShowSuccess("Kitap başarıyla eklendi ✅");
                            } else {
                                Menu.ShowError("Bu ID'ye ait bir kitap zaten mevcut 🚫");
                            }

                            break;
                        }


                        case 2: {   // KİTAP LİSTELE
                            Console.Clear();
                            Console.WriteLine("📌 1- Tüm kitapları listele");

                            var kategoriler = Enum.GetNames(typeof(KitapKategorisi)); // kategoriler burada bir dizi. GetNames dizi döndürür.
                            for (int i = 0; i < kategoriler.Length; i++) {
                                Console.WriteLine($"📌 {i + 2}- {kategoriler[i]} kategorisine ait kitapları listele");
                            }

                            Console.WriteLine("Lütfen listelemek istediğiniz kitap türünü seçin! ");


                            if (int.TryParse(Console.ReadLine(), out int listelemeTercihi)) {
                                kitapIslemleri.KitaplariListele(listelemeTercihi);
                            } else {
                                Menu.ShowError("Geçersiz seçim 🚫!");
                            }


                            Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                            Console.ReadKey();
                            break;
                        }

                        case 3: {   // KİTAP SİL
                            Console.WriteLine(new string('-', 25));
                            Console.WriteLine("   KİTAP LİSTESİ   ");
                            Console.WriteLine(new string('-', 25));

                            Console.WriteLine("📌 1- Tüm kitapları listele");
                            var kategoriler = Enum.GetNames(typeof(KitapKategorisi)); // kategoriler burada bir dizi. GetNames dizi döndürür.
                            
                            for (int i = 0; i < kategoriler.Length; i++) {
                                Console.WriteLine($"📌 {i + 2}- {kategoriler[i]} kategorisine ait kitapları listele");
                            }

                            Console.WriteLine("\nLütfen silmek istediğiniz kitap türünü seçin! \n");

                            int listelemeTercihi = Convert.ToInt32(Console.ReadLine());
                            kitapIslemleri.KitaplariListele(listelemeTercihi);
                            Console.Write("\nSilmek istediğiniz kitabın ID'sini girin : ");

                            try {
                                int silinecekKitapID = Convert.ToInt32(Console.ReadLine());

                                if (kitapIslemleri.KitapSil(silinecekKitapID)) {

                                    Console.WriteLine("\nKitap başarıyla silindi ✅ ");
                                    Console.WriteLine($"{kitapIslemleri.ToplamKitapSayisi} adet kitap bulunmaktadır.");

                                    Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                    Console.ReadKey();

                                } else {
                                    // Hata mesajını gösteriyoruz
                                    Console.WriteLine("\nBu ID'ye sahip bir kitap bulunamadı 🚫!");
                                    Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                    Console.ReadKey();
                                }

                            } catch (FormatException) {
                                // Sayısal olmayan giriş durumunda hata mesajı
                                Console.WriteLine("\nLütfen geçerli bir ID giriniz 🚫!");
                                Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                Console.ReadKey();
                            }

                            break;
                        }

                        case 4: { // Toplam Kitap Sayısı
                            Console.Clear();
                            Console.WriteLine(new string('-', 25));
                            Console.WriteLine("   TOPLAM KİTAP SAYISI   ");
                            Console.WriteLine(new string('-', 25));

                            Console.WriteLine($"\n 🔹 Kütüphanede toplam {kitapIslemleri.ToplamKitapSayisi} adet kitap bulunmaktadır.");

                            Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                            Console.ReadKey();
                            break;
                        }

                        case 5: { // kullanıcı ekleme
                            break;
                        }

                        case 6:
                            Console.WriteLine("Çıkış yapılıyor...");
                            break;
                        default:
                            Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                            break;

                    }

                    if (secim != 6) {
                        Menu.MenuForAdmins();
                    }

                } 


                    if (secim == 6) {
                        currentUser = null; // aynısını normal kullanıcılar için de yapmalıyım.
                        continue;
                    }
                

                
                else { // normal kullanıcı işlemleri

                    switch (secim) {
                        case 1: { 
                            Console.Clear();
                            Console.WriteLine("📌 1- Tüm kitapları listele");

                            var kategoriler = Enum.GetNames(typeof(KitapKategorisi)); // kategoriler burada bir dizi. GetNames dizi döndürür.
                            for (int i = 0; i < kategoriler.Length; i++) {
                                Console.WriteLine($"📌 {i + 2}- {kategoriler[i]} kategorisine ait kitapları listele");
                            }

                            Console.WriteLine("Lütfen listelemek istediğiniz kitap türünü seçin! ");

                            if (int.TryParse(Console.ReadLine(), out int listelemeTercihi)) {
                                kitapIslemleri.KitaplariListele(listelemeTercihi);
                            } else {
                                Menu.ShowError("Geçersiz seçim 🚫!");
                            }


                            Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                            Console.ReadKey();
                            break;
                        }

                        case 2: { // kitap ödünç alma
                            break;
                        }

                        case 3: { // kitap iade etme
                            break;
                        }

                        case 4: {
                            Console.WriteLine("Çıkış yapılıyor...");
                            break;
                        }

                        default:
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                        break;
                    }

                    if (secim != 4) {
                        Menu.MenuForUsers();
                    } 


                }

                    
                    
                if (secim == 4) {
                    currentUser = null; // aynısını normal kullanıcılar için de yapmalıyım.
                    continue;                    
                }

            
            
            
            }

        }
    }


    class Kitap
    {

        public int Id { get; set; }
        public string? KitapAdi { get; set; }
        public string? Yazar { get; set; }
        public string? Category { get; set; }

    }


    class KutuphaneYoneticisi {

        private readonly string dosyaYolu;

        public KutuphaneYoneticisi(string dosyaYolu)
        {
            this.dosyaYolu = dosyaYolu;

            if (File.Exists(dosyaYolu))
            {
                string jsonString = File.ReadAllText(dosyaYolu);
                Kitaplar = JsonSerializer.Deserialize<List<Kitap>>(jsonString) ?? new List<Kitap>();
            }
        }



        private List<Kitap> Kitaplar = new List<Kitap>();

        public KitapKategorisi kategori1 = KitapKategorisi.Tarih;
        public KitapKategorisi kategori2 = KitapKategorisi.Bilim;
        public KitapKategorisi kategori3 = KitapKategorisi.Edebiyat;
        public KitapKategorisi kategori4 = KitapKategorisi.Felsefe;



        public int ToplamKitapSayisi => Kitaplar.Count;

        private void DosyayaKaydet() {

            try {
                string jsonString = JsonSerializer.Serialize(Kitaplar, new JsonSerializerOptions
                {
                    WriteIndented = true // Okunaklı Json formatı için, aksi halde json içeriği tek satırda gelirdi.
                });

                File.WriteAllText(dosyaYolu, jsonString);

            }
            catch (Exception ex) {
                Console.WriteLine($"Dosya kaydetme hatası: {ex.Message}");
                throw;
            }

        }

        public bool KitapEkle(Kitap gelenKitap)
        {

            try
            {

                if (IdMevcutMu(gelenKitap.Id))
                {
                    return false;
                }

                Kitaplar.Add(gelenKitap);
                DosyayaKaydet();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                throw;
            }


        }


        public void KitaplariListele(int listelemeTercihi)
        {
            Console.WriteLine("--- Kitap Listesi ---");

            if (!Kitaplar.Any())
            {
                Console.WriteLine("Henüz hiç kitap eklenmemiş 🚫");
                return;
            }


            var filtreliListe = listelemeTercihi switch
            {
                1 => Kitaplar,
                2 => Kitaplar.Where(k => k.Category == kategori1.ToString()),
                3 => Kitaplar.Where(k => k.Category == kategori2.ToString()),
                4 => Kitaplar.Where(k => k.Category == kategori3.ToString()),
                5 => Kitaplar.Where(k => k.Category == kategori4.ToString()),
                _ => Enumerable.Empty<Kitap>()

            };


            foreach (var item in filtreliListe)
            {
                Console.WriteLine($"🔹 ID: {item.Id}, Kitap: {item.KitapAdi}, Yazar: {item.Yazar}, Kategori: {item.Category}");
            }

        }



        public bool KitapSil(int silinecekKitapID)
        {

            try
            {
                var silBuKitabi = Kitaplar.Find(k => k.Id == silinecekKitapID);

                if (silBuKitabi != null)
                {
                    Kitaplar.Remove(silBuKitabi);
                    DosyayaKaydet();
                    return true;

                }
                else
                {
                    Menu.ShowError("\nBu ID'ye sahip bir kitap bulunamadı 🚫!");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kitap silme işlemi sırasında hata oluştu: {ex.Message}");
                return false;
            }

        }

        public bool IdMevcutMu(int id)
        {
            return Kitaplar.Exists(k => k.Id == id);
        }

        public void ShowCategories()
        {
            Console.WriteLine("--- Kategoriler ---");

            var kategoriler = Enum.GetNames(typeof(KitapKategorisi));
            for (int i = 0; i < kategoriler.Length; i++)
            {
                Console.WriteLine($" 📌 {i + 1} - {kategoriler[i]}");
            }

            Console.WriteLine();

        }

    }



    class User {

        public int      Id       {get; set;}
        
        [JsonInclude]
        public string? Username { get; private set; } // Bu yapı, dışarıdan Username bilgisinin okunabilmesine izin verir, ancak değerin değiştirilmesini kontrol altında tutar.

        // JsonInclude anlamı:
        /* 
        System.Text.Json kütüphanesinin, normalde yalnızca public getter ve setter'ları seri hale getirdiği durumlarda, 
        private setter veya public olmayan alanları da serileştirme ve deseralize etme sürecine dahil etmesini sağlar.
        */ 
        
         public string? Role { get; private set; }
        
        [JsonInclude]
        public string? Password { get; private set; }


        // JSON yüklemesi sırasında kullanılacak constructor (validasyon yapmadan)
        [JsonConstructor] 
        public User(int id, string username, string role, string password)
        {
            Id = id;
            Username = username;
            Role = role;
            Password = password;
        }
        /* JSON yüklemesi sırasında kullanılacak constructor (validasyon yapmadan). eğer herhangi bir kullanıcının Password değerİ 
           null geldiyse doğrulama yapıldığında (password set alanında) Şifre boş olamaz hatası alıyoruz. Dolayısıyla iki farklı constructor var.
        */


        public User() {
            Role = "user";
        }


        // Kayıt sırasında kullanılacak constructor, validasyon uygulanacak.
        public User(string enteredUsername, string enteredPassword, int id) {
            // Validasyon metodlarını kullanarak kontrol ediyoruz.
            try {
                UsernameControl = enteredUsername;
                PasswordControl = enteredPassword;
                Id = id;
                Role = "user";
                
            } catch (Exception ex) {
                Menu.ShowError(ex.Message);
                throw; // Hata yeniden fırlatılıyor
                
            }

            // Try-Catch Logic
            /* 
                `User` sınıfının constructor'ı, kullanıcı adı ve şifre için validasyon yapıyor. Validasyon işlemleri `UsernameControl` ve `PasswordControl` set 
                metotlarında gerçekleşiyor ve eğer bir hata olursa bu metotlar exception fırlatıyor. constructor'daki `catch` bloğu bu exceptionları yakalıyor., 
                `Menu.ShowError` ile hata mesajı gösteriliyor ve sonra `throw` ile exception yeniden fırlatılıyor.

                **Throw Yeniden Fırlatma:** `throw;` ifadesi, yakalanan exception'ı yeniden fırlatır. Bu durumda, exception constructor'dan çıkar ve `User` 
                nesnesinin oluşturulduğu yere (yani `Main` metodundaki `User user = new User(...);` satırına) ulaşır.

                **Main Metodunda Yakalama:** `Main` metodunda, kullanıcı kaydı işlemi sırasında `User` constructor'ı çağrılır. Eğer constructor içinde
                exception fırlatılırsa, bu exception `Main`'deki `try-catch` bloğu tarafından yakalanır. Bu sayede hata mesajı gösterilir ve kullanıcıya 
                tekrar deneme şansı verilir.
            */
        }


        // Kullanıcı girişinden alınan değeri validasyondan geçirerek Username'e atama
        [JsonIgnore]
        public string? UsernameControl {
            get {return Username;}
        
            set {

                if (value == null) {
                    throw new ArgumentException("Kullanıcı adı boş olamaz.");
                }

                value = value.Trim();

                string[] forbiddenChars = { "#", "@", "$", "{", "}", "<", ">" };

                if (value.Length <= 3 || value.Length >= 15) {
                    throw new ArgumentException("Kullanıcı adı 3 ile 15 karakter arasında olmalıdır.");
                }

                foreach (var item in forbiddenChars) {
                    if (value.Contains(item)) {
                        throw new ArgumentException("Kullanıcı adı geçersiz karakter içeriyor.");
                    }
                }

                Username = value;


            }
        
        }

        [JsonIgnore]
        public string? PasswordControl {
            
            get {return Password;}

            set {

                if (value == null) {
                    throw new ArgumentException("Şifre boş olamaz.");
                }

                value = value.Trim();

                if (value.Length <= 3 || value.Length >= 15) {
                    throw new ArgumentException("Şifre 3 ile 15 karakter arasında olmalıdır.");
                }

                bool containsNumber      = value.Any(char.IsDigit);
                bool containsSpecialChar = value.Any(c => "*?&#_".Contains(c)); // burayı pek anlamadım

                if (!containsNumber || !containsSpecialChar) {
                    throw new ArgumentException("Şifre en az 1 sayı ve 1 özel karakter (*?&#_) içermeli.");
                }

                Password = value;
                
            
            }
        }

        private readonly string? dosyaYolu2;
        private List<User> UserDatas = new List<User>();

        // Dosya yolunu alıp, varsa users.json dosyasından kullanıcıları yüklüyoruz.
        public User(string dosyayolu2)
        {
            this.dosyaYolu2 = dosyayolu2;
            if (File.Exists(dosyayolu2)) {
                string jsonString = File.ReadAllText(dosyayolu2);
                UserDatas = JsonSerializer.Deserialize<List<User>>(jsonString) ?? new List<User>();
            }
        }


        public bool AddUser(User newUser) {

            try {
                
                if ( KullaniciMevcutMu(newUser.Username) ) { // eğer burası true gelirse aşağısı false gelir ve kullanıcı eklenmez.
                    return false;
                }

                /* 
                Because in the setter, after showing the error, it calls return; but doesn't throw. So no exception is thrown. 
                Therefore, the try block in the constructor does not catch any exception, and the code proceeds. So, the Username would still 
                be null (since the setter didn't set it), but the user object is created with Id, Role, etc. Then, when AddUser is called, the 
                user is added to the list with a null or invalid username? Wait, but in the validation logic, if the username is invalid, the Username property isn't set. 
                So the user's Username would be null, but the code still adds the user to the list. But if the username is null, then maybe that check passes, 
                and the user is added with a null username.                
                */

                // şifre girdisinde özel karakter girmeden kayıt olmayı deniyorum, ilk başta hata mesajı veriyor ancak ardından kullanıcıyı ekliyor.

                UserDatas.Add(newUser);
                DosyayaKaydet();
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"Bir hata oluştu. {ex.Message}");
                return false;
            }

        }


        public bool KullaniciMevcutMu(string username) { // ÖNEMLİ: KÜÇÜK - BÜYÜK HARF DUYARLILIĞI DÜZENLE
            foreach (var item in UserDatas) {
                if (item.Username.ToLower() == username) { // kayıt olan kullanıcı adı daha önceden alınmışsa true döndürür. 
                    return true;
                } 

                // Döngü yerine bunu da yazabilirsin.  return Kitaplar.Exists(k => k.Id == id); burada 'k' item gibi geçici bir değişkendir.
            }
            return false;
        }


        public bool IdMevcutMu(int id) {
            foreach (var item in UserDatas) {
                if (item.Id == id) {
                    return true;
                } 

                // Döngü yerine bunu da yazabilirsin.  return UserDatas.Exists(k => k.Id == id); burada 'k' item gibi geçici bir değişkendir.
            }
            return false;
        }

        private void DosyayaKaydet() {
            
            try {
                
                string jsonString = JsonSerializer.Serialize(UserDatas, new JsonSerializerOptions {
                    WriteIndented = true
                });

                File.WriteAllText(dosyaYolu2, jsonString);

            } catch (Exception) {
                
                throw;
            }


        }


        public bool AuthenticateUser(string? username, string? password) {

            foreach (var item in UserDatas) {
                
                if ( (item.Username.ToLower() == username) && (item.Password == password) ) {
                    return true;
                } 
                
            }

            throw new ArgumentException("Girdiğiniz bilgilere ait bir kullanıcı bulunamadı 🚫, lütfen tekrar deneyin.");

        }

        public User? GetUserByUsername(string username) {
           
            // UserDatas listesi private olduğundan bu metot ile erişim sağlıyoruz.
            return UserDatas.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // FirstOrDefault örnek kullanım
        /* 
        List<int> sayilar = new List<int> { 5, 10, 15, 20 };

        int sonuc = sayilar.FirstOrDefault(s => s > 12);
        Console.WriteLine(sonuc);  // Çıktı: 15 (çünkü 12'den büyük ilk sayı 15)

        */



    }


    class Menu {

        public static void GirisKontrolu() {
            Console.WriteLine(" 1 - Giriş Yap ");
            Console.WriteLine(" 2 - Kayıt Ol ");
        }

        public static void MenuForAdmins() {
            Console.WriteLine("📌 1 - Yeni Kitap Ekle");
            Console.WriteLine("📌 2 - Kitapları Listele");
            Console.WriteLine("📌 3 - Kitap Sil");
            Console.WriteLine("📌 4 - Toplam Kitap Sayısını Göster");
            Console.WriteLine("📌 5 - Kullanıcı Ekle"); // Örneğin admin'e özel bir seçenek
            Console.WriteLine("📌 6 - Çıkış");
        }

        public static void MenuForUsers() {
            // Normal kullanıcı menüsü: sınırlı seçenekler
            Console.WriteLine("📌 1 - Kitapları Listele");
            Console.WriteLine("📌 2 - Kitap Ödünç Alma İşlemleri");
            Console.WriteLine("📌 3 - Kitap İade Etme İşlemleri");
            Console.WriteLine("📌 4 - Çıkış");
        }
        public static void ShowMenu() {
            Console.Clear();

            Console.WriteLine(new string('-', 25));
            Console.WriteLine("   KÜTÜPHANE YÖNETİM SİSTEMİ   ");
            Console.WriteLine(new string('-', 25));

            Console.WriteLine();

            Console.WriteLine(" 📌 1 - Yeni Kitap Ekle");
            Console.WriteLine(" 📌 2 - Kitapları Listele");
            Console.WriteLine(" 📌 3 - Kitap Sil");
            Console.WriteLine(" 📌 4 - Toplam Kitap Sayısını Göster");
            Console.WriteLine(" 📌 5 - Çıkış");

            Console.WriteLine();
            Console.Write("\nLütfen bir işlem seçiniz (1-5): ");

        }
        public static void ShowError(string message) {
            Console.WriteLine();
            Console.WriteLine(new string('!', 25));
            Console.WriteLine($"HATA: {message}");
            Console.WriteLine(new string('!', 25));
            Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
            Console.ReadKey();
        }

        public static void ShowSuccess(string message) {
            Console.WriteLine();
            Console.WriteLine(new string('*', 25));
            Console.WriteLine($"BAŞARILI: {message}");
            Console.WriteLine(new string('*', 25));
            Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
            Console.ReadKey();
        }

    }
}
