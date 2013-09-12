using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISelectionChange : MonoBehaviour 
{
    void OnSelectionChangeValue( string value )
    {
        if( OnChange != null )
        {
            OnChange( this , new ClickArgs { UserState = this.UserState , Value = value } );
        }
    }

    public object UserState { set; get; }

    /// <summary>
    /// event onclick
    /// </summary>
    public event EventHandler<ClickArgs> OnChange;
    /// <summary>
    /// the click args
    /// </summary>
    public class ClickArgs : EventArgs
    {
        /// <summary>
        /// User state
        /// </summary>
        public object UserState { set; get; }
        public string Value { get; set; }
    }
}
