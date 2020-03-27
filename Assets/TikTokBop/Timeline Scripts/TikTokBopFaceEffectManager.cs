using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The purpose of this manager is to trigger switich in the AR Face Material whenever
/// a condition has been met, such as successfully bopping your head at a drum beat, missing a drum beat, etc. 
/// </summary>
public class TikTokBopFaceEffectManager : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public static TikTokBopFaceEffectManager instance;
    // Start is called before the first frame update

    private static TikTokBopFaceEffectManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        Debug.Log("Instance " + instance);
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("Starting face effect");

    }

    public void Awake()
    {
        Debug.Log("Face Effect Awake");
    }

    public Material normalMaterial; 
    public Material victoryMaterial;
    public Material hitDrumMaterial;
    public Material missedDrumMaterial;
    public Material transitionMaterial; 
    

    public void switchToNormalMaterial()
    {
        Material[] newMat = new Material[1];
        newMat[0] = normalMaterial;
        skinnedMeshRenderer.materials = newMat;  
    }

    public void switchToHitDrumMaterial()
    {
        Material[] newMat = new Material[1];
        newMat[0] = hitDrumMaterial;
        skinnedMeshRenderer.materials = newMat;
    }

    public void switchToMissedDrumMaterial()
    {

        Material[] newMat = new Material[1];
        newMat[0] = missedDrumMaterial;
        skinnedMeshRenderer.materials = newMat;
        Debug.Log("switching to missed drum material");
    }

    public void switchToVictoryDrumMaterial()
    {
        Material[] newMat = new Material[1];
        newMat[0] = victoryMaterial;
        skinnedMeshRenderer.materials = newMat;
    }

    


}
