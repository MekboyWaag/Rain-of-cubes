using UnityEngine;

public class RainCubeView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private MaterialPropertyBlock _propBlock;
    private int _colorPropertyId;
    private bool _isInitialized;

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();

        if (_meshRenderer == null)
        {
            Debug.LogError("[RainCubeView] MeshRenderer is missing!", this);
            return;
        }
    }

    public void Initialize(string colorPropertyName, Color initialColor)
    {
        if (!_isInitialized)
        {
            _colorPropertyId = Shader.PropertyToID(colorPropertyName);
            _isInitialized = true;
        }

        SetColor(initialColor);
    }

    public void SetColor(Color color)
    {
        if (_meshRenderer == null)
        {
            return;
        }

        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(_colorPropertyId, color);
        _meshRenderer.SetPropertyBlock(_propBlock);
    }
}