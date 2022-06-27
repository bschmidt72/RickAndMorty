using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using webapi.Helper;
using webapi.Model;
using webapi.Services;

namespace webapi.Controllers
{
    /// <summary>
    /// Controller for Note CRUD Operations
    /// </summary>
    [ApiController]
    [Route("api/character")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        
        /// <summary>
        /// Retrieves characters
        /// </summary>
        /// <param name="name">Name Filter</param>
        /// <param name="page">Requested Page</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<CharacterResponse> Get(string name, int? page)
        {
            return name == null 
                ? Ok(_characterService.GetAllCharacters(page)) 
                : Ok(_characterService.QueryCharacters(Sanitizer.CleanString(name), page));
        }

        /// <summary>
        /// Retrieves all characters
        /// </summary>
        /// <param name="id">Character Id</param> 
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public ActionResult<Character> GetById(int id)
        {
            var character = _characterService.GetCharacter(id);
            if (character == null)
            {
                return NotFound("Character not found");
            }

            return Ok(character);
        }

    }
}