using System;
using System.Threading.Tasks;
using Gaurav.Scripts.Core;
using UnityEngine;

namespace Gaurav.Scripts.Album
{
    public class AlbumEntry : MonoBehaviour
    {

        public AlbumEntryData Data;
        public static event Action<AlbumEntry> OnClicked;

        [Header("Component References")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Light _light;
        
        #region Private Members
        private Texture2D _texture;
        private bool _animating = false;
        private float _targetScale = 1f;
        private State _state = State.None;
        
        private const float _selectedScale = 1.5f;
        private const float _scaleSpeed = 10f;
        private const float _defaultLightIntensity = 10f;
        private const float _selectedLightIntensity = 30f;

        private static Color _selectedColor = new Color32(255, 255, 255, 255);
        private static Color _defaultColor = new Color32(255, 128, 0, 255);
        #endregion

        /// <summary>
        /// Initializes this Album Entry with an AlbumEntryData
        /// </summary>
        /// <param name="data"></param>
        public async void Initialize(AlbumEntryData data)
        {
            Data = data;
            await LoadImage();
        }

        private void Update()
        {
            if (!_animating) return;
            
            if (Mathf.Abs(transform.localScale.x - _targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _targetScale, Time.deltaTime * _scaleSpeed);
            }
            else
            {
                transform.localScale = Vector3.one * _targetScale;
                _animating = false;
            }
        }

        /// <summary>
        /// Loads image from Album Entry data and assigns it to the material's main texture
        /// Also handles loading and error state textures
        /// </summary>
        private async Task LoadImage()
        {
            if(_state == State.Loaded || _state == State.Loading) return;
        
            _state = State.Loading;
            _renderer.material.mainTexture = AppController.Instance.LoadingTexture;
        
            var result = await DownloadController.GetTexture(Data.Url);
            _texture = result.Data;
            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.mainTexture = result.Success ? _texture : AppController.Instance.ErrorTexture;
            }
            _state = result.Success ? State.Loaded : State.Failed;
        }

        /// <summary>
        /// Selects this Album Entry and updates visuals to match selected state
        /// </summary>
        public void Select()
        {
            _light.color = _selectedColor;
            _light.intensity = _selectedLightIntensity;
            LoadImage();
            _targetScale = _selectedScale;
            _animating = true;
        }

        /// <summary>
        /// Deselects this Album Entry and updates visuals to match deselected state
        /// </summary>
        public void Deselect()
        {
            _light.color = _defaultColor;
            _light.intensity = _defaultLightIntensity;
            _targetScale = 1f;
            _animating = true;
        }
        
        
        public void OnMouseDown()
        {
            OnClicked?.Invoke(this);
        }
        
        /// <summary>
        /// Override ToString method to provide easier and meaningful logging to console
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[AlbumEntry] Id: {Data.Id} | Title: {Data.Title} | Url: {Data.Url}";
        }
        
        private void OnDestroy()
        {
            Destroy(_texture);
            _texture = null;
        }

        /// <summary>
        /// State to represent the texture associated with this Album Entry
        /// </summary>
        private enum State
        {
            None,
            Loading,
            Loaded,
            Failed,
        }

    }
}
