using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{

public class MovePlatform : MonoBehaviour
{
    [SerializeField]
    float m_speed = 1f;
    [SerializeField]
    Vector3 m_moveDirection;

    [SerializeField]
    bool m_smoothMovement;

    [Tooltip("Select to recalculate move points durring run time. Only used durring play.")]
    [SerializeField]
    bool m_recalculatePoints;


    Vector3 m_firstPoint;
    Vector3 m_origin = Vector3.zero;



    // Start is called before the first frame update
    void Start()
    {
        m_recalculatePoints = false;
        m_origin = transform.position;
        m_firstPoint = transform.position + m_moveDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_recalculatePoints)
        {
            m_recalculatePoints = false;
            m_firstPoint = m_origin + m_moveDirection;
        }
    }

    void FixedUpdate()
    {
        if(m_smoothMovement)
            transform.position = Vector3.Lerp(m_firstPoint, m_origin, (Mathf.Sin(m_speed * Time.time) + 1f) / 2f);
        else
            transform.position = Vector3.Lerp(m_firstPoint, m_origin, Mathf.PingPong(Time.time * m_speed, 1.0f));
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.SetParent(gameObject.transform, true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.transform.SetParent(null);
    }


    void OnDrawGizmosSelected() 
    {
        Vector3 basePos;
        if(m_origin != Vector3.zero)
        {
            basePos = m_origin;
        }
        else
        {
            basePos = transform.position;
        }

        Gizmos.DrawLine(basePos, (basePos + m_moveDirection));
    }
}

}