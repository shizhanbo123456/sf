using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BulletParticleController : MonoBehaviour
{
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    public List<ParticleSystem> particleSystemsActiveOnly = new List<ParticleSystem>();

    public void Play()
    {
        foreach (var i in particleSystems) i.Play();
        foreach (var i in particleSystemsActiveOnly) i.Play();
    }
    public void Stop()
    {
        foreach (var i in particleSystems) i.Stop();
        foreach (var i in particleSystemsActiveOnly) i.Stop();
    }
    public void ChangeColor(Color targetColor)
    {
        foreach (var particleSystem in particleSystems)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = targetColor;
            }
        }
    }
}