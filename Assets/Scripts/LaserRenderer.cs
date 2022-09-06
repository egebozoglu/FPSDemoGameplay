using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRenderer : MonoBehaviour
{
    public Camera fpsCamera;
    private LineRenderer lr;
    public Vector3 laserBulletPoint;
    public static LaserRenderer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100)));
        lr.SetPosition(0, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }
        }
        else
        {
            lr.SetPosition(1, transform.forward * 5000);
        }

        laserBulletPoint =   lr.GetPosition(1);
    }
}
