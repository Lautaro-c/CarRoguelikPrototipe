using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SliderController : MonoBehaviour
{
    private Slider slider;
    private AudioController audioController;

    void Start()
    {
        slider = GetComponent<Slider>();
        audioController = AudioController.Instance;
        slider.onValueChanged.AddListener(value => audioController.SetMasterVolume(value));
        slider.value = audioController.GetMasterVolume();
    }
}
