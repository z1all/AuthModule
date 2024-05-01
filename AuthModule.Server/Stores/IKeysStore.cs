namespace AuthModule.Server.Stores
{
    internal interface IKeysStore
    {
        bool FindKey(string key);
        void SaveKey(string key);
    }
}
