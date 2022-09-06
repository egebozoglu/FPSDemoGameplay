using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables
    public GameObject player;
    Animation animationPlayer;
    string animationName;
    public bool dead = false;
    Vector3 directionOfPlayer;
    bool sawPlayer = false;
    float sawRate = 0f;
    float sawTime = 1f;
    CapsuleCollider capsuleCollider;
    float fireRate = 0f;
    float fireTime = .2f;
    public GameObject fireSoundPrefab;
    public GameObject exitPoint;
    public GameObject muzzleSmokePrefab;
    Vector3 firstPos;
    Vector3 secondPos;
    Vector3 targetPos;

    public bool walkable = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = transform.GetComponent<CapsuleCollider>();
        animationPlayer = transform.GetComponent<Animation>();
        animationName = "enemyIdle";
        firstPos = transform.position;
        secondPos = transform.position + transform.forward * 7;
        targetPos = secondPos;
    }

    private void FixedUpdate()
    {
        AI();
    }

    #region AI Section
    void AI()
    {
        if (!dead)
        {
            if (IsLineOfSight() && IsFront())
            {
                transform.rotation = Quaternion.LookRotation(directionOfPlayer.normalized);
                sawRate += Time.deltaTime;
                if (sawRate>sawTime)
                {
                    sawPlayer = true;
                }
                if (sawPlayer)
                {
                    
                    Fire();
                }
                animationName = "enemyAim";
            }
            else
            {
                if (walkable)
                {
                    sawRate = 0f;
                    sawPlayer = false;
                    if (transform.position == targetPos && targetPos == firstPos)
                    {
                        targetPos = secondPos;
                        transform.Rotate(0f, 180f, 0f);
                    }
                    else if (transform.position == targetPos && targetPos == secondPos)
                    {
                        targetPos = firstPos;
                        transform.Rotate(0f, 180f, 0f);
                    }

                    transform.position = Vector3.MoveTowards(transform.position, targetPos, 1.5f * Time.deltaTime);

                    animationName = "enemyWalk";
                }
                else
                {
                    animationName = "enemyIdle";
                }
                
            }

            animationPlayer.Play(animationName);
        }
    }

    bool IsFront()
    {
        Vector3 directionOfPlayer = transform.position - player.transform.position;
        float angle = Vector3.Angle(transform.forward, directionOfPlayer);

        if (Mathf.Abs(angle) >100 && Mathf.Abs(angle)<260)
        {
            Debug.DrawLine(transform.position, player.transform.position, Color.red);
            return true;
        }

        return false;
    }

    bool IsLineOfSight()
    {
        RaycastHit hit;
        directionOfPlayer = player.transform.position - transform.position;

        if (Physics.Raycast(transform.position, directionOfPlayer, out hit, 20f))
        {
            if (hit.transform.gameObject == player)
            {
                Debug.DrawLine(transform.position, player.transform.position, Color.green);
               
                // rotasyon yaptýr
                return true;
            }
        }

        return false;
    }
    #endregion


    public void KIA()
    {
        if (!dead)
        {
            dead = true;
            capsuleCollider.enabled = false;
            Debug.Log("Dead");
            System.Random rand = new System.Random();
            animationName = "enemyDeath" + rand.Next(1, 4).ToString();
            animationPlayer.Play(animationName);
            GameHandler.instance.enemyCount--;
        }
    }

    #region Fire Section
    void Fire()
    {
        fireRate += Time.deltaTime;
        if (fireRate>fireTime)
        {
            fireRate = 0f;
            FireSound();
            InstantiateMuzzleSmoke();
            //animationName = "enemyFire";
        }
    }

    void FireSound()
    {
        GameObject fireSound;

        fireSound = Instantiate(fireSoundPrefab, transform.position, Quaternion.identity);

        Destroy(fireSound.gameObject, 1f);
    }

    void InstantiateMuzzleSmoke()
    {
        GameObject muzzleSmoke;

        muzzleSmoke = Instantiate(muzzleSmokePrefab, exitPoint.transform.position + new Vector3(0f, 0.02f, 0f), Quaternion.identity);

        Destroy(muzzleSmoke, 1.5f);
    }
    #endregion
}
