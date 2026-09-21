using UnityEngine;

[CreateAssetMenu(fileName = "CubeRainSpawnerConfig", menuName = "Systems/Cube Rain Spawner Config")]
public class CubeRainSpawnerConfig : ScriptableObject
{
    [Header("Pool Settings")]
    [SerializeField, Min(10)] private int _initialPoolCapacity = 100;
    [SerializeField, Min(0.1f)] private float _spawnInterval = 0.5f;

    [Header("Spatial Boundaries")]
    [SerializeField] private Vector3 _spawnAreaSize = new Vector3(15f, 0f, 15f);
    [SerializeField] private float _killHeight = -20f;

    public int InitialPoolCapacity => _initialPoolCapacity;
    public float SpawnInterval => _spawnInterval;
    public float KillHeight => _killHeight;
    public Vector3 SpawnAreaSize => _spawnAreaSize;
}