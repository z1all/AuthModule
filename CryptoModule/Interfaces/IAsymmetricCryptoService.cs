namespace CryptoModule.Interfaces
{
    public interface IAsymmetricCryptoService
    {
        byte[] Decrypt(string privateKey, byte[] data);
        byte[] Encrypt(string publicKey, byte[] data);
        Keys MakeKeysPair();
    }
}
