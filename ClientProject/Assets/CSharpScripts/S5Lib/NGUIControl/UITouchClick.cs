
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




public class UITouchClick : MonoBehaviour
{
    Vector3 clickPosition;
    const int ClickRange = 10;

    void OnPress( bool isDown )
    {
        if( null != Press )
        {
            Press( this , new PressArgs { UserState = this.UserState , IsDown = isDown } );
        }

        if( isDown )
        {
            clickPosition = Input.mousePosition;
            return;
        }

        Vector3 pos = Input.mousePosition - clickPosition;

        if( ( Math.Abs( pos.x ) < ClickRange ) && ( Math.Abs( pos.y ) < ClickRange ) )
        {
            if( Click != null )
            {
				if (UIWindowManager.Singleton != null &&
		        UIWindowManager.Singleton.ClickProcesser != null &&
		        UIWindowManager.Singleton.ClickProcesser(gameObject))
		        {
		            return;
		        }
                Click( this , new ClickArgs { UserState = this.UserState } );
            }
        }
    }

    public object UserState { set; get; }

    /// <summary>
    /// event onclick
    /// </summary>
    public event EventHandler<ClickArgs> Click;

    public event EventHandler<PressArgs> Press;


    /// <summary>
    /// the click args
    /// </summary>
    public class ClickArgs : EventArgs
    {
        /// <summary>
        /// User state
        /// </summary>
        public object UserState { set; get; }
    }

    /// <summary>
    /// the click args
    /// </summary>
    public class PressArgs : EventArgs
    {
        /// <summary>
        /// User state
        /// </summary>
        public object UserState { set; get; }
        public bool IsDown { get; set; }
    }




}

