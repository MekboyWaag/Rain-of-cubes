using UnityEngine;
using UnityEngine.Pool;

public class CubeRainSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RainCube _cubePrefab;
    [SerializeField] private Transform _spawnOrigin;

    private CubeRainSpawnerConfig _spawnerConfig;
    private RainCubeConfig _cubeConfig;
    private ObjectPool<RainCube> _pool;

    private float _spawnTimer;
    private bool _isInitialized;

    private void Update()
    {
        if (!_isInitialized || _spawnerConfig.SpawnInterval <= 0f) return;

        _spawnTimer += Time.deltaTime;

        while (_spawnTimer >= _spawnerConfig.SpawnInterval)
        {
            _spawnTimer -= _spawnerConfig.SpawnInterval;
            SpawnCube();
        }
    }

    private void OnDestroy()
    {
        _isInitialized = false;
        _pool?.Dispose();
    }

    public void Initialize(CubeRainSpawnerConfig spawnerConfig, RainCubeConfig cubeConfig)
    {
        if (_isInitialized) return;

        if (_cubePrefab == null || _spawnOrigin == null)
        {
            Debug.LogError("[CubeRainSpawner] Dependencies missing!", this);
            return;
        }

        _spawnerConfig = spawnerConfig;
        _cubeConfig = cubeConfig;

        _pool = new ObjectPool<RainCube>(
            createFunc: CreateCube,
            actionOnGet: null,
            actionOnRelease: cube => cube.gameObject.SetActive(false),
            actionOnDestroy: DestroyCubeTarget,
            defaultCapacity: _spawnerConfig.InitialPoolCapacity
        );

        _isInitialized = true;
    }

    private void SpawnCube()
    {
        RainCube cube = _pool.Get();
        Vector3 randomPos = GetRandomSpawnPosition();

        cube.Initialize(randomPos, _cubeConfig);
        cube.gameObject.SetActive(true);
    }

    private RainCube CreateCube()
    {
        RainCube cube = Instantiate(_cubePrefab, transform);
        cube.gameObject.SetActive(false);
        cube.OnLifetimeEnded += ReturnToPool;
        return cube;
    }

    private void ReturnToPool(RainCube cube)
    {
        if (!_isInitialized)
        {
            Destroy(cube.gameObject);
            return;
        }

        _pool.Release(cube);
    }

    private void DestroyCubeTarget(RainCube cube)
    {
        if (cube != null)
        {
            cube.OnLifetimeEnded -= ReturnToPool;
            Destroy(cube.gameObject);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-_spawnerConfig.SpawnAreaSize.x / 2f, _spawnerConfig.SpawnAreaSize.x / 2f);
        float z = Random.Range(-_spawnerConfig.SpawnAreaSize.z / 2f, _spawnerConfig.SpawnAreaSize.z / 2f);
        return _spawnOrigin.position + new Vector3(x, 0f, z);
    }
}