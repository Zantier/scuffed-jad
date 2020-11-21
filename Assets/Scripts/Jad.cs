using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jad : MonoBehaviour
{
    const float squareWidth = 1.0f;

    int TickCount;
    ProtectPrayer State;
    Animator Animator;
    Vector2Int pos = new Vector2Int(-3, 5);
    AudioSource audioSource;

    public AudioClip audioMagic;
    public AudioClip audioExplode;

    /// <summary>
    /// Tick jad, and get what kind of damage he's doing
    /// </summary>
    public ProtectPrayer Tick(Vector2Int playerPos)
    {
        ProtectPrayer result = ProtectPrayer.None;
        TickCount++;

        switch (State)
        {
            case ProtectPrayer.None:
                if (TickCount >= 3)
                {
                    int distX = Mathf.Abs(playerPos.x - pos.x);
                    int distY = Mathf.Abs(playerPos.y - pos.y);
                    if (distX >= 3 || distY >= 3)
                    {
                        TickCount = 0;
                        int rangeMax = 3;
                        if ((distX == 3 || distY == 3) && distX + distY < 6)
                        {
                            rangeMax++;
                        }

                        State = (ProtectPrayer)Random.Range(1, rangeMax);
                    }
                }
                break;
            case ProtectPrayer.Magic:
                if (TickCount == 1)
                {
                    Animator.SetTrigger("magic");
                    audioSource.PlayOneShot(audioMagic);
                }
                else if (TickCount == 4)
                {
                    result = ProtectPrayer.Magic;
                }
                else if (TickCount == 5)
                {
                    TickCount = 0;
                    State = ProtectPrayer.None;
                }
                break;
            case ProtectPrayer.Ranged:
                if (TickCount == 1)
                {
                    Animator.SetTrigger("ranged");
                }
                else if (TickCount == 4)
                {
                    result = ProtectPrayer.Ranged;
                    audioSource.PlayOneShot(audioExplode);
                }
                else if (TickCount == 5)
                {
                    TickCount = 0;
                    State = ProtectPrayer.None;
                }
                break;
            case ProtectPrayer.Melee:
                if (TickCount == 1)
                {
                    TickCount = 0;
                    result = ProtectPrayer.Melee;
                    State = ProtectPrayer.None;
                    Animator.SetTrigger("melee");
                }
                break;
        }

        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        transform.position = new Vector3((pos.x + 0.5f) * squareWidth, 0, (pos.y + 0.5f) * squareWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
