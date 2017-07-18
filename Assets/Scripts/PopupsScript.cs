using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupsScript : MonoBehaviour {

	public GameObject alertPopupPrefab;
	GameObject alertPopup;
	public void giveAlert(string message){
		Debug.Log ("Inside alert");
		alertPopup = Instantiate(alertPopupPrefab);
		alertPopup.transform.SetParent(gameObject.transform);
		alertPopup.transform.position = gameObject.transform.position;
		alertPopup.transform.localScale = new Vector3(1,1,1);
		alertPopup.GetComponentInChildren<Text>().text = message;
		alertPopup.GetComponentInChildren<Button> ().onClick.AddListener (() => destroyPopup ());
	}

	public void destroyPopup()
	{
		Destroy (alertPopup);
	}

	void start()
	{
		
	}

	void update()
	{
		
	}
}
