using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_bot.Models;

namespace telegram_bot.Services
{
    internal class WaifuService : IWaifuService
    {
        private readonly IWaifuService api;

        public WaifuService(IWaifuService api)
        {
            this.api = api;
        }

        public Task<WaifuResponse> GetRandomNSFWTrap() => api.GetRandomNSFWTrap();

        public Task<WaifuResponse> GetRandomNSFWWaifu() => api.GetRandomNSFWWaifu();

        public Task<WaifuResponse> GetRandomWaifu() => api.GetRandomWaifu();
    }
}
