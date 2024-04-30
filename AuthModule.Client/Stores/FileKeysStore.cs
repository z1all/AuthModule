using System.Text.Json;
using CryptoModule;

namespace AuthModule.Client.Stores
{
    internal class FileKeysStore : IKeysStore
    {
        public void SaveKeys(Keys keys)
        {
            string jsonContent = File.ReadAllText("Assets/.keys.json");

            var profiles = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

            profiles!.GetValueOrDefault("keys")!.Clear();
            profiles!.GetValueOrDefault("keys")!.Add("PrivateKey", keys.PrivateKey);
            profiles!.GetValueOrDefault("keys")!.Add("PublicKey", keys.PublicKey);

            string updatedJson = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("Assets/.keys.json", updatedJson);
        }

        public Keys GetKeys()
        {
            using FileStream fs = File.OpenRead("Assets/.keys.json");
            using JsonDocument jsonDocument = JsonDocument.Parse(fs);

            JsonElement root = jsonDocument.RootElement;
            JsonElement keysElement = root.GetProperty("keys");

            return keysElement.Deserialize<Keys>()!;

            //JsonElement root = jsonDocument.RootElement;
            //JsonElement keysElement = root.GetProperty("keys");

            //string privateKey = keysElement.GetProperty("private_key").GetString()!;
            //string publicKey = keysElement.GetProperty("public_key").GetString()!;

            //return new()
            //{
            //    PrivateKey = privateKey,
            //    PublicKey = publicKey,
            //};
        }
    }
}