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
    public SplineSample result;
    public double startingPos;
    public double from = 0;
    public double to = 1;
    public float storedvel;
    [Range(-100, 100)] public float balance;

    // Start is called before the first frame update
    void Start()
    {
        sc = GetComponent<SkateController>();
        pm = GetComponent<PlayerMovement>();
        splineF = GetComponent<SplineFollower>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
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
                Debug.Log(startingPos);               

                if(this.transform.position.y >= result.position.y)
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

    public void Grind()
    {
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
        splineF.follow = false;
        sc.grinding = false;
        GetComponent<Rigidbody>().velocity = storedvel * transform.forward;
        GetComponent<Rigidbody>().AddForce(transform.up *15, ForceMode.Impulse);
        pm.anim.SetTrigger("Jump");
    }
}
