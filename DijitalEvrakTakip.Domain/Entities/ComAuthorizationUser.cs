namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Kullanıcı listesi. EGovernmentCmsCommon.dbo.ComAuthorizationUser tablosunun birebir kopyası;
    /// anahtar ve kolonlar kaynakla aynı tutulur, veri oradan aktarılır.
    /// </summary>
    public class ComAuthorizationUser
    {
        public Guid UserId { get; set; }
        public int PersonId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int? AgencyId { get; set; }
        public int? TitleId { get; set; }
        public int? CountryId { get; set; }
        public string? NTLogonName { get; set; }
        public string? Email { get; set; }
        public string? ConnectsFrom { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Phone { get; set; }
        public DateTime? LeaveDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public Guid CreatorUserId { get; set; }
        public Guid ModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public string? Password { get; set; }
        public string? SecureMobileNumber { get; set; }
        public string? SecureMobileDynamicCode { get; set; }
        public string? InfoNote { get; set; }
        public int DigitalSignatureStateId { get; set; }
        public bool IsActive { get; set; }
        public bool ForceToChangePassword { get; set; }
        public bool LoginBlocked { get; set; }
        public string? StaffId { get; set; }
    }
}
