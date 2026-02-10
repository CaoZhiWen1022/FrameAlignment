using UnityEngine;
using System.Collections;

public class ShaderModify : MonoBehaviour {

	// Use this for initialization
	private Material mat;
	public float frameTimer;
	public int Xcount;
	public int Ycount;
	private float timer = 0;
	private float maxTimer = 0;
	private float offsetX = 0;
	private float offsetY = 0;
	private float tileX;
	private float tileY;
	private float maxOffsetX;
	private float maxOffsetY;
	public bool isLeftToRight = true;
	public bool isTopToBottom = true;
//	private string textureName;
	void Start () {
		mat = GetComponent<Renderer>().material;
		tileX = 1.0f / Xcount;
		tileY = 1.0f / Ycount;
		maxOffsetX = tileX * (Xcount - 1);
		maxOffsetY = tileY * (Ycount - 1);
		//textureName = renderer.material.GetTexture (0).name;
//		Debug.Log (textureName);
//		Debug.Log (tileX);
//		Debug.Log (tileY);
//		Debug.Log (Xcount);
//		Debug.Log (Ycount);
		if (!isLeftToRight) {offsetX=maxOffsetX;}
		if (isTopToBottom) {offsetY=maxOffsetY;}
		mat.mainTextureScale = new Vector2 (tileX, tileY);
		mat.mainTextureOffset = new Vector2 (offsetX, offsetY);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.fixedDeltaTime;
		if (timer >= frameTimer) {
						timer = 0;
						if(isLeftToRight){
							offsetX+=tileX;
							if(offsetX>maxOffsetX){
								if(!isTopToBottom){
									offsetY += tileY;
									offsetX = 0;
									if(offsetY>maxOffsetY){
										offsetY = 0;
									}
								}
								else{
									offsetY -= tileY;
									offsetX = 0;
									if(offsetY<0){
										offsetY = maxOffsetY;
									}
								}
							}
						}
						else{
							offsetX -=tileX;
							if(offsetX<0){
								if(!isTopToBottom){
									offsetY += tileY;
									offsetX = maxOffsetX;
									if(offsetY>maxOffsetY){
										offsetY = 0;
									}
								}
								else{
									offsetY -= tileY;
									offsetX = maxOffsetX;
									if(offsetY<0){
										offsetY = maxOffsetY;
									}
								}
							}
						}
//			if(isLeftToRight){
//				offsetX+=tileX;
//				if(offsetX>maxOffsetX){
//					offsetY += tileY;
//					offsetX = 0;
//					if(offsetY>maxOffsetY){
//						offsetY = 0;
//					}
//				}
//			}
//			else{
//				offsetX -=tileX;
//				if(offsetX<0){
//					offsetY += tileY;
//					offsetX = maxOffsetX;
//					if(offsetY>maxOffsetY){
//						offsetY = 0;
//					}
//				}
//			}
			mat.mainTextureOffset = new Vector2(offsetX,offsetY);
		}
	}
}
