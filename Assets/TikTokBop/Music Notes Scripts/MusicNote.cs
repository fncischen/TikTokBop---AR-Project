using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "MusicNote", menuName = "My Assets/MusicNote")]
public class MusicNote : ScriptableObject
{
    public Image musicIcon;
    /// <summary>
    /// The time interval where the music note will be placed, in seconds 
    /// </summary>
    public float TimeStamp;

    /// <summary>
    /// Enables / disables checking the face blendshapes to trigger event
    /// </summary>
    public bool checkBlendShape;

    /// <summary>
    /// Enables / disables checking the head rotation to trigger event
    /// </summary>
    public bool checkHeadRotation;

    /// <summary>
    /// The time duration of the effect, in seconds
    /// </summary>
    [Range(0, 5)]
    public float timeDuration; 
    
    /// <summary>
    /// Enables / disables face effects when the event is triggered by head bop
    /// </summary>
    public bool isFaceEffect;

    /// <summary>
    /// The Material to use to swap with the face 
    /// </summary>
    public Material faceEffectMaterial;  
    
    /// <summary>
    /// Enables / disables post processing effects on the screen when the event is triggered by head bop 
    /// </summary>
    public bool isPostProcessingEffect;

    /// <summary>
    /// The post processing shader used to change the camera screen 
    /// </summary>
    public Shader postProcessingShader;

    public bool timelineTickerTouched = false;
    public bool successfullyNodded = false;

}
