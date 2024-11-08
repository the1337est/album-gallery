using System.Collections.Generic;
using Gaurav.Scripts.Album;
using UnityEngine;
using UnityEngine.UI;

namespace Gaurav.Scripts.Core
{
    public class AppController : MonoBehaviour
    {

        [SerializeField] private DownloadController _downloadController;
        [SerializeField] private Button _logButton;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _removeButton;
    
        [SerializeField] private Button _scrollLeftButton;
        [SerializeField] private Button _scrollRightButton;

        [SerializeField] private Transform _albumContainer;
        [SerializeField] private AlbumEntry _albumEntryPrefab;
    
        public Texture2D LoadingTexture;
        public Texture2D ErrorTexture;
        public static Texture2D TestImage;
    
        private Transform _cameraTransform;
        private Vector3 _cameraPos;
        private Vector3 _cameraTargetPos;
        private const float _cameraPanSpeed = 10f;
    
        private List<AlbumEntryData> _albumCatalog;
        private const string _apiUrl = "https://jsonplaceholder.typicode.com/photos";
    
        private Dictionary<string, AlbumEntry> _albumEntries = new Dictionary<string, AlbumEntry>();
    
        private int _currentIndex = 0;
        private int _focusIndex = 0;

        private AlbumEntry _currentAlbumEntry = null;
    
        public static AppController Instance;

        private void Awake()
        {
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
            _cameraTransform = Camera.main.transform;
            _cameraPos = _cameraTransform.position;
            _cameraTargetPos = _cameraPos;
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

        private void AddListeners()
        {
            AlbumEntry.OnClicked += SelectAlbumEntry;
        
            _addButton.onClick.AddListener(AddAlbumEntry);
            _removeButton.onClick.AddListener(RemoveCurrentAlbumEntry);
            _logButton.onClick.AddListener(LogAlbumEntryInstances);
        
            _scrollLeftButton.onClick.AddListener(ScrollLeft);
            _scrollRightButton.onClick.AddListener(ScrollRight);
        }
    
        private void RemoveListeners()
        {
            AlbumEntry.OnClicked -= SelectAlbumEntry;
        
            _addButton.onClick.RemoveListener(AddAlbumEntry);
            _removeButton.onClick.RemoveListener(RemoveCurrentAlbumEntry);
            _logButton.onClick.RemoveListener(LogAlbumEntryInstances);
        
            _scrollLeftButton.onClick.RemoveListener(ScrollLeft);
            _scrollRightButton.onClick.RemoveListener(ScrollRight);
        
        }

        private void Update()
        {
            var temp = Mathf.Abs(_cameraTargetPos.x - _cameraPos.x);
            if (temp > 0.01f)
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraTargetPos, Time.deltaTime * _cameraPanSpeed);
            }
            else
            {
                _cameraTransform.position = _cameraTargetPos;
            }
        }

        private async void DownloadData()
        { 
            var json= await _downloadController.DownloadContent(_apiUrl);
            _albumCatalog = _downloadController.GetJsonAs<List<AlbumEntryData>>(json);
        }

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
    
        private void RemoveCurrentAlbumEntry()
        {
            if (_currentAlbumEntry != null && _albumEntries.ContainsKey(_currentAlbumEntry.Data.Id))
            {
                _albumEntries.Remove(_currentAlbumEntry.Data.Id);
                Destroy(_currentAlbumEntry.gameObject);
                _currentAlbumEntry = null;
                UpdateRemoveButton();
            }
        }

        private void UpdateRemoveButton()
        {
            _removeButton.interactable = _currentAlbumEntry != null;
        }
    
        private void UpdateNavigationButtons()
        {
            //known bug: this will allow scrolling all the way from index 1 to end, even if those entries have been removed
            _scrollLeftButton.interactable = _focusIndex > 1;
            _scrollRightButton.interactable = _focusIndex < _currentIndex;
        }

        private void SelectAlbumEntry(AlbumEntry entry)
        {
            if (entry != null)
            {
                if (_currentAlbumEntry != null)
                {
                    _currentAlbumEntry.Deselect();
                }
                
                int.TryParse(entry.Data.Id, out _focusIndex);
                _cameraTargetPos = new Vector3((_focusIndex - 1) * 2, _cameraPos.y, _cameraPos.z);
                entry.Select();
                _currentAlbumEntry = entry;
            }
            UpdateRemoveButton();
            UpdateNavigationButtons();
        }

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

        private void ScrollLeft()
        {
            if (_focusIndex > 1)
            {
                _focusIndex--;
                _cameraTargetPos = new Vector3((_focusIndex - 1) * 2, _cameraPos.y, _cameraPos.z);
            }

            UpdateNavigationButtons();
        }

        private void ScrollRight()
        {
            if (_focusIndex < _currentIndex)
            {
                _focusIndex++;
                _cameraTargetPos = new Vector3((_focusIndex - 1) * 2, _cameraPos.y, _cameraPos.z);
            }
        
            UpdateNavigationButtons();
        }

    }
}
