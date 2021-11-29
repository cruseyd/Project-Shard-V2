using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureScroller : MonoBehaviour
{
    public float speed_x;
    public float speed_y;
    void Update()
    {
        float dx = Time.time * speed_x;
        float dy = Time.time * speed_y;
        GetComponent<Image>().material.mainTextureOffset = new Vector2(-dx, -dy);
    }
}
