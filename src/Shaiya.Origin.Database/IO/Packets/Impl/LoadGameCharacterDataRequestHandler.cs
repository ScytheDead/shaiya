using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Logging;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Database.Connector;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Shaiya.Origin.Database.IO.Packets.Impl
{
    public class LoadGameCharacterDataRequestHandler : PacketHandler
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

            int userId = BitConverter.ToInt32(data);
            int characterId = BitConverter.ToInt32(data.Skip(4).ToArray());
            int serverId = BitConverter.ToInt32(data.Skip(8).ToArray());

            try
            {
                using (SqlConnection connection = new DatabaseConnector().GetConnection("origin_gamedata"))
                {
                    SqlCommand cmd = new SqlCommand("load_game_character", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(":user_id", userId);
                    cmd.Parameters.AddWithValue(":character_id", characterId);
                    cmd.Parameters.AddWithValue(":server_id", serverId);

                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CharacterLoadInfo character = new CharacterLoadInfo();

                        // Define the character details
                        character.characterId = reader.GetInt32(0);
                        character.profession = (byte)reader.GetInt32(1);
                        character.race = (byte)reader.GetInt32(2);
                        character.mode = (byte)reader.GetInt32(3);
                        character.level = (byte)reader.GetInt32(4);
                        character.hair = (byte)reader.GetInt32(5);
                        character.face = (byte)reader.GetInt32(6);
                        character.height = (byte)reader.GetInt32(7);
                        character.sex = (byte)reader.GetInt32(8);
                        character.strength = reader.GetInt32(9);
                        character.dexterity = reader.GetInt32(10);
                        character.resistance = reader.GetInt32(11);
                        character.intelligence = reader.GetInt32(12);
                        character.wisdom = reader.GetInt32(13);
                        character.luck = reader.GetInt32(14);
                        character.currentHp = reader.GetInt32(15);
                        character.currentMp = reader.GetInt32(16);
                        character.currentSp = reader.GetInt32(17);
                        character.statPoints = reader.GetInt16(18);
                        character.skillPoints = reader.GetInt16(19);
                        character.gold = reader.GetInt32(20);
                        character.kills = reader.GetInt32(21);
                        character.deaths = reader.GetInt32(22);
                        character.victories = reader.GetInt32(23);
                        character.defeats = reader.GetInt32(24);

                        // TODO: Guild ID
                        character.guildId = 0;

                        // The position details
                        character.mapId = reader.GetInt16(26);
                        character.direction = (byte)reader.GetInt32(27);
                        character.positionX = reader.GetFloat(28);
                        character.positionY = reader.GetFloat(29);
                        character.positionHeight = reader.GetFloat(30);

                        character.name = Encoding.UTF8.GetBytes(reader.GetString(31));

                        byte[] array = Serializer.Serialize(character);

                        // Write a successful load response
                        bldr.WriteByte(0);

                        bldr.WriteBytes(array);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                // Write a failed load response
                bldr.WriteByte(1);

                Logger.Error(e.ToString());
            }

            session.Write(bldr.ToPacket());

            return true;
        }
    }
}