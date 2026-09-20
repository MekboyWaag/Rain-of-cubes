using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class RainCube : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RainCubeView _view;

    private RainCubeConfig _config;
    private Rigidbody _rigidbody;

    private bool _hasHitPlatform;
    private bool _isDying;
    private bool _isReleased;
    private bool _isFaulted;

    private float _lifetimeTimer;
    private float _targetLifetime;

    public event Action<RainCube> OnLifetimeEnded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_view == null)
        {
            Debug.LogError("[RainCube] View reference is missing!", this);
            _isFaulted = true;
            gameObject.SetActive(false);
            return;
        }
    }

    private void Update()
    {
        if (_isFaulted || _isReleased || _config == null) return;

        if (transform.position.y < _config.KillHeight)
        {
            ReleaseCube();
            return;
        }

        if (!_isDying) return;

        _lifetimeTimer += Time.deltaTime;

        if (_lifetimeTimer >= _targetLifetime)
        {
            ReleaseCube();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFaulted || _hasHitPlatform || _isReleased || _config == null) return;

        if (collision.gameObject.TryGetComponent<Platform>(out _))
        {
            _hasHitPlatform = true;

            Color targetColor = GetRandomHitColor();
            float targetLifetime = Random.Range(_config.MinLifetime, _config.MaxLifetime);

            TriggerDeathSequence(targetColor, targetLifetime);
        }
    }

    private void OnDisable()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        OnLifetimeEnded = null;
    }

    public void Initialize(Vector3 position, RainCubeConfig config)
    {
        if (_isFaulted) return;

        _config = config;
        _isReleased = false;
        _hasHitPlatform = false;
        _isDying = false;
        _lifetimeTimer = 0f;

        transform.position = position;
        _rigidbody.position = position;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _view.Initialize(_config.ShaderColorProperty, _config.DefaultSpawnColor);
    }

    public void TriggerDeathSequence(Color hitColor, float lifetime)
    {
        if (_isFaulted) return;

        _isDying = true;
        _targetLifetime = lifetime;
        _view.SetColor(hitColor);
    }

    private Color GetRandomHitColor()
    {
        if (_config.AvailableHitColors == null || _config.AvailableHitColors.Count == 0) return Color.red;
        return _config.AvailableHitColors[Random.Range(0, _config.AvailableHitColors.Count)];
    }

    private void ReleaseCube()
    {
        if (_isReleased) return;

        _isReleased = true;
        OnLifetimeEnded?.Invoke(this);
    }
}