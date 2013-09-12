//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;

/// <summary>
/// Simple checkbox functionality. If 'option' is enabled, checking this checkbox will uncheck all other checkboxes with the same parent.
/// </summary>

[AddComponentMenu("NGUI/Aoc/Checkbox")]
public class AocCheckbox : UICheckbox 
{
    void OnClick()
    {
    }

    void OnPress( bool selected )
    {
        //Debug.LogError( "OnPress = " + selected );
        if( selected && enabled )
            isChecked = !isChecked;
    }
}