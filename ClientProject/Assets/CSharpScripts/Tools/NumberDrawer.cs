using UnityEngine;
using System.Collections;

public enum NumberAlign
{
	Left,
	Right,
	Center,
}

public class NumberDrawer : MonoBehaviour {
    public int maxIntLenth = 3;
    public UISprite[] m_numbers;
    int m_curNumber = -1;
    public string SurName;
    public int NumberInterval = 20;
	public NumberAlign Align = NumberAlign.Center;

    int m_targetNumber;             //动画特效的目标值
    int m_startNumber;
    float m_duration;
    float m_curMotionStartTime;     //当前特效的开始时间
    float m_lastChangeTime;         //上次改变数字的时间

    // Use this for initialization
    void Start()
    {
        //SetNumber(Number);
		m_curNumber = -1;
    }

    void Update()           //这里看看是否可以优化掉，因为大部分数字是不需要Update的
    {
        if (m_curMotionStartTime > 0)
        {
                int number = (int)((m_targetNumber - m_curNumber) * (Time.realtimeSinceStartup - m_lastChangeTime) / m_duration + m_curNumber);
                if (number != m_curNumber)
                {
                    m_lastChangeTime = Time.realtimeSinceStartup;
                    SetNumberRapid(number);
                }
				
				if(number == m_targetNumber)
				{
					m_curMotionStartTime = 0;       //结束
					SetNumberRapid(m_targetNumber);
				}
        }
    }

    public void SetNumber(int number, float duration = 0.1f)
    {
		if(number == m_targetNumber) return;
        if (duration == 0)
        {
            SetNumberRapid(number);
        }
        else
        {
			if(m_targetNumber == number)return;
            m_targetNumber = number;
            m_startNumber = m_curNumber;
            m_duration = duration;
            m_curMotionStartTime = Time.realtimeSinceStartup;
            m_lastChangeTime = m_curMotionStartTime;
        }
    }

    void SetNumberRapid(int number)
    {
        if (m_curNumber == number)
        {
            return;
        }
		
		m_curNumber = number;
		
		if(number == 0)		//0特殊处理一下
		{
            for (int i = 0; i < maxIntLenth; ++i)
            {
                if (i==0)
                {
                    m_numbers[i].GetComponent<UISprite>().spriteName = SurName + 0;
                    m_numbers[i].gameObject.SetActive(true);
                    m_numbers[i].LocalPositionX(0);
                }
                else
                {
                    m_numbers[i].gameObject.SetActive(false);
                }
            }
            return;
		}
		
        int factor = 10;									//用来取某个位的数字的因子
        //第一遍找到开始的数字位置
        int curNumStartIndex = maxIntLenth - 1;
        for (int i = 0; i < maxIntLenth; ++i)		//处理整数部分
        {
            int tempNumber = (number % factor) / (factor / 10);		//取出正在处理的位的数字
            if (tempNumber != 0)
            {
                curNumStartIndex = maxIntLenth - 1 - i;
            }
            factor *= 10;
        }

        factor = 10;
        string newName = "";
        for (int i = 0; i < maxIntLenth; ++i)
        {
            if (i < curNumStartIndex)
            {
                m_numbers[i].gameObject.SetActive(false);
            }
            else
            {
                int tempNumber = (number % factor) / (factor / 10);		//取出正在处理的位的数字
                m_numbers[i].gameObject.SetActive(true);
                newName = SurName + tempNumber;
                m_numbers[i].GetComponent<UISprite>().spriteName = newName;
				if(Align == NumberAlign.Center)
				{
					m_numbers[i].LocalPositionX((maxIntLenth - curNumStartIndex - 1) * NumberInterval / 2 - NumberInterval * (i - curNumStartIndex));	
				}
                else if(Align == NumberAlign.Left)
				{
					m_numbers[i].LocalPositionX((maxIntLenth - curNumStartIndex - 1) * NumberInterval - NumberInterval * (i - curNumStartIndex));
				}
				else if(Align == NumberAlign.Right)
				{
					m_numbers[i].LocalPositionX(- (i - curNumStartIndex) * NumberInterval);
				}
                factor *= 10;
            }
        }
    }
}