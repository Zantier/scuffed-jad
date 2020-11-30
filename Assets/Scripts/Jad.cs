using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jad : MonoBehaviour
{
    const float squareWidth = 1.0f;

    /// <summary>
    /// How many ticks have we been in the current state
    /// </summary>
    int TickCount;
    ProtectPrayer State;
    Animator Animator;
    AudioSource audioSource;

    public Vector2Int Pos = new Vector2Int(-3, 5);
    /// <summary>
    /// How many ticks between ranged attacks
    /// </summary>
    public int RangedSpeed = 8;
    public int MeleeSpeed = 4;
    public AudioClip AudioMagic;
    public AudioClip AudioExplode;
    public GameObject ProjectilePrefab;
    public Transform PlayerTransform;

    /// <summary>
    /// Delay Jad from attacking at the start for this many ticks
    /// </summary>
    public void SetTickDelay(int count)
    {
        TickCount -= count;
    }

    /// <summary>
    /// Tick jad, and get what kind of damage he's doing
    /// </summary>
    public ProtectPrayer Tick(Vector2Int playerPos)
    {
        ProtectPrayer result = ProtectPrayer.None;
        TickCount++;

        bool doAttack = State == ProtectPrayer.None && TickCount > 0 ||
            State == ProtectPrayer.Melee && TickCount > MeleeSpeed ||
            TickCount > RangedSpeed;

        if (doAttack)
        {
            int distX = Mathf.Abs(playerPos.x - Pos.x);
            int distY = Mathf.Abs(playerPos.y - Pos.y);
            if (distX >= 3 || distY >= 3)
            {
                TickCount = 1;
                int rangeMax = 3;
                if ((distX == 3 || distY == 3) && distX + distY < 6)
                {
                    rangeMax++;
                }

                State = (ProtectPrayer)Random.Range(1, rangeMax);
            }
        }

        switch (State)
        {
            case ProtectPrayer.Magic:
                if (TickCount == 1)
                {
                    Animator.SetTrigger("magic");
                    audioSource.PlayOneShot(AudioMagic);
                }
                else if (TickCount == 4)
                {
                    result = ProtectPrayer.Magic;
                    Vector3 projectilePos = transform.position + new Vector3(0, 4.5f, 0) + transform.forward;
                    GameObject projectile = Instantiate(ProjectilePrefab, projectilePos, Quaternion.identity);
                    projectile.GetComponent<Projectile>().Target = PlayerTransform;
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
                    audioSource.PlayOneShot(AudioExplode);
                    GameObject projectile = Instantiate(ProjectilePrefab, PlayerTransform.position + new Vector3(0, 10, 0), Quaternion.identity);
                    projectile.GetComponent<Projectile>().Target = PlayerTransform;
                }
                break;
            case ProtectPrayer.Melee:
                if (TickCount == 1)
                {
                    result = ProtectPrayer.Melee;
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
        transform.position = new Vector3((Pos.x + 0.5f) * squareWidth, 0, (Pos.y + 0.5f) * squareWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
