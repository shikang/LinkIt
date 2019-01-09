using UnityEngine;
using System.Collections;

public class BackgroundOverlay : MonoBehaviour
{
    public const float MOVE_SPEED = 2.0f;

    public GameObject m_Overlay1;
    public GameObject m_Overlay2;
    public float m_WrapPosY;
    
    float m_PosDiff = 0.0f;

    // Use this for initialization
    void Start ()
    {
        m_PosDiff = Mathf.Max( m_Overlay1.transform.position.y, m_Overlay2.transform.position.y ) - Mathf.Min( m_Overlay1.transform.position.y, m_Overlay2.transform.position.y );
    }
	
	// Update is called once per frame
	void Update ()
    {
        MoveOverlay( m_Overlay1 );
        MoveOverlay( m_Overlay2 );
        Debug.Log( "Overlay 1: " + m_Overlay1.transform.position.y );
        Debug.Log( "Overlay 2: " + m_Overlay2.transform.position.y );

        WrapOverlay( m_Overlay1, m_Overlay2 );
        WrapOverlay( m_Overlay2, m_Overlay1 );
    }

    void MoveOverlay( GameObject overlay )
    {
        Vector3 pos = overlay.transform.position;
        pos.y -= MOVE_SPEED * Time.deltaTime;
        overlay.transform.position = pos;
    }

    bool WrapOverlay( GameObject overlay, GameObject otherOverlay )
    {
        if ( overlay.transform.position.y <= m_WrapPosY )
        {
            Vector3 pos = overlay.transform.position;
            pos.y = otherOverlay.transform.position.y + m_PosDiff;
            overlay.transform.position = pos;
            return true;
        }

        return false;
    }
}
