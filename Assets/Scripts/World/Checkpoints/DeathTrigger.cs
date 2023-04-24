using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{
    public Transform respawnPos;
    PlayerInteract player;

    private void Start()
    {
        player = FindObjectOfType<PlayerInteract>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            //player.transform.position = respawnPos.position;
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.transform.position = respawnPos.position;
            SceneManager.LoadScene("SampleScene");
        }
    }
}
