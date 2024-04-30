namespace AuthModule.Server.Store
{
    internal interface IKeysStore
    {
        bool FindKey(string key);
        void SaveKey(string key);
    }
}
