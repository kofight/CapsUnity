using UnityEngine;
using System.Collections;

public class SimpleTimer 
{
    float m_startTime;
    float m_interval;
    public SimpleTimer()
    {

    }

    public void Start(float interval)
    {
        m_startTime = Time.realtimeSinceStartup;
        m_interval = interval;
    }

    public bool IsEnd()
    {
        if (Time.realtimeSinceStartup > m_interval + m_startTime)
        {
            return true;
        }
        return false;
    }
}
