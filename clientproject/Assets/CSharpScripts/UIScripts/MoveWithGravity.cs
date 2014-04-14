using UnityEngine;
using System;
using System.Collections;

public class MoveWithGravity : MonoBehaviour {

    public float MoveSpeed = 100.0f;                //每秒多少个像素
    public float Width = 100;
    public float Up = 100;
    public float Down = 100;

	// Use this for initialization
	void Start () {
        UISprite sprite = GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.width = sprite.width + (int)Width * 2;
            sprite.height = sprite.height + (int)(Up + Down);
        }
	}
	
	// Update is called once per frame
	void Update () {
        float x = Mathf.Lerp(transform.localPosition.x, -Input.acceleration.x * Width, Time.deltaTime * 3);
        float y = Mathf.Lerp(transform.localPosition.y, -Input.acceleration.y * Up, Time.deltaTime * 3);
        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
	}
}
