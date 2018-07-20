using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woodpecker : MonoBehaviour
{
    public Animation Anima;

    Level level;
    Transform target;
    WoodpeckerState state;
    new CameraControl camera;

    public float normalStrikeTime;
    public float angryStrikeTime;

    public float normalStrikeShake;
    public float angryStrikeShake;

    public float normalStrikeAmp;
    public float angryStrikeAmp;

    float timeBetweenStrikes;

    // Distance to target after which woodpecker becomes angry
    public float angryDistance;

    public float BlinkTime;

    public SpriteRenderer dangerSprite;

    public Color DangerColor;


    Coroutine destroyCoroutine;

    private void Start()
    {
        level = FindObjectOfType<Level>();
        target = FindObjectOfType<Player>().transform;
        level.onTilesReady += OnTilesReady;
        camera = FindObjectOfType<CameraControl>();

        timeBetweenStrikes = normalStrikeTime;
        dangerSprite.size = new Vector2(5, level.grid.GetLength(1));
    }

    private void OnDisable()
    {
        if (destroyCoroutine != null)
            StopCoroutine(destroyCoroutine);

        level.onTilesReady -= OnTilesReady;
    }

    void OnTilesReady()
    {
        destroyCoroutine = StartCoroutine(DestroyCoroutine());
    }

    // Play animation
    void SetAnimation(string animName)
    {
        Anima.Play(animName);
    }

    // Wait for timer
    IEnumerator WaitForCooldown()
    {
        StartCoroutine(BlinkDangerZone());
        yield return new WaitForSeconds(timeBetweenStrikes);
    }
    // Blink the area
    IEnumerator BlinkDangerZone()
    {
        float blinkTime = BlinkTime;

        while (!GameManager.Instance.isMoving)
        {
            yield return new WaitForSeconds(blinkTime);
            blinkTime /= 1.3f;
            dangerSprite.color = dangerSprite.color.a > 0 ? new Color(0, 0, 0, 0) : DangerColor;
        }

        dangerSprite.color = new Color(0, 0, 0, 0);
    }
    // Set ne danger zone position
    void SetDangerZonePosition()
    {
        switch (state)
        {
            case WoodpeckerState.Normal:
                dangerSprite.size = new Vector2(5, dangerSprite.size.y);
                dangerSprite.transform.position = new Vector3(level.currentGridEnd + 2, dangerSprite.transform.position.y);
                break;
            case WoodpeckerState.Angry:
                dangerSprite.size = new Vector2(6, dangerSprite.size.y);
                dangerSprite.transform.position = new Vector3(level.currentGridEnd + 2.5f, dangerSprite.transform.position.y);
                break;
            default:
                break;
        }
    }

    // Destroy the tiles
    void DestroyTiles()
    {
        switch (state)
        {
            case WoodpeckerState.Normal:
                camera.ScreenShake(normalStrikeShake, normalStrikeAmp);
                break;
            case WoodpeckerState.Angry:
                camera.ScreenShake(angryStrikeShake, angryStrikeAmp);
                break;
            default:
                break;
        }

        if(target != null)
        {
            level.DestroyTiles(1);
            if (target.position.x - transform.position.x < angryDistance)
            {
                state = WoodpeckerState.Normal;
                timeBetweenStrikes = normalStrikeTime;
            }
            else
            {
                state = WoodpeckerState.Angry;
                timeBetweenStrikes = angryStrikeTime;
            }
            // CHANGE THIS TO LERP
            transform.position += new Vector3(1, 0);
        }
    }

    void StartPecking()
    {
        GameManager.Instance.ChangePhase();
    }

    void FinishPecking()
    {
        GameManager.Instance.ChangePhase();

        if (destroyCoroutine != null)
            StopCoroutine(destroyCoroutine);

        destroyCoroutine = StartCoroutine(DestroyCoroutine());
    }


    void StartPlayingSound()
    {
        GetComponent<AudioSource>().Play();
    }
    void StopPlayingSound()
    {
        GetComponent<AudioSource>().Stop();
    }

    IEnumerator DestroyCoroutine()
    {
        Debug.Log("Waiting for cooldown");
        SetDangerZonePosition();
        yield return WaitForCooldown();

        Debug.Log("Starting attack");
        switch (state)
        {
            case WoodpeckerState.Normal:
                SetAnimation("Normal");
                break;
            case WoodpeckerState.Angry:
                SetAnimation("Angry");
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position + new Vector3(angryDistance, 5), transform.position + new Vector3(angryDistance, -5));
    }

    public enum WoodpeckerState
    {
        Normal,
        Angry
    }
}
