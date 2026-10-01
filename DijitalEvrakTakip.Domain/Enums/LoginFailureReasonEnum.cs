using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum LoginFailureReasonEnum
{
    [Description("Kullanıcı Bulunamadı")]
    UserNotFound = 1,

    [Description("Şifre Hatalı")]
    InvalidPassword = 2,

    [Description("Kullanıcı Silinmiş")]
    UserDeleted = 3,

    [Description("Kullanıcı Pasif")]
    UserInactive = 4
}
