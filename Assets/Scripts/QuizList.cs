using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuizList : MonoBehaviour {
	
	string defaultPath ;
	ManageXML mgXml;
	Button obj;
	public Button buttonprefab;
	void Start () {
	
		mgXml = new ManageXML ();

		defaultPath = mgXml.Settings();
		if (!Directory.Exists (defaultPath)) {
			Application.LoadLevel ("SettingsScene");
		} else {
			DirectoryInfo info = new DirectoryInfo (defaultPath);
			FileInfo[] fileInfo = info.GetFiles ("*.qz");
			foreach (FileInfo files in fileInfo) {
				//for(int i=0;i<5;i++){

				obj = Instantiate (buttonprefab) as Button;
				obj.name = files.Name;
				obj.GetComponentInChildren <Text> ().text = files.Name;
				string name = obj.name;
				obj.onClick.AddListener (() => OnClicked (name));
				obj.transform.SetParent (GameObject.FindGameObjectWithTag ("quizpanel").transform, false);
			}
		}
	
}
	public void OnClicked(string name)
	{
		PlayGameScript.quizname = name;
		Debug.Log (PlayGameScript.quizname);
		LevelManager.Load ("startQuiz");
		//SceneManager.LoadScene("startQuiz");
	}

	public void Back()
	{
		SceneManager.LoadScene ("StartScene");
	}


	public void DeleteQuiz()
	{
		SceneManager.LoadScene ("DeleteQuizList");
	}
}