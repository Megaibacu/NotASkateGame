using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Customizationshop : MonoBehaviour
{
    //Cada tienda requiere que le arrastres el player, la camara y la shopUI
    //Este scrit se encuentra en la triggerzone de cada tienda

    //objetos que hay que arrastrar desde la escena
    public GameObject Skate;
    public GameObject skateimagereal;
    public GameObject playercam;
    public GameObject shopui;
    public GameObject player;
    public PlayerInput _playerInput;

    public CinemachineVirtualCamera shopcam;

    public GameObject mockskate;
    public GameObject mockskateimage;
    public GameObject keyprompt;
    NewAudioManager manager;

    Image skatecolor;
    Image skateimage;
    Image blackbox;

    Vector3 originalposition;

    public Image[] blackarrows;
    public Sprite[] skateimages;
 

    public bool isshoping;
    public bool isclose;
    public bool ispaused;

    float transitioning;
    float blackboxtimer;
    float rightarrowtimer;
    float leftarrowtimer;
    public float isbrowsing;

    int number;
    public int currentimage;
    public int selectedimage;
    public int currentmaterial;
    public int selectedmaterial;
    void Start()
    {
        skatecolor = shopui.transform.GetChild(1).GetComponent<Image>();
        skateimage = shopui.transform.GetChild(7).GetComponent<Image>();
        blackbox = shopui.transform.GetChild(0).GetComponent<Image>();
        originalposition = blackbox.transform.position;
        blackarrows[0] = shopui.transform.GetChild(2).GetComponent<Image>();
        blackarrows[1] = shopui.transform.GetChild(3).GetComponent<Image>();
        blackarrows[2] = shopui.transform.GetChild(8).GetComponent<Image>();
        blackarrows[3] = shopui.transform.GetChild(9).GetComponent<Image>();
        manager = GetComponent<NewAudioManager>();
        shopui.GetComponent<Finderscript>().numberofobjects--;
        //la camara del jugador deberia ser idealmente una virtual cam, este script es algo chusco ahora
        shopcam.Priority = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            ispaused = true;
        }
        else
        {
            ispaused = false;
        }

        if (transitioning > 0)
        {
            transitioning -= Time.deltaTime;
        }

        if(_playerInput.actions["Interact"].WasPressedThisFrame() && isclose == true && transitioning <= 0)
        {
            
            changecams();
        }

        if (isshoping == true && transitioning <= 0)
        {
            mockskate.SetActive(true);
            shopui.SetActive(true);
            if (ispaused == false)
            {
                browse();
            }
            changecolorandimage("Mock");
            placeblackbox();
            placearrows();
        }
    }
    public void changecams()
    {
        if (isshoping == false)
        {
            manager.PlaySound("Enter");
            isshoping = true;
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<StateChange>().enabled = false;
            playercam.GetComponent<ThirdPersonCam>().enabled = false;
            shopcam.Priority = 11;
            currentmaterial = selectedmaterial;
            currentimage = selectedimage;
            transitioning = 2;
            player.GetComponent<Animator>().SetFloat("XSpeed", 0);
            _playerInput.SwitchCurrentActionMap("ShopControls");
        }
        else
        {
            shopcam.Priority = 1;
            isshoping = false;
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<StateChange>().enabled = true;
            playercam.GetComponent<ThirdPersonCam>().enabled = true;
            transitioning = 2;
            shopui.SetActive(false);
            blackboxtimer = 0;
            rightarrowtimer = 0;
            leftarrowtimer = 0;
            _playerInput.SwitchCurrentActionMap("Gameplay_p");
            mockskate.SetActive(false);
            changecolorandimage("Real");
        }
    }
    public void browse ()
    {
        if (_playerInput.actions["Right"].WasPressedThisFrame())
        {
            if (isbrowsing == 0)
            {
                currentmaterial++;
                if (currentmaterial > 2)
                {
                    currentmaterial = 0;
                }
            }
            else
            {
                currentimage++;
                if (currentimage > 2)
                {
                    currentimage = 0;
                }
            }
            rightarrowtimer = 0.3f;
            manager.PlaySound("Arrow");
        }
        if (_playerInput.actions["Left"].WasPressedThisFrame())
        {
            if (isbrowsing == 0)
            {
                currentmaterial--;
                if (currentmaterial < 0)
                {
                    currentmaterial = 2;
                } 
            }
            else
            {
                currentimage--;
                if (currentimage < 0)
                {
                    currentimage = 2;
                }
            }
            leftarrowtimer = 0.3f;
            manager.PlaySound("Arrow");
        }
        if (_playerInput.actions["Up"].WasPressedThisFrame())
        {
            isbrowsing++;
            if (isbrowsing > 1)
            {
                isbrowsing = 0;
            }
        }
        if (_playerInput.actions["Down"].WasPressedThisFrame())
        {
            isbrowsing--;
            if (isbrowsing < 0)
            {
                isbrowsing = 1;
            }
        }
        if (_playerInput.actions["Select"].WasPressedThisFrame())
        {
            if (isbrowsing == 0)
            {
                selectedmaterial = currentmaterial;
            }
            else
            {
                selectedimage = currentimage;
            }
            manager.PlaySound("Select");
            blackboxtimer = 0.4f;
        }

        //esto cambia el color del icono de la UI
        iconchange();
    }
    public void iconchange()
    {
        if (currentmaterial == 0)
        {
            skatecolor.color = new Color(255, 255, 255);
        }
        if (currentmaterial == 1)
        {
            skatecolor.color = new Color(0, 164, 255);
        }
        if (currentmaterial == 2)
        {
            skatecolor.color = new Color(0, 255, 0);
        }

        skateimage.sprite = skateimages[currentimage];
        if (currentimage == 0)
        {
            mockskateimage.GetComponent<Image>().enabled = false;
        }
        else
        {
            mockskateimage.GetComponent<Image>().enabled = true;
            mockskateimage.GetComponent<Image>().sprite = skateimages[currentimage];
        }
    }
    public void changecolorandimage(string skatetype)
    {
        if (skatetype == "Real")
        {
            if (selectedmaterial == 0)
            {
                Skate.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
            }
            if (selectedmaterial == 1)
            {
                Skate.GetComponent<Renderer>().material.color = new Color(0, 98, 255);
            }
            if (selectedmaterial == 2)
            {
                Skate.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            }
        }
        else
        {
            if (currentmaterial == 0)
            {
                mockskate.GetComponent<Renderer>().material.color = Color.white;
            }
            if (currentmaterial == 1)
            {
                mockskate.GetComponent<Renderer>().material.color = new Color(0, 98, 255);
            }
            if (currentmaterial == 2)
            {
                mockskate.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            }
        }

        skateimagereal.GetComponent<Image>().sprite = skateimages[selectedimage];
    }
    public void placeblackbox()
    {
        if (isbrowsing == 0)
        {
            blackbox.transform.position = originalposition;
        }
        else
        {
            blackbox.transform.position = new Vector3(originalposition.x, skateimage.transform.position.y, originalposition.z);
        }
        if (blackboxtimer > 0)
        {
            blackboxtimer -= Time.deltaTime;
            blackbox.rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else
        {
            blackbox.rectTransform.sizeDelta = new Vector2(90, 90);
        }
    }
    public void placearrows()
    {
        if (rightarrowtimer > 0)
        {
            if (isbrowsing == 0)
            {
                blackarrows[0].enabled = true;
            }
            else
            {
                blackarrows[2].enabled = true;
            }
            rightarrowtimer -= Time.deltaTime;
        }
        else
        {
            blackarrows[0].enabled = false;
            blackarrows[2].enabled = false;
        }

        if (leftarrowtimer > 0)
        {
            if (isbrowsing == 0)
            {
                blackarrows[1].enabled = true;
            }
            else
            {
                blackarrows[3].enabled = true;
            }
            leftarrowtimer -= Time.deltaTime;
        }
        else
        {
            blackarrows[1].enabled = false;
            blackarrows[3].enabled = false;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            isclose = true;
            if (isshoping == false)
            {
                keyprompt.SetActive(true);
            }
            else
            {
                keyprompt.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            isclose = false;
        }
    }

}
