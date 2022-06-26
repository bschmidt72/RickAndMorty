using Newtonsoft.Json.Linq;

namespace webapi.Services
{
    public interface ICharacterService
    {
        JObject GetAllCharacters(int? page);

        JObject QueryCharacters(string query, int? page);

        JObject GetCharacter(int id);
    }
}