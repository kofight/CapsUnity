
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




public class UIMouseClick : MonoBehaviour
{
    //public System.Action Click;
	void OnClick()
    {
		if( Click!=null)
		{
            if (UIWindowManager.Singleton != null &&
                UIWindowManager.Singleton.ClickProcesser != null &&
                UIWindowManager.Singleton.ClickProcesser(gameObject))
            {
                return;
            }
			Click(this, new ClickArgs{ UserState = this.UserState} );
		}
    }	
	
	public object UserState{set;get;}
	
	/// <summary>
	/// event onclick
	/// </summary>
	public event EventHandler<ClickArgs> Click;
	/// <summary>
	/// the click args
	/// </summary>
	public class ClickArgs:EventArgs
	{
		/// <summary>
		/// User state
		/// </summary>
		public object UserState{set;get;}
	}
}

