using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{

public class MovePlatform_MultiDirection : MonoBehaviour
{
    [SerializeField]
    float m_speed = 1f;
    [SerializeField]
    List<Vector3> m_pointList;


    Vector3 m_origin = Vector3.zero;
    Vector3 targetPos;

    int m_currentPoint = 0;
    bool reverse = false;



    // Start is called before the first frame update
    void Start()
    {
        m_origin = transform.position;
        targetPos = transform.position + m_pointList[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            //Debug.Log(m_currentPoint + " of " + (m_pointList.Count - 1));

            if(!reverse)
            {
                if(!(m_currentPoint >= m_pointList.Count - 1))
                {
                    m_currentPoint += 1;
                    targetPos = targetPos + m_pointList[m_currentPoint];
                }
                else
                    reverse = true;
            }
            else
            {
                if(!(m_currentPoint < 0))
                {
                    targetPos = targetPos - m_pointList[m_currentPoint];          
                    m_currentPoint -= 1;  
                }
                
                if(m_currentPoint <= -1)
                {
                    reverse = false;
                }
            }
        }
        
        transform.position += ((targetPos - transform.position).normalized * m_speed) * Time.deltaTime;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);
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

        Vector3 lastPos = Vector3.zero;;
        for(int i = 0; i < m_pointList.Count; ++i)
        {
            if(i == 0)
            {
                lastPos = (basePos + m_pointList[i]);
                Gizmos.DrawLine(basePos, lastPos);
            }
            else
            {
                Gizmos.DrawLine((lastPos), (lastPos + m_pointList[i]));
                lastPos = lastPos + m_pointList[i];
            }
        }
    }

}

}