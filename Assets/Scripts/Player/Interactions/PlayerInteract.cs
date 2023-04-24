using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Atributes")]
    public float distance;

    [Header("References")]
    public LayerMask interactObjectMask;
    public LayerMask untaggedMask;
    UnityEvent onInteract;
    public GameObject obj;
    public Transform orientation;
    public PlayerInput _playerInput;

    private void Start()
    {
        //FindObjectOfType<AudioManager>().PlaySound("Song");
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Physics.Raycast(rayPos, orientation.forward, out hit, distance, interactObjectMask))
        {
            if (hit.collider.GetComponent<InteractObjects>() != false)
            {
                Debug.Log("Colliding with: " + hit.collider);
                onInteract = hit.collider.GetComponent<InteractObjects>().onInteract;

                obj = hit.collider.gameObject;

                if (_playerInput.actions["Interact"].WasPressedThisFrame())
                {
                    onInteract.Invoke();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward, Color.green, distance);
    }
}
