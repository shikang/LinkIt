using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScaler : MonoBehaviour
{
    public bool m_ScaleWidthOnly;
	// Use this for initialization
	void Start ()
    {
		if ( ( (float)Screen.width / (float)Screen.height ) > ( 10.0f / 16.0f ) )
        {
            float scale = ( (float)Screen.width / (float)Screen.height ) / ( 10.0f / 16.0f );

            if (m_ScaleWidthOnly)
            {
                Vector3 s = transform.localScale;
                s.x *= scale;
                transform.localScale = s;
            }
            else
            {
                transform.localScale *= scale;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
