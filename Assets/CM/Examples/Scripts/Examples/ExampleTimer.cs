using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Creates new coroutine job to update a text object with time since startup. Adds job to global job manager so
/// that it can be paused and resumed as required.
/// </summary>
[RequireComponent (typeof (Text))]
public class ExampleTimer : MonoBehaviour 
{
	private Text _text;
	
	void Start () 
	{
		_text = GetComponent<Text> ();
		CM_JobManager.Global.AddJob (
				CM_Job.Make (UpdateTime ())
					.NotifyOnJobPaused ((object sender, CM_JobEventArgs e) => {
						Debug.Log ("Job Paused");
					})
					.NotifyOnJobResumed ((object sender, CM_JobEventArgs e) => {
						Debug.Log ("Job Resumed");
					}).Start ());
	}
	
	private IEnumerator UpdateTime ()
	{
		var waitTime =  new WaitForEndOfFrame ();

		while (true) {
			_text.text = "" + Time.realtimeSinceStartup;
			yield return waitTime;
		}
	}
}
