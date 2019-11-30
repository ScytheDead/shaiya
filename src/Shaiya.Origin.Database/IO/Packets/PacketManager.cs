using Shaiya.Origin.Common.Database;
using Shaiya.Origin.Database.IO.Packets.Impl;
using System.Collections.Generic;

namespace Shaiya.Origin.Database.IO.Packets
{
    public class PacketManager
    {
        private Dictionary<int, PacketHandler> _handlers = new Dictionary<int, PacketHandler>();
        private PacketHandler _handler;

        public PacketManager()
        {
            // The default packet handler
            _handlers[0] = new DefaultPacketHandler();

            // Define the database server packet handlers
            _handlers[Opcodes.DELETE_SESSION] = new DeleteSessionRequestHandler();
            _handlers[Opcodes.SERVER_LIST] = new ServerListRequestHandler();
            _handlers[Opcodes.USER_GAME_CONNECT] = new UserGameConnectRequestHandler();
            _handlers[Opcodes.GAME_USER_LOAD] = new GameUserLoadRequestHandler();
            _handlers[Opcodes.GET_CHARACTER_LIST_DATA] = new CharacterScreenDataRequestHandler();
            _handlers[Opcodes.CHECK_AVAILABLE_NAME] = new CheckAvailableNameRequestHandler();
            _handlers[Opcodes.SELECT_FACTION] = new SelectFactionRequestHandler();
            _handlers[Opcodes.CREATE_CHARACTER] = new CreateCharacterRequestHandler();
            _handlers[Opcodes.DELETE_CHARACTER] = new DeleteCharacterRequestHandler();
            _handlers[Opcodes.LOAD_GAME_CHARACTER] = new LoadGameCharacterDataRequestHandler();

        }

        /// <summary>
        /// Gets the handler assigned to a specific opcode
        /// </summary>
        /// <param name="opcode">The opcode</param>
        /// <returns>The handler if found. If not, the <see cref="DefaultPacketHandler"/> instance is returned</returns>
        public PacketHandler GetHandler(int opcode)
        {
            if (_handlers.TryGetValue(opcode, out _handler))
            {
                return _handler;
            }

            _handler = new DefaultPacketHandler();

            return _handler;
        }
    }
}