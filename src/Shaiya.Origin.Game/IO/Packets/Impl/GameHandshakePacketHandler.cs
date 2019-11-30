using Shaiya.Origin.Common.Database.Structs.Game;
using Shaiya.Origin.Common.Logging;
using Shaiya.Origin.Common.Networking.Client;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Common.Serializer;
using System;
using System.Net;

namespace Shaiya.Origin.Game.IO.Packets.Impl
{
    public class GameHandshakePacketHandler : PacketHandler
    {
        /// <summary>
        /// Handles an incoming game handshake request
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, byte[] data)
        {
            // Verify that the length is correct
            if (length != 20)
            {
                return true;
            }

            GameHandshakeRequest handshake = new GameHandshakeRequest();

            handshake.userId = BitConverter.ToInt32(data, 0);
            Array.Copy(data, 4, handshake.identityKeys, 0, 16);

            var dbClient = GameService.GetDbClient();

            var bldr = new PacketBuilder(Common.Database.Opcodes.USER_GAME_CONNECT);

            var array = Serializer.Serialize(handshake);

            bldr.WriteBytes(array);

            dbClient.Write(bldr.ToPacket(), (_data, _length) =>
            {
                int result = _data[0] & 0xFF;

                var builder = new PacketBuilder(opcode);

                builder.WriteByte((byte)result);

                // If the result is 0, we should create a player instance, and request to load it's data
                if (result == 0)
                {
                    Logger.Info("Accepted successful handshake from address: {0}, with the user id of {1}", session.GetRemoteAdress(), handshake.userId.ToString());

                    var player = new Model.Entity.Player.Player(handshake.userId, session);

                    GameService.LoadPlayer(player);
                }

                session.Write(builder.ToPacket());
            });

            return true;
        }
    }
}