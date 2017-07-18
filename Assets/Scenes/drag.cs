using UnityEngine;
using System.Collections;

public class drag : MonoBehaviour {

	public void OnDrag(){ transform.position = Input.mousePosition; }
}
