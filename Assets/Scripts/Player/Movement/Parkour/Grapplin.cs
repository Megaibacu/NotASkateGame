using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapplin : MonoBehaviour
{

    [Header("References")]
    private PlayerMovement pm;
    public Transform player;
    public Transform guntip;
    public LayerMask whatIsGrap;
    public LineRenderer lr;
    public Camera cam;
    public GameObject grappleObject;
    public Collider[] colliders;
    public GameObject closest;
    public PlayerInput _playerInput;
    private StateChange _st;


    [Header("Grapplin")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;


    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplinCd;
    private float grapplinCdTimer;

    private bool grappling;

    public bool isVisible(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach(var plane in planes)
        {
            if(plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }          
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GrapplingDetection());
        pm = GetComponent<PlayerMovement>();
        _playerInput = GetComponent<PlayerInput>();
        _st = GetComponent<StateChange>();
    }

    // Update is called once per frame
    void Update()
    {     
        if (grapplinCdTimer > 0)
            grapplinCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, guntip.position);
    }

    public void StartGrapple()
    {
        if(_st.state == States.parkour)
        {
            if (grapplinCdTimer > 0) return;

            grappling = true;



            if (grappleObject != null && isVisible(cam, grappleObject))
            {
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
                grapplePoint = grappleObject.transform.position;
                lr.enabled = true;
                lr.SetPosition(1, grapplePoint);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.grapple, transform.position);
            }
            else
            {
                Invoke(nameof(EndGrapple), grappleDelayTime);
            }


        }

    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;
        Vector3 grappleDirection = (grapplePoint - transform.position);
        if(pm.grounded)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        }
        pm.Jump(new Vector3(grappleDirection.x, grappleDirection.y * 1.5f, grappleDirection.z));
        Invoke(nameof(EndGrapple), 1f);
    }

    private void EndGrapple()
    {
        pm.freeze = false;

        grappling = false;

        pm.activeGrapple = false;

        grapplinCdTimer = grapplinCd;

        lr.enabled = false;
    }

    IEnumerator GrapplingDetection()
    {
        while (GetComponent<Grapplin>().isActiveAndEnabled)
        {
            yield return new WaitForSeconds(0.2f);
            
            
            colliders = Physics.OverlapSphere(transform.position + Vector3.up, maxGrappleDistance, whatIsGrap);
            if(colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (closest == null)
                    {
                        closest = col.gameObject;
                    }                   
                    if (Vector3.Distance(transform.position, closest.transform.position) > Vector3.Distance(transform.position, col.transform.position))
                    {
                        closest = col.gameObject;
                    }
                    else if (Vector3.Distance(transform.position, closest.transform.position) < Vector3.Distance(transform.position, col.transform.position) && !isVisible(cam, closest) && isVisible(cam, col.gameObject))
                    {
                        closest = col.gameObject;
                    }
                    Debug.Log(closest.gameObject.name);
                    grappleObject = closest;
                }
                
                
            }
            else
            grappleObject = null;
        }
        
    }
}
