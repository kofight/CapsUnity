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
};

public enum TSpecialBlock
{
    ESpecial_Normal,			//普通块
    ESpecial_EatLineDir0,			//一次消一线，方向0
    ESpecial_EatLineDir1,			//一次消一线，方向1
    ESpecial_EatLineDir2,			//一次消一线，方向2
    ESpecial_Bomb,					//一次消周围9个
    ESpecial_EatAColor,				//一次消一个颜色
};

public class CapBlock  
{
    public TBlockColor color;							//颜色

    	int x_move;
	int y_move;
	bool m_bEating;						//正在消失的标记
	bool isDropping;
	bool isCanMove;
	bool m_bNeedCheckEatLine;			//一旦落地就被标记，然后EatAllLine逻辑用这个变量区分是否需要检测消行

	TSpecialBlock special;				//特殊功能块
	UISprite blockSprite;			//精灵动画

	void Eat()							//吃掉这个块
	{
		m_bEating = true;
	}

	bool IsEating()
	{
		return m_bEating;
	}

	void Reset()
	{
		x_move = 0;
		y_move = 0;
		special = TSpecialBlock.ESpecial_Normal;
		m_bEating = false;
		isDropping = false;
	}

	bool SelectAble(){

		if (isCanMove==false||
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

	CapBlock()
    {
        isCanMove = true;
    }
}
