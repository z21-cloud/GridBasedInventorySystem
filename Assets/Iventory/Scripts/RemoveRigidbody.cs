using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveRigidbody : MonoBehaviour
{
    private Rigidbody rb;
    private int i = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5f);
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if(rb.velocity == Vector3.zero)
            {
                Destroy(rb);
                yield break;
            }
            i++;
            if(i == 10)
            {
                Destroy(rb);
            }
        }
    }
}
