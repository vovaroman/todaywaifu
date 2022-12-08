using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_bot.Models;

namespace telegram_bot.Services
{
    internal interface IWaifuService
    {
        [Get("/sfw/waifu")]
        public Task<WaifuResponse> GetRandomWaifu();

        [Get("/nsfw/waifu")]
        public Task<WaifuResponse> GetRandomNSFWWaifu();

        [Get("/nsfw/trap")]
        public Task<WaifuResponse> GetRandomNSFWTrap();
    }
}
