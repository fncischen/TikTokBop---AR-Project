using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARFaceDebugger : MonoBehaviour
{
    public GameObject ARFaceToDebug;
    private ARFaceDebugData aRFaceDebugData;

    private void Start()
    {
        aRFaceDebugData = GameObject.FindObjectOfType<ARFaceDebugData>();
    }

    public void Update()
    {
        aRFaceDebugData.rotationX_ARHead = ARFaceToDebug.transform.eulerAngles.x;
        aRFaceDebugData.rotationY_ARHead = ARFaceToDebug.transform.eulerAngles.y;
        aRFaceDebugData.rotationZ_ARHead = ARFaceToDebug.transform.eulerAngles.z;

        aRFaceDebugData.rotationText.text = "Rotation X: " + ARFaceToDebug.transform.eulerAngles.x.ToString() + "degrees \n";
        aRFaceDebugData.rotationText.text += "Rotation Y: " + ARFaceToDebug.transform.eulerAngles.y.ToString() + "degrees \n";
        aRFaceDebugData.rotationText.text += "Rotation Z: " + ARFaceToDebug.transform.eulerAngles.z.ToString() + "degrees \n";
    }
}
