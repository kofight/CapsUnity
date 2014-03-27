using UnityEngine;
using System.Collections;

public class MyRandom
{
	private int m_Value = 0;
	public MyRandom(){}
	public MyRandom(int aSeed)
	{
		m_Value = aSeed;
	}
	public int Next()
	{
		m_Value = (m_Value * 2317 + 1) % 1000;
		return m_Value;
	}

    public int Next(int aMin, int aMax)
    {
        return Range(aMin, aMax);
    }

	public int Range(int aMin, int aMax)
	{
		return aMin + Next() % (aMax-aMin);
	}
}