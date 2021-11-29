using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCardParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;


    private void Awake()
    {
        _particles.gameObject.SetActive(false);
        _particles.gameObject.SetActive(true);
    }

    public void ToggleParticles()
    {
        if (_particles.IsAlive())
        {
            StopParticles();
        } else if (!_particles.isPlaying)
        {
            PlayParticles();
        }
    }

    public void StopParticles()
    {
        Print();
        _particles.Stop();
        _particles.Clear();
        Print();
    }

    public void PlayParticles()
    {
        Print();
        _particles.Play();
        Print();
    }

    public void Print()
    {
        Debug.Log("isAlive: " + _particles.IsAlive()
         + " | isPlaying: " + _particles.isPlaying
         + " | isEmitting: " + _particles.isEmitting
         + " | isStopped: " + _particles.isStopped);
    }
}
