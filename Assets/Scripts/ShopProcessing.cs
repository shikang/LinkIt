using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopProcessing : MonoBehaviour
{
    public const float FRAME_RATE = 0.3f;
    
    public GameObject m_Overlay;
    public GameObject m_Text;

    private float m_Timer = 0.0f;
    private int m_Frame = 0;
    private bool m_Start = false;

    // Use this for initialization
    void Start ()
    {
        ShowBlock(false);

        m_Timer = 0.0f;
        m_Frame = 0;
        m_Start = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_Start) return;

        m_Timer += Time.deltaTime;

        if (m_Timer >= FRAME_RATE)
        {
            m_Timer -= FRAME_RATE;

            string t = "Processing";

            int num = (m_Frame++) % 4;
            for (int i = 0; i < num; ++i)
            {
                t += '.';
            }

            m_Text.GetComponent<Text>().text = t;
        }
    }

    void ShowBlock(bool show)
    {
        if (m_Overlay != null)
        {
            m_Overlay.SetActive(show);
        }

        if (m_Text != null)
        {
            m_Text.SetActive(show);
        }
    }

    public void StartBlocking()
    {
#if UNITY_IOS
        m_Timer = 0.0f;
        m_Frame = 0;
        m_Start = true;
        ShowBlock(true);
#endif
    }

    public void StopBlocking()
    {
#if UNITY_IOS
        m_Start = false;
        ShowBlock(false);
#endif
    }
}
