using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Settings : MonoBehaviour {
	
	public InputField path;
	string defaultpath;
	ManageXML mg;
	void Start () {
         mg = new ManageXML();
	    defaultpath = mg.Settings();
		path.text = defaultpath;
	}
	public void save(){
		string tmp = path.text;
		if(tmp!=defaultpath){
			mg.editSettings (tmp);
			Application.LoadLevel ("StartScene");

	}
	}
	public void cancel()
	{
		Application.LoadLevel ("StartScene");
	}

	public void onBrowse(){
#if UNITY_ANDROID
		print("Browse started");
	 AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	 AndroidJavaObject currentActivity= jc.GetStatic<AndroidJavaObject>("currentActivity");
		string[] tmp={ "MainCamera", "setBrowsedPath"};
		currentActivity.Call("selectDirectory",tmp);
		print("function called");
#else
		setBrowsedPath(Browse.browseForFolder());
		#endif
	}

	public void setBrowsedPath( string thisPath){
		print("function returned");
		if((thisPath!=""))
		path.text = thisPath;
	}
}
