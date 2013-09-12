using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 
/// </summary>
public class UICheckboxActivate : MonoBehaviour
{
    public void OnActivate(bool isActivated)
    {
        if (OnSelected == null) return;
        OnSelected(this, new ActivateArgs{ UserState = this.UserState, IsSelected = isActivated });
    }
    /// <summary>
    /// when is selected
    /// </summary>
    public event EventHandler<ActivateArgs> OnSelected;
    /// <summary>
    /// use state
    /// </summary>
    public object UserState { set; get; }
    /// <summary>
    /// 
    /// </summary>
    public class ActivateArgs : EventArgs
    {
        public bool IsSelected { set; get; }
        public object UserState { set; get; }
    }
}

