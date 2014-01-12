using UnityEngine;
using System.Collections;

public enum TBlockColor
{
    EColor_None,				//尚未分配颜色
    EColor_White,
    EColor_Purple,
    EColor_Green,
    EColor_Red,
    EColor_Golden,
    EColor_Blue,
    EColor_Grey,

    EColor_Nut1,                  //
    EColor_Nut2,                  //
};

public enum TSpecialBlock
{
    ESpecial_Normal,			    //普通块
    ESpecial_NormalPlus5,			//普通块加5时间
    ESpecial_EatLineDir0,			//一次消一线，方向0
    ESpecial_EatLineDir1,			//一次消一线，方向1
    ESpecial_EatLineDir2,			//一次消一线，方向2
    ESpecial_Bomb,					//一次消周围9个
    ESpecial_EatAColor,				//一次消一个颜色
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
    }

    public TBlockColor color;							//颜色

    public int x_move;
    public int y_move;

    public float m_eatStartTime;                //开始消失的时间

    public Position droppingFrom;               //从某个点掉落过来
    public float DropingStartTime;              //下落的开始时间

    public TSpecialBlock special;				//特殊功能块

    public float EatDelay;
    public string EatAnimationName;             //消块的动画名称
    public string EatEffectName;             //消块的动画名称

    public float m_dropDownStartTime;                  //下落开始时间，用来停止下落动画

    public Animation m_animation;
    public UISprite m_blockSprite;		//精灵动画
    public UISprite m_shadowSprite;		//影子精灵动画，用来在传送门另一端表现另一半
    public Transform m_blockTransform;         //
	public UISprite m_addColorSprite;		//

    public static int EatingBlockCount = 0;     //正在消除的块的数量
    public static int DropingBlockCount = 0;    //下落中的块的数量

    public void Eat(float delay)							//吃掉这个块
	{
		if (CurState == BlockState.MovingEnd)
        {
            --DropingBlockCount;
        }
        CurState = BlockState.Eating;
        m_eatStartTime = Timer.GetRealTimeSinceStartUp() + delay;
        ++EatingBlockCount;
        EatDelay = delay;
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
        m_blockSprite.transform.localScale = Vector3.one;
        m_blockSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
    }
}
