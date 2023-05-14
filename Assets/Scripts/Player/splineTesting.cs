using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Cinemachine;


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
    public float grindRadious;
    [Range(-100, 100)] public float balance;
    public LayerMask grindable;
    public Collider[] grindables;
    private float smoothing = 1;
    private Vector3 offset;
    private Vector3 pTransform;

    //This is for changing the camera while you grind
    public GameObject secondcamobj;
    public GameObject maincam;
    GrindCamera secondcam;
    CinemachineBrain brain;


    void Start()
    {
        cd_countdown = 0;
        sc = GetComponent<SkateController>();
        pm = GetComponent<PlayerMovement>();
        splineF = GetComponent<SplineFollower>();
        rb = GetComponent<Rigidbody>();
        secondcam = secondcamobj.GetComponent<GrindCamera>();
        brain = maincam.GetComponent<CinemachineBrain>();
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
        grindables = Physics.OverlapSphere(transform.position, grindRadious, grindable);

        if (sc.grinding && splineF.motion.offset.magnitude > 0)
        {
            
            splineF.motion.offset = Vector2.Lerp(Vector2.zero, splineF.motion.offset, smoothing -= Time.deltaTime * 2);
        }
        else if (!sc.grinding)
        {
            smoothing = 1;
        }
    }

    public void StartGrind()
    {
        if (!sc.grinding && cd_countdown == 0 && GetComponent<StateChange>().state == States.skating && grindables.Length != 0)
        {
            if (grindables[0].transform.tag == "Spline")
            {
                        pTransform = transform.position;
                        sp = grindables[0].gameObject.GetComponent<SplineComputer>();
                        splineF.spline = sp;
                        sp.Project(pTransform, ref result, from = 0, to = 1);
                        startingPos = result.percent;
                        splineF._startPosition = startingPos;

                        if (this.transform.position.y >= result.position.y)
                        {
                            if (Vector3.Angle(orientation.transform.forward, result.forward) > 90)
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
        SplineSample projection = new SplineSample();
        splineF.Project(pTransform, ref projection);
        Matrix4x4 worldToSpline = Matrix4x4.TRS(projection.position, projection.rotation, Vector3.one * projection.size).inverse;
        offset = worldToSpline.MultiplyPoint(pTransform);
        splineF.motion.offset = offset;

        pCollider.isTrigger = true;
        sc.canMove = false;
        Vector3 flatVel = new Vector3(pm.rb.velocity.x, 0f, pm.rb.velocity.z);
        sc.grinding = true;
        splineF._startPosition = startingPos;
        splineF.SetPercent(splineF._startPosition);
        splineF.RebuildImmediate();
        splineF.follow = true;
        storedvel = sc.currentSpeed;
        splineF.followSpeed = sc.currentSpeed * (int)splineF.direction;

        //This is for the camera
        secondcam.active = true;
        brain.m_DefaultBlend.m_Time = 3.4f;
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
        rb.transform.position +=  transform.up * 0.3f;
        rb.velocity = storedvel * transform.forward * 3;
        rb.velocity = new Vector3(rb.velocity.x,20, rb.velocity.z);
        pm.anim.SetTrigger("Jump");
        cd_countdown = grind_cd;

        secondcam.active = false;
        brain.m_DefaultBlend.m_Time = 5;
    }
}
