using System;
using System.Buffers;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEmissionController : MonoBehaviour
{
    #region inspector

    public int maxParticles = 100;
    public float smoothStopDuration = 0.25f;

    #endregion
    
    private ParticleSystem[] _particleSystems;
    private float _stoppedTime;
    private ParticleSystem.Particle[] _remainingParticles;

    public void Play()
    {
        ResetStop();
        SetEmission(true);
    }

    public void Stop(bool withAcceleration = true)
    {
        SetEmission(false);

        if (withAcceleration)
        {
            _stoppedTime = Time.time;
            _remainingParticles ??= ArrayPool<ParticleSystem.Particle>.Shared.Rent(maxParticles);
        }
    }

    private void SetEmission(bool isActive)
    {
        foreach (var ps in _particleSystems)
        {
            var emissionModule = ps.emission;
            emissionModule.enabled = isActive;
        }
    }

    private void ResetStop()
    {
        _stoppedTime = 0;

        if (_remainingParticles != null)
        {
            ArrayPool<ParticleSystem.Particle>.Shared.Return(_remainingParticles);
            _remainingParticles = null;
        }
    }
    
    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        Play();
    }

    private void LateUpdate()
    {
        if (_remainingParticles == null) return;
        
        var elapsedTime = Time.time - _stoppedTime;
        var elapsedRate = elapsedTime / smoothStopDuration;
        var remainsRate = 1 - elapsedRate;
        var totalCount = 0;
        
        foreach (var ps in _particleSystems)
        {
            var count = ps.GetParticles(_remainingParticles, maxParticles);
            for (var i = 0; i < count; i++)
            {
                var remainingParticle = _remainingParticles[i];
                var newLifetime = remainsRate * remainingParticle.startLifetime;
                _remainingParticles[i].remainingLifetime = Mathf.Min(remainingParticle.remainingLifetime,  newLifetime);
            }
            
            ps.SetParticles(_remainingParticles, count);
            totalCount += count;
        }
        
        if (totalCount == 0)
        {
            ResetStop();
        }
    }

    private void OnDestroy()
    {
        ResetStop();
    }
}
