using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;
using System;
using System.Data.SqlClient;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class ServerListRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles a server list request from the login server
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            var bldr = new PacketBuilder(opcode);

            bldr.WriteInt(requestId);

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {
                var rowCmd = new SqlCommand("SELECT COUNT(*) FROM worlds", connection);
                var cmd = new SqlCommand("SELECT * FROM worlds", connection);

                connection.Open();

                rowCmd.Prepare();

                int rowCount = Convert.ToInt32(rowCmd.ExecuteScalar());

                // Execute the prepared statement
                var reader = cmd.ExecuteReader();

                bldr.WriteInt(rowCount);

                // Loop through the results
                while (reader.Read())
                {
                    Common.Database.Structs.Game.Server server = new Common.Database.Structs.Game.Server();

                    server.serverId = reader.GetInt16(0);
                    server.serverName = reader.GetString(1);
                    server.population = reader.GetInt16(2);
                    server.status = reader.GetInt16(3);
                    server.ipAddress = reader.GetString(4);
                    server.maxPlayers = reader.GetInt16(5);
                    server.clientVersion = reader.GetInt16(6);

                    var array = Serializer.Serialize(server);

                    bldr.WriteBytes(array, array.Length);
                }
                reader.Close();
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}