using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMouseHover : MonoBehaviour 
{
    void OnHover( bool b)
    {
        if( Hover != null )
        {
            Hover( this , new HoverArgs { UserState = this.UserState, IsEnter = b } );
        }
    }

    public object UserState { set; get; }

    /// <summary>
    /// event onclick
    /// </summary>
    public event EventHandler<HoverArgs> Hover;
    /// <summary>
    /// the click args
    /// </summary>
    public class HoverArgs : EventArgs
    {
        /// <summary>
        /// User state
        /// </summary>
        public object UserState { set; get; }
        public bool IsEnter { set; get; }

    }
}
