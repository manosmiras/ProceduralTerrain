using MonoBehaviours;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ProceduralTerrainUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private Label _chunkGenerationLabel;
        private Label _lodUpdatesLabel;
        private Label _totalLabel;
        private Label _fpsLabel;
        private SliderInt _chunkRadiusSlider;
        private Slider _heightMultiplierSlider;
        private Button _restartButton;
        private Toggle _moveCameraToggle;
        private Toggle _fogToggle;
        private double _lastChunkGenerationTime;
        private double _lastLodUpdatesTime;
        private PlayerCamera _playerCamera;
        private float _timeSinceLastUpdate;
        private const float UpdateInterval = .25f;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            _playerCamera = FindFirstObjectByType<PlayerCamera>();
            var root = _uiDocument.rootVisualElement;
            _chunkGenerationLabel = root.Q<Label>("ChunkGeneration");
            ProceduralTerrain.Instance.TerrainRegenerated += OnTerrainRegenerated;
            _lodUpdatesLabel = root.Q<Label>("LodUpdates");
            ProceduralTerrain.Instance.TerrainLodsUpdated += OnTerrainLodsUpdated;
            _totalLabel = root.Q<Label>("Total");
            _fpsLabel = root.Q<Label>("FPS");
            _heightMultiplierSlider = root.Q<Slider>("HeightMultiplier");
            _heightMultiplierSlider.RegisterValueChangedCallback(OnHeightMultiplierChanged);
            _heightMultiplierSlider.value = ProceduralTerrain.Instance.HeightMultiplier;
            _chunkRadiusSlider = root.Q<SliderInt>("ChunkRadius");
            _chunkRadiusSlider.RegisterValueChangedCallback(OnChunkRadiusChanged);
            _chunkRadiusSlider.value = ProceduralTerrain.Instance.ChunkRadius;
            _moveCameraToggle = root.Q<Toggle>("MoveCamera");
            _moveCameraToggle.RegisterValueChangedCallback(OnMoveCameraToggleChanged);
            _moveCameraToggle.value = _playerCamera.ShouldMove;
            _fogToggle = root.Q<Toggle>("Fog");
            _fogToggle.RegisterValueChangedCallback(OnFogToggleChanged);
            _fogToggle.value = RenderSettings.fog;
            
            
            _restartButton = root.Q<Button>("Restart");
            _restartButton.clicked += OnRestartButtonClicked;
        }

        private void OnDisable()
        {
            if (ProceduralTerrain.Instance != null)
            {
                ProceduralTerrain.Instance.TerrainRegenerated -= OnTerrainRegenerated;
                ProceduralTerrain.Instance.TerrainLodsUpdated -= OnTerrainLodsUpdated;
            }
            _heightMultiplierSlider.UnregisterValueChangedCallback(OnHeightMultiplierChanged);
            _chunkRadiusSlider.UnregisterValueChangedCallback(OnChunkRadiusChanged);
            _moveCameraToggle.UnregisterValueChangedCallback(OnMoveCameraToggleChanged);
            _fogToggle.UnregisterValueChangedCallback(OnFogToggleChanged);
            _restartButton.clicked -= OnRestartButtonClicked;
        }

        private void Update()
        {
            _timeSinceLastUpdate += Time.deltaTime;
            if (_timeSinceLastUpdate < UpdateInterval) return;
            _timeSinceLastUpdate = 0f;
            _fpsLabel.text = $"{(int) (1f / Time.deltaTime)} FPS ({Time.deltaTime * 1000:F2}ms)";
        }

        private void OnHeightMultiplierChanged(ChangeEvent<float> evt)
        {
            ProceduralTerrain.Instance.HeightMultiplier = evt.newValue;
        }
        
        private void OnRestartButtonClicked()
        {
            _ = ProceduralTerrain.Instance.InitializeChunks();
        }

        private void OnTerrainRegenerated(double elapsedTime)
        {
            _lastChunkGenerationTime = elapsedTime;
            _chunkGenerationLabel.text = $"{elapsedTime:F2}ms";
            _totalLabel.text = $"{_lastChunkGenerationTime + _lastLodUpdatesTime:F2}ms";
        }
        
        private void OnTerrainLodsUpdated(double elapsedTime)
        {
            _lastLodUpdatesTime = elapsedTime;
            _lodUpdatesLabel.text = $"{elapsedTime:F2}ms";
            _totalLabel.text = $"{_lastChunkGenerationTime + _lastLodUpdatesTime:F2}ms";
        }
        
        private void OnMoveCameraToggleChanged(ChangeEvent<bool> evt)
        {
            _playerCamera.ShouldMove = evt.newValue;
        }
        
        private void OnChunkRadiusChanged(ChangeEvent<int> evt)
        {
            ProceduralTerrain.Instance.ChunkRadius = evt.newValue;
        }
        
        private void OnFogToggleChanged(ChangeEvent<bool> evt)
        {
            RenderSettings.fog = evt.newValue;
        }

    }
}