using UnityEngine;
using System.Collections;

public class MyRandom
{
	private int m_Value = 0;
	public MyRandom(){}
	public MyRandom(int aSeed)
	{
        UnityEngine.Random.seed = aSeed;
		m_Value = aSeed;
	}
	public int Next()
	{
        return UnityEngine.Random.Range(0, 10000);
		//m_Value = (m_Value * 2317 + 1) % 1000;
		//return m_Value;
	}

    public int Next(int aMin, int aMax)
    {
        return UnityEngine.Random.Range(aMin, aMax);
        //return Range(aMin, aMax);
    }

	public int Range(int aMin, int aMax)
	{
        return UnityEngine.Random.Range(aMin, aMax);
		//return aMin + Next() % (aMax-aMin);
	}
}