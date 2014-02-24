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
    float mNextChar = 0f;

	void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
		}

        if( mOffset < mText.Length )
        {
            if (mNextChar < RealTime.time)
            {
                float delay = 1f / charsPerSecond;

                char c = mText[mOffset];

                if (c == '.' || c == '\n' || c == '!' || c == '?') delay *= 4f;

                if (c == '[')
                {
                    if (mText[mOffset + 1] == '-')
                        mOffset += 2;
                    else
                        mOffset += 7;
                }

                mNextChar = RealTime.time + delay;
                mLabel.text = mText.Substring(0, ++mOffset);
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
		mOffset = 0;
    }



    public void ImmediatelyCompleteAnime()
    {
        if (mText == null) return;
        mOffset = mText.Length;
        mLabel.text = mText;
    }




}