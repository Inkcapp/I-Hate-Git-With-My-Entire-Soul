using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace KUNAI.Player {
public class PlayerAnimation : MonoBehaviour
{
    private Animator mAnimator;
    
    PlayerMovement playerMove;
    PlayerKunai playerKunai;
    public GameObject player;
    
    public GameObject smokeTop;
    public GameObject smokeBottom;
    public GameObject swordTrail;

    void Awake()
    {
      mAnimator = GetComponent<Animator>(); 
      playerMove = player.GetComponent<PlayerMovement>();
      playerKunai = player.GetComponent<PlayerKunai>();
    }

    void Start()
    {
        StartCoroutine(WaitForFunction());
    }

    IEnumerator WaitForFunction()
    {
        yield return new WaitForSeconds(0.2f); 
        swordTrail.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        if(mAnimator != null)
        {
            if(Input.GetMouseButtonDown(1))
            {
                mAnimator.SetTrigger("trThrow");
            }
            else
            {
                mAnimator.ResetTrigger("trThrow");
            }

            if(Input.GetMouseButtonDown(0))
            {
                mAnimator.SetTrigger("trAttack");
                swordTrail.SetActive(true);
                StartCoroutine(WaitForFunction());
            }
            else
            {
                mAnimator.ResetTrigger("trAttack");
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                mAnimator.SetTrigger("trRecall");
                smokeTop.GetComponent<VisualEffect>().Play();
                smokeBottom.GetComponent<VisualEffect>().Play();
            }
            else
            {
                mAnimator.ResetTrigger("trRecall");
            }

            if(Input.GetKey(KeyCode.Space))
            {
                mAnimator.SetTrigger("trJump");
            }
            else
            {
                mAnimator.ResetTrigger("trJump");
            }

            if (playerMove.onGround == true)
            {
                mAnimator.SetBool("inAir", false);
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    mAnimator.SetBool("isRunning", true);
                }
                else
                {
                    mAnimator.SetBool("isRunning", false);
                }
            }
            
            if(playerMove.onGround == false)
            {
                mAnimator.SetBool("inAir", true);
            }

            if(playerMove.wallLeft == true)
            {
                mAnimator.SetBool("isWallRunning", true);
                mAnimator.SetBool("inAir", false);
                mAnimator.SetTrigger("trWallLeft");
            }
            else
            {
                mAnimator.SetBool("isWallRunning", false);
                mAnimator.ResetTrigger("trWallLeft");
            }

            if(playerMove.wallRight == true)
            {
                mAnimator.SetBool("isWallRunning", true);
                mAnimator.SetBool("inAir", false);
                mAnimator.SetTrigger("trWallRight");
            }
            else
            {
                mAnimator.SetBool("isWallRunning", false);
                mAnimator.ResetTrigger("trWallRight");
            }
        }
    }
}
}