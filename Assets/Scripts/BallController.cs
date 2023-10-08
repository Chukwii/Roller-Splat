using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 CurrentSwipe;

    private Color solveColor;
    public ParticleSystem trail;
    private AudioSource gameMusic;
    public AudioClip dash;

    private void Start()
    {
        solveColor = Random.ColorHSV();
        GetComponent<MeshRenderer>().material.color = solveColor;
        trail.GetComponent<Renderer>().material.color = solveColor;
        gameMusic = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitcolliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitcolliders.Length)
        {
            GroundPiece ground = hitcolliders[i].transform.GetComponent<GroundPiece>(); 
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
                gameMusic.PlayOneShot(dash, 0.6f);
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTraveling)
            return;

        if (Input.GetMouseButton(0))
        {
            trail.Play();
            
            swipePosCurrentFrame = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

            if(swipePosLastFrame != Vector2.zero)
            {
                CurrentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if(CurrentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                CurrentSwipe.Normalize();

                // UP/DOWN
                if(CurrentSwipe.x > -0.5f && CurrentSwipe.x < 0.5)
                {
                    // GO UP/DOWN
                    SetDestinatin(CurrentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                    if(CurrentSwipe.y > 0)
                    {
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        transform.rotation = rotation;
                    }
                    else if(CurrentSwipe.y < 0)
                    {
                        Quaternion rotation = Quaternion.Euler(0, 180, 0);
                        transform.rotation = rotation;
                    }
                }

                if (CurrentSwipe.y > -0.5f && CurrentSwipe.y < 0.5)
                {
                    // GO Left/Right
                    SetDestinatin(CurrentSwipe.x > 0 ? Vector3.right : Vector3.left);
                    if(CurrentSwipe.x > 0)
                    {
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        transform.rotation = rotation;
                    }
                    else if(CurrentSwipe.x < 0)
                    {
                        Quaternion rotation = Quaternion.Euler(0, -90, 0);
                        transform.rotation = rotation;
                    }
                }
            }

            swipePosLastFrame = swipePosCurrentFrame;
        }

        if(Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            CurrentSwipe = Vector2.zero;
        }
    }

    private void SetDestinatin(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }

    
}
