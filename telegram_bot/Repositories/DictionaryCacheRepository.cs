using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_bot.Models;

namespace telegram_bot.Repositories
{
    internal class DictionaryCacheRepository : ICacheRepository
    {
        private readonly Dictionary<long, UserResponse> _cache = new Dictionary<long, UserResponse>();

        public void AddToRepository(UserResponse userResponse)
        {
            _cache.Add(userResponse.Id, userResponse);
        }

        public UserResponse GetFromRepository(long id)
        {
            return _cache[id];
        }

        public bool HasInRepository(long id)
        {
            return _cache.ContainsKey(id);
        }

        public void RemoveFromRepository(UserResponse userResponse)
        {
            _cache.Remove(userResponse.Id);
        }

        public void UpdateInRepository(UserResponse userResponse)
        {
            _cache[userResponse.Id] = userResponse;
        }
    }
}
