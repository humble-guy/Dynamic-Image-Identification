using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageRendering : MonoBehaviour {
	 GameObject ParentPanel;
		
	public static Sprite sprite;
	string imagePath;
	public static bool imagePicked;
	//public static int c =0;
	/*public void showImage(){
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("selectImage", "Interface", "onBrowseForOption");
		#else
		onBrowseForOption();
		#endif
	}*/

	public void onBrowseForOption(){
		imagePath = Browse.imagePath;
		imagePicked = false;
		if (imagePath == "") {
			imagePicked = false;
			return;
		}

		//LineDrawingScript.imageIsPicked ();
		imagePicked = true;
		ParentPanel = GameObject.FindGameObjectWithTag("image");
		RectTransform rt = ParentPanel.GetComponent<RectTransform> ();

		Texture2D thisTexture = new Texture2D(100,100);
		WWW www = new WWW("file:///" + imagePath);
		Debug.Log ("Image path:" + imagePath);
		//LoadImageIntoTexture compresses JPGs by DXT1 and PNGs by DXT5    
		www.LoadImageIntoTexture(thisTexture);
		sprite = new Sprite();
		//RectTransform rt = ParentPanel.GetComponent<RectTransform> ();
		sprite = Sprite.Create(thisTexture, new Rect(0,0, thisTexture.width, thisTexture.height), new Vector2(0.5f, 0.0f));
		// create gameobject

		ParentPanel.GetComponent<Image>().overrideSprite = sprite;

		Color c = ParentPanel.GetComponent<Image> ().color;
		c.a = 255;
		ParentPanel.GetComponent<Image> ().color = c;
	}



}
