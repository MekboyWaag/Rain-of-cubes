using UnityEngine;

public class CubeRainCoordinator : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private CubeRainConfigSO _config;

    [Header("Systems")]
    [SerializeField] private CubeRainSpawner _spawner;

    private void Awake()
    {
        if (_config == null || _spawner == null)
        {
            Debug.LogError("[CubeRainCoordinator] Dependencies missing! Initialization aborted.");
            return;
        }

        _spawner.Initialize(_config);
    }
}