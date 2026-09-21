using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CubeRainSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RainCube _prefab;
    [SerializeField] private Transform _origin;

    private CubeRainSpawnerConfig _config;
    private RainCubeConfig _elementConfig;
    private ObjectPool<RainCube> _pool;

    private Coroutine _spawnRoutine;
    private WaitForSeconds _spawnDelay;
    private bool _isInitialized;

    private void OnDestroy()
    {
        StopSpawning();

        _isInitialized = false;
        _pool?.Dispose();
    }

    public void Initialize(CubeRainSpawnerConfig config, RainCubeConfig elementConfig)
    {
        if (_isInitialized)
        {
            return;
        }

        if (_prefab == null || _origin == null)
        {
            Debug.LogError("[CubeRainSpawner] Dependencies missing!", this);
            return;
        }

        _config = config;
        _elementConfig = elementConfig;
        _spawnDelay = new WaitForSeconds(_config.SpawnInterval);

        _pool = new ObjectPool<RainCube>(
            createFunc: CreateElement,
            actionOnGet: null,
            actionOnRelease: element => element.gameObject.SetActive(false),
            actionOnDestroy: DestroyElement,
            defaultCapacity: _config.InitialPoolCapacity
        );

        _isInitialized = true;

        StartSpawning();
    }

    private void StartSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }

        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return _spawnDelay;
            Spawn();
        }
    }

    private void Spawn()
    {
        RainCube element = _pool.Get();
        Vector3 randomPos = GetRandomSpawnPosition();

        element.Initialize(randomPos, _elementConfig, _config.KillHeight);
        element.gameObject.SetActive(true);
    }

    private RainCube CreateElement()
    {
        RainCube element = Instantiate(_prefab, transform);
        element.gameObject.SetActive(false);
        element.OnLifetimeEnded += ReturnToPool;

        return element;
    }

    private void ReturnToPool(RainCube element)
    {
        if (!_isInitialized)
        {
            Destroy(element.gameObject);
            return;
        }

        _pool.Release(element);
    }

    private void DestroyElement(RainCube element)
    {
        if (element != null)
        {
            element.OnLifetimeEnded -= ReturnToPool;
            Destroy(element.gameObject);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-_config.SpawnAreaSize.x / 2f, _config.SpawnAreaSize.x / 2f);
        float z = Random.Range(-_config.SpawnAreaSize.z / 2f, _config.SpawnAreaSize.z / 2f);

        return _origin.position + new Vector3(x, 0f, z);
    }
}