using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Tarea04.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tarea04.ViewModels
{
    class YoutubePageViewModel : INotifyPropertyChanged
    {
        private const string ApiKey = "AIzaSyDwQU4KaJ4jYc-tlnSG2x5kQANqPupbJ40";
        private string ApiUrlChannel = "https://www.googleapis.com/youtube/v3/search?part=id&maxResults=20&channelId="
            + "UCBYESMnGmdfFISXEhfF5PlA"
            + "&key="
            + ApiKey;
        private string ApiUrlVideoDetails = "https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&id="
            + "{0}"
            + "&key="
            + ApiKey;


        private List<YoutubeItem> youtubeItems;
        public List<YoutubeItem> YoutubeItems
        {
            get { return youtubeItems; }
            set
            {
                youtubeItems = value;
                OnPropertyChanged();
            }
        }
        public YoutubePageViewModel()
        {
            InitDataAsync();
        }
        

        public async Task InitDataAsync()
        {
            var videosIds = await GetVideoIdsFromChannelAsync();
        }
        private async Task<List<string>> GetVideoIdsFromChannelAsync()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(ApiUrlChannel);
            var videoIds = new List<string>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);
                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    videoIds.Add(item.Value<JObject>("id")?.Value<string>("videoId"));
                }
                YoutubeItems = await GetVideoDetailsAsync(videoIds); 
            }
            catch(Exception exception)
            {

            }
            return videoIds;
        }
        private async Task<List<YoutubeItem>> GetVideoDetailsAsync(List<string> videoIds)
        {
            var videoId = " ";
            foreach (var s in videoIds)
            {
                videoId += s + ",";
            }
            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(string.Format(ApiUrlVideoDetails, videoId));

            var youtubeItems = new List<YoutubeItem>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");
                foreach (var item in items)
                {
                    var snippet = items.Value<JObject>("snippet");
                    var statistics = items.Value<JObject>("statistics");
                    var youtubeItem = new YoutubeItem
                    {
                        Title = snippet.Value<string>("title"),
                        Description = snippet.Value<string>("description"),
                        ChannelTitle = snippet.Value<string>("channelTitle"),
                        PublishedAt = snippet.Value<DateTime>("publishedAt"),
                        VideoId = item?.Value<string>("id"),
                        DefaultThumbnailUrl = snippet?.Value<JObject>
                        ("thumbnails")?.Value<JObject>("default")?.Value<string>("url"),
                        MediumThumbnailUrl = snippet?.Value<JObject>
                        ("thumbnails")?.Value<JObject>("medium")?.Value<string>("url"),
                        HighThumbnailUrl = snippet?.Value<JObject>
                        ("thumbnails")?.Value<JObject>("high")?.Value<string>("url"),
                        StandardThumbnailUrl = snippet?.Value<JObject>
                        ("thumbnails")?.Value<JObject>("standard")?.Value<string>("url"),
                        MaxResThumbnailUrl = snippet?.Value<JObject>
                        ("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url"),

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
            catch(Exception ex)
            {
                return youtubeItems;    
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
     
}
