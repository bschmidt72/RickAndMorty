using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using webapi.Cache;
using webapi.Helper;
using webapi.Model;

namespace webapi.Services
{
    /// <summary>
    /// Service mimicking the RickAndMorty Character Api
    /// Implemented as a proxy to the original api service
    /// </summary>
    public class CharacterApiService : ICharacterService
    {
        private readonly ILogger<CharacterApiService> _logger;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICache<Character> _characterCache;
        private readonly ICache<CharacterResponse> _queryCache;
        private readonly bool _useCache;
        public CharacterApiService(ILogger<CharacterApiService> logger, IConfiguration config,
            IHttpContextAccessor httpContextAccessor) 
        {
            _logger = logger;
            _useCache = config.GetValue<bool?>("EnableCaching") ?? false;
            _baseUrl = config.GetValue<string>("RickAndMortyApi");
            if (_useCache)
            {
                _characterCache = new LfuCache<Character>(200);
                _queryCache = new LruCache<CharacterResponse>(100);
            }
            _httpContextAccessor = httpContextAccessor;
        }

        public CharacterResponse GetAllCharacters(int? page)
        {
            var builder = new StringBuilder();
            const string relativePath = "/character/";
            var characterBaseUrl = $"{_baseUrl}{relativePath}";
            builder.Append(characterBaseUrl);
            if (page.HasValue)
            {
                builder.Append($"?page={page}");
            }

            var response = GetJsonResponseFromHttp(builder.ToString());
            var characterResponse = JsonConvert.DeserializeObject<CharacterResponse>(response);
            UpdateResponseLinks(characterResponse, relativePath, page);
            return characterResponse;
        }

        public Character GetCharacter(int id)
        {
            if (_useCache)
            {
                var cached = _characterCache.Get(id);
                if (cached != null)
                {
                    return cached;
                }
            }
            var characterJson = GetJsonResponseFromHttp($"{_baseUrl}/character/{id}");
            var character = JsonConvert.DeserializeObject<Character>(characterJson);
            if (_useCache)
            {
                _characterCache.Add(id, character);
            }

            return character;
        }

        public CharacterResponse QueryCharacters(string query, int? page)
        {
            var sanitized = HttpUtility.UrlEncode(Sanitizer.CleanString(query));
            var builder = new StringBuilder();
            var relativePath = $"/character/?name={sanitized}";
            var characterBaseUrl = $"{_baseUrl}{relativePath}";
            builder.Append(characterBaseUrl);
            if (page.HasValue)
            {
                builder.Append($"&page={page}");
            }

            var url = builder.ToString();
            if (_useCache)
            {
                var cached = _queryCache.Get(url);
                if (cached != null)
                {
                    return cached;
                }
            }
            var responseJson = GetJsonResponseFromHttp(url);
            var response = JsonConvert.DeserializeObject<CharacterResponse>(responseJson);
            UpdateResponseLinks(response, relativePath, page);
            if (_useCache)
            {
                _queryCache.Add(url, response);
            }
            return response;
        }
        
        public List<Character> FetchCharactersRecursively()
        {
            var allCharacters = new List<Character>();
            var url = $"{_baseUrl}/character/";
            while (url != null)
            {
                var resultJson = GetJsonResponseFromHttp(url);
                if (resultJson == null)
                {
                    break;
                }

                var result = JsonConvert.DeserializeObject<CharacterResponse>(resultJson);
                if (result != null)
                {
                    allCharacters.AddRange(result.Results);
                }
                url = result?.Info?.Next;
            }

            return allCharacters;
        }

        private string GetJsonResponseFromHttp(string url)
        {
            using var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url);
            var respMessage = result.Result;
            var jsonResponse = respMessage.Content.ReadAsStringAsync().Result;

            if (respMessage.IsSuccessStatusCode)
            {
                return jsonResponse;
            }
            _logger.LogError($"[{respMessage.StatusCode}] [{jsonResponse}]");
            return null;
        }
        
        /// <summary>
        /// modifies the character response to fit the new api
        /// </summary>
        private void UpdateResponseLinks(CharacterResponse characterResponse, string relativePath, int? currentPage)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }

            var queryParamSeparator = relativePath.Contains('?') ? "&" : "?";
            currentPage ??= 1;
            var request = _httpContextAccessor.HttpContext.Request;
            var pathBase = request.PathBase.HasValue ? $"{request.PathBase.Value}/" : "";
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var baseUrl = $"{scheme}://{host}/{pathBase}api{relativePath}";
            // for the sake of simplicity we're only updating the info part and leave all the other hosting on rickandmortyapi.com
            var next = characterResponse.Info.Next;
            if (next != null)
            {
                characterResponse.Info.Next = $"{baseUrl}{queryParamSeparator}page={currentPage + 1}";
            }

            var prev = characterResponse.Info.Prev;
            if (prev != null)
            {
                characterResponse.Info.Prev = $"{baseUrl}{queryParamSeparator}page={currentPage - 1}";
            }
        }
        
    }
}