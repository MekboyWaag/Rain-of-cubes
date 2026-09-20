using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RainCubeConfig", menuName = "Systems/Rain Cube Config")]
public class RainCubeConfig : ScriptableObject
{
    [Header("Lifecycle Settings")]
    [SerializeField, Min(1f)] private float _minLifetime = 2f;
    [SerializeField, Min(1f)] private float _maxLifetime = 5f;

    [SerializeField, Tooltip("Высота по оси Y, ниже которой куб уничтожается")] private float _killHeight = -20f;

    [Header("Visual Settings")]
    [SerializeField, Tooltip("Имя свойства цвета (_Color для Built-in, _BaseColor для URP)")] private string _shaderColorProperty = "_BaseColor";
    [SerializeField] private Color _defaultSpawnColor = Color.white;
    [SerializeField] private Color[] _availableHitColors;

    public float MinLifetime => _minLifetime;
    public float MaxLifetime => _maxLifetime;
    public float KillHeight => _killHeight;
    public string ShaderColorProperty => _shaderColorProperty;
    public Color DefaultSpawnColor => _defaultSpawnColor;
    public IReadOnlyList<Color> AvailableHitColors => _availableHitColors;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_minLifetime > _maxLifetime)
        {
            _maxLifetime = _minLifetime;
        }
    }
#endif
}