using UnityEngine;
using System.Collections;

public class Destory : MonoBehaviour {
	public float WaitTime;
	// Use this for initialization
	void Start () {
//	StartCoroutine(EffectDestory());
			Destroy(this.gameObject,WaitTime);
	}
	
	
}
