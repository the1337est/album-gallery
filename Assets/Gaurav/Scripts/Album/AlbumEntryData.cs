using Newtonsoft.Json;

/// <summary>
/// Data struct for an Album Entry. JsonProperty attribute is from Newtonsoft Json
/// </summary>
[System.Serializable]
public struct AlbumEntryData
{
    [JsonProperty("albumId")]
    public string AlbumId;
    
    [JsonProperty("id")]
    public string Id;
    
    [JsonProperty("title")]
    public string Title;
    
    [JsonProperty("url")]
    public string Url;
    
    [JsonProperty("thumbnailUrl")]
    public string ThumbnailUrl;
}
