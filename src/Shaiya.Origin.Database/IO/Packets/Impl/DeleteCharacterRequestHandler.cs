using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Database.Connector;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class DeleteCharacterRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the deleting of a character.
        /// </summary>
        /// <param name="session">The session instace</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            PacketBuilder bldr = new PacketBuilder(opcode);

            bldr.WriteInt(requestId);

            int userId = BitConverter.ToInt32(data);
            int characterId = BitConverter.ToInt32(data.Skip(4).ToArray());
            int serverId = data[8];

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {
                var cmd = new SqlCommand("UPDATE characters SET remain_deletion_time = NOW() WHERE character_id = @character_id AND user_id = @user_id AND server_id = @server_id", connection);
                cmd.Parameters.AddWithValue(":character_id", characterId);
                cmd.Parameters.AddWithValue(":user_id", userId);
                cmd.Parameters.AddWithValue(":server_id", serverId);

                connection.Open();

                bldr.WriteByte((byte)(cmd.ExecuteNonQuery() != 0 ? 0 : 1));
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}