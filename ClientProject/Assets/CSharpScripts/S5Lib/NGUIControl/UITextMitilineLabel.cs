using UnityEngine;
using System.Collections;
using System;


[AddComponentMenu("NGUI/UI/UITextMitiLineLabel")]
public class UITextMitilineLabel : MonoBehaviour
{


    public enum DirectionType
    {
        Vertical,
        Horizontal
    }

    public enum Status
    {
        Stop,
        Begining,
        ShowText,
        MoveUp
    }

    // Use this for initialization
    void Start()
    {

    }

    private float delay = 0;
    public float lineDuation = 3f;
    public float moveEffectDuation = 0.5f;
    private float y;
    private Vector2? initPosition;


    public DirectionType direnction;

    public Status State { private set; get; }
    // Update is called once per frame
    void Update()
    {
        if (State == Status.Stop) return;
        if (direnction == DirectionType.Vertical)
            VerticalSroll();
        else if (direnction == DirectionType.Horizontal)
        {
            HorizontalSroll();
        }
    }

    private void HorizontalSroll()
    {
        var label = this.Label;
        if (label == null)
        {
            State = Status.Stop;
            return;
        }

        if (State == Status.Begining)
        {
            delay = Time.time;
            State = Status.ShowText;
            return;
        }

        if (Time.time - delay < lineDuation)
        {
            float x;

            float offsetX = ((Time.time - delay) / lineDuation) * (Width + fixWidth);
            x = initPosition.Value.x + Width - offsetX;

            label.gameObject.transform.LocalPositionX(x);
        }
        else
        {
            if (rotation)
            {
                delay = Time.time;
                return;
            }
            else
            {
                State = Status.Stop;
                if (CallWhenFinished != null)
                {
                    var Call = CallWhenFinished;
                    CallWhenFinished = null;
                    Call();
                }
                return;
            }
        }

    }

    private void VerticalSroll()
    {

        var label = this.Label;
        if (label == null)
        {
            State = Status.Stop;
            return;
        }

        if (State == Status.Begining)
        {
            //Debug.Log("State Begin");
            delay = Time.time;
            y = label.gameObject.transform.localPosition.y;
            State = Status.ShowText;
            return;
        }

        if (delay + lineDuation > Time.time)
        {
            //Debug.Log("State Show Text");
            return;
        }

        if (!rotation)
        {
            //end 
            if (y >= (initPosition.Value.y + (fixHeight - LineHeight)))
            {
                Debug.Log(string.Format("end {0} >= ({1} + ({2} - {3}) IS Call Not empty{4}", y, initPosition.Value.y, fixHeight, LineHeight, CallWhenFinished != null));
                State = Status.Stop;
                if (CallWhenFinished != null)
                {
                    var Call = CallWhenFinished;
                    CallWhenFinished = null;
                    Call();
                }

                return;
            }
        }


        if (delay + lineDuation + moveEffectDuation > Time.time)
        {
            // Debug.Log("State Move Up");
            State = Status.MoveUp;
            var offset = (delay + lineDuation + moveEffectDuation) - Time.time;
            var yOffset = LineHeight - LineHeight * (offset / moveEffectDuation);
            if (yOffset > LineHeight)
            {
                yOffset = LineHeight;
            }
            label.gameObject.transform.LocalPositionY(y + yOffset);
        }
        else
        {
            //Debug.Log(string.Format("{0} >= ({1} + ({2} - {3})",y ,y_init,fixHeight,LineHeight));
            //move end
            label.gameObject.transform.LocalPositionY(y + LineHeight);
            delay = Time.time;
            y = label.gameObject.transform.localPosition.y;
            if (rotation)
            {
                if (y >= (initPosition.Value.y + (fixHeight)))
                {
                    label.gameObject.transform.LocalPositionY(initPosition.Value.y - LineHeight);
                    y = label.gameObject.transform.localPosition.y;
                    delay = Time.time - lineDuation;
                }
            }
            State = Status.ShowText;
        }


    }

    /// <summary>
    /// Play text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lineDuration"></param>
    public void PlayText(string text, float lineDuration, Action callWhenFinished)
    {
        CallWhenFinished = callWhenFinished;
        this.text = text;

        var label = Label;
        if (label == null) return;
        if (initPosition != null)
        {
            Label.gameObject.transform.LocalPositionY(initPosition.Value.y);
            Label.gameObject.transform.LocalPositionX(initPosition.Value.x);
        }

        delay = 0f;
        y = 0;
        initPosition = new Vector2(label.gameObject.transform.localPosition.x, label.gameObject.transform.localPosition.y);
        LineHeight = label.gameObject.transform.localScale.y;

        lineDuation = lineDuration;
        label.pivot = UIWidget.Pivot.TopLeft;
        label.lineWidth = this.direnction == DirectionType.Vertical ? Width : 0;
        label.text = text;
        this.fixHeight = label.Height();
        this.fixWidth = label.Width();
        UIPanel panel = Panel;
        if (panel == null) return;
        panel.clipping = UIDrawCall.Clipping.SoftClip;
        panel.clipSoftness = new Vector2(0, 0);
        panel.clipRange = new Vector4(Width / 2f, -LineHeight / 2f, Width, LineHeight);
        State = Status.Begining;
        Update();
        Update();

    }

    public UILabel Label
    {
        get
        {
            var uiLabel = this.gameObject.transform.FindChild<UILabel>("Label");
            if (uiLabel == null) return null;
            return uiLabel;

        }
    }

    public UIPanel Panel
    {
        get
        {
            var panel = this.gameObject.transform.GetComponent<UIPanel>();
            if (panel == null)
            {
                return this.gameObject.AddComponent<UIPanel>();
            }
            else
            {
                return panel; ;
            }

        }
    }

    /// <summary>
    /// the width
    /// </summary>
    public int Width;
    /// <summary>
    /// the height of line
    /// </summary>
    public float LineHeight;
    private float fixHeight;

    private float fixWidth;
    /// <summary>
    /// text
    /// </summary>
    public string text;
    /// <summary>
    /// rotation
    /// </summary>
    public bool rotation;
    /// <summary>
    /// call when it finished
    /// </summary>
    private Action CallWhenFinished;
}
