using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class CharacterScreenDataRequestHandler : PacketHandler
    {
        /// <summary>
        /// Handles the loading of character data.
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="requestId"></param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, int requestId, byte[] data)
        {
            PacketBuilder bldr = new PacketBuilder(opcode);

            bldr.WriteInt(requestId);

            List<Character> characters = new List<Character>();

            using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
            {
                int userId = BitConverter.ToInt32(data, 0);
                byte serverId = data[4];

                SqlCommand rowCmd = new SqlCommand("SELECT COUNT(*) FROM load_game_characters(@user_id, @server_id)", connection);
                rowCmd.Parameters.AddWithValue("@user_id", userId);
                rowCmd.Parameters.AddWithValue("@server_id", (int)serverId);

                SqlCommand cmd = new SqlCommand("load_game_characters", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(":user_id", userId);
                cmd.Parameters.AddWithValue(":server_id", (int)serverId);

                connection.Open();

                int rowCount = Convert.ToInt32(rowCmd.ExecuteScalar());

                // Execute the prepared statement
                SqlDataReader reader = cmd.ExecuteReader();

                bldr.WriteInt(rowCount);

                // Loop through the results
                while (reader.Read())
                {
                    Character character = new Character();

                    character.characterId = reader.GetInt32(0);
                    character.name = Encoding.UTF8.GetBytes(reader.GetString(1));
                    character.slot = (byte)reader.GetInt32(2);
                    character.level = reader.GetInt16(3);
                    character.race = (byte)reader.GetInt32(4);
                    character.gameMode = (byte)reader.GetInt32(5);
                    character.face = (byte)reader.GetInt32(6);
                    character.hair = (byte)reader.GetInt32(7);
                    character.height = (byte)reader.GetInt32(8);
                    character.profession = (byte)reader.GetInt32(9);
                    character.sex = (byte)reader.GetInt32(10);
                    character.map = reader.GetInt16(11);
                    character.strength = reader.GetInt16(12);
                    character.dexterity = reader.GetInt16(13);
                    character.resistance = reader.GetInt16(14);
                    character.intelligence = reader.GetInt16(15);
                    character.wisdom = reader.GetInt16(16);
                    character.luck = reader.GetInt16(17);
                    character.isPendingDeletion = (byte)(reader.IsDBNull(18) ? 0 : 1);

                    characters.Add(character);
                }
                reader.Close();
            }

            byte[] array = Serializer.Serialize(characters);

            bldr.WriteBytes(array);

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}