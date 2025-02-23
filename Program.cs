using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Linq;



namespace libraryApp {

    public enum KitapKategorisi { 
        Tarih, 
        Bilim,
        Edebiyat,
        Felsefe
    }


    class Program {
        
        static void Main(string[] args) {

            string dosyaYolu = "kitaplar.json";
            KutuphaneYoneticisi kitapIslemleri = new KutuphaneYoneticisi(dosyaYolu);
            Random rnd                         = new Random(); 


            while (true) {
                Menu.ShowMenu();

                int secim;
                try {
                    secim = Convert.ToInt32(Console.ReadLine());

                    if (secim < 1 || secim > 5) {
                        Console.WriteLine("\nGeçersiz seçim! Devam etmek için bir tuşa basınız...");
                        Console.ReadKey();
                        continue;
                    }

                } catch (FormatException) {
                    Console.WriteLine("Lütfen geçerli bir sayı giriniz!");
                    continue;
                }

                Console.WriteLine($"Seçiminiz : {secim}");

                switch (secim) {
                    case 1: {   // KİTAP EKLE

                        Console.Clear(); // Yeni işlem için ekranı temizle
                        Console.WriteLine(new string('-', 25));
                        Console.WriteLine("   YENİ KİTAP EKLEME   ");
                        Console.WriteLine(new string('-', 25));
                        Console.WriteLine();


                        kitapIslemleri.ShowCategories();

                        Console.Write("Kitap Adı : ");
                        string? eklenecekKitap = Console.ReadLine();
                    
                        if (string.IsNullOrWhiteSpace(eklenecekKitap)) {
                            Menu.ShowError("Kitap adı boş olamaz!");
                            continue;
                        }


                        Console.Write("Yazar : ");
                        string? eklenecekYazar = Console.ReadLine();
                        
                        if (string.IsNullOrWhiteSpace(eklenecekYazar)) {
                            Menu.ShowError("Yazar adı boş olamaz!");
                            continue;
                        }


                        Console.WriteLine("Lütfen kitabınız için yukarıdaki kategorilerden birini ekleyin! \n");
                        Console.Write("Kategori Adı : ");
                        string? eklenecekKategori = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(eklenecekKategori)) {
                            Menu.ShowError("Kategori adı boş olamaz!");
                            continue;
                        }

                        eklenecekKategori = char.ToUpper(eklenecekKategori[0]) + eklenecekKategori.Substring(1).ToLower();

                        if (!Enum.GetNames(typeof(KitapKategorisi)).Contains(eklenecekKategori)) { 
                            Menu.ShowError("Lütfen doğru bir kategori adı giriniz!");
                            continue;
                        }

                        
                        Console.WriteLine("");
                        Console.WriteLine("");


                        int kitapID;
                        do {
                            kitapID = rnd.Next(1, 1000);
                        } while(kitapIslemleri.IdMevcutMu(kitapID));


                        var kitap = new Kitap() {
                            Id       = kitapID,
                            KitapAdi = eklenecekKitap,
                            Yazar    = eklenecekYazar,
                            Category = eklenecekKategori
                        };


                        if ( kitapIslemleri.KitapEkle(kitap) ) {
                            Menu.ShowSuccess("Kitap başarıyla eklendi.");
                        } else {
                            Menu.ShowError("Bu ID'ye ait bir kitap zaten mevcut.");
                        }

                        break;
                    }                    


                    case 2: {   // KİTAP LİSTELE
                        Console.Clear();
                        Console.WriteLine("1- Tüm kitapları listele");

                        var kategoriler = Enum.GetNames(typeof(KitapKategorisi)); // kategoriler burada bir dizi. GetNames dizi döndürür.
                        for (int i = 0; i < kategoriler.Length; i++) {
                            Console.WriteLine($"{i + 2}- {kategoriler[i]} kategorisine ait kitapları listele");
                        }

                        Console.WriteLine("Lütfen listelemek istediğiniz kitap türünü seçin! ");


                        if (int.TryParse(Console.ReadLine(), out int listelemeTercihi)) {
                            kitapIslemleri.KitaplariListele(listelemeTercihi);
                        } else {
                            Menu.ShowError("Geçersiz seçim!");
                        }


                        Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                        Console.ReadKey();
                        break;
                    }      



                    case 3: {   // KİTAP SİL
                        Console.WriteLine(new string('-', 25));
                        Console.WriteLine("   KİTAP LİSTESİ   ");
                        Console.WriteLine(new string('-', 25));

                        Console.WriteLine("1- Tüm kitapları listele");
                        var kategoriler = Enum.GetNames(typeof(KitapKategorisi)); // kategoriler burada bir dizi. GetNames dizi döndürür.
                        for (int i = 0; i < kategoriler.Length; i++) {
                            Console.WriteLine($"{i + 2}- {kategoriler[i]} kategorisine ait kitapları listele");
                        }

                        Console.WriteLine("\nLütfen silmek istediğiniz kitap türünü seçin! \n");

                        int listelemeTercihi = Convert.ToInt32(Console.ReadLine());
                        kitapIslemleri.KitaplariListele(listelemeTercihi);                        
                        Console.Write("\nSilmek istediğiniz kitabın ID'sini girin : ");

                        try {
                            int silinecekKitapID = Convert.ToInt32(Console.ReadLine());                        
                            
                            if (kitapIslemleri.KitapSil(silinecekKitapID)) {

                                Console.WriteLine("\nKitap başarıyla silindi.");
                                Console.WriteLine($"{kitapIslemleri.ToplamKitapSayisi} adet kitap bulunmaktadır.");

                                Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                Console.ReadKey();

                            } else {
                                // Hata mesajını gösteriyoruz
                                Console.WriteLine("\nBu ID'ye sahip bir kitap bulunamadı!");
                                Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                Console.ReadKey();
                            }

                        }  catch (FormatException) {
                                // Sayısal olmayan giriş durumunda hata mesajı
                                Console.WriteLine("\nLütfen geçerli bir ID giriniz!");
                                Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                                Console.ReadKey();
                        }
                    

                        break;
                    }

                    case 4: {
                        Console.Clear();
                        Console.WriteLine(new string('-', 25));
                        Console.WriteLine("   TOPLAM KİTAP SAYISI   ");
                        Console.WriteLine(new string('-', 25));    

                        Console.WriteLine($"\nKütüphanede toplam {kitapIslemleri.ToplamKitapSayisi} adet kitap bulunmaktadır.");

                        Console.WriteLine("\nDevam etmek için bir tuşa basınız...");
                        Console.ReadKey();
                        break;
                    }


                    case 5: {
                        Console.WriteLine("Çıkış yapılıyor...");
                        return;
                    }


                    default: {
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                        break;
                    }


                }

            }

        }  
    }


    class Kitap {    

        public int     Id {get; set;} 
        public string? KitapAdi {get; set;}
        public string? Yazar {get; set;}
        public string? Category {get; set;}

    }


    class KutuphaneYoneticisi {

        private readonly string dosyaYolu;
        
        public KutuphaneYoneticisi(string dosyaYolu) {
            this.dosyaYolu = dosyaYolu;

            if (File.Exists(dosyaYolu)) {
                string jsonString = File.ReadAllText(dosyaYolu);
                Kitaplar          = JsonSerializer.Deserialize<List<Kitap>>(jsonString) ?? new List<Kitap>();
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
                string jsonString = JsonSerializer.Serialize(Kitaplar, new JsonSerializerOptions{
                    WriteIndented = true // Okunaklı Json formatı için, aksi halde json içeriği tek satırda gelirdi.
                });

                File.WriteAllText(dosyaYolu, jsonString);   

            } catch (Exception ex) {
                Console.WriteLine($"Dosya kaydetme hatası: {ex.Message}");
                throw;
            }
            
        }

        public bool KitapEkle(Kitap gelenKitap) {
            
            try {
                
                if (IdMevcutMu(gelenKitap.Id)) {
                    return false;
                }
                
                Kitaplar.Add(gelenKitap); 
                DosyayaKaydet();
                    
                return true;            
                    
            } catch (Exception ex) {
               Console.WriteLine($"Hata oluştu: {ex.Message}");
               throw;
            }
                    

        }

        
        public void KitaplariListele(int listelemeTercihi) {
            Console.WriteLine("--- Kitap Listesi ---");
            
            if (!Kitaplar.Any()) {
                Console.WriteLine("Henüz hiç kitap eklenmemiş");
                return;
            }


            var filtreliListe = listelemeTercihi switch {
                1 => Kitaplar,
                2 => Kitaplar.Where(k => k.Category == kategori1.ToString()),
                3 => Kitaplar.Where(k => k.Category == kategori2.ToString()),
                4 => Kitaplar.Where(k => k.Category == kategori3.ToString()),
                5 => Kitaplar.Where(k => k.Category == kategori4.ToString()),
                _ =>  Enumerable.Empty<Kitap>()

            };

   
            foreach (var item in filtreliListe) {
                Console.WriteLine($"ID: {item.Id}, Kitap: {item.KitapAdi}, Yazar: {item.Yazar}, Kategori: {item.Category}");
            }

        }



       public bool KitapSil(int silinecekKitapID) {

            try {
                var silBuKitabi = Kitaplar.Find(k => k.Id == silinecekKitapID);
            
                if (silBuKitabi != null) {
                    Kitaplar.Remove(silBuKitabi);
                    DosyayaKaydet(); 
                    return true;
                    
                } else {
                    Menu.ShowError("\nBu ID'ye sahip bir kitap bulunamadı!");
                    return false;
                }

            } catch (Exception ex) {
                Console.WriteLine($"Kitap silme işlemi sırasında hata oluştu: {ex.Message}");
                return false;
            }

        }

        public bool IdMevcutMu(int id) {
            return Kitaplar.Exists(k => k.Id == id);
        }

        public void ShowCategories() {
            Console.WriteLine("--- Kategoriler ---");
            
            var kategoriler = Enum.GetNames(typeof(KitapKategorisi));
            for (int i = 0; i < kategoriler.Length; i++) {
                Console.WriteLine($" {i + 1} - {kategoriler[i]}");
            }
            
            Console.WriteLine();

        }

    }

    class Menu {
        public static void ShowMenu() {
            Console.Clear();

            Console.WriteLine(new string('-', 25));
            Console.WriteLine("   KÜTÜPHANE YÖNETİM SİSTEMİ   ");
            Console.WriteLine(new string('-', 25));

            Console.WriteLine();

            Console.WriteLine("  1 - Yeni Kitap Ekle");
            Console.WriteLine("  2 - Kitapları Listele");
            Console.WriteLine("  3 - Kitap Sil");
            Console.WriteLine("  4 - Toplam Kitap Sayısını Göster");
            Console.WriteLine("  5 - Çıkış");


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
        