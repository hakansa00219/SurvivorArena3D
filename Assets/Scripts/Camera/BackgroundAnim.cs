using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnim : MonoBehaviour
{
    public float exposure = 0.6f;
    public float elapsedTime = 0f;
    private void Start()
    {
        //StartCoroutine(ExposureChange());
    }
    void FixedUpdate()
    {
        //elapsedTime += Time.deltaTime;
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);
        RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }

    IEnumerator ExposureChange()
    {
        while(true)
        {
            while (elapsedTime < 30)
            {
                if (elapsedTime < 10)
                {
                    exposure = Mathf.Lerp(0.6f, 1f, elapsedTime * 0.1f);
                }
                else if (elapsedTime >= 10 && elapsedTime < 20)
                {
                    exposure = Mathf.Lerp(1f, 0.3f, (elapsedTime-10) * 0.1f);
                }
                else if (elapsedTime >= 20)
                {
                    exposure = Mathf.Lerp(0.3f, 0.6f, (elapsedTime-20) * 0.1f);
                }
                yield return new WaitForEndOfFrame();
            }
            elapsedTime = 0f;
            yield return new WaitForEndOfFrame();
        }        
    }
}
