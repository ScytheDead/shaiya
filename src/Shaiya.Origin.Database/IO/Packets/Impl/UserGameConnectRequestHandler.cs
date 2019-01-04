using System.Data.SqlClient;
using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class UserGameConnectRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the verification of a game handshake request
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            GameHandshakeRequest handshake = new GameHandshakeRequest();

            handshake = Serializer.Deserialize<GameHandshakeRequest>(data);

            var bldr = new PacketBuilder(opcode);

            bldr.WriteInt(requestId);

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_userdata"))
            {
                var cmd = new SqlCommand("validate_game_connect", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(":user_id", handshake.userId);
                cmd.Parameters.AddWithValue(":identity_keys", handshake.identityKeys);

                connection.Open();

                // Execute the prepared statement
                var reader = cmd.ExecuteReader();

                // Loop through the results
                while (reader.Read())
                {
                    byte response = (byte)reader.GetInt16(0);

                    bldr.WriteByte(response);
                }
                reader.Close();
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}