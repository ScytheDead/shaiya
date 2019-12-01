namespace Shaiya.Origin.Login.Model
{
    public enum SelectServer : sbyte
    {
        ServerSaturated = -4,
        VersionDoesntMatch = -3,
        CannotConnect = -2,
        TryAgainLater = -1,
        Success = 0
    }
}
