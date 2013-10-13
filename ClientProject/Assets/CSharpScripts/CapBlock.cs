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
    ESpecial_EatLineDir0,			//一次消一线，方向0
    ESpecial_EatLineDir1,			//一次消一线，方向1
    ESpecial_EatLineDir2,			//一次消一线，方向2
    ESpecial_Bomb,					//一次消周围9个
    ESpecial_Painter,				//刷子
    ESpecial_EatAColor,				//一次消一个颜色
};

public class CapBlock  
{
    public CapBlock Clone()
    {
        CapBlock newBlock = new CapBlock();
        newBlock.color = color;
        newBlock.x_move = x_move;
        newBlock.y_move = y_move;
        newBlock.m_bEating = m_bEating;
        newBlock.isDropping = isDropping;
        newBlock.isCanMove = isCanMove;
        newBlock.m_bNeedCheckEatLine = m_bNeedCheckEatLine;
        newBlock.special = special;
        newBlock.m_blockSprite = m_blockSprite;
        newBlock.m_blockTransform = m_blockTransform;
        return newBlock;
    }

    public void Assign(CapBlock block)
    {
        color = block.color;
        x_move = block.x_move;
        y_move = block.y_move;
        m_bEating = block.m_bEating;
        isDropping = block.isDropping;
        isCanMove = block.isCanMove;
        m_bNeedCheckEatLine = block.m_bNeedCheckEatLine;
        special = block.special;
        m_blockSprite = block.m_blockSprite;
        m_blockTransform = block.m_blockTransform;
    }

    public void RefreshBlockSprite(int flag)
    {
        if ((flag & (int)GridFlag.Stone) > 0)
        {
            m_blockSprite.spriteName = "Block1";
            isLocked = true;
			return;
        }

        if ((flag & (int)GridFlag.Chocolate) > 0)
        {
            m_blockSprite.spriteName = "Block2";
            isLocked = true;
			return;
        }

        if ((flag & (int)GridFlag.Cage) > 0)
        {
            m_blockSprite.spriteName = "Block3-" + (int)(color - TBlockColor.EColor_None);
            isLocked = true;
			return;
        }

        switch (special)
        {
            case TSpecialBlock.ESpecial_Normal:
                {
                    m_blockSprite.spriteName = "Item" + (int)(color - TBlockColor.EColor_None);
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
            case TSpecialBlock.ESpecial_Painter:
                {
                    m_blockSprite.spriteName = "Painter" + (int)(color - TBlockColor.EColor_None);
                }
                break;
            case TSpecialBlock.ESpecial_EatAColor:
                m_blockSprite.spriteName = "Rainbow";
                break;
            default:
                break;
        }
    }

    public bool IsEmpty()
    {
        return color == TBlockColor.EColor_None;
    }

    public TBlockColor color;							//颜色

    public int x_move;
    public int y_move;
    public bool m_bEating;						//正在消失的标记
    public bool isDropping;
    public Position droppingFrom;                 //从某个点掉落过来
    public bool isCanMove;
    public bool m_bNeedCheckEatLine;			//一旦落地就被标记，然后EatAllLine逻辑用这个变量区分是否需要检测消行
    public bool isLocked;                       //是否被锁定
    public TSpecialBlock special;				//特殊功能块

    public UISprite m_blockSprite;		//精灵动画
    public Transform m_blockTransform;         //

    public void Eat()							//吃掉这个块
	{
		m_bEating = true;
	}

    public bool IsEating()
	{
		return m_bEating;
	}

    public void Reset()
	{
		x_move = 0;
		y_move = 0;
		special = TSpecialBlock.ESpecial_Normal;
		m_bEating = false;
		isDropping = false;
	}

    public bool SelectAble()
    {

		if (isCanMove==false||
            isLocked ||
			m_bEating||
			isDropping==true||
			x_move>0||
			y_move>0 ||
			color == TBlockColor.EColor_None)
		{
			return false;
		}
		return true;
	}

	public CapBlock()
    {
        isCanMove = true;
    }
}
