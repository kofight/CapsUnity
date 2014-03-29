using UnityEngine;
using System.Collections;

public class PressReceiver : MonoBehaviour {
	
	public UIWindow.WindowEffectFinished OnPressFunc;
	
	public void OnPress (bool isPressed)
	{
		if(OnPressFunc != null)
		{
			OnPressFunc();
		}
	}
}
