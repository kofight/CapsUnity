using UnityEngine;
using System.Collections;

public class NumberDrawer : MonoBehaviour {
    public int maxIntLenth = 3;
    public UISprite[] m_numbers;
    public int Number;
    public string SurName;
    public int NumberInterval = 20;
    // Use this for initialization
    void Start()
    {
        //SetNumber(Number);
    }

    public void SetNumber(int number)
    {
        int factor = 10;									//用来取某个位的数字的因子
        //第一遍找到开始的数字位置
        int curNumStartIndex = 7;
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
                m_numbers[i].LocalPositionX((maxIntLenth - curNumStartIndex) * NumberInterval / 2 - NumberInterval * (i - curNumStartIndex));
                factor *= 10;
            }
        }
    }
}