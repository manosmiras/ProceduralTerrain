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
        private Slider _heightMultiplierSlider;
        private Button _restartButton;
        private long _lastChunkGenerationTime;
        private long _lastLodUpdatesTime;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;
            _chunkGenerationLabel = root.Q<Label>("ChunkGeneration");
            ProceduralTerrain.Instance.TerrainRegenerated += OnTerrainRegenerated;
            _lodUpdatesLabel = root.Q<Label>("LodUpdates");
            ProceduralTerrain.Instance.TerrainLodsUpdated += OnTerrainLodsUpdated;
            _totalLabel = root.Q<Label>("Total");
            _heightMultiplierSlider = root.Q<Slider>("HeightMultiplier");
            _heightMultiplierSlider.RegisterValueChangedCallback(OnHeightMultiplierChanged);
            _heightMultiplierSlider.value = ProceduralTerrain.Instance.HeightMultiplier;
            
            _restartButton = root.Q<Button>("Restart");
            _restartButton.clicked += OnRestartButtonClicked;
        }

        private void OnDisable()
        {
            /*if (ProceduralTerrain.Instance != null)
            {
                ProceduralTerrain.Instance.TerrainRegenerated -= OnTerrainRegenerated;
                ProceduralTerrain.Instance.TerrainLodsUpdated -= OnTerrainLodsUpdated;
            }*/
            _heightMultiplierSlider.UnregisterValueChangedCallback(OnHeightMultiplierChanged);
            _restartButton.clicked -= OnRestartButtonClicked;
        }

        private void OnHeightMultiplierChanged(ChangeEvent<float> evt)
        {
            Debug.Log($"Chunk Radius changed from {evt.previousValue} to {evt.newValue}");
            ProceduralTerrain.Instance.HeightMultiplier = evt.newValue;
        }
        
        private void OnRestartButtonClicked()
        {
            _ = ProceduralTerrain.Instance.InitializeChunks();
        }

        private void OnTerrainRegenerated(long elapsedTime)
        {
            _lastChunkGenerationTime = elapsedTime;
            _chunkGenerationLabel.text = $"{elapsedTime}ms";
            _totalLabel.text = $"{_lastChunkGenerationTime + _lastLodUpdatesTime}ms";
        }
        
        private void OnTerrainLodsUpdated(long elapsedTime)
        {
            _lastLodUpdatesTime = elapsedTime;
            _lodUpdatesLabel.text = $"{elapsedTime}ms";
            _totalLabel.text = $"{_lastChunkGenerationTime + _lastLodUpdatesTime}ms";
        }
    }
}