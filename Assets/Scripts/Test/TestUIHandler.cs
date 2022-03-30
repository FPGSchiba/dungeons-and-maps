using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIHandler : MonoBehaviour
{
    [Header("References")]
    public LevelGeneration generation;
    public Slider heightSlider;
    public Slider scaleSlider;

    public void OnGenerateClicked()
    {
        generation.RegenerateMap();
    }
}
