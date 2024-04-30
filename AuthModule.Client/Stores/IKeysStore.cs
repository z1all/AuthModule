using CryptoModule;

namespace AuthModule.Client.Stores
{
    internal interface IKeysStore
    {
        void SaveKeys(Keys keys);
        Keys GetKeys();
    }
}
