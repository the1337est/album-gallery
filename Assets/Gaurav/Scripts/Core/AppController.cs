using System.Collections.Generic;
using Gaurav.Scripts.Album;
using UnityEngine;
using UnityEngine.UI;

namespace Gaurav.Scripts.Core
{
    public class AppController : MonoBehaviour
    {

        [Header("Controllers")]
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private DownloadController _downloadController;
        
        [Header("Buttons")]
        [SerializeField] private Button _logButton;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _removeButton;
    
        [SerializeField] private Button _scrollLeftButton;
        [SerializeField] private Button _scrollRightButton;

        [Header("Misc")]
        [SerializeField] private Transform _albumContainer;
        [SerializeField] private AlbumEntry _albumEntryPrefab;
        
        [Header("Texture references")]
        public Texture2D LoadingTexture;
        public Texture2D ErrorTexture;
        public static Texture2D TestImage;
        
        private List<AlbumEntryData> _albumCatalog;
        private Dictionary<string, AlbumEntry> _albumEntries = new Dictionary<string, AlbumEntry>();
        
        private const string _apiUrl = "https://jsonplaceholder.typicode.com/photos";
    
        private int _currentIndex = 0;
        private int _focusIndex = 0;

        private AlbumEntry _currentAlbumEntry = null;
    
        public static AppController Instance;

        private void Awake()
        {
            //singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }

            _albumCatalog = new List<AlbumEntryData>();
            _cameraController.Initialize();
        }

        private void Start()
        {
            DownloadData();
            AddListeners();
            UpdateRemoveButton();
            UpdateNavigationButtons();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        /// <summary>
        /// Adds listeners
        /// </summary>
        private void AddListeners()
        {
            AlbumEntry.OnClicked += SelectAlbumEntry;
        
            _addButton.onClick.AddListener(AddAlbumEntry);
            _removeButton.onClick.AddListener(RemoveCurrentAlbumEntry);
            _logButton.onClick.AddListener(LogAlbumEntryInstances);
        
            _scrollLeftButton.onClick.AddListener(ScrollLeft);
            _scrollRightButton.onClick.AddListener(ScrollRight);
        }
    
        /// <summary>
        /// Removes listeners
        /// </summary>
        private void RemoveListeners()
        {
            AlbumEntry.OnClicked -= SelectAlbumEntry;
        
            _addButton.onClick.RemoveListener(AddAlbumEntry);
            _removeButton.onClick.RemoveListener(RemoveCurrentAlbumEntry);
            _logButton.onClick.RemoveListener(LogAlbumEntryInstances);
        
            _scrollLeftButton.onClick.RemoveListener(ScrollLeft);
            _scrollRightButton.onClick.RemoveListener(ScrollRight);
        }
        
        /// <summary>
        /// Downloads album data from the API
        /// </summary>
        private async void DownloadData()
        { 
            var json= await _downloadController.DownloadContent(_apiUrl);
            _albumCatalog = _downloadController.GetJsonAs<List<AlbumEntryData>>(json);
        }

        /// <summary>
        /// Adds a new album entry until there's a new one available in the album catalog
        /// </summary>
        private void AddAlbumEntry()
        {
            if (_albumCatalog != null && _albumCatalog.Count > _currentIndex + 1)
            {
                _currentIndex++;
                var currentEntry = _albumCatalog[_currentIndex-1];
                var albumEntry = Instantiate(_albumEntryPrefab, _albumContainer);
                albumEntry.Initialize(currentEntry);
                albumEntry.transform.position = new Vector3((_currentIndex - 1) * 2, 0f, 0f);
                _albumEntries.Add(currentEntry.Id, albumEntry);
                SelectAlbumEntry(albumEntry);
            }
        }
    
        /// <summary>
        /// Removes currently selected album entry
        /// </summary>
        private void RemoveCurrentAlbumEntry()
        {
            //known issue / out of scope: doesn't adjust positions to fill any gaps caused by removing an album entry
            if (_currentAlbumEntry != null && _albumEntries.ContainsKey(_currentAlbumEntry.Data.Id))
            {
                _albumEntries.Remove(_currentAlbumEntry.Data.Id);
                Destroy(_currentAlbumEntry.gameObject);
                _currentAlbumEntry = null;
                UpdateRemoveButton();
            }
        }
        
        /// <summary>
        /// Updates state of Remove Button
        /// </summary>
        private void UpdateRemoveButton()
        {
            _removeButton.interactable = _currentAlbumEntry != null;
        }
        
        /// <summary>
        /// Updates state of navigation buttons based on current focusIndex
        /// </summary>
        private void UpdateNavigationButtons()
        {
            //known bug: this will allow scrolling all the way from index 1 to end, even if those entries have been removed
            _scrollLeftButton.interactable = _focusIndex > 1;
            _scrollRightButton.interactable = _focusIndex < _currentIndex;
        }

        /// <summary>
        /// Selects an album entry and deselects previously selected one
        /// </summary>
        /// <param name="entry"></param>
        private void SelectAlbumEntry(AlbumEntry entry)
        {
            if (entry != null)
            {
                if (_currentAlbumEntry != null)
                {
                    _currentAlbumEntry.Deselect();
                }
                
                int.TryParse(entry.Data.Id, out _focusIndex);
                _cameraController.SetTargetX((_focusIndex - 1) * 2);
                entry.Select();
                _currentAlbumEntry = entry;
            }
            UpdateRemoveButton();
            UpdateNavigationButtons();
        }

        /// <summary>
        /// Logs all active Album Entry instances to Console
        /// </summary>
        private void LogAlbumEntryInstances()
        {
            if (_albumEntries != null && _albumEntries.Count > 0)
            {
                foreach (var entry in _albumEntries.Values)
                {
                    Debug.Log(entry.ToString());
                }
            }
        }

        /// <summary>
        /// Scrolls View to the left without explicitly selecting an album entry 
        /// </summary>
        private void ScrollLeft()
        {
            if (_focusIndex > 1)
            {
                _focusIndex--;
                _cameraController.SetTargetX((_focusIndex - 1) * 2);
            }

            UpdateNavigationButtons();
        }

        /// <summary>
        /// Scrolls View to the right without explicitly selecting an album entry 
        /// </summary>
        private void ScrollRight()
        {
            if (_focusIndex < _currentIndex)
            {
                _focusIndex++;
                _cameraController.SetTargetX((_focusIndex - 1) * 2);
            }
        
            UpdateNavigationButtons();
        }
    }
}
