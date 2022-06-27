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
        public CharacterResponse Get(string name, int? page)
        {
            return name == null 
                ? _characterService.GetAllCharacters(page) 
                : _characterService.QueryCharacters(Sanitizer.CleanString(name), page);
        }

        /// <summary>
        /// Retrieves all characters
        /// </summary>
        /// <param name="id">Character Id</param> 
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public Character GetById(int id)
        {
            return _characterService.GetCharacter(id);
        }

    }
}