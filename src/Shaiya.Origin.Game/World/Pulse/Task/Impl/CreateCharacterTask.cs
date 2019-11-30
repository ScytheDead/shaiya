using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Serializer;
using Shaiya.Origin.Game.Model.Entity.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shaiya.Origin.Game.World.Pulse.Task.Impl
{
    /// <summary>
    /// Represents a task used to create a new character instance.
    /// </summary>
    public class CreateCharacterTask : Task
    {
        private Player _player;
        private CreateCharacterRequest _request;

        /// <summary>
        /// The constructor for this task, which will define the player instance
        /// to operate on.
        /// </summary>
        /// <param name="player">The player instance</param>
        public CreateCharacterTask(Player player, CreateCharacterRequest request)
        {
            _player = player;
            _request = request;
        }

        /// <summary>
        /// Begin processing the checking of a character creation request.
        /// </summary>
        public override void Execute()
        {
            var localPlayer = _player;
            var localRequest = _request;

            var dbClient = GameService.GetDbClient();

            var bldr = new PacketBuilder(Common.Database.Opcodes.CREATE_CHARACTER);

            var requestArray = Serializer.Serialize(_request);

            bldr.WriteByte((byte)GameService.GetServerId());

            bldr.WriteInt(localPlayer.index);

            bldr.WriteBytes(requestArray);

            dbClient.Write(bldr.ToPacket(), (_data, _length) =>
            {
                // Write the player's character list
                GameService.PushTask(new SendPlayerCharacterListTask(localPlayer));

                var responseBldr = new PacketBuilder(Common.Packets.Opcodes.CREATE_CHARACTER);

                // Write the response (if the response from the database server is -1, then character creation failed)
                // So we send the value 2 to show an error
                responseBldr.WriteByte((byte)((_data[0] & 0xFF) == -1 ? 2 : 0));

                localPlayer.Write(responseBldr.ToPacket());
            });

        }
    }
}
