using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform target;
    Transform targetWorm;

    public static float verticalPosition;

    Vector3 targetPosition;

    public Vector3 Offset;

    Coroutine shakeCoroutine;

    private void Start()
    {
        Woodpecker dolbush = FindObjectOfType<Woodpecker>();
        if(dolbush != null)
            target = dolbush.transform;
        else
            targetWorm = FindObjectOfType<Player>().transform;
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            //if(target.transform.position.x > transform.position.x)
            //{
                targetPosition = target.position + Offset;
                targetPosition.y = verticalPosition;
                targetPosition.z = transform.position.z;

                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.125f);
            //}
        }
        else if (targetWorm != null)
        {
            if (targetWorm.transform.position.x > transform.position.x)
            {
                targetPosition = targetWorm.position;
                targetPosition.y = verticalPosition;
                targetPosition.z = transform.position.z;

                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.125f);
            }
        }
    }

    public void ScreenShake(float sec, float amplitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        StartCoroutine(Shake(sec, amplitude));
    }

    IEnumerator Shake(float seconds, float amp)
    {
        float timer = 0;

        while (timer < seconds)
        {
            transform.position += new Vector3(UnityEngine.Random.Range(-amp, amp), UnityEngine.Random.Range(-amp, amp));
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (-Offset), Vector3.one);
    }
}