namespace Shaiya.Origin.Common.Database
{
    /// <summary>
    /// Contains all the opcodes, that are used for communication between the login
    /// and game servers, and the database server.
    /// </summary>
    public class Opcodes
    {
        /// <summary>
        /// Represents a packet sent from the login server to the database server,
        /// requesting the server list.
        /// </summary>
        public const int SERVER_LIST = 2;

        /// <summary>
        /// Represents a packet sent from the login server, informing the
        /// database server that a session should be deleted
        /// </summary>
        public const int DELETE_SESSION = 3;

        /// <summary>
        /// Represents a packet sent from the game server, informing the database
        /// server that a user has connected to the game server, and that it should
        /// verify the session.
        /// </summary>
        public const int USER_GAME_CONNECT = 4;

        /// <summary>
        /// Represents a packet sent from the game server, requesting the details
        /// of a game account to be loaded from the database, and sent to the game server.
        /// </summary>
        public const int GAME_USER_LOAD = 5;

        /// <summary>
        /// Handles the selection of a faction for the player
        /// </summary>
        public const int SELECT_FACTION = 6;

        /// <summary>
        /// Gets the data displayed by the character list for a player.
        /// </summary>
        public const int GET_CHARACTER_LIST_DATA = 7;

        /// <summary>
        /// Checks if a character name is available.
        /// </summary>
        public const int CHECK_AVAILABLE_NAME = 8;

        /// <summary>
        /// Handles the creation of a character.
        /// </summary>
        public const int CREATE_CHARACTER = 9;

        /// <summary>
        /// Handles the request to delete a character.
        /// </summary>
        public const int DELETE_CHARACTER = 10;

        /// <summary>
        /// Handles the loading of a game character's data
        /// </summary>
        public const int LOAD_GAME_CHARACTER = 12;
    }
}