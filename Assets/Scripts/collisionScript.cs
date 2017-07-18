using UnityEngine;
using System.Collections;

public class collisionScript : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col)
	{
		//Just for example
		if (col.gameObject.tag == "line") {
			//LineDrawingScript.isColliding = true;
			Debug.Log ("Inside the collision script");
		}
	}
}
