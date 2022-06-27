using webapi.Model;

namespace webapi.Services
{
    public interface ICharacterService
    {
        CharacterResponse GetAllCharacters(int? page);

        CharacterResponse QueryCharacters(string query, int? page);

        Character GetCharacter(int id);
    }
}