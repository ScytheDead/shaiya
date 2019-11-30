using Shaiya.Origin.Common.Networking.Client;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Game.Model.Entity.Player;
using System;
using System.Net;
using System.Text;

namespace Shaiya.Origin.Game.World.Pulse.Task.Impl
{
    /// <summary>
    /// Represents a task used to check an available name from the database server.
    /// </summary>
    public class CheckAvailableNameTask : Task
    {
        private Player _player;
        private string _name;

        /// <summary>
        /// The constructor for this task, which will define the player instance
        /// to operate on.
        /// </summary>
        /// <param name="player">The player instance.</param>
        /// <param name="name">The name to check.</param>
        public CheckAvailableNameTask(Player player, string name)
        {
            _player = player;
            _name = name;
        }

        public override void Execute()
        {
            var localPlayer = _player;

            var dbClient = GameService.GetDbClient();

            var bldr = new PacketBuilder(Common.Database.Opcodes.CHECK_AVAILABLE_NAME);

            bldr.WriteBytes(Encoding.UTF8.GetBytes(_name));

            dbClient.Write(bldr.ToPacket(), (_data, _length) =>
            {
                int result = _data[0] & 0xFF;

                var responseBldr = new PacketBuilder(Common.Packets.Opcodes.AVAILABLE_CHARACTER_NAME);

                // Write the result to the player
                responseBldr.WriteByte(_data[0]);

                localPlayer.Write(responseBldr.ToPacket());
            });


        }
    }
}