using MonoBehaviours;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class Settings : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private Slider _heightMultiplierSlider;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;
            _heightMultiplierSlider = root.Q<Slider>("HeightMultiplier");
            if (_heightMultiplierSlider != null)
            {
                _heightMultiplierSlider.RegisterValueChangedCallback(OnHeightMultiplierChanged);
                _heightMultiplierSlider.value = ProceduralTerrain.Instance.HeightMultiplier;
            }
        }

        private void OnDisable()
        {
            if (_heightMultiplierSlider != null)
            {
                _heightMultiplierSlider.UnregisterValueChangedCallback(OnHeightMultiplierChanged);
            }
        }

        private void OnHeightMultiplierChanged(ChangeEvent<float> evt)
        {
            Debug.Log($"Chunk Radius changed from {evt.previousValue} to {evt.newValue}");
            ProceduralTerrain.Instance.HeightMultiplier = evt.newValue;
        }
    }
}