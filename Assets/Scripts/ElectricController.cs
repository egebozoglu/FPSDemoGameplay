using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricController : MonoBehaviour
{
    public GameObject lights;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Contains("Bullet"))
        {
            lights.SetActive(false);
        }
    }
}
