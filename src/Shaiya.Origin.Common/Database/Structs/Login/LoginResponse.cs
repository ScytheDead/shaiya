using ProtoBuf;

namespace Shaiya.Origin.Common.Database.Structs.Login
{
    /// <summary>
    /// Represents the response to an <see cref="LoginRequest"/>
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class LoginResponse
    {
        /// <summary>
        /// The user id.
        /// </summary>
        [ProtoMember(1)]
        public int userId;

        /// <summary>
        /// The status of the user (result of login request, ie valid, banned, invalid password)
        /// </summary>
        [ProtoMember(2)]
        public int status;

        /// <summary>
        /// Admin or normal user.
        /// </summary>
        [ProtoMember(3)]
        public int privilegeLevel;

        /// <summary>
        /// TODO: ?
        /// </summary>
        [ProtoMember(4)]
        public byte[] identityKeys = new byte[16];
    }
}