using UnityEngine;
using System.Collections;
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif
public class Browse : MonoBehaviour {
#if UNITY_STANDALONE_WIN
	[DllImport("user32.dll")]
	private static extern void OpenFileDialog();
	[DllImport("user32.dll")]
	private static extern void FolderBrowserDialog();
#endif
	// Use this for initialization
	public static string imagePath;
	void Start () {
	
	}
	public void openPanel()
	{
#if UNITY_EDITOR 
		imagePath = browseForImage ();
		ImageRendering ir = new ImageRendering ();
		Debug.Log (imagePath);
		ir.onBrowseForOption();
#elif UNITY_STANDALONE_WIN
		imagePath = browseForImage ();
		ImageRendering ir = new ImageRendering ();
		Debug.Log (imagePath);
		ir.onBrowseForOption();
#else
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		string[] obj={"MainCamera","BrowseForPath"};
		currentActivity.Call("selectImage",obj);

#endif

	}
	public static string browseForFolder(){
#if UNITY_EDITOR
		return UnityEditor.EditorUtility.OpenFolderPanel("Select Folder", "", "");
		#endif
#if    UNITY_ANDROID
		return Application.persistentDataPath;
#endif
#if UNITY_STANDALONE_WIN
		System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
		fbd.Description = 
			"Select the directory that you want to use as the default.";
		fbd.ShowNewFolderButton = true;
		fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
		if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK){
			return fbd.SelectedPath;
		}else{
			return Application.persistentDataPath;
		}
#endif
	}

public static string browseForImage(){
#if UNITY_EDITOR		
		return UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "jpg");
#endif
#if UNITY_STANDALONE_WIN
		System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
		ofd.InitialDirectory = Application.persistentDataPath;
		ofd.Filter =  "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
		if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK){
			return ofd.FileName;
		}else{
			return "";
		}
#endif
		return "";
	}
	public void BrowseForPath(string image){
		Debug.Log ("yeh select kiya!!!"+image);
		imagePath = image;
		ImageRendering ir2 = new ImageRendering ();
		Debug.Log (imagePath);
		ir2.onBrowseForOption();
	}

}