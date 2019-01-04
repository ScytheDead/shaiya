using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Database.Connector;
using System;
using System.Data.SqlClient;
using System.Text;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class CheckAvailableNameRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the checking of an available character name.
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            byte[] nameArray = new byte[length];

            Array.Copy(data, nameArray, length);

            string name = Encoding.UTF8.GetString(nameArray);

            var bldr = new PacketBuilder(opcode);

            bldr.WriteInt(requestId);

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {
                var cmd = new SqlCommand("check_available_name", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(":name", name);

                connection.Open();

                var reader = cmd.ExecuteReader();

                // Loop through the results
                while (reader.Read())
                {
                    // Write the result
                    bldr.WriteByte((byte)reader.GetInt32(0));
                }
                reader.Close();
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}