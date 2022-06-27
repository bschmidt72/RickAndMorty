using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using webapi.Cache;
using webapi.Helper;
using webapi.Model;

namespace webapi.Services
{
    public class CharacterDbService : ICharacterService, IDisposable
    {
        private const int PageSize = 20;
        private readonly CharacterApiService _characterApiService;

        private LiteDatabase _database;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly bool _useCache;
        private readonly ICache<Character> _characterCache;
        private readonly ICache<CharacterResponse> _queryCache;
        
        public CharacterDbService(CharacterApiService characterApiService, IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor)
        {
            _characterApiService = characterApiService;
            _httpContextAccessor = httpContextAccessor;
            _useCache = configuration.GetValue<bool?>("EnableCaching") ?? false;
            if (_useCache)
            {
                _characterCache = new LfuCache<Character>(200);
                _queryCache = new LruCache<CharacterResponse>(100);
            }
            _database = new LiteDatabase(@".\RickAndMorty.db");
            Init();
        }

        private void Init()
        {
            // Get a collection (or create, if doesn't exist)
            var col = GetCollection();
            var count = col.Query().Count();
            if (count == 0)
            {
                var allCharacters = _characterApiService.FetchCharactersRecursively();
                col.InsertBulk(allCharacters);
            }
            col.EnsureIndex(x => x.Name);
        }

        private ILiteCollection<Character> GetCollection()
        {
            return _database.GetCollection<Character>("characters");
        }

        private string GetPaginationUrl(string relativePath, int? page, int totalPages)
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            if (request == null || page < 2 || page > totalPages)
            {
                return null;
            }
            page ??= 1;
            var pathBase = request.PathBase.HasValue ? $"{request.PathBase.Value}/" : "";
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var queryParamSeparator = relativePath.Contains('?') ? "&" : "?";
            return $"{scheme}://{host}/{pathBase}api{relativePath}{queryParamSeparator}page={page}";
        }
        
        private CharacterResponse CreateResponseFromResults(List<Character> results, int total, string relativePath, int page)
        {
            var totalPages = total / PageSize + 1;
            var info = new ResponseInfo()
            {
                Count = total,
                Pages = totalPages,
                Prev = GetPaginationUrl(relativePath, page - 1, totalPages),
                Next = GetPaginationUrl(relativePath, page + 1, totalPages)
            };
            return new CharacterResponse()
            {
                Info = info,
                Results = results
            };
        }

        public CharacterResponse GetAllCharacters(int? page)
        {
            page ??= 1;
            var count = GetCollection()
                .Query()
                .Count();
            return CreateResponseFromResults(
                GetCollection()
                    .Query()
                    .OrderBy(c => c.Id)
                    .ToList() // apparently liteDb cannot sort and paginate at the same time, so we have to do it in Linq
                    .Skip((page.Value - 1) * PageSize)
                    .Take(PageSize)
                    .ToList(),
                count,
                "/character/", page.Value);
        }
        
        public CharacterResponse QueryCharacters(string query, int? page)
        {
            var sanitizedQuery = Sanitizer.CleanString(query);
            page ??= 1;
            var cacheKey = $"{sanitizedQuery}/{page}";
            if (_useCache)
            {
                var cached = _queryCache.Get(cacheKey);
                if (cached != null)
                {
                    return cached;
                }
            }
            var count = GetCollection()
                .Query()
                .Where(c => c.Name.Contains(sanitizedQuery))
                .Count();
            var result = CreateResponseFromResults(
                GetCollection()
                    .Query()
                    .Where(c => c.Name.Contains(sanitizedQuery))
                    .Offset(page.Value - 1)
                    .ToList() // apparently liteDb cannot sort and paginate at the same time, so we have to do it in Linq
                    .OrderBy(c => c.Id)
                    .Skip((page.Value - 1) * PageSize)
                    .Take(PageSize)
                    .ToList(),
                count,
                $"/character/?name={sanitizedQuery}", page.Value);
            if (_useCache)
            {
                _queryCache.Add(cacheKey, result);
            }

            return result;
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
            var character = GetCollection().FindOne(c => c.Id == id);
            if (_useCache)
            {
                _characterCache.Add(id, character);
            }

            return character;
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}