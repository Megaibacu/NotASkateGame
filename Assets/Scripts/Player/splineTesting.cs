using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


public class splineTesting : MonoBehaviour
{
    public PlayerMovement pm;
    public SkateController sc;
    public SplineComputer sp;
    public SplineFollower splineF;
    public GameObject orientation;
    public CapsuleCollider pCollider;
    private Rigidbody rb;
    public SplineSample result;
    public double startingPos;
    public double from = 0;
    public double to = 1;
    public float storedvel;
    public float grind_cd = 0.5f;
    public float cd_countdown;
    [Range(-100, 100)] public float balance;

    // Start is called before the first frame update
    void Start()
    {
        cd_countdown = 0;
        sc = GetComponent<SkateController>();
        pm = GetComponent<PlayerMovement>();
        splineF = GetComponent<SplineFollower>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cd_countdown < 0)
        {
            cd_countdown = 0;
        }       
        else if(cd_countdown > 0)
        {
            cd_countdown -= Time.deltaTime;
        }
        if (cd_countdown == 0)
        {
            pCollider.isTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!sc.grinding && cd_countdown == 0)
        {
            if (GetComponent<StateChange>().state == States.skating)
            {
                if (collision.transform.tag == "Spline")
                {
                    sp = collision.gameObject.GetComponent<SplineComputer>();
                    splineF.spline = sp;
                    sp.Project(collision.GetContact(0).point, ref result, from = 0, to = 1);
                    startingPos = result.percent;
                    splineF._startPosition = startingPos;

                    if (this.transform.position.y >= result.position.y)
                    {
                        if (Mathf.RoundToInt(result.forward.normalized.z) > Mathf.RoundToInt(orientation.transform.forward.normalized.z))
                        {
                            splineF.direction = Spline.Direction.Backward;
                        }
                        else
                            splineF.direction = Spline.Direction.Forward;
                        Grind();
                    }

                }
            }
        }      
        
    }

    public void Grind()
    {
        pCollider.isTrigger = true;
        sc.canMove = false;
        Vector3 flatVel = new Vector3(pm.rb.velocity.x, 0f, pm.rb.velocity.z);
        sc.grinding = true;
        splineF._startPosition = startingPos;
        splineF.SetPercent(splineF._startPosition);
        splineF.follow = true;
        storedvel = sc.currentSpeed;
        splineF.followSpeed = sc.currentSpeed * (int)splineF.direction;

    }
    public void EndGrindForward()
    {
        if (splineF.direction == Spline.Direction.Forward)
        {
            EndGrind();
        }

    }
    public void EndGrindBackwards()
    {
        if(splineF.direction == Spline.Direction.Backward)
        {
            EndGrind();
        }

    }

    public void EndGrind()
    {
        sc.canMove = true;
        splineF.follow = false;
        sc.grinding = false;
        rb.transform.position = transform.position + transform.up * 0.3f;
        rb.velocity = storedvel * transform.forward * 3;
        rb.velocity = new Vector3(rb.velocity.x,20, rb.velocity.z);
        pm.anim.SetTrigger("Jump");
        cd_countdown = grind_cd;
    }
}
