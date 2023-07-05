using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeDirection : MonoBehaviour
{
    #region =====Slope Detection Variables=====
    [Header("==========Slope Detection==========")]
    public Transform orientation; //The forward vector where the player is looking
    //Both rays will compare their distances to the floor. The one that is the furthest marks if the player is uphill or downhill
    public Transform rearRay; //The position where the player's rear is looking. Will shoot a ray to know the distance from the rear of the player to the ground
    public Transform frontRay; //The position where the player's front is looking. Will shoot a ray to know the distance from the front of the player to the ground
    public LayerMask whatIsGround; //Layer that signals that the player is touching ground

    public float surfaceAngle; //the angle of the slope
    public bool upHill; //True of the front ray is smaller
    public bool downHill; //True if the rear ray is smaller
    public bool flatSurface; //True if they are the same
    #endregion

    #region =====Slope Velocities Varibales=====
    [Header("==========Slope Velocities==========")]
    public float maxSlopeVel; //The maximum velocity that the player can get to in a downwards slope
    public float minSlopeVel; //the minimum velocity that the player can get to in an upwards slope
    #endregion

    SkateController _sC;
    Rigidbody rb;

    private void Start()
    {
        _sC = GetComponent<SkateController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetSlopeAngle();
        VelocityChanges();
        SlopeHops();
    }

    #region =====Get Slope Angle=====
    public void GetSlopeAngle()
    {
        rearRay.rotation = Quaternion.Euler(-orientation.rotation.x, 0, 0);
        RaycastHit rearHit;
        if (Physics.Raycast(rearRay.position, rearRay.TransformDirection(-Vector3.up), out rearHit, Mathf.Infinity, whatIsGround))
        {
            Debug.DrawRay(rearRay.position, rearRay.TransformDirection(-Vector3.up) * rearHit.distance, Color.green);
            surfaceAngle = Vector3.Angle(rearHit.normal, Vector3.up);
        }
        else
        {
            Debug.DrawRay(rearRay.position, rearRay.TransformDirection(-Vector3.up) * 1000, Color.red);
            upHill = false;
            downHill = true;
            flatSurface = false;
            Debug.Log("Downhill");
        }

        RaycastHit frontHit;
        Vector3 frontRayStartPos = new Vector3(frontRay.position.x, rearRay.position.y, frontRay.position.z);
        if (Physics.Raycast(frontRayStartPos, rearRay.TransformDirection(-Vector3.up), out frontHit, Mathf.Infinity, whatIsGround))
        {
            Debug.DrawRay(frontRayStartPos, frontRay.TransformDirection(-Vector3.up) * frontHit.distance, Color.green);
            surfaceAngle = Vector3.Angle(rearHit.normal, Vector3.up);
        }
        else
        {
            upHill = true;
            downHill = false;
            flatSurface = false;
            Debug.Log("Uphill");
        }

        if (frontHit.distance < rearHit.distance)
        {
            upHill = true;
            downHill = false;
            flatSurface = false;
            Debug.Log("Uphill");
        }
        else if (frontHit.distance > rearHit.distance)
        {
            upHill = false;
            downHill = true;
            flatSurface = false;
            Debug.Log("Downhill");
        }
        else if (frontHit.distance == rearHit.distance)
        {
            flatSurface = true;
            upHill = false;
            downHill = false;
            Debug.Log("Flat");
        }
    }
    #endregion

    #region =====Slope Velocity Modifications=====
    public void VelocityChanges()
    {
        if (_sC.verticalInput != 0)
        {
            if (upHill) { _sC.maxSpeed = Mathf.Lerp(_sC.maxSpeed, minSlopeVel, 0.5f * Time.deltaTime); }
            else if (downHill) { _sC.maxSpeed = Mathf.Lerp(_sC.maxSpeed, maxSlopeVel, 0.5f * Time.deltaTime); }
        }
        
        if(flatSurface) { _sC.maxSpeed = Mathf.Lerp(_sC.maxSpeed, _sC.ogMaxSpeed, Time.deltaTime); }
    }
    #endregion

    #region =====Elimates Slope Hops=====
    public void SlopeHops()
    {
        if (!flatSurface && _sC.grounded)
        {
            rb.AddForce(-_sC.orientation.transform.up * 10f, ForceMode.Force); //Change to velocities ASAP
        }
    }
    #endregion
}
