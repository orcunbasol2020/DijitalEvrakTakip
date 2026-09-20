using System.ComponentModel;

namespace DijitalEvrakTakip.Domain.Enums;

public enum UrgencyDegreeEnum
{
    [Description("Normal")]
    Normal = 10005001,

    [Description("Acele")]
    Urgent = 10005002,

    [Description("Çok Acele")]
    VeryUrgent = 10005003,

    [Description("Yıldırım")]
    Lightning = 10005004,

    [Description("Günlüdür")]
    Dated = 10005005,

    [Description("İvedi Süreli")]
    UrgentTimeLimited = 10005006
}
