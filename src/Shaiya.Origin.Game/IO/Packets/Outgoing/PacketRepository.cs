using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Game.Model.Entity.Player;
using System;

namespace Shaiya.Origin.Game.IO.Packets.Outgoing
{
    /// <summary>
    /// Represents a utility class that is used to easily construct and send outgoing packets.
    /// </summary>
    public static class PacketRepository
    {
        /// <summary>
        /// Sends the standard character data.
        /// </summary>
        /// <param name="character">The character instance to send the data to</param>
        public static void SendCharacterData(Character character)
        {
            var bldr = new PacketBuilder(Common.Packets.Opcodes.CHARACTER_DETAILS);

            var attributes = character.GetAttributes();

            bldr.WriteShort(attributes.strength);
            bldr.WriteShort(attributes.dexterity);
            bldr.WriteShort(attributes.resistance);
            bldr.WriteShort(attributes.intelligence);
            bldr.WriteShort(attributes.wisdom);
            bldr.WriteShort(attributes.luck);
            bldr.WriteShort(character.statPoints);
            bldr.WriteShort(character.skillPoints);
            bldr.WriteInt(9); // Max HP
            bldr.WriteInt(10); // Max MP
            bldr.WriteInt(11); // Max SP

            bldr.WriteShort(character.GetPosition().direction);

            // EXP Values are multiplied by 10

            // Previous EXP was 1000
            bldr.WriteInt(100);

            // Next EXP is at 2500
            // Client takes the previous value, calculates the difference = 1500
            bldr.WriteInt(250); // 2500

            // Current EXP is at 1200
            // Client takes the previous value, difference = 200
            bldr.WriteInt(120); // 1200

            // Client should display 200 / 1500 for EXP

            bldr.WriteInt(character.gold);

            var position = character.GetPosition();

            // Write the position values
            bldr.WriteBytes(BitConverter.GetBytes(position.x));
            bldr.WriteBytes(BitConverter.GetBytes(position.height));
            bldr.WriteBytes(BitConverter.GetBytes(position.y));

            bldr.WriteInt(character.kills);
            bldr.WriteInt(character.deaths);
            bldr.WriteInt(character.victories);
            bldr.WriteInt(character.defeats);

            bldr.WriteByte(0); // No guild

            character.Write(bldr.ToPacket());
        }

        /// <summary>
        /// Sends the player's current Aeria Points.
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void SendAp(Player player)
        {
            var bldr = new PacketBuilder(Common.Packets.Opcodes.ACCOUNT_AERIA_POINTS);

            bldr.WriteInt(player.points);
            player.Write(bldr.ToPacket());
        }
    }
}