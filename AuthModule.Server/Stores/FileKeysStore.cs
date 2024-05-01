using System.Text.Json;

namespace AuthModule.Server.Stores
{
    internal class FileKeysStore : IKeysStore
    {
        public bool FindKey(string key)
        {
            using FileStream fs = File.OpenRead("Assets/.keys.json");
            using JsonDocument jsonDocument = JsonDocument.Parse(fs);

            JsonElement root = jsonDocument.RootElement;
            JsonElement keysElement = root.GetProperty("keys");

            foreach (JsonElement keyElement in keysElement.EnumerateArray())
            {
                string? keyFromJson = keyElement.GetString();
                if (keyFromJson == key)
                {
                    return true;
                }
            }

            return false;
        }

        public void SaveKey(string key)
        {
            string jsonContent = File.ReadAllText("Assets/.keys.json");

            var profiles = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent);
            profiles!.GetValueOrDefault("keys")!.Add(key);

            string updatedJson = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("Assets/.keys.json", updatedJson);
        }
    }
}
