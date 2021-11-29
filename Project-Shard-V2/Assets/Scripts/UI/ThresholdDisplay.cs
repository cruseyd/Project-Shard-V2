using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThresholdDisplay : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private ParticleSystem _particles;

    public void SetColor(Card.Color a_color)
    {
        _image.color = CardGameParams.ThresholdColorBase(a_color);
        ParticleSystem.MainModule main = _particles.main;
        main.startColor = CardGameParams.ThresholdColorParticle(a_color);
    }

}
