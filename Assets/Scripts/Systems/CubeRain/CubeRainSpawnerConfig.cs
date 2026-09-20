using UnityEngine;

[CreateAssetMenu(fileName = "CubeRainSpawnerConfig", menuName = "Systems/Cube Rain Spawner Config")]
public class CubeRainSpawnerConfig : ScriptableObject
{
    [Header("Pool Settings")]
    [SerializeField, Min(10)] private int _initialPoolCapacity = 100;
    [SerializeField, Min(0.1f)] private float _spawnInterval = 0.5f;

    [Tooltip("Размер зоны спавна. Ось Y игнорируется (высота задается позицией Спавнера).")]
    [SerializeField] private Vector3 _spawnAreaSize = new Vector3(15f, 0f, 15f);

    public int InitialPoolCapacity => _initialPoolCapacity;
    public float SpawnInterval => _spawnInterval;
    public Vector3 SpawnAreaSize => _spawnAreaSize;
}