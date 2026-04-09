using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BulletParticleController : MonoBehaviour
{
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    public List<SpriteRenderer>spriteRenderers = new List<SpriteRenderer>();

    public void Play()
    {
        foreach (var i in particleSystems)
        {
            i.Play();
        }
        foreach(var i in spriteRenderers)
        {
            i.gameObject.SetActive(true);
        }
    }
    public void Stop()
    {
        foreach (var i in particleSystems)
        {
            i.Stop();
            i.Clear();
        }
        foreach (var i in spriteRenderers)
        {
            i.gameObject.SetActive(false);
        }
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
        foreach(var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = targetColor;
        }
    }
}