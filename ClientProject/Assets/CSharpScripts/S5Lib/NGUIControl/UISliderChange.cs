
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




public class UISliderChange : MonoBehaviour
{

    void OnSliderChange( float value )
    {
        if( OnChange != null )
		{
            OnChange( this , new ChangeEventArgs { UserState = this.UserState , Value = value } );
		}
    }
	
	public object UserState{set;get;}
	
	/// <summary>
	/// event onclick
	/// </summary>
	public event EventHandler<ChangeEventArgs> OnChange;
	/// <summary>
	/// the click args
	/// </summary>
	public class ChangeEventArgs:EventArgs
	{
		/// <summary>
		/// User state
		/// </summary>
		public object UserState{set;get;}
        public float Value { get; set; }
	}
}

