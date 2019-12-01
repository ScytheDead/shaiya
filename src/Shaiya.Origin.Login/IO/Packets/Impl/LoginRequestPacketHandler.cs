using Shaiya.Origin.Common.Networking.Packets;
using Shaiya.Origin.Common.Networking.Server.Session;
using Shaiya.Origin.Database;
using Shaiya.Origin.Login.Model;
using System;
using System.Linq;
using System.Text;

namespace Shaiya.Origin.Login.IO.Packets.Impl
{
    public class LoginRequestPacketHandler : PacketHandler
    {
        private static readonly object _syncObject = new object();

        /// <summary>
        /// Handles an incoming login request
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="length">The lenght of the packet</param>
        /// <param name="opcode">The opcode of the incoming packet</param>
        /// <param name="data">The packet data</param>
        /// <returns></returns>
        public override bool Handle(ServerSession session, int length, int opcode, byte[] data)
        {
            // If the packet is not the correct length
            if (length != 48)
            {
                return true;
            }

            byte[] usernameBytes = new byte[18];
            byte[] passwordBytes = new byte[16];

            Array.Copy(data, 0, usernameBytes, 0, 18);
            Array.Copy(data, 32, passwordBytes, 0, 16);

            string username = Encoding.UTF8.GetString(usernameBytes).TrimEnd('\0');
            string password = Encoding.UTF8.GetString(passwordBytes).TrimEnd('\0');

            using (var dbContext = new UsersDbContext())
            {
                var user = dbContext.Users.SingleOrDefault(u => u.Name == username && u.Password == password);
                var builder = new PacketBuilder(Common.Packets.Opcodes.LOGIN_REQUEST);
                if (user is null) // User not found.
                {
                    builder.WriteByte((byte)UserStatus.WrongUsernameOrPassword);
                    session.Write(builder.ToPacket());
                    return true;
                }

                builder.WriteByte((byte)user.Status);

                if (user.Status == UserStatus.Active)
                {
                    builder.WriteInt(user.Id);
                    builder.WriteByte(user.AdminLevel);
                    HandleServerList(session);
                }

                session.Write(builder.ToPacket());
            }

            return true;
        }

        /// <summary>
        /// Handles a server list response, and sends the server list to the user
        /// </summary>
        /// <param name="session">The session instance</param>
        public void HandleServerList(ServerSession session)
        {
            var bldr = new PacketBuilder(Common.Packets.Opcodes.SERVER_LIST_DETAILS);

            lock (_syncObject)
            {
                var servers = LoginService.GetServers();

                byte serverCount = (byte)servers.Count;

                bldr.WriteByte(serverCount);

                // Loop through the servers
                for (int i = 0; i < serverCount; i++)
                {
                    var server = servers.ElementAt(i);

                    // TODO: Properly find the values that these fields should be
                    // or how they should be manipulated
                    bldr.WriteShort(server.serverId);
                    bldr.WriteShort(server.status);
                    bldr.WriteShort(server.population);
                    bldr.WriteBytes(Encoding.UTF8.GetBytes(server.serverName));
                }
            }

            session.Write(bldr.ToPacket());
        }
    }
}