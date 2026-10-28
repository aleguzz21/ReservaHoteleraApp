namespace ReservaHoteleraApp.DAL.Models.Base
{
    public abstract class AuditEntity : IdEntity
    {
        public DateTime CreatedDate { get; set; }
        public int? CreatedUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyUser { get; set; }
    }
}
