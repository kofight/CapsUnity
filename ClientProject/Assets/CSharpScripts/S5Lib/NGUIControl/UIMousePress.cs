
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




public class UIMousePress : MonoBehaviour
{
    void OnPress( bool isDown )
    {
        if( Press != null )
        {
            Press( this , new ClickArgs { UserState = this.UserState , IsDown = isDown } );
        }
    }

    public object UserState { set; get; }

    /// <summary>
    /// event onclick
    /// </summary>
    public event EventHandler<ClickArgs> Press;
    /// <summary>
    /// the click args
    /// </summary>
    public class ClickArgs : EventArgs
    {
        /// <summary>
        /// User state
        /// </summary>
        public object UserState { set; get; }

        public bool IsDown { get; set; }
    }

}

