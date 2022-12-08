using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_bot.Models;

namespace telegram_bot.Repositories
{
    internal interface ICacheRepository
    {
        public void AddToRepository(UserResponse userResponse);
        public void RemoveFromRepository(UserResponse userResponse);
        public UserResponse GetFromRepository(long id);
        public void UpdateInRepository(UserResponse userResponse);

        public bool HasInRepository(long id);
    }
}
