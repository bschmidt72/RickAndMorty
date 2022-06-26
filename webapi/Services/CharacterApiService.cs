using System.Net.Http;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using webapi.Cache;
using webapi.Helper;

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
        private readonly ICache<JObject> _characterCache;
        private readonly ICache<JObject> _queryCache;
        private readonly bool _useCache;
        public CharacterApiService(ILogger<CharacterApiService> logger, IConfiguration config,
            IHttpContextAccessor httpContextAccessor) 
        {
            _logger = logger;
            _useCache = config.GetValue<bool?>("EnableCaching") ?? false;
            _baseUrl = config.GetValue<string>("RickAndMortyApi");
            if (_useCache)
            {
                _characterCache = new LfuCache<JObject>(200);
                _queryCache = new LruCache<JObject>(100);
            }
            _httpContextAccessor = httpContextAccessor;
        }

        public JObject GetAllCharacters(int? page)
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
            UpdateResponseLinks(response, relativePath, page);
            return response;
        }

        public JObject GetCharacter(int id)
        {
            if (_useCache)
            {
                var cached = _characterCache.Get(id);
                if (cached != null)
                {
                    return cached;
                }
            }
            var character = GetJsonResponseFromHttp($"{_baseUrl}/character/{id}");
            if (_useCache)
            {
                _characterCache.Add(id, character);
            }

            return character;
        }

        public JObject QueryCharacters(string query, int? page)
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
            var response = GetJsonResponseFromHttp(url);
            UpdateResponseLinks(response, relativePath, page);
            if (_useCache)
            {
                _queryCache.Add(url, response);
            }
            return response;
        }
        
        public JArray FetchCharactersRecursively(string url)
        {
            var allCharacters = new JArray();
            while (url != null)
            {
                var result = GetJsonResponseFromHttp(url);
                if (result == null)
                {
                    break;
                }

                var characters = GetCharactersFromResult(result);
                foreach (var character in characters.Children<JObject>())
                {
                    allCharacters.Add(character);
                }

                url = GetNextUrlFromResult(result);
            }

            return allCharacters;
        }
        
        private string GetNextUrlFromResult(JObject result)
        {
            return (string) result["info"]["next"];
        }

        private JArray GetCharactersFromResult(JObject result)
        {
            return (JArray) result["results"];
        }
        
        private JObject GetJsonResponseFromHttp(string url)
        {
            using var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url);
            var respMessage = result.Result;
            var jsonResponse = respMessage.Content.ReadAsStringAsync().Result;

            if (respMessage.IsSuccessStatusCode)
            {
                return JObject.Parse(jsonResponse);
            }
            _logger.LogError($"[{respMessage.StatusCode}] [{jsonResponse}]");
            return null;
        }
        
        /// <summary>
        /// modifies the character response to fit the new api
        /// </summary>
        private void UpdateResponseLinks(JObject response, string relativePath, int? currentPage)
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
            var next = (string) response["info"]?["next"];
            if (next != null)
            {
                response["info"]["next"] = $"{baseUrl}{queryParamSeparator}page={currentPage + 1}";
            }

            var prev = (string) response["info"]?["prev"];
            if (prev != null)
            {
                response["info"]["prev"] = $"{baseUrl}{queryParamSeparator}page={currentPage - 1}";
            }
        }
        
    }
}