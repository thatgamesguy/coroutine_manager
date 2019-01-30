using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Simple GUI controller. Uses a CM_JobQueue to enqueue a number of gui actions.
/// </summary>
public class ExampleGUI : MonoBehaviour 
{
	/// <summary>
	/// THe GUI text
	/// </summary>
	public Text titleText, subtitleText, instructionsText;

	/// <summary>
	/// Enqueues jobs to fade in the title, subtitle, and move the instruction text into place.
	/// </summary>
	void Start () 
	{
		CM_JobQueue.Make ()
			.Enqueue (FadeInText (titleText))
			.Enqueue (FadeInText (subtitleText))
			.Enqueue (MoveText (instructionsText))
			.Start ();
	}
	
	private IEnumerator FadeInText (Text text)
	{
		var waitTime = new WaitForEndOfFrame ();

		var origColour = text.color;

		while (text.color.a < 1f) {
			text.color = new Color (origColour.r, origColour.g, origColour.b, text.color.a + 0.1f);
			yield return waitTime;
		}
	}


	private IEnumerator MoveText (Text text)
	{
		var waitTime = new WaitForEndOfFrame ();

		while (text.transform.position.y < 60f) {
			text.transform.position = new Vector2 (text.transform.position.x, text.transform.position.y + 4f);
			yield return waitTime;
		}
	}
}
