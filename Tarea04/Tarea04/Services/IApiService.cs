using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tarea04.Models;

namespace Tarea04.Services
{
    public interface IApiService
    {
        Task InitDataAsync();
        
        Task<List<string>> GetVideoIdsFromChannelAsync();
        Task<List<YoutubeItem>> GetVideoDetailsAsync(List<string> videoIds);

    }
}
