using Shaiya.Origin.Common.Networking.Client;
using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using System.Net;

namespace Shaiya.Origin.Login.IO.Packets.Impl
{
    public class ConnectionTerminatedPacketHandler : PacketHandler
    {
        /// <summary>
        /// Handles a terminated connection packet
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, byte[] data)
        {
            var identityKeys = session.GetIdentityKeys();

            var dbClient = LoginService.GetDbClient();

            var bldr = new PacketBuilder(Common.Database.Opcodes.DELETE_SESSION);

            bldr.WriteBytes(identityKeys);

            dbClient.Write(bldr.ToPacket());

            byte[] emptyKeys = new byte[16];

            session.SetIdentityKeys(emptyKeys);

            session.Close();

            return false;
        }
    }
}