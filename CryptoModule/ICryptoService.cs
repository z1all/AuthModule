namespace CryptoModule
{
    public interface ICryptoService
    {
        byte[] Decrypt(string privateKey, byte[] data);
        byte[] Encrypt(string publicKey, byte[] data);
        Keys MakeKeysPair();
    }
}
