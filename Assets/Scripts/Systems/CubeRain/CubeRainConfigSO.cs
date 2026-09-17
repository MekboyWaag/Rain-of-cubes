using UnityEngine;

[CreateAssetMenu(fileName = "CubeRainConfig", menuName = "Systems/Cube Rain Config")]
public class CubeRainConfigSO : ScriptableObject
{
    [Header("Visual Settings (Private)")]
    [SerializeField] private Color[] _availableHitColors;

    [field: Header("Pool Settings")]
    [field: SerializeField, Min(10)] public int InitialPoolCapacity { get; private set; } = 100;
    [field: SerializeField, Min(0.1f)] public float SpawnInterval { get; private set; } = 0.5f;

    [field: Tooltip("Размер зоны спавна. Ось Y игнорируется (высота задается позицией Спавнера).")]
    [field: SerializeField] public Vector3 SpawnAreaSize { get; private set; } = new Vector3(15f, 0f, 15f);

    [field: Header("Lifecycle Settings")]
    [field: SerializeField, Min(1f)] public float MinLifetime { get; private set; } = 2f;
    [field: SerializeField, Min(1f)] public float MaxLifetime { get; private set; } = 5f;

    [field: SerializeField, Tooltip("Высота по оси Y, ниже которой куб уничтожается")]
    public float KillHeight { get; private set; } = -20f;

    [field: Header("Visual Settings")]
    [field: SerializeField, Tooltip("Имя свойства цвета (_Color для Built-in, _BaseColor для URP)")]
    public string ShaderColorProperty { get; private set; } = "_BaseColor";

    [field: SerializeField] public Color DefaultSpawnColor { get; private set; } = Color.white;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (MinLifetime > MaxLifetime)
        {
            MaxLifetime = MinLifetime;
        }
    }
#endif

    public Color GetRandomHitColor()
    {
        if (_availableHitColors == null || _availableHitColors.Length == 0)
            return Color.red;

        return _availableHitColors[Random.Range(0, _availableHitColors.Length)];
    }

    public float GetRandomLifetime() => Random.Range(MinLifetime, MaxLifetime);

    public Vector3 GetRandomLocalSpawnPosition()
    {
        float x = Random.Range(-SpawnAreaSize.x / 2f, SpawnAreaSize.x / 2f);
        float z = Random.Range(-SpawnAreaSize.z / 2f, SpawnAreaSize.z / 2f);

        return new Vector3(x, 0f, z);
    }
}