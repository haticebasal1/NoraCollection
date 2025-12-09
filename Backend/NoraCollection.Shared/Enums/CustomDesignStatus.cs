namespace NoraCollection.Shared.Enums;

public enum CustomDesignStatus
{
    Pending = 0,        // Kullanıcı gönderdi, beklemede
    Reviewed = 1,       // Admin inceledi
    Priced = 2,         // Fiyat belirlendi
    InProduction = 3,   // Üretimde
    Completed = 4,      // Tamamlandı
    Rejected = 5        // Reddedildi
}

