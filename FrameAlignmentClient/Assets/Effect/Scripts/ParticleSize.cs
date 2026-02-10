
/*using UnityEngine;
using System.Collections;

public class ParticleSize : MonoBehaviour {

    public float PicScale = 1f;
    public float Lifetime = 1f;
    public bool isloop = true;
    public float speedScale = 1f;

void Start ()         
    {
   
        ParticleSystem[] psarr = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < psarr.Length; i++ )
        {
            psarr.startSize *= PicScale;
            psarr.startLifetime *= Lifetime;
            psarr.startSpeed *= speedScale;
            psarr.loop = isloop;
			
        }
}  
}
*/