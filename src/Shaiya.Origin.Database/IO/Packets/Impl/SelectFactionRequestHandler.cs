using System;
using System.Data.SqlClient;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Database.Connector;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class SelectFactionRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the selection of a faction for the player.
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            int userId = BitConverter.ToInt32(data, 0);
            byte faction = data[4];
            byte serverId = data[5];

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {

                var cmd = new SqlCommand("UPDATE users SET faction = @faction WHERE user_id = @user_id AND server_id = @server_id", connection);
                cmd.Parameters.AddWithValue(":faction", faction);
                cmd.Parameters.AddWithValue(":user_id", userId);
                cmd.Parameters.AddWithValue(":server_id", serverId);

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
