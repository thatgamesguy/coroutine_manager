using UnityEngine;
using System.Collections;

/// <summary>
/// Pauses all jobs associated with the JobManager when user clicks left mouse button 
/// and resumes all jobs with user clicks right mouse button.
/// </summary>
public class ExampleTImerPauseResume : MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetMouseButtonUp (0)) {
			CM_JobManager.Global.PauseAll ();
		} else if (Input.GetMouseButtonUp (1)) {
			CM_JobManager.Global.ResumeAll ();
		}
	
	}
}
