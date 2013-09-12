
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




public class UIOnInput : MonoBehaviour
{
    public System.Action<string> Input;


    void OnInput( string text )
    {
        if( null != Input )
        {
            Input( text );
        }

    }
}

//~ void OnHover (bool isOver) 每 Sent out when the mouse hovers over the collider or moves away from it. Not sent on touch-based devices.
//~ void OnPress (bool isDown) 每 Sent when a mouse button (or touch event) gets pressed over the collider.
//~ void OnSelect (bool selected) 每 Sent when a mouse button or touch event gets released on the same collider as OnPress.
//~ void OnClick() 〞 Same conditions as OnSelect, with the added check to make sure the mouse or touch event hasn＊t turned into a drag event.
//~ void OnDrag (Vector2 delta) 每 Sent when the mouse or touch moves past a specific threshold while pressed down.
//~ void OnDrop (GameObject drag) 每 Sent out when the mouse or touch get released on a different collider than the one that triggered OnDrag. The passed parameter is the game object that received the OnDrag event.
//~ void OnInput (string text) 每 Sent to the same collider that received OnSelect(true) message after typing something. You likely won＊t need this, but it＊s used by UIInput
//~ void OnTooltip (bool show) 每 Sent after the mouse hovers over a collider without moving for longer than tooltipDelay, and when the tooltip should be hidden. Not sent on touch-based devices.
