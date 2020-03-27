using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TikTokHeadBopTimeline : MonoBehaviour
{
    public UINotificationsManager UI_NotificationsManager;
    public enum TimelineState
    {
        Stop,
        Play,
        Pause,
    }

    public TimelineTicker timelineTicker;
    public MusicNote[] musicNotes;
    private List<MusicNoteGameObject> currentMusicNotesInTimeline;

    public RectTransform timelinePanel;
    public float TimelineScrollVelocity = 1.0f;

    [Range(0,1)]
    public float scrollBeginThreshold;

    public TimelineTimeMarker timelineTimeMarker;
    private TimelineTimeMarker[] numberOfCurrentTimeLineTimeMarkers;
    public TimelineState timelineState = TimelineState.Stop;
    public MusicNoteGameObject musicNoteGameObjectPrefab;

    /// <summary>
    /// The location of the start of the timeline track
    /// </summary>
    public RectTransform trackSectionFromClampPosition;
    public RectTransform trackSectionToClampPosition;
    public RectTransform trackSectionHeight;

    
    [Range(0, 10)]
    public float timelineScale = 1;

    [Range (0, 1f)]
    public float drumHitThreshold = 0.5f;

    private float timelineTrackWidth;
    private float timelineTrackSectionHeight;
    private float timelineHeaderHeight;

    private TimelineTimeMarker[] timelineTimeMarkers;

    /// <summary>
    /// Maximum time duration of animation clip;
    /// </summary>
    public float timelineMaximumTimeDuration;

    /// <summary>
    /// The current "From" Time value in the timeline range being rendered   
    /// </summary>
    private float currentFromTimeClamp;
    
    /// <summary>
    /// The current "To" Time value in the timeline range being rendered 
    /// </summary>
    private float currentToTimeClamp;

    /// <summary>
    /// The ratio of the width of the timeline track (in Timeline transform local coordinates) to the actual time duration 
    /// </summary>
    private float ratioBtwnMaxTimeAndMaxWidth;


    public int drumScorePoints = 0;
    public int drumScoreMissed = 0; 
    public Text DrumScoreHit;
    public Text DrumScoreMiss;

    public void Start()
    {
        SetupInitialTimelineConfiguiration();
        GenerateMusicNotes();
        SetAllMusicNotesToBeUnTouched();
    }

    #region timeline configuration methods 
    public void SetupInitialTimelineConfiguiration()
    {
        SetUpTimelineCoordinateSystem();
        SetupInitalTimelineTimeIntervals();
        GenerateTimelineTickers();

        currentMusicNotesInTimeline = new List<MusicNoteGameObject>();
    }
    /// <summary>
    /// Set up timeline coordinate system based on the origin of the Timeline Panel
    /// </summary>
    private void SetUpTimelineCoordinateSystem()
    {
        // heights established;
        timelineHeaderHeight = (trackSectionHeight.transform.localPosition.y - trackSectionToClampPosition.transform.localPosition.y);
        timelineTrackSectionHeight = (trackSectionHeight.transform.localPosition.y - trackSectionFromClampPosition.transform.localPosition.y);

        // widths established;
        timelineTrackWidth = trackSectionToClampPosition.transform.localPosition.x - trackSectionFromClampPosition.transform.localPosition.x;
        // Debug.Log("timeline track width : " + timelineTrackWidth);

        // ensure a proper ratio between timeline's track actual width and time such that the right scale can be achieved
        ratioBtwnMaxTimeAndMaxWidth = (timelineMaximumTimeDuration /timelineScale / timelineTrackWidth) * timelinePanel.transform.localScale.x ;
    }

    private void SetupInitalTimelineTimeIntervals()
    {
        currentFromTimeClamp = timelineTicker.currentTime;
        currentToTimeClamp = timelineMaximumTimeDuration/timelineScale;
    }

    /// <summary>
    /// Generate timeline tickers to indicate current time range of the timeline. 
    /// 
    /// Use when instantiating a Timeline OR when the timeline is shifting 
    /// </summary>
    private void GenerateTimelineTickers()
    {
        // Debug.Log("timelineDiff " + timelineTrackUVwidth);
        // that difference is X units of time clamped inside the timeline (at current scale) --- this is our INITIAL clamp
        // Debug.Log("toclamptime " + currentToTimeClamp);

        // the amount of timeline markers 
        // int numOfTimelineMarkers = Mathf.FloorToInt((currentToTimeClamp - currentFromTimeClamp)) + 1; //
        float timelineDelta = (timelineTrackWidth) / (timelineMaximumTimeDuration/timelineScale); // adjust for timeline scale

        float currentNumber = currentFromTimeClamp;
        int numOfTimelineMarkers = 0;

        float timelineStartCoordinate;
        if (currentNumber != Mathf.FloorToInt(currentNumber))
        {
            // convert currentNumber to timelineCoordinate
            int nextNumber = Mathf.FloorToInt(currentNumber) + 1;

            float nextIntegerTimelineMarkerCoordinate = ConvertFromTimeToTimelineXPosition(nextNumber);
            // Debug.Log("have to switch to "+ nextNumber);
            timelineStartCoordinate = nextIntegerTimelineMarkerCoordinate;
        }
        else
        {
            timelineStartCoordinate = trackSectionFromClampPosition.localPosition.x;
            numOfTimelineMarkers += 1;
            currentNumber += 1;
        }

        while (currentNumber <= currentToTimeClamp)
        {
            if (currentNumber == Mathf.RoundToInt(currentNumber))
            {
                numOfTimelineMarkers += 1;
                currentNumber += 1;
            }
            else
            {
                currentNumber = (Mathf.RoundToInt(currentNumber) + 1);
            }
        }

        if (numberOfCurrentTimeLineTimeMarkers != null)
        {
           DestroyTimelineTimeMarkers();
        }

        // Debug.Log("TimelineMarkers " + numOfTimelineMarkers);
        // Debug.Log("timelineDelta " + timelineDelta);

        // set up timeline track markers
        numberOfCurrentTimeLineTimeMarkers = new TimelineTimeMarker[Mathf.FloorToInt(numOfTimelineMarkers)];
        float timeDelta = (timelineMaximumTimeDuration/timelineScale) / numOfTimelineMarkers;

        // set up positions for these timeline track markers
        for (int i = 0; i < numberOfCurrentTimeLineTimeMarkers.Length; i++)
        {
            TimelineTimeMarker t = Instantiate(timelineTimeMarker, timelinePanel.transform);

            int time = Mathf.RoundToInt(i * timeDelta + currentFromTimeClamp);
            string timeText = time.ToString();

            t.time.text = timeText;

            // center coord of timeline track + amount of y needed to get to top of timeline renderer + amount o

            // Debug.Log("timeline trackSection height :" + timelineTrackSectionHeight);

            t.transform.localPosition = new Vector3(timelineStartCoordinate + i * timelineDelta, timelineTrackSectionHeight, 0.2f / timelinePanel.transform.localScale.z);
            // t.transform.localScale = new Vector3(1 / t.transform.parent.localScale.x, 1 / t.transform.parent.localScale.y, 1 / t.transform.parent.localScale.z);
            numberOfCurrentTimeLineTimeMarkers[i] = t; 
        }
    }

    private void DestroyTimelineTimeMarkers()
    {
        foreach(TimelineTimeMarker t in numberOfCurrentTimeLineTimeMarkers)
        {
            Destroy(t.gameObject);
        }
    }
    #endregion

    #region Timeline lifecycle methods 
    public void Update()
    {
        switch (timelineState)
        {
            case TimelineState.Play:
                PlayTimeline();
                break;
            case TimelineState.Pause:
                PauseTimeline();
                break;
            case TimelineState.Stop:
                StopTimeline();
                break;
        }
    }

    private void PlayTimeline()
    {
        if(timelineTicker.currentTime < timelineMaximumTimeDuration)
        {
            GenerateMusicNotes();

            if(timelineTicker.currentTime >= currentToTimeClamp * scrollBeginThreshold)
            {
                Debug.Log("expand timeline");
                currentFromTimeClamp += Time.deltaTime;
                currentToTimeClamp += Time.deltaTime;            
                GenerateTimelineTickers();
            }

            // Debug.Log("fromTimeClamp : " + currentFromTimeClamp + " / toTimeClamp : " + currentToTimeClamp);

            timelineTicker.currentTime += Time.deltaTime;

            float x = ConvertFromTimeToTimelineXPosition(timelineTicker.currentTime);

            timelineTicker.transform.localPosition = new Vector3(x, timelineTicker.transform.localPosition.y, -0.50f);

        }
        else
        {
            currentFromTimeClamp = 0f;
            currentToTimeClamp = timelineMaximumTimeDuration / timelineScale;
            timelineState = TimelineState.Stop;
        }

    }

    private void PauseTimeline()
    {

    }

    private void StopTimeline()
    {
        Debug.Log("stop timeline");
        RemoveMusicNotes();
        drumScorePoints = 0;
        drumScoreMissed = 0;
        DrumScoreHit.text = "0";
        DrumScoreMiss.text = "0";
        SetAllMusicNotesToBeUnTouched();
        this.gameObject.SetActive(false);
        timelineState = TimelineState.Pause;
        timelineTicker.currentTime = 0;
        GenerateTimelineTickers();
        UI_NotificationsManager.setUIConfiguration();

    }

    private void OnDisable()
    {
        SetAllMusicNotesToBeUnTouched();
    }

    #endregion

    #region Timeline Music Note Methods

    private void GenerateMusicNotes()
    {
        RemoveMusicNotes();

        foreach(MusicNote musicNote in musicNotes)
        {
           // Debug.Log("looping thru music notes");
            if (musicNote.TimeStamp >= currentFromTimeClamp && musicNote.TimeStamp <= currentToTimeClamp)
            {
                // Debug.Log("generating music node");
                GameObject noteObj = Instantiate(musicNoteGameObjectPrefab.gameObject, timelinePanel);
                float xPos = ConvertFromTimeToTimelineXPosition(musicNote.TimeStamp);
                // Debug.Log(xPos + " for music Note Time Stamp : " + musicNote.TimeStamp);
                noteObj.GetComponent<MusicNoteGameObject>().musicNoteData = musicNote;
                noteObj.transform.localPosition = new Vector3(xPos, 0, 0.2f/timelinePanel.localScale.z);
                noteObj.GetComponent<Image>().sprite = musicNote.musicIcon.sprite;
                currentMusicNotesInTimeline.Add(noteObj.GetComponent<MusicNoteGameObject>());
            }
        }
    }

    private void RemoveMusicNotes()
    {
        foreach(MusicNoteGameObject musicNote in currentMusicNotesInTimeline)
        {
            Destroy(musicNote.gameObject);
        }

        currentMusicNotesInTimeline = new List<MusicNoteGameObject>();
    }

    /// <summary>
    /// Public method that checks to see if the timeline ticker is near a music note / drum beat. We use 1/4 of a second buffer range between
    /// the given time stamp and the timeline ticker.
    ///
    /// If the timeline ticker is near the music note, indicate to the music note that the music note has been touched.
    /// 
    /// However, it is the timeline ticker's job to get data from the AR Face to see if a successful nod
    /// has been made. 
    /// </summary>
    /// <returns></returns>
    public bool isTimelineTickerNearMusicNote(out MusicNote mNote)
    {
        float currentTime = timelineTicker.currentTime;
        foreach(MusicNoteGameObject musicNote in currentMusicNotesInTimeline)
        {
                if (currentTime >= musicNote.musicNoteData.TimeStamp - drumHitThreshold & currentTime <= musicNote.musicNoteData.TimeStamp)
                {
                        mNote = musicNote.musicNoteData;
                        return true;
                }
            }
        mNote = null;
        return false;
    }

    public bool isTimelineTickerPastMusicNote(out MusicNote mNote)
    {
        float currentTime = timelineTicker.currentTime;
        foreach (MusicNoteGameObject musicNote in currentMusicNotesInTimeline)
        {

            if (currentTime >= musicNote.musicNoteData.TimeStamp & currentTime <= (musicNote.musicNoteData.TimeStamp + drumHitThreshold))
            {
                Debug.Log("It is past the music note");
                mNote = musicNote.musicNoteData;
                return true;
            }
        }
        Debug.Log("it is not past the music note");
        mNote = null;
        return false;
    }

    public void SetAllMusicNotesToBeUnTouched()
    {
        foreach(MusicNote m in musicNotes)
        {
            m.timelineTickerTouched = false; 
        }
    }

    #endregion

    #region timeline scoreboard methods

    public void AddScoreToDrum()
    {
        drumScorePoints += 1;
        DrumScoreHit.text = drumScorePoints.ToString();
    }

    public void AddMissToDrum()
    {
        drumScoreMissed += 1;
        DrumScoreMiss.text = drumScoreMissed.ToString();
    }

    #endregion

    #region helper functions 
    /// <summary>
    /// Calculates the current time the timeline ticker is located at; 
    /// </summary>
    public void CalculateTimelineTickerPosition()
    {
        timelineTicker.currentTime = (-timelineTicker.transform.localPosition.x + (trackSectionFromClampPosition.transform.localPosition.x)) * ratioBtwnMaxTimeAndMaxWidth + currentFromTimeClamp;
        
        //     V 
        // 0 --T-- 5

        // to ensure the timeline stays within the range 
        timelineTicker.currentTime = Mathf.Clamp(timelineTicker.currentTime, currentFromTimeClamp, currentToTimeClamp);
    }

    /// <summary>
    /// Convert a input type, time (float) into a z value transform value to be placed on the timeline music track.  
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public float ConvertFromTimeToTimelineXPosition(float time)
    {
        float z;
        z = -1* (- time + currentFromTimeClamp) / ratioBtwnMaxTimeAndMaxWidth + (trackSectionFromClampPosition.transform.localPosition.x);

        // Debug.Log("time: " + time + " currrentFromTimeClamp : " + currentFromTimeClamp + " ratio btwn time and width : " + ratioBtwnMaxTimeAndMaxWidth + "clampPosFrom xPos : " + trackSectionFromClampPosition.transform.localPosition.x + " result of z :" + z);

        // already in timelineSpace 
        // (0 - fromClampPos) + (currentTimeFromClamp - time) / (time/width)
        return z;
    }
    #endregion
}
