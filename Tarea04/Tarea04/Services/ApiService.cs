using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tarea04.Models;
using Tarea04.ViewModels;

namespace Tarea04.Services
{
    class ApiService : IApiService
    {
        public YoutubePageViewModel test = new YoutubePageViewModel();
        public ApiService()
        {

        }
      
        public List<YoutubeItem> youtube = new List<YoutubeItem>();
        public async Task InitDataAsync()
        {
            var videosIds = await GetVideoIdsFromChannelAsync();
        }
       public async Task<List<string>> GetVideoIdsFromChannelAsync()
        {
            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(test.ApiUrlChannel);

            var videoIds = new List<string>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    videoIds.Add(item.Value<JObject>("id")?.Value<string>("videoId"));
                }

                youtube = await GetVideoDetailsAsync(videoIds);
            }
            catch (Exception exception) { }

            return videoIds;
        }
        public  async Task<List<YoutubeItem>> GetVideoDetailsAsync(List<string> videoIds)
        {
            var videoIdsString = "";
            foreach (var s in videoIds)
            {
                videoIdsString += s + ",";
            }

            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(string.Format(test.ApiUrlVideoDetails, videoIdsString));

            var youtubeItems = new List<YoutubeItem>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    var snippet = item.Value<JObject>("snippet");
                    var statistics = item.Value<JObject>("statistics");

                    var youtubeItem = new YoutubeItem
                    {
                        Title = snippet.Value<string>("title"),
                        Description = snippet.Value<string>("description"),
                        ChannelTitle = snippet.Value<string>("channelTitle"),
                        PublishedAt = snippet.Value<DateTime>("publishedAt"),
                        VideoId = item?.Value<string>("id"),
                        DefaultThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url"),
                        MediumThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("medium")?.Value<string>("url"),
                        HighThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("high")?.Value<string>("url"),
                        StandardThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("standard")?.Value<string>("url"),
                        MaxResThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url"),

                        ViewCount = statistics.Value<int>("viewCount"),
                        LikeCount = statistics.Value<int>("likeCount"),
                        DislikeCount = statistics.Value<int>("dislikeCount"),
                        FavoriteCount = statistics.Value<int>("favoriteCount"),
                        CommentCount = statistics.Value<int>("commentCount"),
                        Tags = (from tag in snippet?.Value<JArray>("tags") select tag.ToString())?.ToList(),
                    };

                    youtubeItems.Add(youtubeItem);
                }

                return youtubeItems;
            }
            catch (Exception exception)
            {
                return youtubeItems;
            }
        }

    }
}
