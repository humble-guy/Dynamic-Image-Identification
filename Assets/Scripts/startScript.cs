using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class startScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void play()
	{
		LevelManager.Load("QuizList");

	}

	public void setting()
	{
		LevelManager.Load("SettingScene");

	}

	public void exit()
	{
		Application.Quit ();
	}


	public void createQuizFile()
	{
		LevelManager.Load ("CreateQuizScene");
	}
}
