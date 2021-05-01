using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SloMo : MonoBehaviour
{
    PlayerScript playerScript;

    public float sloMoFactor, timeToSlo, slomoTime, fillUpTime;
    public PostProcessVolume volume;
    public SliderScript slomoBar;

    Vignette ppv;
    LensDistortion ppln;
    ChromaticAberration ppch;

    private float tempTime = 1, v, ln, ch, vv, lnv, chv;
    bool slomo;

    public float[] maxValues, minValues;

    private void Awake()
    {
        playerScript = GetComponent<PlayerScript>();

        ppv = volume.profile.GetSetting<Vignette>();
        ppln = volume.profile.GetSetting<LensDistortion>();
        ppch = volume.profile.GetSetting<ChromaticAberration>();
    }
    void Start()
    {
        playerScript.input.Player.SloMo.performed += _ => slomo = true;
        playerScript.input.Player.SloMo.canceled += _ => slomo = false;
    }
    void Update()
    {
        if (slomo)
        {
            startSloMo();

            slomoBar.setValue(slomoBar.slider.value - slomoTime * (1 + (1 - Time.timeScale)) * Time.deltaTime);
            if (slomoBar.slider.value <= 0) slomo = false;
        }
        else
        {
            stopSloMo();

            slomoBar.setValue(slomoBar.slider.value + fillUpTime * Time.deltaTime);
        }

        UpdateIntensities();
    }
    void UpdateIntensities()
    {
        v = maxValues[0] * (1 - tempTime);
        ln = maxValues[1] * (1 - tempTime);
        ch = maxValues[2] * (1 - tempTime);

        ppv.intensity.value = Mathf.SmoothDamp(ppv.intensity.value, v, ref vv, timeToSlo);
        ppln.intensity.value = Mathf.SmoothDamp(ppln.intensity.value, ln, ref lnv, timeToSlo);
        ppch.intensity.value = Mathf.SmoothDamp(ppch.intensity.value, ch, ref chv, timeToSlo);

        if (ppv.intensity.value < minValues[0]) ppv.intensity.value = minValues[0];
        if (ppln.intensity.value < minValues[1]) ppln.intensity.value = minValues[1];
        if (ppch.intensity.value < minValues[2]) ppch.intensity.value = minValues[2];

        if (tempTime == 1)
        {
            ppv.intensity.value = minValues[0];
            ppln.intensity.value = minValues[1];
            ppch.intensity.value = minValues[2];
        }
    }
    void startSloMo()
    {
        if (tempTime == sloMoFactor) return;

        float vel = 0;
        tempTime = Mathf.SmoothDamp(Time.timeScale, sloMoFactor, ref vel, timeToSlo * Time.deltaTime);

        Time.timeScale = tempTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    void stopSloMo()
    {
        if (tempTime == 1) return;

        float vel = 0;
        tempTime = Mathf.SmoothDamp(Time.timeScale, 1, ref vel, timeToSlo * Time.deltaTime);

        Time.timeScale = tempTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
