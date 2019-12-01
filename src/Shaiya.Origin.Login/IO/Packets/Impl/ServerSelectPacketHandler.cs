using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Database;
using Shaiya.Origin.Login.Model;
using System.Linq;
using System.Net;

namespace Shaiya.Origin.Login.IO.Packets.Impl
{
    public class ServerSelectPacketHandler : PacketHandler
    {
        private readonly object _syncObject = new object();

        /// <summary>
        /// Handles an incoming game server selection
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The length of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, byte[] data)
        {
            // If the length is not 5
            if (length != 5)
            {
                return true;
            }

            var bldr = new PacketBuilder(opcode);

            int serverId = (data[0] & 0xFF);

            // TODO: not sure why, but here is always -1. If someone can ever answer me I would be very grateful.
            int clientVersion = ((data[1] & 0xFF) + ((data[2] & 0xFF) << 8) + ((data[3] & 0xFF) << 16) + ((data[4] & 0xFF) << 24));

            using (var dbContext = new ServersDbContext())
            {
                // TODO: somehow check client version...
                var foundServer = dbContext.Servers.SingleOrDefault(s => s.Id == serverId);
                if (foundServer is null) // Server not found.
                {
                    bldr.WriteByte(unchecked((byte)SelectServer.CannotConnect));
                    session.Write(bldr.ToPacket());
                    return true;
                }

                // TODO: remove || true, when you get how to check client version.
                if (clientVersion == foundServer.ClientVersion || true)
                {
                    bldr.WriteByte((byte)SelectServer.Success);

                    IPAddress ipAddress = IPAddress.Parse(foundServer.IpAddress.Trim());
                    foreach (var i in ipAddress.GetAddressBytes())
                    {
                        bldr.WriteByte(i);
                    }
                }
                session.Write(bldr.ToPacket());
            }
            return true;
        }
    }
}