using UnityEngine;
using UnityEngine.Pool;

public class CubeRainSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RainCube _cubePrefab;
    [SerializeField] private Transform _spawnOrigin;

    private CubeRainConfigSO _config;
    private ObjectPool<RainCube> _pool;
    private float _spawnTimer;
    private bool _isInitialized;
    private int _shaderColorPropertyId;

    private void Update()
    {
        if (!_isInitialized || _config.SpawnInterval <= 0f)
            return;

        _spawnTimer += Time.deltaTime;

        while (_spawnTimer >= _config.SpawnInterval)
        {
            _spawnTimer -= _config.SpawnInterval;
            SpawnCube();
        }
    }

    private void OnDestroy()
    {
        _isInitialized = false;
        _pool?.Dispose();
    }

    public void Initialize(CubeRainConfigSO config)
    {
        if (_isInitialized) return;

        if (_cubePrefab == null)
        {
            Debug.LogError("[CubeRainSpawner] Cube Prefab is missing! Assign it in the Inspector.", this);
            return;
        }

        if (_spawnOrigin == null)
        {
            Debug.LogError("[CubeRainSpawner] Spawn Origin is missing!", this);
            return;
        }

        _config = config;
        _shaderColorPropertyId = Shader.PropertyToID(_config.ShaderColorProperty);

        _pool = new ObjectPool<RainCube>(
            createFunc: CreateCube,
            actionOnGet: null,
            actionOnRelease: cube => cube.gameObject.SetActive(false),
            actionOnDestroy: DestroyCubeTarget,
            defaultCapacity: _config.InitialPoolCapacity
        );

        _isInitialized = true;
    }

    private void SpawnCube()
    {
        RainCube cube = _pool.Get();
        Vector3 randomPos = _spawnOrigin.position + _config.GetRandomLocalSpawnPosition();

        cube.Initialize(randomPos, _config.DefaultSpawnColor, _config.KillHeight, _shaderColorPropertyId);
        cube.gameObject.SetActive(true);
    }

    private void HandleCubeHit(RainCube cube)
    {
        if (!_isInitialized) return;

        Color targetColor = _config.GetRandomHitColor();
        float targetLifetime = _config.GetRandomLifetime();

        cube.TriggerDeathSequence(targetColor, targetLifetime);
    }

    private RainCube CreateCube()
    {
        RainCube cube = Instantiate(_cubePrefab, transform);
        cube.gameObject.SetActive(false);
        cube.OnPlatformHit += HandleCubeHit;
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
            cube.OnPlatformHit -= HandleCubeHit;
            cube.OnLifetimeEnded -= ReturnToPool;
            Destroy(cube.gameObject);
        }
    }
}