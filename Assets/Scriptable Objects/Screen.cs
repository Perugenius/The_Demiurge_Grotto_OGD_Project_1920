using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Screen")]
public class Screen : ScriptableObject
{
    [Header("Screen Parameter")]
    public float Width;
    public float Height;

    public Vector2 Random()
    {
        float x = UnityEngine.Random.value * Width - Width * .5f;
        float y = UnityEngine.Random.value * Height - Height * .5f;
        
        return new Vector2(x,y);
    }
}
