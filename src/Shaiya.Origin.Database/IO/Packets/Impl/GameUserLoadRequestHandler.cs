using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;
using System;
using System.Data.SqlClient;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class GameUserLoadRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the loading of a player instance
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
                int userId = BitConverter.ToInt16(data, 0);
                byte serverId = data[4];

                var cmd = new SqlCommand("load_game_account", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(":user_id", userId);
                cmd.Parameters.AddWithValue(":server_id", serverId);

                connection.Open();

                // Execute the prepared statement
                var reader = cmd.ExecuteReader();

                // Loop through the results
                while (reader.Read())
                {
                    GameLoadPlayerResponse gameLoadPlayerResponse = new GameLoadPlayerResponse
                    {
                        faction = reader.GetInt16(0),
                        maxGameMode = reader.GetInt16(1),
                        privilegeLevel = reader.GetInt16(2),
                        points = reader.GetInt16(3)
                    };

                    var array = Serializer.Serialize(gameLoadPlayerResponse);

                    bldr.WriteBytes(array);

                }
                reader.Close();
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}