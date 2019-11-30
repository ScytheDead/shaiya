namespace Shaiya.Origin.Login.Model
{
    public enum UserStatus : short
    {
        Active,
        Banned, // This will show a message, that the account doesn't exist. Maybe can be used for banned accounts.
        NotFree, // No idea how this can be used... It will show "account is not selected as free challenger".
        WrongUsernameOrPassword
    }
}
