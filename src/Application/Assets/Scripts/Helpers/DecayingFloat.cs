using UnityEngine;
using System.Collections;

public class DecayingFloat {
    public float Value
    {
        get
        {
            float ratio = 1 - Mathf.Clamp01((Time.time - m_updated - Delay) / DecayTime);
            return ratio * m_value;
        }
    }

    public float Delay = 0.1f;
    public float DecayTime = 0.1f;
    private float m_value;
    private float m_updated = float.MinValue;


    public DecayingFloat()
    {
        m_value = 0;
    }

    public void Update(float val)
    {
        if (val > Value)
        {
            m_value = val;
            m_updated = Time.time;
        }
    }
}
