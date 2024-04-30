namespace AuthModule.Server.Stores
{
    internal interface IProfileStore
    {
        bool CheckUserNameAndPassword(string userName, string password);
    }
}
