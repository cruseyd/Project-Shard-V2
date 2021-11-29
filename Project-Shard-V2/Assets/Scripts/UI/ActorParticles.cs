using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorParticles : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> edge_particles;
    [SerializeField] private List<ParticleSystem> face_particles;

    [SerializeField] private Gradient edge_gradient;
    [SerializeField] private float edge_emission;
    [SerializeField] private float edge_size;

    [SerializeField] private List<Color> face_colors;

    private bool _edgePlaying;
    private bool _facePlaying;
    private void Awake()
    {
        Set();
        StopAll();
    }

    private void Set()
    {
        foreach (ParticleSystem p in edge_particles)
        {
            ParticleSystem.ColorOverLifetimeModule col = p.colorOverLifetime;
            col.color = edge_gradient;
            ParticleSystem.EmissionModule emission = p.emission;
            emission.rateOverTime = edge_emission;
            ParticleSystem.SizeOverLifetimeModule sol = p.sizeOverLifetime;
            sol.sizeMultiplier = edge_size;
        }

        for (int ii = 0; ii < face_particles.Count; ii++)
        {
            ParticleSystem p = face_particles[ii];
            ParticleSystem.MainModule main = p.main;
            main.startColor = face_colors[ii];
        }
    }

    public void PlayEdge(bool a_flag)
    {
        foreach (ParticleSystem p in edge_particles)
        {
            if (a_flag && !_edgePlaying)
            {
                p.Play();
            }
            if (!a_flag)
            {
                p.Stop();
                p.Clear();
            }
        }
        _edgePlaying = a_flag;
    }
    public void PlayFace(bool a_flag)
    {
        foreach (ParticleSystem p in face_particles)
        {
            if (a_flag && !_facePlaying)
            {
                p.Play();
            }
            if (!a_flag)
            {
                p.Stop();
                p.Clear();
            }
        }
        _facePlaying = a_flag;
    }

    public void StopAll()
    {
        PlayEdge(false);
        PlayFace(false);
    }

    public void SetEdgeGradient(Gradient a_grad)
    {
        edge_gradient = a_grad;
        Set();
    }

    public void SetFaceColor(Color a_color, int a_index = 0)
    {
        face_colors[a_index] = a_color;
        Set();
    }
}
