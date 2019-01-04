using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class CreateCharacterRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the creation of a character.
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            var request = new CreateCharacterRequest();

            int serverId = data[0];

            int userId = (data[1] & 0xFF) + ((data[2] & 0xFF) << 8) + ((data[3] & 0xFF) << 16) + ((data[4] & 0xFF) << 24);

            request = Serializer.Deserialize<CreateCharacterRequest>(data.Skip(5).ToArray());

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {

                var cmd = new SqlCommand("create_character", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(":server_id", serverId);
                cmd.Parameters.AddWithValue(":user_id", userId);
                cmd.Parameters.AddWithValue(":character_name", Encoding.UTF8.GetString(request.name).TrimEnd('\0'));
                cmd.Parameters.AddWithValue(":race", request.race);
                cmd.Parameters.AddWithValue(":mode", request.mode);
                cmd.Parameters.AddWithValue(":profession", request.profession);
                cmd.Parameters.AddWithValue(":hair", request.hair);
                cmd.Parameters.AddWithValue(":face", request.face);
                cmd.Parameters.AddWithValue(":height", request.height);
                cmd.Parameters.AddWithValue(":sex", request.sex);


                connection.Open();

                // Execute the prepared statement
                var reader = cmd.ExecuteReader();

                // Loop through the results
                while (reader.Read())
                {
                    var bldr = new PacketBuilder(opcode);

                    bldr.WriteInt(requestId);

                    // Write the result
                    bldr.WriteByte((byte)reader.GetInt32(0));

                    session.Write(bldr.ToPacket());

                }
                reader.Close();
            }


                return true;
        }
    }
}
