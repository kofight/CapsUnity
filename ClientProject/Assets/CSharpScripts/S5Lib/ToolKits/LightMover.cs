using UnityEngine;
using System.Collections;

public class LightMover : MonoBehaviour {
	
	public Vector3 WindDirction = new Vector3(10, 10, 10);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    transform.position += WindDirction * Time.deltaTime;
	}
}
