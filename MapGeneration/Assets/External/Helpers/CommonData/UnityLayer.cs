using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnityLayer
{
    [SerializeField] private int m_layer;

    public int LayerIndex
    {
        get
        {
            return m_layer;
        }
        set
        {
            if (m_layer > 0 && m_layer < 32)
                m_layer = value;
        }
    }

    public int Mask
    {
        get { return 1 << m_layer; }
    }
}
