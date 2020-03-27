using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineTicker : MonoBehaviour
{
    public ARFaceDebugData ARFaceDebugData;
    public TikTokHeadBopTimeline timeline;

    // singleton pattern
    public TikTokBopFaceEffectManager faceEffectManagerInstance;

    /// <summary>
    /// The current time of the timeline ticker 
    /// </summary>
    [Range(0,60)]
    public float currentTime = 0;
    public RectTransform timeline_x_position;

    public float missMaterialTimeDuration = 5f;
    public float winMaterialTimeDuration = 5f;
    public float victoryTimeDuration = 10f;

    public void OnEnable()
    {
        faceEffectManagerInstance = GameObject.FindObjectOfType<TikTokBopFaceEffectManager>();
        Debug.Log("face effect manager instance is set");
    }

    public void Update()
    {
        if(timeline.timelineState == TikTokHeadBopTimeline.TimelineState.Play)
        {
            MusicNote mNote;
            if (timeline.isTimelineTickerNearMusicNote(out mNote))
            {
                if(ARFaceDebugData.hasHeadNodded())
                {
                    if (mNote.timelineTickerTouched == false)
                    {
                        mNote.timelineTickerTouched = true;
                        ARFaceDebugData.headBopNotificationText.text = "Head is near music note " + mNote.ToString() + " and successfully nodded at approximately " + currentTime.ToString() + " seconds";
                        timeline.AddScoreToDrum();
                        activateSuccessfulHitMaterialOfFace();
                       // Debug.Log("Timeline Ticker " + mNote.ToString() + " touched " + mNote.timelineTickerTouched);
                    }
                }
            }
            else if(timeline.isTimelineTickerPastMusicNote(out mNote))
            {
                if(!mNote.timelineTickerTouched)
                {
                    mNote.timelineTickerTouched = true;
                    ARFaceDebugData.headBopNotificationText.text = "Head is past music note " + mNote.ToString() + " and did NOT nod successfully  at approximately " + currentTime.ToString() + " seconds";
                    timeline.AddMissToDrum();
                    activateMissedHitMaterialOfFace();
                    // Debug.Log("Timeline Ticker " + mNote.ToString() + " touched " + mNote.timelineTickerTouched);
                }

            }
            else if(!timeline.isTimelineTickerNearMusicNote(out mNote) & ARFaceDebugData.hasHeadNodded())
            {
                ARFaceDebugData.headBopNotificationText.text = "Head is nodding at approximately " + currentTime.ToString() + " seconds, but is NOT hitting a music note";
            }
            else if (!timeline.isTimelineTickerNearMusicNote(out mNote) & !ARFaceDebugData.hasHeadNodded())
            {
                ARFaceDebugData.headBopNotificationText.text = "Head is NOT nodding and is not near ANY music note at approximately " + currentTime.ToString() + " seconds";
            }
        }
    }

    #region helper methods
    public void activateNormalMaterialOfFace()
    {
        faceEffectManagerInstance.switchToNormalMaterial();
    }

    public void activateSuccessfulHitMaterialOfFace()
    {
        faceEffectManagerInstance.switchToHitDrumMaterial();
        StartCoroutine(materialTimeCounter(winMaterialTimeDuration));
        // insert coroutine here
    }

    public void activateMissedHitMaterialOfFace()
    {
        Debug.Log("face effect manager instance " + faceEffectManagerInstance);
        faceEffectManagerInstance.switchToMissedDrumMaterial();
        StartCoroutine(materialTimeCounter(missMaterialTimeDuration));
        // insert coroutine here
    }

    public void activateVictorMaterialOfFace()
    {
        faceEffectManagerInstance.switchToVictoryDrumMaterial();
        StartCoroutine(materialTimeCounter(victoryTimeDuration));
        // insert corotuine here 
    }

    private IEnumerator materialTimeCounter(float timeDuration)
    {
        float time = 0;
        while (time < timeDuration)
        {
            Debug.Log("this lasts this long");
            time += Time.deltaTime;
            yield return null; 
        }

        activateNormalMaterialOfFace();

    }

    #endregion
}
