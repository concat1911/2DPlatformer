using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace CharacterController2D
{ 

[System.Serializable]
public class CollisionCheckComponent
{
    //list of compatable collider types for this class
    enum ColliderTypes
    {
        Collider2D,
        CapsuleCollider2D,
        BoxCollider2D,
        CircleCollider2D
    }

    ColliderTypes m_colliderType = ColliderTypes.Collider2D;
    Rigidbody2D m_rigidBody;
    Collider2D m_collider;
    LayerMask m_layers;
    RaycastHit2D m_hit;

    List<Collider2D> m_collisionIgnoredEntities = new List<Collider2D>();

    Vector2 m_centerSlopeCheck;
    Vector2 m_leftSlopeCheck;
    Vector2 m_rightSlopeCheck;

    Vector2 m_slopeNormPerp;
    Vector2 m_slopeNorm;

    bool m_isSlopePositive;
    bool m_isOnSlope;
    float m_slopeDownAngle;



    public CollisionCheckComponent(Rigidbody2D rigidBody, Collider2D collider, LayerMask layers)
    {
        m_rigidBody = rigidBody;
        m_collider = collider;
        m_layers = layers;

        if(collider.GetType() == typeof(CapsuleCollider2D))
        {
            //Debug.Log("Capsule");
            m_colliderType = ColliderTypes.CapsuleCollider2D;
        }
        else if(collider.GetType() == typeof(BoxCollider2D))
        {
            //Debug.Log("Box");
            m_colliderType = ColliderTypes.BoxCollider2D;
        }
        else if(collider.GetType() == typeof(CircleCollider2D))
        {
            //Debug.Log("Circle");
            m_colliderType = ColliderTypes.CircleCollider2D;
        }
        else
            Debug.Log("Unsupported collder type passed to " + this.ToString());
    }


    public void UpdateSlopeCheck(float checkDist, float slopeThreshold, int checkSetting)
    {
        //update ray positions
        Vector3 temp = m_rigidBody.position + GetCollider2DOffset() - new Vector2(0f, GetCollider2DSize().y * 0.25f);

        m_centerSlopeCheck = temp;

        temp.x -= (GetCollider2DSize().x / 2f) - 0.02f;
        m_leftSlopeCheck = temp;

        temp = m_centerSlopeCheck;
        temp.x += (GetCollider2DSize().x / 2f) - 0.02f;
        m_rightSlopeCheck = temp;

        //cast rays
        m_hit = Physics2D.Raycast(m_centerSlopeCheck, Vector2.down, checkDist, m_layers);
        RaycastHit2D leftHit = Physics2D.Raycast(m_leftSlopeCheck, Vector2.down, checkDist, m_layers);
        RaycastHit2D rightHit = Physics2D.Raycast(m_rightSlopeCheck, Vector2.down, checkDist, m_layers);

        //check for slope
        if(m_hit || leftHit || rightHit)
        {
            switch (checkSetting)
            {
                case 3:
                {
                    if(m_hit && leftHit && rightHit)
                    {
                        float leftAngle = Vector2.Angle(leftHit.normal, Vector2.up);
                        float middleAngle = Vector2.Angle(m_hit.normal, Vector2.up);
                        float rightAngle = Vector2.Angle(rightHit.normal, Vector2.up);

                        if(leftAngle >= slopeThreshold && middleAngle >= slopeThreshold && rightAngle >= slopeThreshold)
                        {
                            m_slopeNormPerp = Vector2.Perpendicular(m_hit.normal).normalized;
                            m_slopeDownAngle = Vector2.Angle(m_hit.normal, Vector2.up);
                            m_slopeNorm = m_hit.normal;    
                        }
                        else
                        {
                            if(leftAngle < middleAngle && leftAngle < rightAngle)
                                m_hit = leftHit;
                            else if(rightAngle < middleAngle && rightAngle < leftAngle)
                                m_hit = rightHit;

                            m_slopeNormPerp = Vector2.Perpendicular(m_hit.normal).normalized;
                            m_slopeDownAngle = Vector2.Angle(m_hit.normal, Vector2.up);
                            m_slopeNorm = m_hit.normal;
                        }
                        break;
                    }
                    else
                        goto case 2;
                }
                case 2:
                {
                    if((m_hit && leftHit) || (m_hit && rightHit))
                    {
                        m_slopeNormPerp = Vector2.Perpendicular(m_hit.normal).normalized;
                        m_slopeDownAngle = Vector2.Angle(m_hit.normal, Vector2.up);
                        m_slopeNorm = m_hit.normal;  
                        break;
                    } 
                    else 
                        goto case 1;
                }
                case 1:
                {
                    //find ray with largest slope
                    SetSlopeInfoToLargest(leftHit, m_hit, rightHit);
                    break;
                }
                default:
                {
                    Debug.Log(m_rigidBody.gameObject.name + " slope check setting out of range " + checkSetting);
                    m_slopeNormPerp = Vector2.Perpendicular(m_hit.normal).normalized;
                    m_slopeDownAngle = Vector2.Angle(m_hit.normal, Vector2.up);
                    m_slopeNorm = m_hit.normal;
                    break;
                }
            }
            
            //check side slope is on
            if(m_slopeDownAngle >= slopeThreshold)
            {
                m_isOnSlope = true;

                if(leftHit && rightHit)
                {
                    if(leftHit.point.y < rightHit.point.y)
                        m_isSlopePositive = true;
                    else if(leftHit.point.y > rightHit.point.y)
                        m_isSlopePositive = false;
                }
            }
            else
                m_isOnSlope = false;

        }
        else
            m_isOnSlope = false;

/*      //draw the three rays used for slope checking
        Debug.DrawRay(m_leftSlopeCheck, Vector2.down * checkDist, Color.red);
        Debug.DrawRay(m_centerSlopeCheck, Vector2.down * checkDist, Color.red);
        Debug.DrawRay(m_rightSlopeCheck, Vector2.down * checkDist, Color.red);
*/
/*      //draw the ground normal and output angle of collider hit
        Debug.DrawRay(m_hit.point, m_slopeNormPerp, Color.yellow);
        Debug.DrawRay(m_hit.point, m_hit.normal, Color.green);
        Debug.DrawRay(m_leftSlopeCheck, Vector2.up, Color.red);
        Debug.Log(m_slopeDownAngle + " " + m_hit.collider.name);
*/      
    }


    #region GettersAndSetters
    public void SetRigidBody(Rigidbody2D rigidBody)
    {
        m_rigidBody = rigidBody;
    }
    public Rigidbody2D GetRigidbody2D()
    {
        return m_rigidBody;
    }

    public void SetCollider2D(Collider2D collider)
    {
        m_collider = collider;
    }
    public Collider2D GetCollider2D()
    {
        return m_collider;
    }

    public Vector2 GetCollider2DSize()
    {
        if(m_colliderType == ColliderTypes.CapsuleCollider2D)
        {
            return ((CapsuleCollider2D)m_collider).size;
        }
        else if(m_colliderType == ColliderTypes.BoxCollider2D)
        {
            return ((BoxCollider2D)m_collider).size;
        }
        else if(m_colliderType == ColliderTypes.CircleCollider2D)
        {
            float size = ((CircleCollider2D)m_collider).radius;
            return new Vector2(size, size);
        }

        return Vector2.zero;
    }
    public void SetCollider2DSize(Vector2 size)
    {
        if(m_colliderType == ColliderTypes.CapsuleCollider2D)
        {
            ((CapsuleCollider2D)m_collider).size = size;
        }
        else if(m_colliderType == ColliderTypes.BoxCollider2D)
        {
            ((BoxCollider2D)m_collider).size = size;
        }
        else if(m_colliderType == ColliderTypes.CircleCollider2D)
        {
            ((CircleCollider2D)m_collider).radius = size.x;
        }
    }

    public Vector2 GetCollider2DOffset()
    {
        if(m_colliderType == ColliderTypes.CapsuleCollider2D)
        {
            return ((CapsuleCollider2D)m_collider).offset;
        }
        else if(m_colliderType == ColliderTypes.BoxCollider2D)
        {
            return ((BoxCollider2D)m_collider).offset;
        }
        else if(m_colliderType == ColliderTypes.CircleCollider2D)
        {
            return ((CircleCollider2D)m_collider).offset;
        }

        return Vector2.zero;
    }
    public void SetCollider2DOffset(Vector2 offset)
    {
        if(m_colliderType == ColliderTypes.CapsuleCollider2D)
        {
            ((CapsuleCollider2D)m_collider).offset = offset;
        }
        else if(m_colliderType == ColliderTypes.BoxCollider2D)
        {
            ((BoxCollider2D)m_collider).offset = offset;
        }
        else if(m_colliderType == ColliderTypes.CircleCollider2D)
        {
            ((CircleCollider2D)m_collider).offset = offset;
        }
    }

    public void SetCollisionLayers(LayerMask layers)
    {
        m_layers = layers;
    }

    public float GetGroundSlope()
    {
        return m_slopeDownAngle;
    }

    public Vector2 GetGroundNorm()
    {
        return m_slopeNorm;
    }

    public Vector2 GetGroundPerpendicular()
    {
        return m_slopeNormPerp;
    }

    public RaycastHit2D GetGroundInfo()
    {
        return m_hit;
    }

    public Vector2 GetRaycastPos()
    {
        return m_centerSlopeCheck;
    }

    public bool IsSlopePositive()
    {
        return m_isSlopePositive;
    }

    public bool IsOnSLope()
    {
        return m_isOnSlope;
    }


    public Vector2 GetLowerCheckPosition(float checkRadius, float checkOffset, Vector2 colliderSize, Vector2 colliderOffset)
    {
        return new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + colliderOffset.y - (colliderSize.y * 0.5f) + (checkRadius * checkOffset));
    }
    public Vector2 GetLowerCheckPosition(float checkRadius, float checkOffset)
    {
        return GetLowerCheckPosition(checkRadius, checkOffset, GetCollider2DSize(), GetCollider2DOffset());
    }
    public Vector2 GetUpperCheckPosition(float checkRadius, float checkOffset, Vector2 colliderSize, Vector2 colliderOffset)
    {
        return new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + colliderOffset.y + (colliderSize.y * 0.5f) - (checkRadius * checkOffset));
    }
    public Vector2 GetUpperCheckPosition(float checkRadius, float checkOffset)
    {
        return GetUpperCheckPosition(checkRadius, checkOffset, GetCollider2DSize(), GetCollider2DOffset());
    }
    public Vector2 GetMiddleCheckPosition(Vector2 colliderOffset)
    {
        return new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + colliderOffset.y);
    }
    public Vector2 GetMiddleCheckPosition()
    {
        return GetMiddleCheckPosition(GetCollider2DOffset());
    }
    #endregion


    #region Checks
    public bool CheckLower(float checkSize, float checkOffset, bool useSquare = false)
    {
        return Check(GetLowerCheckPosition(checkSize, checkOffset), checkSize, useSquare);
    }
    public bool CheckLower(float checkSize, float checkOffset, Vector2 colliderSize, Vector2 colliderOffset, bool useSquare = false)
    {
        return Check(GetLowerCheckPosition(checkSize, checkOffset, colliderSize, colliderOffset), checkSize, useSquare);
    }

    public bool CheckUpper(float checkSize, float checkOffset, bool useSquare = false)
    {
        return Check(GetUpperCheckPosition(checkSize, checkOffset), checkSize, useSquare);
    }
    public bool CheckUpper(float checkSize, float checkOffset, Vector2 colliderSize, Vector2 colliderOffset, bool useSquare = false)
    {
        return Check(GetUpperCheckPosition(checkSize, checkOffset, colliderSize, colliderOffset), checkSize, useSquare);
    }

    public bool CheckMiddle(float checkSize, bool useSquare = false)
    {
        return Check(GetMiddleCheckPosition(), checkSize, useSquare);
    }
    public bool CheckMiddle(float checkSize, Vector2 colliderOffset, bool useSquare = false)
    {
        return Check(GetMiddleCheckPosition(colliderOffset), checkSize, useSquare);
    }

    bool Check(Vector2 position, float checkSize, bool useSquare)
    {
        return OverlapCheck(position, checkSize, m_layers, useSquare);
    }

    public Collider2D OverlapCheck(Vector2 position, float checkSize, LayerMask layers, bool useSquare)
    {
        Collider2D[] results;

        if(useSquare)
        {
            results = Physics2D.OverlapBoxAll(position, new Vector2(checkSize, checkSize), 0f, layers);
        }
        else
        {
            results = Physics2D.OverlapCircleAll(position, checkSize, layers);
        }

        if(results != null && results.Length > 0)
        {
            foreach(Collider2D collider in results)
            {
                if(collider != null && collider != m_collider)
                {       
                    return collider;
                }
            }
        }

        /* Old checking method if preferd. May result in better performance at cost of character having to be on layer seperate from walkable objects (Can not be in a layer marked in GroundLayers)
        
        if(useSquare)
        {
            return Physics2D.OverlapBox(position, new Vector2(checkSize, checkSize), 0f, layers);
        }
        else
        {
            return Physics2D.OverlapCircle(position, checkSize, layers);
        }

        */

        return null;
    }
    #endregion


    void SetSlopeInfoToLargest(RaycastHit2D leftHit, RaycastHit2D centerHit, RaycastHit2D rightHit)
    {
        float leftAngle = 0;
        float centerAngle = 0;
        float rightAngle = 0;

        if(leftHit.collider != null)
            leftAngle = Vector2.Angle(leftHit.normal, Vector2.up);
        
        if(centerHit.collider != null)
            centerAngle = Vector2.Angle(centerHit.normal, Vector2.up);

        if(rightHit.collider != null)
            rightAngle = Vector2.Angle(rightHit.normal, Vector2.up);

        //Debug.Log(leftAngle + "  " + centerAngle + "  " + rightAngle);

        if(leftAngle >= rightAngle && leftAngle >= centerAngle)
        {
            //set to left
            m_hit = leftHit;
        }
        else if(rightAngle >= leftAngle && rightAngle >= centerAngle)
        {
            //set to right
            m_hit = rightHit;
        }
        //else, center is the hit

        m_slopeNormPerp = Vector2.Perpendicular(m_hit.normal).normalized;
        m_slopeDownAngle = Vector2.Angle(m_hit.normal, Vector2.up);
        m_slopeNorm = m_hit.normal;
    }

    public void IgnoreCollidersInRange(GameObject self, LayerMask layerToCheck, float range, string tags = "")
    {
        //if a self is in range of colliders in layerToCheck disable collision with them and add to m_collisionIgnoredEntities
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.transform.position, range, layerToCheck);
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject != self)
            {
                if(tags == "")
                {
                    Physics2D.IgnoreCollision(colliders[i], self.GetComponent<Collider2D>(), true);
                    m_collisionIgnoredEntities.Add(colliders[i]);
                }
                else if(tags.Contains(colliders[i].gameObject.tag))
                {
                    Physics2D.IgnoreCollision(colliders[i], self.GetComponent<Collider2D>(), true);
                    m_collisionIgnoredEntities.Add(colliders[i]);
                }
            }  
        } 
        
    }

    public void ReenableIgnoredColliders(GameObject self, LayerMask layerToCheck)
    {
        if(m_collisionIgnoredEntities.Count != 0)
        {
            //get collisions within collider
            Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2(self.transform.position.x + GetCollider2DOffset().x, self.transform.position.y + GetCollider2DOffset().y), GetCollider2DSize(), layerToCheck);

            //loop through ignore list
            for (int i = 0; i < m_collisionIgnoredEntities.Count; i++)
            {
                //if objects in ignore list are not in collider then restor collision
                if(!colliders.Contains(m_collisionIgnoredEntities[i]))
                {
                    Physics2D.IgnoreCollision(m_collisionIgnoredEntities[i], self.GetComponent<Collider2D>(), false);
                    m_collisionIgnoredEntities.Remove(m_collisionIgnoredEntities[i]);
                }
            }
        }
    }
}

}