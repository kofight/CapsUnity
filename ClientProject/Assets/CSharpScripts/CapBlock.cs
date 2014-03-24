using UnityEngine;
using System.Collections;

public enum TBlockColor
{
    EColor_None,				//0尚未分配颜色
    EColor_Purple,              //1紫
    EColor_Green,               //2绿
    EColor_Blue,                //3蓝
    EColor_Yellow,              //4黄
    EColor_Red,                 //5红
    EColor_Orange,              //6橙
    EColor_Cyan,                //7青

    EColor_Nut1,                  //坚果1
    EColor_Nut2,                  //坚果2
};

public enum TSpecialBlock
{
    ESpecial_Normal,			    //0普通块
    ESpecial_NormalPlus5,			//1普通块加5时间
    ESpecial_EatLineDir0,			//2一次消一线，方向0
    ESpecial_EatLineDir1,			//3一次消一线，方向1
    ESpecial_EatLineDir2,			//4一次消一线，方向2
    ESpecial_Bomb,					//5一次消周围9个
    ESpecial_EatAColor,				//6一次消一个颜色
};

public enum BlockState
{
    Normal,                         //普通状态（静止状态）
    Locked,                         //被锁状态
    Moving,                         //移动状态
    MovingEnd,                      //移动结束状态
    Eating,                         //吃块状态
}

public class CapBlock  
{
    public BlockState CurState = BlockState.Normal;         //当前状态

    public void RefreshBlockSprite(int flag)
    {
        if (m_blockSprite == null)
        {
            return;
        }

        if ((flag & (int)GridFlag.Cage) > 0)
        {
            CurState = BlockState.Locked;
        }

        switch (special)
        {
            case TSpecialBlock.ESpecial_Normal:
                {
                    m_blockSprite.spriteName = "Item" + (int)(color - TBlockColor.EColor_None);
                }
                break;
            case TSpecialBlock.ESpecial_NormalPlus5:
                {
                    m_blockSprite.spriteName = "TimeAdded" + (int)(color - TBlockColor.EColor_None);
                }
                break;
            case TSpecialBlock.ESpecial_EatLineDir0:
                m_blockSprite.spriteName = "Line" + (int)(color - TBlockColor.EColor_None) + "_3";
                break;
            case TSpecialBlock.ESpecial_EatLineDir1:
                m_blockSprite.spriteName = "Line" + (int)(color - TBlockColor.EColor_None) + "_1";
                break;
            case TSpecialBlock.ESpecial_EatLineDir2:
                m_blockSprite.spriteName = "Line" + (int)(color - TBlockColor.EColor_None) + "_2";
                break;
            case TSpecialBlock.ESpecial_Bomb:
                m_blockSprite.spriteName = "Bomb" + (int)(color - TBlockColor.EColor_None);
                break;
            case TSpecialBlock.ESpecial_EatAColor:
                m_blockSprite.spriteName = "Rainbow";
                break;
            default:
                break;
        }
        if (m_addColorSprite != null)
		{
            m_addColorSprite.spriteName = m_blockSprite.spriteName + "Additive";
		}

        int k = 0;
        CollectIndex = -1;
        for (; k < 3; ++k)
        {
            if (GlobalVars.CurStageData.CollectCount[k] > 0)
            {
                if (GlobalVars.CurStageData.CollectSpecial[k] == special)
                {
                    if (special == TSpecialBlock.ESpecial_EatAColor || GlobalVars.CurStageData.CollectColors[k] == color)
                    {
                        CollectIndex = k;
                        break;
                    }
                }
            }
        }
    }

    public TBlockColor color;							//颜色

    public int x_move;
    public int y_move;

    public float m_eatStartTime;                //开始消失的时间

    public Position droppingFrom;               //从某个点掉落过来
    public float DropingStartTime;              //下落的开始时间

    public TSpecialBlock special;				//特殊功能块

    public bool EatEffectPlayed;                //吃块特效是否已经播放了？

    public bool EatProgressAdded;               //是否已经加过分数了（防止重复加分数）

    public float EatDelay;
    public string EatAnimationName;             //消块的动画名称
    public string EatEffectName;             //消块的动画名称
    public AudioEnum EatAudio;               //消块的声音
    public float EatDuration;                   //消块的时长

    public float m_dropDownStartTime;                  //下落开始时间，用来停止下落动画
    public long m_resortEffectStartTime;              //重排特效开始时间

    public Animation m_animation;
    public UISprite m_blockSprite;		//精灵动画
    public UISprite m_shadowSprite;		//影子精灵动画，用来在传送门另一端表现另一半
    public Transform m_blockTransform;         //
	public UISprite m_addColorSprite;		//

    public static int EatingBlockCount = 0;     //正在消除的块的数量
    public static int DropingBlockCount = 0;    //下落中的块的数量

    public TweenScale m_tweenScale;
    public TweenPosition m_tweenPosition;
    public TweenAlpha m_tweenAlpha;

    public int CollectIndex = -1;               //改块是搜集的第几个目标

    public void Eat(float delay = 0)							//吃掉这个块
	{
        if (CurState == BlockState.Eating)
        {
			if(m_eatStartTime > Timer.GetRealTimeSinceStartUp() + delay)
				m_eatStartTime = Timer.GetRealTimeSinceStartUp() + delay;
            return;
        }
		if (CurState == BlockState.MovingEnd)
        {
            --DropingBlockCount;
        }
        CurState = BlockState.Eating;
        m_eatStartTime = Timer.GetRealTimeSinceStartUp() + delay;
        ++EatingBlockCount;
        EatEffectPlayed = false;
	}

    public bool IsEating()
	{
        return CurState == BlockState.Eating;
	}

    public bool IsDroping()
    {
        return CurState == BlockState.Moving || CurState == BlockState.MovingEnd;
    }

    public bool IsDropDownAble()
    {
        return CurState == BlockState.Normal || CurState == BlockState.MovingEnd;
    }

    public void Reset()
	{
        m_blockTransform.localScale = Vector3.one;
        m_blockSprite.transform.localScale = Vector3.one;
        m_blockSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        if (m_tweenPosition.enabled)
        {
            m_blockSprite.depth = 2;
            m_tweenPosition.enabled = false;
            m_tweenScale.enabled = false;
            m_tweenAlpha.enabled = false;
        }
		x_move = 0;
		y_move = 0;
		special = TSpecialBlock.ESpecial_Normal;
        CurState = BlockState.Normal;
        if (IsDroping())
        {
            --DropingBlockCount;
        }
        m_eatStartTime = 0;
	}

    public bool SelectAble()
    {
		if (CurState != BlockState.Normal)
		{
			return false;
		}
		return true;
	}

	public CapBlock()
    {
        //创建新Sprite
        GameObject capObj = GameObject.Find("CapInstance");
        GameObject newObj = GameObject.Instantiate(capObj) as GameObject;
        newObj.SetActive(false);

        newObj.transform.parent = capObj.transform.parent;
        newObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        m_blockTransform = newObj.transform;
        m_blockSprite = UIToolkits.FindComponent<UISprite>(m_blockTransform);
        m_animation = UIToolkits.FindComponent<Animation>(m_blockTransform);
        m_addColorSprite = m_blockSprite.transform.Find("AddColor").GetComponent<UISprite>();

        m_blockSprite.transform.localScale = Vector3.one;
        m_blockSprite.transform.localPosition = Vector3.zero;

        m_tweenPosition = m_blockTransform.GetComponent<TweenPosition>();
        m_tweenScale = m_blockTransform.GetComponent<TweenScale>();
        m_tweenAlpha = m_blockSprite.transform.GetComponent<TweenAlpha>();
    }
}
