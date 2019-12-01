using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shaiya.Origin.Login.Model
{
    [Table("worlds")]
    public class Server
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("serverId")]
        public short Id { get; set; }

        [Column("serverName")]
        public string Name { get; set; }

        [Column("population")]
        public short Population { get; set; }

        [Column("status")]
        public short Status { get; set; }

        [Column("ipAddress")]
        public string IpAddress { get; set; }

        [Column("maxPlayers")]
        public short MaxPlayers { get; set; }

        [Column("clientVersion")]
        public short ClientVersion { get; set; }
    }
}
