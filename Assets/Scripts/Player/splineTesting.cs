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

    Tricking trickingManager;
    public SkateTricks[] grindTricks;


    [Header("===============Tricking===============")]
    public bool darkSlide;

    void Start()
    {
        splineF = GetComponent<SplineFollower>();
        cd_countdown = 0;
        sc = GetComponent<SkateController>();
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        secondcam = secondcamobj.GetComponent<GrindCamera>();
        brain = maincam.GetComponent<CinemachineBrain>();

        trickingManager = FindObjectOfType<Tricking>();
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
            trickingManager.scoreM.curretnScore += (int)(grindTricks[0].scoreAwarded);
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
                splineF.RebuildImmediate();
                sp.Project(pTransform, ref result, from = 0, to = 1);
                startingPos = result.percent;
                splineF._startPosition = startingPos;

                if (this.transform.position.y >= result.position.y)
                {
                    if (Vector3.Angle(sc.groundOrientation.forward, result.forward) > 90)
                    {
                        splineF.direction = Spline.Direction.Backward;
                    }
                    else
                        splineF.direction = Spline.Direction.Forward;

                    AudioManager.instance.PlayOneShot(FMODEvents.instance.grinding);
                    Grind();

                }

                SelectGrindType();
                //If flipTicking then boardslide        
                //trickingManager.currentTrick = grindTricks[0];

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
        //secondcam.active = true;
        //brain.m_DefaultBlend.m_Time = 1f;

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
        AudioManager.instance.StopSound(FMODEvents.instance.grinding);

        sc.canMove = true;
        splineF.follow = false;
        sc.grinding = false;
        rb.transform.position +=  transform.up * 0.3f;
        rb.velocity = storedvel * transform.forward * 3;
        rb.velocity = new Vector3(rb.velocity.x,20, rb.velocity.z);
        pm.anim.SetTrigger("Jump");
        cd_countdown = grind_cd;

        //secondcam.active = false;
        //brain.m_DefaultBlend.m_Time = 1;
    }


    public void SelectGrindType()
    {
        Vector3 playerDirection = sc.orientation.transform.forward;
        float angle = Vector3.Angle(playerDirection, result.forward);

        //Find if the player is at the right or the left of the rail

        if (trickingManager.flipTricking) { darkSlide = true; }
        else { darkSlide = false; }

        if (angle >= 0 && angle < 45)
        {
            if (sc.verticalInput > 0 || sc.verticalInput == 0) { Debug.Log("50/50"); }
            else if(sc.verticalInput < 0) { Debug.Log("5-0"); }
        }
        else if (angle >= 45 && angle < 135)
        {
            Debug.Log("Tailgrind");
        }
        else if (angle >= 135 && angle < 225)
        {
            Debug.Log("Backwards Grind");
        }
        else if (angle >= 225 && angle < 360)
        {
            Debug.Log("Nosegrind");
        }
    }
}
