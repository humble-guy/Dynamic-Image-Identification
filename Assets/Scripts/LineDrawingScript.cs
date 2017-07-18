using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;


public class LineDrawingScript : MonoBehaviour {
	public GameObject ParentSpritePanel;
	public GameObject obj;
	public GameObject warningPrefab,saveDescriptionPrefab;
	public Button undoButton, clearButton ,exitButton , addDescriptionButton ,saveDescriptionButton, imagePickerButton ,saveFileButton;
	LineRenderer lineRenderer;
	public GameObject point;

	private DisplayManager displayManager;

	List <myLine>AllLines;

	List<Vector3> pointsList;
	Vector2[] colliderPoints;
	Ray ray;
	RaycastHit hit;
	int count = 0;
	Vector3 startPos,endPos;
	bool gameStarted,descriptionState;
	public  bool imagePicked;
	Vector3 initialPoint;
	public GameObject imagePanel; 

	struct myLine
	{
		public Vector3 StartPoint;
		public Vector3 EndPoint;
	};
	ImageRendering ir;








	private ModalPanel modalPanel;


	private UnityAction myYesAction;
	private UnityAction myNoAction;
	private UnityAction myCancelAction;


	public GameObject popupPanelOnBack;

	public void onBackYes()
	{
	
		saveFile ();
		popupPanelOnBack.SetActive (false);
	}

	public void onBackNo()
	{
		ImageRendering.sprite = null;
		string oldFilePath = mng.Settings() + "/" + fileName+ ".qz";
		System.IO.File.Delete (oldFilePath);
		SceneManager.LoadScene ("StartScene");
	}


	void Awake () {
		modalPanel = ModalPanel.Instance ();
		displayManager = DisplayManager.Instance ();

		myYesAction = new UnityAction (TestYesFunction);
		myNoAction = new UnityAction (TestNoFunction);
		myCancelAction = new UnityAction (TestCancelFunction);
	}

	public void saveFile()
	{
		enablePopupMode ();
		descriptionText.placeholder.GetComponent<Text>().text = "Enter file name...";
		modalPanel.Choice ("SaveFileMode", SaveFileFunction, TestNoFunction, TestCancelFunction);
	}

	public void descriptionAddingStage()
	{
		undoButton.interactable = true;
		addDescriptionButton.interactable = true;
		clearButton.interactable = true;
	}

	public void SaveFileFunction()
	{
		if (descriptionText.text.Length > 0) {
			string oldFilePath = mng.Settings() + "/" + fileName+ ".qz";
			string newFilePath = mng.Settings() + "/" + descriptionText.text + ".qz";
			if (System.IO.File.Exists (newFilePath)) {
				displayManager.DisplayMessage ("File already exists!!!");
				disablePopupMode ();
			} else {
				System.IO.File.Copy (oldFilePath, newFilePath);
				System.IO.File.Delete (oldFilePath);
				ImageRendering.sprite = null;
				SceneManager.LoadScene ("StartScene");
			}
		} else {
			displayManager.DisplayMessage ("Please enter file name correctly!!!");
			disablePopupMode ();
		}
	}

	public void enablePopupMode()
	{
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="PopupLayer";
		temp = GameObject.FindGameObjectsWithTag ("spherePoint");
		foreach (GameObject g in temp)
			g.SetActive (false);
	}

	public void disablePopupMode()
	{
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="Default";

		foreach (GameObject g in temp)
			g.SetActive (true);
	}
	//  Send to the Modal Panel to set up the Buttons and Functions to call
	public void TestYNC () {
		addDescriptionState ();
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="PopupLayer";
		temp = GameObject.FindGameObjectsWithTag ("spherePoint");
		foreach (GameObject g in temp)
			g.SetActive (false);
		descriptionText.placeholder.GetComponent<Text>().text = "Enter Description...";
		modalPanel.Choice ("Do you want to spawn this sphere?", TestYesFunction, TestNoFunction, TestCancelFunction);


		//      modalPanel.Choice ("Would you like a poke in the eye?\nHow about with a sharp stick?", myYesAction, myNoAction, myCancelAction);
	}

	//  These are wrapped into UnityActions
	void TestYesFunction () {
		if (descriptionText.text.Length > 0)
			saveDescription ();
		else {
			descriptionAddingStage ();
			displayManager.DisplayMessage ("Please enter description!!!");
			GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="Default";

			foreach (GameObject g in temp)
				g.SetActive (true);
		}

	}

	void TestNoFunction () {
		descriptionText.text = string.Empty;
		//displayManager.DisplayMessage ("No way, José!");
	}

	void TestCancelFunction () {
		//Destroy (GameObject.FindGameObjectWithTag ("descriptionPanel"));
		//displayManager.DisplayMessage ("I give up!");
		descriptionAddingStage ();
		descriptionText.text = string.Empty;
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="Default";

		foreach (GameObject g in temp)
			g.SetActive (true);
	}











	string fileName;
	// Use this for initialization
	void Start () {
		AllLines = new List<myLine>();
		Debug.Log ("Start Script Executed");
		//Camera.main.aspect = 480f / 800f;
		fileName ="temp";
		ir = new ImageRendering ();
		displayManager = DisplayManager.Instance ();
		imagePanel = GameObject.Find ("imagePanel");
		pointsList = new List<Vector3> ();
		gameStarted = false;
		imagePicked = false;
		descriptionState = false;
		sceneStartState ();
		mng = new ManageXML ();
		String filePath = mng.Settings() + "/" +fileName+".qz";
		if (File.Exists (filePath)) {
			int count = 1;
			while (File.Exists (filePath)) {
				fileName = "temp" + count;
				filePath = mng.Settings() + "/" +fileName+".qz";
				count++;
			}
		}

		Debug.Log ("Filename Decided:" + fileName);
		//initialiseLineRenderer ();
	}


	public void imageIsPicked()
	{
		imagePicked = true;
		controlButton.GetComponentInChildren<Text> ().text = "Draw Area";
		undoButton.interactable = false;
		clearButton.interactable = false;
		addDescriptionButton.interactable = false;
		controlButton.interactable = false;
		saveFileButton.interactable = true;
		imagePickerButton.interactable = false;
		controlButton.interactable = true;
		exitButton.interactable = true;
		count = 0;
	}

	public GameObject alertPopupPrefab;
	GameObject alertPopup;
	public void giveAlert(string message){
		Debug.Log ("Inside alert");
		alertPopup = Instantiate(alertPopupPrefab);
		//alertPopup.transform.SetParent(gameObject.transform);
		alertPopup.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform);
		alertPopup.transform.position = gameObject.transform.position;
		alertPopup.transform.localScale = new Vector3(1,1,1);
		alertPopup.GetComponentInChildren<Text>().text = message;
		//alertPopup.GetComponentInChildren<Button> ().onClick.AddListener (() => destroyPopup ());
	}

	public void destroyPopup()
	{
		Destroy (alertPopup);
	}

	/*private void addColliderToLine()
	{
		go = new GameObject ("Collider");
		go.AddComponent<collisionScript> ();
		Rigidbody2D body = go.AddComponent<Rigidbody2D> ();
		body.gravityScale = 0;
		BoxCollider2D col = go.AddComponent<BoxCollider2D> ();
		go.tag = "line";
		col.transform.parent = lineRenderer.transform; // Collider is added as child object of line
		float lineLength = Vector3.Distance (startPos, endPos); // length of line
		col.size = new Vector3 (lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
		Vector3 midPoint = (startPos + endPos)/2;
		col.transform.position = midPoint; // setting position of collider object
		// Following lines calculate the angle between startPos and endPos
		float angle = (Mathf.Abs (startPos.y - endPos.y) / Mathf.Abs (startPos.x - endPos.x));
		if((startPos.y<endPos.y && startPos.x>endPos.x) || (endPos.y<startPos.y && endPos.x>startPos.x))
		{
			angle*=-1;
		}
		angle = Mathf.Rad2Deg * Mathf.Atan (angle);
		col.transform.Rotate (0, 0, angle);
	}*/


	/*
		Line Collision Detection Functions
	*/

	private bool isLineCollide ()
	{
		if (pointsList.Count < 2)
			return false;
		int TotalLines = pointsList.Count - 1;
		myLine[] lines = new myLine[TotalLines];
		if (TotalLines > 1) {
			for (int i=0; i<TotalLines; i++) {
				lines [i].StartPoint = (Vector3)pointsList [i];
				lines [i].EndPoint = (Vector3)pointsList [i + 1];
			}
		}
		for (int i=0; i<TotalLines-2; i++) {
			myLine currentLine;
			currentLine.StartPoint = (Vector3)pointsList [pointsList.Count - 2];
			currentLine.EndPoint = (Vector3)pointsList [pointsList.Count - 1];
			if (doIntersect (lines [i].StartPoint,lines[i].EndPoint, currentLine.StartPoint,currentLine.EndPoint))
				return true;
		}
		return false;
	}

	bool doIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2){
		float m1 = getSlope(p1, q1);
		float m2 = getSlope(p2, q2);
		float c1 = p1.y - m1*p1.x;
		float c2 = p2.y - m2*p2.x;
		if(m1 == m2){
			if(c1 == c2){
				if(liesBetween(p1, p2, q2) || liesBetween(q1, p2, q2))
					return true;
				return false;
			}
			return false;
		}
		float x = (c2-c1)/(m1-m2);
		float y = (m1*c2 - m2*c1)/(m1 - m2);

		if( liesBetween(new Vector3(x,y,0), p1, q1) && liesBetween(new Vector3(x,y,0), p2, q2) )
			return true;
		return false;

	}

	float getSlope(Vector3 p1, Vector3 p2){
		return (p2.y - p1.y)/(p2.x - p1.x);
	}

	bool liesBetween(Vector3 p, Vector3 p1, Vector3 p2){
		if(((p.x<=p2.x && p.x>=p1.x)||(p.x<=p1.x && p.x>=p2.x)) && ((p.y<=p2.y && p.y>=p1.y)||(p.y<=p1.y && p.y>=p2.y)))
			return true;
		return false;
	}


	/* End line collision detection */
	private bool isPoylogonCollide ()
	{
		if (pointsList.Count < 2)
			return false;
		int TotalLines = pointsList.Count - 1;
		myLine[] lines = new myLine[TotalLines];

		if (TotalLines > 1) {
			for (int i=0; i<TotalLines; i++) {
				lines [i].StartPoint = (Vector3)pointsList [i];
				lines [i].EndPoint = (Vector3)pointsList [i + 1];
			}
		}
		for (int i = 0; i < TotalLines; i++) {
			for (int j = 0; j < AllLines.Count; j++) {
				if (doIntersect (lines [i].StartPoint, lines [i].EndPoint, AllLines [j].StartPoint, AllLines [j].EndPoint))
					return true;
			}
		}
		return false;
	}

	public void initialiseLineRenderer()
	{
		obj = new GameObject ();
		obj.AddComponent<LineRenderer> ();
		lineRenderer = obj.GetComponent<LineRenderer> ();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetWidth (0.1f, 0.1f);
	}

	/*Variable to maintain game states:-
		gameStarted : for start button
		checkClickedArea : to identify whether polygon is clicked
	*/


	GameObject go;
	GameObject newPoint;
	Vector3 pos;
	// Update is called once per frame
	void Update () {
		if (ImageRendering.sprite == null) {
			//Debug.Log ("Image not picked till now...");
		} else if (!gameStarted && !descriptionState) {
			imageIsPicked ();
			//checkClickedArea ();
		}
		//Start making lines only if user clicked on start and the area clicked is the image panel
		else if (gameStarted && RectTransformUtility.RectangleContainsScreenPoint(
			imagePanel.GetComponent<RectTransform>(), 
			Input.mousePosition, 
			Camera.main)) {
			if (Input.GetMouseButtonDown (0)) {
				//Creating a small sphere to denote a point

				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Debug.Log (mousePos);
				pos = new Vector3 (mousePos.x, mousePos.y, 0);
				pointsList.Add (pos);
				// in case line collides the added point is removed
				if (!isLineCollide ()) {

					//Make a sphere
					newPoint = Instantiate (point, pos, Quaternion.identity) as GameObject;
					newPoint.GetComponent<Transform> ().localScale = new Vector3 (0.25f, 0.25f, 0.25f);
					//Transform t = newPoint.GetComponent<Transform>();
					//t.transform.position.z = 0;
					//newPoint.GetComponent<Transform> () = t;
					//newPoint.transform = t;
					//newPoint.transform.SetParent (GameObject.FindGameObjectWithTag ("canvas").transform);
					//Add a new point to line renderer
					count++;
					lineRenderer.SetVertexCount (count);
					int index = count - 1;
					lineRenderer.SetPosition (index, pos);
					if (count < 3)
						controlButton.interactable = false;
					else
						controlButton.interactable = true;
					if (count == 1) {
						startPos = pos;
						initialPoint = pos;
					} else if (count == 2) {
						endPos = pos;
						//addColliderToLine ();
					} else if (count > 2) {
						startPos = endPos;
						endPos = pos;
						//addColliderToLine ();
					}
				} else {
					//show a fading popup that the line was intersecting
					showLineIntersectingWarning();
					pointsList.RemoveAt (pointsList.Count - 1);
					Debug.Log ("Cannot Add the line is colliding");
				}

			}
		}
	}




	public void showLineIntersectingWarning()
	{
		displayManager.DisplayMessage ("The lines are intersecting");
	}


	public void startDrawing()
	{
		gameStarted = true;
		initialiseLineRenderer ();
		lineDrawingState ();
	}

	public void lineDrawingState()
	{
		exitButton.interactable = true;
		addDescriptionButton.interactable = false;
		saveFileButton.interactable = false;
		controlButton.interactable = true;
		undoButton.interactable = true;
		clearButton.interactable = true;
		controlButton.GetComponentInChildren<Text> ().text = "Stop Drawing";
	}

	public void addDescriptionState()
	{
		controlButton.interactable = false;
		undoButton.interactable = false;
		saveFileButton.interactable = false;
		addDescriptionButton.interactable = true;
		exitButton.interactable = false;
		clearButton.interactable = false;
		addDescriptionButton.interactable = false;
		Debug.Log ("Inside add description state");
	}

	public void undo()
	{
		//Destroy sphere
		addDescriptionButton.interactable =false;
		controlButton.interactable = true;
		if (count > 1) {
			GameObject[] spheres = GameObject.FindGameObjectsWithTag ("spherePoint");
			int total = spheres.Length;
			if (controlButton.GetComponentInChildren<Text> ().text == "Stop Drawing")
				Destroy (spheres [total - 1]);
			else
				Destroy (pc);
			//Destroy (go);   // in case if collider is used 
			//Destroy (newPoint);
			lineRenderer.SetVertexCount (count - 1);
			count--;
			pointsList.RemoveAt (pointsList.Count - 1);
			lineDrawingState ();
			if (count < 3)
				controlButton.interactable = false;
		} else {
			GameObject[] spheres = GameObject.FindGameObjectsWithTag ("spherePoint");
			int total = spheres.Length;
			if (controlButton.GetComponentInChildren<Text> ().text == "Stop Drawing")
				Destroy (spheres [total - 1]);
			Destroy (obj);
			imageIsPicked ();
		}

	}

	public void RemoveAll()
	{
		Destroy (obj);
		Destroy (pc);
		lineRenderer.SetVertexCount (0);
		int temp = count;
		count = 0;
		pointsList.Clear ();
		/*GameObject[] points = GameObject.FindGameObjectsWithTag ("line");
		foreach (GameObject child in points)
			Destroy (child);*/
		//Remove all spheres
		GameObject [] spheres = GameObject.FindGameObjectsWithTag("spherePoint");
		int length = spheres.Length;
		int i = length - temp+1;
		for(;i<length;i++)
			Destroy (spheres[i]);
		//lineDrawingState ();
		imageIsPicked ();

	}

	public void stopDrawing()
	{
		controlButton.interactable = false;
		count++;
		lineRenderer.SetVertexCount (count);
		lineRenderer.SetPosition (count-1, initialPoint);
		int i = 0;
		colliderPoints = new Vector2[pointsList.Count+1];
		foreach (Vector3 v in pointsList) {
			colliderPoints [i++] = new Vector2 (v.x,v.y);
		}
		colliderPoints [i] = new Vector2 (initialPoint.x, initialPoint.y);
		gameStarted = false;
		createPolygonCollider ();

	}

	public Button controlButton;

	public void lineDrawingController()
	{
		Text buttonText = controlButton.GetComponentInChildren<Text> ();
		if (buttonText.text == "Draw Area") {
			startDrawing ();
			buttonText.text = "Stop Drawing";
			//controlButton.GetComponentInChildren<Text> ().text = "Stop Drawing";
		} else {
			stopDrawing ();
			buttonText.text = "Draw Area";
			TestYNC ();
			//controlButton.GetComponentInChildren<Text> () = buttonText;
		}
	}
	RaycastHit2D hitPolygon;
	GameObject pc;
	public void createPolygonCollider()
	{
		
		pc = new GameObject ("polygonCollider");
		PolygonCollider2D polygon = pc.AddComponent<PolygonCollider2D>();
		polygon.gameObject.tag = "temp";
		polygon.SetPath (0, colliderPoints);
		GameObject[] polygons = GameObject.FindGameObjectsWithTag ("polygonCollider");
		Debug.Log ("Currently No. of pc:" + polygons.Length);
		bool isCollision = false;
		if(polygons.Length!=0){

			if (isPoylogonCollide()) {
				//show a popup that overlapping
				displayManager.DisplayMessage ("The polygons were overlapping");
				isCollision = true;
				RemoveAll ();
			}
		}


		polygon.gameObject.tag = "polygonCollider";
		if (!isCollision) {
			//addDescriptionState ();
			addDescriptionButton.interactable = true;
			descriptionState = true;
		}
		/*pc.AddComponent<RaycastHit2D>();
		if (hitPolygon) {
			if (hitPolygon.collider.gameObject.name == "polygonCollider")
				Debug.Log ("Overlap between polygon colliders");
		}*/
	}




	public void checkClickedArea()
	{
		Ray ray;
		RaycastHit2D hit;
		if (Input.GetMouseButtonDown (0)) {
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit) {
				if (hit.collider.gameObject.name == "polygonCollider")
					Debug.Log ("PC clicked");
			}
		}
	}
	GameObject[] temp;
	public void AddDescription()
	{
		//This function will be triggerd on button click
		//addDescriptionState();
		GameObject go = Instantiate(saveDescriptionPrefab);
		go.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform);
		go.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, -2475f);
		//go.transform.position.z = -2475f;
		go.transform.localScale = new Vector3(1, 1, 1);
		//Destroy (go, 500);
		//Transform t =  go.GetComponent<Transform>();
		Vector3 newPos = go.GetComponent<RectTransform> ().localPosition;
		newPos.z = -2475f;
		go.GetComponent<RectTransform> ().localPosition = newPos;
		saveDescriptionButton = go.GetComponentInChildren<Button> ();
		saveDescriptionButton.onClick.AddListener(() => saveDescription());
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="PopupLayer";
		temp = GameObject.FindGameObjectsWithTag ("spherePoint");
		foreach (GameObject g in temp)
			g.SetActive (false);
		//lineRenderer.sortingLayerName="Background";
		//lineRenderer.sortingOrder (-1);
		//t.transform.position.z = 10f;
		//go.GetComponent<Transform> () = t;
	}

	String description;
	public void saveDescription()
	{
		int TotalLines = pointsList.Count - 1;
		myLine line;
		if (TotalLines > 1) {
			for (int i=0; i<TotalLines; i++) {
				line.StartPoint = (Vector3)pointsList [i];
				line.EndPoint = (Vector3)pointsList [i + 1];
				AllLines.Add(line);
			}
		}
		//write description to xml
		writeToXML();
		GameObject.FindGameObjectWithTag ("canvas").GetComponent<Canvas> ().sortingLayerName ="Default";

		foreach (GameObject g in temp)
			g.SetActive (true);
		Destroy (GameObject.FindGameObjectWithTag ("descriptionPanel"));
		controlButton.interactable = true;
		resetLineRenderingParameters ();
		descriptionState = false;
		undoButton.interactable = false;
		clearButton.interactable = false;
		saveFileButton.interactable = true;
	}

	public void back(){
		//ImageRendering.sprite = null;
		//string oldFilePath = mng.Settings() + "/" + fileName+ ".qz";
		//System.IO.File.Delete (oldFilePath);
		//SceneManager.LoadScene ("StartScene");
		popupPanelOnBack.SetActive(true);
	}
	public void resetLineRenderingParameters()
	{
		count = 0;
		pointsList.Clear ();
	}
	ManageXML mng ;
	public InputField descriptionText;
	public void writeToXML()
	{
		 
		description = descriptionText.text;
		//description = "fjfkr";
		mng.createInfoFile (colliderPoints,description,"polygon",mng.spriteToString(ImageRendering.sprite),fileName);

		descriptionText.text = string.Empty;
		saveFileButton.interactable = true;
	}


	public void sceneStartState()
	{
		undoButton.interactable = false;
		clearButton.interactable = false;
		addDescriptionButton.interactable = false;
		controlButton.interactable = false;
		exitButton.interactable = true;
		saveFileButton.interactable = false;
		imagePickerButton.interactable = true;
	}
}
