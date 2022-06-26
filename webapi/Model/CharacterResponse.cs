using System.Collections.Generic;

namespace webapi.Model
{
    public class CharacterResponse
    {
        public ResponseInfo Info { get; set; }

        public List<Character> Results { get; set; }
    }
}