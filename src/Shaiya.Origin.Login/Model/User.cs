using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shaiya.Origin.Database.Model
{
    [Table("Users_Master")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserUID")]
        public int Id { get; set; }

        [Column("UserID")]
        public string Name { get; set; }

        [Column("Pw")]
        public string Password { get; set; }

        [Column("Admin")]
        public bool IsAdmin { get; set; }

        [Column("AdminLevel")]
        public byte AdminLevel { get; set; }

        [Column("Status")]
        public short Status { get; set; }
    }
}
