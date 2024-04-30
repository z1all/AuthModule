using System.Text.Json;

namespace AuthModule.Server.Stores
{
    internal class FileProfileStore : IProfileStore
    {
        public bool CheckUserNameAndPassword(string userName, string password)
        {
            using FileStream fs = File.OpenRead("Assets/.profiles.json");
            using JsonDocument jsonDocument = JsonDocument.Parse(fs);

            JsonElement root = jsonDocument.RootElement;
            JsonElement profilesElement = root.GetProperty("profiles");

            foreach (JsonElement profile in profilesElement.EnumerateArray())
            {
                string? profileUserName = profile.GetProperty("userName").GetString();
                string? profilePassword = profile.GetProperty("password").GetString();

                if (profileUserName == userName && profilePassword == password)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
