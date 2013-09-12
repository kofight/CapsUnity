using UnityEngine;

/// <summary>
/// Trivial script that fills the label's contents gradually, as if someone was typing.
/// </summary>

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Aoc/Typewriter Effect")]
public class AocTypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 320;
    
	UILabel mLabel;
	string mText = string.Empty;
	int mOffset = 0;

    float delay;
    float lastTime = 0.0f;

	void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
		}

        if( mOffset < mText.Length )
        {
            if( lastTime + delay < Time.time )
            {
                int Count = (int)( ( Time.time - lastTime ) / delay );

                if( mOffset + Count < mText.Length )
                {
                    mOffset += Count;
                    mLabel.text = mText.Substring( 0 , mOffset );
                }
                else
                {
                    mOffset = mText.Length;
                    mLabel.text = mText;
                }
                lastTime = Time.time;
            }
        }
        else
        {
            if( null != CallWhenFinished )
            {
                CallWhenFinished();
                CallWhenFinished = null;
            }

            this.enabled = false;
        }
	}

    System.Action CallWhenFinished;

    public void Play( string str , System.Action action )
    {
        mText = str;
        enabled = true;
        CallWhenFinished = action;
        
        charsPerSecond = Mathf.Max( 1 , charsPerSecond );
        delay = 1f / charsPerSecond;
		mOffset = 0;
        lastTime = Time.time;
    }



    public void ImmediatelyCompleteAnime()
    {
        if (mText == null) return;
        mOffset = mText.Length;
        mLabel.text = mText;
    }




}