using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Enums;

public enum PaymentMethod
{
    [Display(Name = "Kredi Kartı")]
    CreditCard = 0,
    [Display(Name = "Havale / EFT")]
    WireTransfer = 1
}
