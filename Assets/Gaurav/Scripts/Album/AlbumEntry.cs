using System;
using System.Threading.Tasks;
using Gaurav.Scripts.Core;
using UnityEngine;

namespace Gaurav.Scripts.Album
{
    public class AlbumEntry : MonoBehaviour
    {

        public AlbumEntryData Data;

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Light _light;
    
        private static Color _selectedColor = new Color32(255, 255, 255, 255);
        private static Color _defaultColor = new Color32(255, 128, 0, 255);
    
        private const float _defaultLightIntensity = 10f;
        private const float _selectedLightIntensity = 30f;
    
        public static event Action<AlbumEntry> OnClicked;
    
        private State _state = State.None;

        private Texture2D _texture;

        public async void Initialize(AlbumEntryData data)
        {
            Data = data;
            await LoadImage();
        }

        private async Task LoadImage()
        {
            if(_state == State.Loaded || _state == State.Loading) return;
        
            _state = State.Loading;
            _renderer.material.mainTexture = AppController.Instance.LoadingTexture;
        
            var result = await DownloadController.GetTexture(Data.ThumbnailUrl);
            _texture = result.Data;
            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.mainTexture = result.Success ? _texture : AppController.Instance.ErrorTexture;
            }
            _state = result.Success ? State.Loaded : State.Failed;
        }

        public void Select()
        {
            _light.color = _selectedColor;
            _light.intensity = _selectedLightIntensity;
            LoadImage();
        }

        public void Deselect()
        {
            _light.color = _defaultColor;
            _light.intensity = _defaultLightIntensity;
        }

        public void OnMouseDown()
        {
            OnClicked?.Invoke(this);
        }

        private void OnDestroy()
        {
            Destroy(_texture);
            _texture = null;
        }

        public override string ToString()
        {
            return $"[AlbumEntry] Id: {Data.Id} | Title: {Data.Title} | Url: {Data.Url}";
        }

        public enum State
        {
            None,
            Loading,
            Loaded,
            Failed,
        }

    }
}
