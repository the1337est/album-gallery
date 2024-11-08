using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Gaurav.Scripts.Core
{
    public class DownloadController : MonoBehaviour
    {
        /// <summary>
        /// Downloads and returns string data from url
        /// </summary>
        /// <param name="url">source url</param>
        /// <returns>content from URL</returns>
        public async Task<string> DownloadContent(string url)
        {
            HttpClient client = new HttpClient();
            var response= await client.GetStringAsync(url);
            return response;
        }

        /// <summary>
        /// Tries to parse a json into a given type T
        /// </summary>
        /// <param name="json">json string</param>
        /// <typeparam name="T">generic type to parse in</typeparam>
        /// <returns>parsed T or default</returns>
        public T GetJsonAs<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DownloadController] Error deserialing json: {ex.Message}]");
                return default;
            }
        }

        /// <summary>
        /// Gets a texture from URL, and returns DownloadResult of Texture2D
        /// </summary>
        /// <param name="url">source url of the texture</param>
        /// <returns>DownloadResult of Texture2D</returns>
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

    /// <summary>
    /// Generic struct that represents result of a download and its data
    /// </summary>
    /// <typeparam name="T">Type for the data</typeparam>
    public struct DownloadResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}