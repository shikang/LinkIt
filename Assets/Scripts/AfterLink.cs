using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterLink : MonoBehaviour
{
    const float DELAY_BEFORE_START = 0.5f;
    const float SPEED = 5.0f;

    float m_Timer = 0.0f;
    Vector3 m_Target;
    Vector3 m_Dir;

    bool m_ReachX = false;
    bool m_ReachY = false;

    bool m_ChildActive = false;
    bool m_DoNothing = false;

    // Use this for initialization
    void Start ()
    {
        m_Timer = 0.0f;

        m_ReachX = false;
        m_ReachY = false;

        ParticleSystem cps = GetComponentInChildren<ParticleSystem>();
        cps.Stop();
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Play(false);
        m_ChildActive = false;
        m_DoNothing = false;
    }

    public void SetTarget( Vector3 t )
    {
        m_Target = t;
        m_Dir = m_Target - transform.position;
        m_Dir.Normalize();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if ( m_DoNothing ) return;

        m_Timer += Time.deltaTime;

        if ( m_Timer < DELAY_BEFORE_START )
            return;

        if ( !m_ChildActive )
        {
            m_ChildActive = true;
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.Play();
        }

        transform.position += m_Dir * SPEED * Time.deltaTime;

        if ( ( m_Dir.x > 0.0f && transform.position.x > m_Target.x ) ||
             ( m_Dir.x < 0.0f && transform.position.x < m_Target.x ) )
        {
            m_Dir.x = 0.0f;
            Vector3 p = transform.position;
            p.x = m_Target.x;
            transform.position = p;

            m_ReachX = true;
        }

        if ( ( m_Dir.y > 0.0f && transform.position.y > m_Target.y ) ||
             ( m_Dir.y < 0.0f && transform.position.y < m_Target.y ) )
        {
            m_Dir.y = 0.0f;
            Vector3 p = transform.position;
            p.y = m_Target.y;
            transform.position = p;

            m_ReachY = true;
        }

        if ( m_ReachX && m_ReachY )
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ps.Stop(true);
            Destroy( gameObject, ps.startLifetime + Time.deltaTime );
            m_DoNothing = true;
        }
    }
}
