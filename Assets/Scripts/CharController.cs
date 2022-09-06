using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    #region Variables
    [Header("Movement Section")]
    public float speed = 2f;
    string animationClip = "M4IdleAnimation";
    int vertical, horizontal;
    public Camera fpsCamera;
    public AudioSource walkingAudio;
    public AudioClip walkingClip;

    [Header("Firing Section")]
    public bool aiming = false;
    public bool firing = false;
    public GameObject arms;
    public GameObject crosshair;
    public GameObject bulletPrefab;
    public GameObject m4ExitPoint;
    public GameObject suppressedSoundPrefab;
    public GameObject casePrefab;
    public GameObject caseExitPoint;
    public GameObject muzzleSmokePrefab;
    float fireRate = 0.12f;
    float fireTime = 0.1f;
    bool reload = false;
    float reloadRate = 0f;
    float reloadTime = 1.2f;
    public GameObject reloadSoundPrefab;

    [Header("Night Vision Section")]
    bool nightVision = false;
    public GameObject ligths;
    public GameObject fogVolume;
    public GameObject charLight;
    public GameObject postProcessVolume;
    public GameObject laser;
    float visionRate = 0.21f;
    float visionTime = 0.2f;
    bool visionClickEnable = true;
    public GameObject nightVisionSoundPrefab;

    [Header("Camera Movement Section")]
    public float horizontalSpeed = 0.5f;
    public float verticalSpeed = 0.5f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            ligths.SetActive(false);
        }
        if (Input.GetKey(KeyCode.L))
        {
            ligths.SetActive(true);
        }

        if (!visionClickEnable)
        {
            visionRate += Time.deltaTime;
            if (visionRate>visionTime)
            {
                visionClickEnable = true;
                visionRate = 0.21f;
            }
        }

        Aiming();
        Firing();
        NightVisionActivating();
        Reload();
    }

    private void FixedUpdate()
    {
        Movement();
        CameraMovement();

        if (transform.position.y > 1.8f)
        {
            transform.position = new Vector3(transform.position.x, 1.8f, transform.position.z);
        }
    }

    #region Movement
    void Movement()
    {
        #region Vertical Movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                vertical = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                vertical = -1;
            }
        }
        else
        {
            vertical = 0;
        }
        #endregion

        #region Horizontal Movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
            }
        }
        else
        {
            horizontal = 0;
        }
        #endregion

        #region Walking Sound
        if (horizontal != 0 || vertical != 0)
        {
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.clip = walkingClip;
                walkingAudio.Play();
            }
        }
        else
        {
            walkingAudio.Pause();
        }
        #endregion

        if (horizontal != 0 && vertical != 0)
        {
            speed = 2.25f;
        }
        else
        {
            speed = 3f;
        }

        transform.Translate(Vector3.forward * vertical * speed * Time.deltaTime);
        transform.Translate(Vector3.right * horizontal * speed * Time.deltaTime);

        if (!aiming && !firing)
        {
            if (!reload)
            {
                if (vertical == 0 && horizontal == 0)
                {

                    animationClip = "M4IdleAnimation";
                }
                else
                {
                    animationClip = "M4WalkAnimation";
                }
            }
            transform.gameObject.GetComponent<Animation>().Play(animationClip);
        }
    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);

        transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }
    #endregion

    #region Fire
    void Aiming()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (aiming)
            {
                laser.SetActive(false);
                arms.gameObject.transform.position = transform.position + new Vector3(-0.1048066f, 0.07213533f, -0.3140207f);
                arms.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                crosshair.SetActive(true);
                aiming = false;
            }
            else
            {
                crosshair.SetActive(false);
                switch (nightVision)
                {
                    case true:
                        laser.SetActive(true);
                        animationClip = "M4NightAimingAnimation";
                        break;
                    case false:
                        animationClip = "M4AimingAnimation";
                        break;
                }
                transform.gameObject.GetComponent<Animation>().Play(animationClip);
                aiming = true;
            }
        }
    }

    void Firing()
    {
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            firing = true;
            if (aiming)
            {
                switch (nightVision)
                {
                    case true:
                        animationClip = "M4NightAimingFireAnimation";
                        break;
                    case false:
                        animationClip = "M4AimingFireAnimation";
                        break;
                }
            }
            else
            {
                animationClip = "M4FireAnimation";
            }
            
            transform.gameObject.GetComponent<Animation>().Play(animationClip);
            //InstantiateMuzzleFlash();
            //InstantiateBullet();
            RaycastFire();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            fireRate = 0.12f;
            firing = false;
            if (aiming)
            {
                transform.gameObject.GetComponent<Animation>().Stop();
                //switch (nightVision)
                //{
                //    case true:
                //        arms.gameObject.transform.position = new Vector3(0f, 1.8f, 0f) + new Vector3(-0.057f, -0.002f, -0.312f); //transform.position
                //        break;
                //    case false:
                //        arms.gameObject.transform.position = new Vector3(0f, 1.8f, 0f) + new Vector3(-0.21f, 0.101f, -0.318f); //transform.position
                //        break;
                //}
            }
        }
    }

    void RaycastFire()
    {
        fireRate += Time.deltaTime;
        if (fireRate>fireTime)
        {
            fireRate = 0f;

            RaycastHit hit;

            Vector3 directionOfTarget = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100)) - m4ExitPoint.transform.position;

            if (Physics.Raycast(m4ExitPoint.transform.position, directionOfTarget, out hit, 100f))
            {
                if (hit.transform.gameObject.tag == "enemy")
                {
                    hit.transform.GetComponent<EnemyController>().KIA();
                    WallShoot(hit.transform.position, fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100)) - hit.transform.position);
                }
                else if (hit.transform.gameObject.tag == "electric")
                {
                    ligths.SetActive(false);
                }
            }

            SuppressedSound();
            InstantiateCase();
            InstantiateMuzzleSmoke();
        }
    }

    void WallShoot(Vector3 from, Vector3 to)
    {
        RaycastHit hit;

        if (Physics.Raycast(from, to, out hit, 100f))
        {
            if (hit.transform.gameObject.tag == "enemy")
            {
                hit.transform.GetComponent<EnemyController>().KIA();
            }
            else if (hit.transform.gameObject.tag == "electric")
            {
                ligths.SetActive(false);
            }
        }
    }

    void InstantiateMuzzleSmoke()
    {
        GameObject muzzleSmoke;

        muzzleSmoke = Instantiate(muzzleSmokePrefab, m4ExitPoint.transform.position + new Vector3(0f, 0.02f, 0f), Quaternion.identity);

        Destroy(muzzleSmoke, 1.5f);
    }

    void InstantiateCase()
    {
        GameObject caseObject;

        caseObject = Instantiate(casePrefab, caseExitPoint.transform.position, transform.rotation);

        Destroy(caseObject, 3f);
    }

    void Reload()
    {
        if (reload)
        {
            reloadRate += Time.deltaTime;
            if (reloadRate>reloadTime)
            {
                reloadRate = 0f;
                reload = false;
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (!reload)
            {
                ReloadSound();
                reload = true;
                animationClip = "M4ReloadAnimation";
            }
        }
    }

    void NightVisionActivating()
    {
        if (Input.GetKey(KeyCode.N))
        {
            if (visionClickEnable)
            {
                if (!nightVision)
                {
                    NightVisionSound();
                    visionClickEnable = false;
                    visionRate = 0f;
                    fogVolume.SetActive(false);
                    postProcessVolume.SetActive(true);
                    charLight.SetActive(true);
                    nightVision = true;
                }
                else
                {
                    visionClickEnable = false;
                    visionRate = 0f;
                    postProcessVolume.SetActive(false);
                    charLight.SetActive(false);
                    fogVolume.SetActive(true);
                    nightVision = false;
                }
            }
        }
    }

    //void InstantiateBullet()
    //{
    //    fireRate += Time.deltaTime;
    //    if (fireRate>fireTime)
    //    {
    //        fireRate = 0;
    //        GameObject bullet;

    //        bullet = Instantiate(bulletPrefab, m4ExitPoint.transform.position, Quaternion.identity);
    //        SuppressedSound();
    //        InstantiateCase();

    //        if (nightVision && aiming)
    //        {
    //            Vector3 direction = LaserRenderer.instance.laserBulletPoint - m4ExitPoint.transform.position;
    //            bullet.transform.rotation = Quaternion.LookRotation(direction.normalized);
    //            //bullet.transform.LookAt(LaserRenderer.instance.laserBulletPoint);
    //        }
    //        else
    //        {
    //            Vector3 direction = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100)) - m4ExitPoint.transform.position;
    //            bullet.transform.rotation = Quaternion.LookRotation(direction.normalized);
    //            //bullet.transform.LookAt(fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100)));
    //        }
    //    }
    //}

    #endregion

    #region Sound Effects
    void NightVisionSound()
    {
        GameObject nightVisionSound;

        nightVisionSound = Instantiate(nightVisionSoundPrefab, transform.position, Quaternion.identity);

        Destroy(nightVisionSound, 2f);
    }

    void SuppressedSound()
    {
        GameObject suppressedSound;

        suppressedSound = Instantiate(suppressedSoundPrefab, transform.position, Quaternion.identity);

        Destroy(suppressedSound, 2f);
    }

    void ReloadSound()
    {
        GameObject reloadSound;

        reloadSound = Instantiate(reloadSoundPrefab, transform.position, Quaternion.identity);

        Destroy(reloadSound, 2f);
    }
    #endregion
}
