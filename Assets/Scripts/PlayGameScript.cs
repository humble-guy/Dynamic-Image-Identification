using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine.SceneManagement;


public class PlayGameScript : MonoBehaviour {
	GameObject ParentPanel;
	string filePath;
	public static string quizname;
	List<string> result;
	ManageXML mng;

	public GameObject pointerButton;


	public GameObject DescriptionPanel;

	// Use this for initialization
	void Start () {
		//Voice v = new Voice ();
		//v.test ();


		ParentPanel = GameObject.FindGameObjectWithTag("image");
		mng = new ManageXML ();
		filePath = mng.Settings ()+"/"+quizname;
		Debug.Log ("Current Quiz:" + quizname);
		parseXMLFile (filePath);
		printXMLData ();
		showImage ();
		drawPolygons ();
		showFlag ();
		pointerButton.GetComponentsInChildren<Text> () [0].text = "Hide Pointers";
	}
	
	// Update is called once per frame
	void Update () {
		checkClickedArea ();
	}

	public void checkClickedArea()
	{
		Ray ray;
		RaycastHit2D hit;
		if (Input.GetMouseButtonDown (0)) {
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit) {
				if (hit.collider.gameObject.tag == "polygonCollider") {
					Debug.Log (hit.collider.gameObject.name);
					changeDescription (hit.collider.gameObject.name);
					speakDescription (hit.collider.gameObject.name);
					PolygonCollider2D col = hit.collider.gameObject.GetComponent<PolygonCollider2D>();
					//showPolygonCollider (col);

				}
			}
		}
	}


	public void hideDescriptionPanel()
	{
		DescriptionPanel.SetActive (false);
	}



	public void showFlagPointer()
	{
		if (pointerButton.GetComponentsInChildren<Text> () [0].text == "Show Pointers") {
			showFlag ();
			pointerButton.GetComponentsInChildren<Text> () [0].text = "Hide Pointers";
		} else {
			deleteFlag ();
			pointerButton.GetComponentsInChildren<Text> () [0].text = "Show Pointers";
		}
	}


	GameObject obj;
	GameObject newPoint;
	public GameObject point;
	public void showPolygonCollider(PolygonCollider2D col)
	{
		if (obj != null)
			Destroy (obj);
		GameObject[] spheres = GameObject.FindGameObjectsWithTag ("spherePoint");
		if (spheres.Length > 0) {
			foreach (GameObject child in spheres)
				Destroy (child);
		}
	    obj = new GameObject ();
		obj.AddComponent<LineRenderer> ();
		LineRenderer lineRenderer = obj.GetComponent<LineRenderer> ();
		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.SetWidth (0.1f, 0.1f);
		//lineRenderer. SetColors(Color. black, Color. black);
		Vector2[] points = col.points;
		lineRenderer.SetVertexCount (points.Length + 1);
		int i;
		for(i = 0;i<points.Length ;i++)
		{
			newPoint = Instantiate (point, points[i], Quaternion.identity) as GameObject;
			newPoint.GetComponent<Transform> ().localScale = new Vector3 (0.25f, 0.25f, 0.25f);
			lineRenderer.SetPosition (i,points[i]);
			//points[i] = //do something with vector2
		}
		lineRenderer.SetPosition (i, points [0]);

		//lineRenderer.SetVertexCount (number_points[i]);
	}


	public void showFlag()
	{
		/*Vector2[] points = col.points;
		GameObject[] spheres = GameObject.FindGameObjectsWithTag ("spherePoint");
		if (spheres.Length > 0) {
			foreach (GameObject child in spheres)
				Destroy (child);
		}

		newPoint = Instantiate (point, points[0], Quaternion.identity) as GameObject;
		newPoint.GetComponent<Transform> ().localScale = new Vector3 (0.5f, 0.5f, 0.5f);*/





		int num_of_poly = description.Count;
		int start = 0;
		int sum = 0;
		for (int i = 0; i < num_of_poly; i++) {
			
			int j;
			Vector2[] colliderPoints = new Vector2[number_points[i]];
			for ( j = start; j < number_points [i]+sum; j++) {
				if(j == start)
				{
					newPoint = Instantiate (point,  new Vector2 (float.Parse (x_cord [j]), float.Parse (y_cord [j])), Quaternion.identity) as GameObject;
					newPoint.GetComponent<Transform>().localScale = new Vector3 (0.01f, 0.01f, 0.5f);
					newPoint.transform.SetParent(GameObject.FindGameObjectWithTag("image").transform);
				}


			}
			start = j;
			sum += number_points [i];
		}




	}


	public void deleteFlag()
	{
		GameObject[] spheres = GameObject.FindGameObjectsWithTag ("spherePoint");
		if (spheres.Length > 0) {
			foreach (GameObject child in spheres)
				Destroy (child);
		}
	}


	public void speakDescription(string text)
	{
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		currentActivity.Call("speakText",text);
		#endif
	}

	public void playGame()
	{
		SceneManager.LoadScene ("playScene");
	}


	public void changeDescription(string name)
	{
		DescriptionPanel.SetActive (true);
		GameObject.FindGameObjectWithTag ("descriptionText").GetComponent<Text> ().text = name;
	}

	public void drawPolygons()
	{
		int num_of_poly = description.Count;
		int start = 0;
		int sum = 0;
		for (int i = 0; i < num_of_poly; i++) {
			//GameObject obj = new GameObject ();
			//obj.AddComponent<LineRenderer> ();
			//LineRenderer lineRenderer = obj.GetComponent<LineRenderer> ();
			//lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
			//lineRenderer.SetWidth (0.1f, 0.1f);
			//lineRenderer.SetVertexCount (number_points[i]);
			int j;
			Vector2[] colliderPoints = new Vector2[number_points[i]];
			for ( j = start; j < number_points [i]+sum; j++) {
				colliderPoints [j - sum] = new Vector2 (float.Parse (x_cord [j]), float.Parse (y_cord [j]));
			//	lineRenderer.SetPosition (j-sum,colliderPoints [j - sum]);

			}
			start = j;
			sum += number_points [i];

			GameObject pc = new GameObject (description[i]);
			PolygonCollider2D polygon = pc.AddComponent<PolygonCollider2D>();
			polygon.gameObject.tag = "polygonCollider";
			polygon.SetPath (0, colliderPoints);
		}
	}

	public void showImage()
	{
		Sprite sprite = new Sprite ();
		sprite = mng.stringToSprite (imageString);
		ParentPanel.GetComponent<Image>().overrideSprite = sprite;

		Color c = ParentPanel.GetComponent<Image> ().color;
		c.a = 255;
		ParentPanel.GetComponent<Image> ().color = c;
	}

	public void parseXML(string filepath, List<string> result,string tagName){

		XmlDocument xmlDoc= new XmlDocument();
		if(File.Exists (filepath))
		{ 	
			XmlNodeList x;
			xmlDoc.Load( filepath );
			x = xmlDoc.GetElementsByTagName(tagName);
			Debug.Log (x);
			for (var i=0;i<x.Count;i++)
			{ 
				result.Add(x.Item(i).InnerText);
			}
		}
	}


	string imageString;
	List<string> description;
	List<int> number_points;
	List<string> x_cord;
	List<string> y_cord;


	public void printXMLData()
	{
		Debug.Log ("Image:" + imageString);
		foreach(string desc in description)
		Debug.Log ("Description:" + desc);
		for (int i = 0; i < number_points [0]; i++) {
			Debug.Log ("X:"+x_cord[i]+"     Y:"+y_cord[i]);
		}
	}


	public void Back()
	{
		SceneManager.LoadScene ("QuizList");
	}
	public void parseXMLFile(string filePath)
	{
		description = new List<string> ();
		number_points = new List<int> ();
		x_cord = new List<string>();
		y_cord = new List<string>();
		print("Reading XML File in " + filePath);

		// Read plain text XML file

			XmlDocument xmlDoc = new XmlDocument(); 
			xmlDoc.Load(filePath); 


			// First we will parse the profile data
			XmlNodeList rootNode = xmlDoc.GetElementsByTagName("ImageDescription");
			XmlNodeList root = rootNode[0].ChildNodes;

			foreach (XmlNode child in root)
			{
				
			if (child.Name == "image") {
				imageString = child.InnerText;
			} else if (child.Name == "polygon") {

				XmlNodeList childNodes = child.ChildNodes;
				foreach (XmlNode childNode in childNodes) {
					if (childNode.Name == "description") {					
						//Save in description array
						description.Add(childNode.InnerText);
					}
					else if (childNode.Name == "point") {
						XmlNodeList items = childNode.ChildNodes;
						int count = 0;
						foreach (XmlNode item in items) {
							//Debug.Log ("X cord is:" + item.Attributes ["x"].Value);
							x_cord.Add (item.Attributes ["x"].Value);
							y_cord.Add (item.Attributes ["y"].Value);
							count++;
						}
						number_points.Add (count);
					}
				}

			}


		}
	}


}
