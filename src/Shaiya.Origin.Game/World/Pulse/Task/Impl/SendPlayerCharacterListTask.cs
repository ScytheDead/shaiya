using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Client;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Game.Model.Entity.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Shaiya.Origin.Game.World.Pulse.Task.Impl
{
    /// <summary>
    /// Represents a task used to send the player's character list.
    /// </summary>
    public class SendPlayerCharacterListTask : Task
    {
        private Player _player;

        /// <summary>
        /// The constructor for this task, which will define the player instance
        /// to operate on.
        /// </summary>
        /// <param name="player">The player instance</param>
        public SendPlayerCharacterListTask(Player player)
        {
            _player = player;
        }

        /// <summary>
        /// Handle the sending of a player's character list.
        /// </summary>
        public override void Execute()
        {
            var localPlayer = _player;

            var dbClient = GameService.GetDbClient();

            var bldr = new PacketBuilder(Common.Database.Opcodes.GET_CHARACTER_LIST_DATA);

            bldr.WriteInt(localPlayer.index);

            bldr.WriteByte((byte)GameService.GetServerId());

            dbClient.Write(bldr.ToPacket(), (_data, _length) =>
            {
                int characterCount = BitConverter.ToInt32(_data);

                List<Common.Database.Structs.Game.Character> characters = Serializer.Deserialize<List<Common.Database.Structs.Game.Character>>(_data.Skip(4).ToArray());

                List<int> slots = new List<int>(new int[] { 0, 1, 2, 3, 4 });

                foreach (var character in characters)
                {

                    slots.Remove(character.slot);

                    var responseBldr = new PacketBuilder(Common.Packets.Opcodes.CHARACTER_LIST);

                    responseBldr.WriteByte(character.slot);

                    responseBldr.WriteInt(character.characterId);

                    // Unknown (Character creation time?)
                    responseBldr.WriteInt(0);

                    responseBldr.WriteShort(character.level);

                    responseBldr.WriteByte(character.race);

                    responseBldr.WriteByte(character.gameMode);

                    responseBldr.WriteByte(character.hair);

                    responseBldr.WriteByte(character.face);

                    responseBldr.WriteByte(character.height);

                    responseBldr.WriteByte(character.profession);

                    responseBldr.WriteByte(character.sex);

                    responseBldr.WriteShort(character.map);

                    responseBldr.WriteShort(character.strength);
                    responseBldr.WriteShort(character.dexterity);
                    responseBldr.WriteShort(character.resistance);
                    responseBldr.WriteShort(character.intelligence);
                    responseBldr.WriteShort(character.wisdom);
                    responseBldr.WriteShort(character.luck);

                    // Unknown array
                    for (int j = 0; j < 11; j++)
                    {
                        responseBldr.WriteByte(0);
                    }

                    // The item types
                    responseBldr.WriteBytes(character.itemTypes);

                    // The item type ids
                    responseBldr.WriteBytes(character.itemTypeIds);

                    // Write 535 null bytes
                    for (int j = 0; j < 535; j++)
                    {
                        responseBldr.WriteByte(0);
                    }

                    responseBldr.WriteBytes(character.name);

                    // Write the character deletion flag
                    responseBldr.WriteByte(0);

                    localPlayer.Write(responseBldr.ToPacket());
                }

                // Loop through the empty slots
                foreach (byte slot in slots)
                {
                    var responseBldr = new PacketBuilder(Common.Packets.Opcodes.CHARACTER_LIST);

                    responseBldr.WriteByte(slot);

                    // Write the empty character id
                    responseBldr.WriteInt(0);

                    localPlayer.Write(responseBldr.ToPacket());
                }
            });

            
        }
    }
}