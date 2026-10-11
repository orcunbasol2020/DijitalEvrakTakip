namespace DijitalEvrakTakip.Domain.Constants;

/// <summary>
/// Birim türleri (ComDepartment.DepartmentTypeId / Department.Type). Değerler kaynak sistemden
/// (EGovernmentCmsCommon) gelir.
/// </summary>
public static class DepartmentTypes
{
    public const int Embassy = 30001;
    public const int ConsulateGeneral = 30002;
    public const int PermanentRepresentation = 30003;
    public const int TradeOffice = 30004;
    public const int HonoraryConsulate = 30005;

    /// <summary>Merkez birim (Ankara teşkilatı)</summary>
    public const int Central = 30006;
}
