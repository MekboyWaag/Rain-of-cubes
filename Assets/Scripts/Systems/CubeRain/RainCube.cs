using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RainCube : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer _meshRenderer;

    private Rigidbody _rigidbody;
    private MaterialPropertyBlock _propBlock;

    private bool _hasHitPlatform;
    private bool _isDying;
    private bool _isReleased;
    private bool _isFaulted;
    private float _lifetimeTimer;
    private float _targetLifetime;
    private float _killHeight;
    private int _colorPropertyId;

    public event Action<RainCube> OnPlatformHit;
    public event Action<RainCube> OnLifetimeEnded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _propBlock = new MaterialPropertyBlock();

        if (_meshRenderer == null)
        {
            Debug.LogError("[RainCube] MeshRenderer reference is missing! Assign it in the Inspector.", this);
            _isFaulted = true;
            gameObject.SetActive(false);
            return;
        }
    }

    private void Update()
    {
        if (_isFaulted || _isReleased)
            return;

        if (transform.position.y < _killHeight)
        {
            ReleaseCube();
            return;
        }

        if (!_isDying)
            return;

        _lifetimeTimer += Time.deltaTime;

        if (_lifetimeTimer >= _targetLifetime)
        {
            ReleaseCube();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFaulted || _hasHitPlatform || _isReleased)
            return;

        if (collision.gameObject.TryGetComponent<Platform>(out _))
        {
            _hasHitPlatform = true;
            OnPlatformHit?.Invoke(this);
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
        OnPlatformHit = null;
        OnLifetimeEnded = null;
    }

    public void Initialize(Vector3 position, Color initialColor, float killHeight, int colorPropertyId)
    {
        if (_isFaulted) return;

        _isReleased = false;
        _hasHitPlatform = false;
        _isDying = false;
        _lifetimeTimer = 0f;
        _killHeight = killHeight;
        _colorPropertyId = colorPropertyId;

        transform.position = position;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        SetColor(initialColor);
    }

    public void TriggerDeathSequence(Color hitColor, float lifetime)
    {
        if (_isFaulted) return;

        _isDying = true;
        _targetLifetime = lifetime;
        SetColor(hitColor);
    }

    private void SetColor(Color color)
    {
        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(_colorPropertyId, color);
        _meshRenderer.SetPropertyBlock(_propBlock);
    }

    private void ReleaseCube()
    {
        if (_isReleased) return;

        _isReleased = true;
        OnLifetimeEnded?.Invoke(this);
    }
}