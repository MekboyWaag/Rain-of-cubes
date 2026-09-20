using UnityEngine;

public class CubeRainCoordinator : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private CubeRainSpawnerConfig _spawnerConfig;
    [SerializeField] private RainCubeConfig _cubeConfig;

    [Header("Systems")]
    [SerializeField] private CubeRainSpawner _spawner;

    private void Awake()
    {
        if (_cubeConfig == null || _spawner == null || _spawnerConfig == null)
        {
            Debug.LogError("[CubeRainCoordinator] Dependencies missing! Initialization aborted.", this);
            return;
        }

        _spawner.Initialize(_spawnerConfig, _cubeConfig);
    }
}