using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARFaceDebugData : MonoBehaviour
{
    public Text rotationText;
    public Text blendShapeText;
    public Text headBopNotificationText;

    public Text RotationAngleDirectionText;
    public Text RotationAngleValueText;
    public Text MinRotationAngleThresholdText; 

    public static ARFaceDebugData instance;

    public float prevRotationX_ARHead;
    public float prevRotationY_ARHead;
    public float prevRotationZ_ARHead;

    public float rotationX_ARHead;
    public float rotationY_ARHead;
    public float rotationZ_ARHead;

    public float minRotationY;
    public float maxRotationY;

    public float minRotationZ;
    public float maxRotationZ;

    public enum RotationToCheck
    {
        x,
        y,
        z
    }

    // structure based on Unity collider data structure 
    private bool headHasNodded;

    public RotationToCheck rotationToCheck = RotationToCheck.x;

    public float maxAngleRoation = 25.0f; 
    public float minAngleRotation = 15.0f;
    private float angleRotationDifference;
    private float previousAngleValue;
    private float currentAngleValue;

    private static ARFaceDebugData Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        SetupDeuggerText();
    }

    private void SetupDeuggerText()
    {
        RotationAngleDirectionText.text = rotationToCheck.ToString() + " direction.";
        MinRotationAngleThresholdText.text = minAngleRotation.ToString() + "degrees";
    }

    public void Update()
    {
        previousAngleValue = currentAngleValue;


        switch (rotationToCheck)
        {
            case RotationToCheck.x:
                currentAngleValue = rotationX_ARHead;
                break;
            case RotationToCheck.y:
                currentAngleValue = rotationY_ARHead;
                break;
            case RotationToCheck.z:
                currentAngleValue = rotationZ_ARHead;
                break;
        }

        if(minAngleRotation > 0)
        {
            if (currentAngleValue - previousAngleValue > 0 & angleRotationDifference >= 0)
            {
                angleRotationDifference += (currentAngleValue - previousAngleValue);
            }
            else if (currentAngleValue - previousAngleValue < 0 & angleRotationDifference > 0)
            {
                angleRotationDifference -= (currentAngleValue - previousAngleValue);
            }

            RotationAngleValueText.text = angleRotationDifference.ToString() + "degrees";

            if (angleRotationDifference >= minAngleRotation)
            {
                // Debug.Log("Victory! You've hit the minimum angle rotation requirement");
            }
            else if (angleRotationDifference < 0)
            {
                angleRotationDifference = 0;
                // Debug.Log("Unfortunately youve rotated too far backwards");
            }

            if (angleRotationDifference > maxAngleRoation)
            {
                angleRotationDifference = 0;
            }
        }
        else
        {
            if (currentAngleValue - previousAngleValue < 0 & angleRotationDifference <= 0)
            {
                angleRotationDifference += (currentAngleValue - previousAngleValue);
            }
            else if (currentAngleValue - previousAngleValue > 0 & angleRotationDifference < 0)
            {
                angleRotationDifference -= (currentAngleValue - previousAngleValue);
            }

            RotationAngleValueText.text = angleRotationDifference.ToString() + "degrees";

            if (angleRotationDifference <= minAngleRotation)
            {
                // Debug.Log("Victory! You've hit the minimum angle rotation requirement");
            }
            else if (angleRotationDifference > 0)
            {
                angleRotationDifference = 0;
                // Debug.Log("Unfortunately youve rotated too far backwards");
            }


            if (angleRotationDifference < maxAngleRoation)
            {
                angleRotationDifference = 0;
            }
        }



    }

    public bool hasHeadNodded()
    {
        if(angleRotationDifference >= minAngleRotation & angleRotationDifference <= maxAngleRoation)
        {
            return true;
        }
        return false; 
    }


}
