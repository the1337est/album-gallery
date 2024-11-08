using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Gaurav.Scripts.Core
{
    public class DownloadController : MonoBehaviour
    {
        public async Task<string> DownloadContent(string url)
        {
            HttpClient client = new HttpClient();
            var response= await client.GetStringAsync(url);
            return response;
        }

        public T GetJsonAs<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<DownloadResult<Texture2D>> GetTexture(string url)
        {
            var downloadResult = new DownloadResult<Texture2D>();
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var response = await client.GetByteArrayAsync(url);
                var texture = new Texture2D(1, 1);
                texture.LoadImage(response);
                downloadResult.Data = texture;
                downloadResult.Success = true;
            }
            catch (Exception e)
            {
                Debug.Log($"[DownloadController] Unable to load image: {e}");
                downloadResult.Success = false;
            }

            return downloadResult;
        }
    }

    public struct DownloadResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}