using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

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
