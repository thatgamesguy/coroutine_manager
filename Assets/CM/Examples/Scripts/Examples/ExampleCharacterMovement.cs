using UnityEngine;
using System.Collections;

/// <summary>
/// Example Script. A simple script showing how you can use <see cref="CM_JobQueue"/> to create easily repeatable character movement.
/// </summary>
public class ExampleCharacterMovement : MonoBehaviour 
{
	/// <summary>
	/// The movement speed of the character.
	/// </summary>
	public float moveSpeed = 10f;

	/// <summary>
	/// Creates a repeatable queue of jobs that moves the character around the screen.
	/// </summary>
	void Start () 
	{
		CM_JobQueue.Make ()
			.Enqueue (MoveTo (new Vector2 (13f, -10f)))
			.Enqueue (MoveTo (new Vector2 (-13f, -10f)))
			.Enqueue (MoveTo (new Vector2 (-13f, 10f)))
			.Enqueue (MoveTo (new Vector2 (13f, 10f)))
			.Repeat ()
			.Start ();
	}


	private IEnumerator MoveTo (Vector2 targetPosition)
	{
		if ((targetPosition.x < transform.position.x && transform.localScale.x > 0) 
		    	|| (targetPosition.x > transform.position.x && transform.localScale.x < 0)) {
			transform.localScale = new Vector2 (transform.localScale.x * -1f, transform.localScale.y);
		}

		do {

			Vector2 movDiff = targetPosition - (Vector2) transform.position;
			Vector2 movDir = movDiff.normalized * moveSpeed * Time.deltaTime;

			transform.position += (Vector3)movDir;

			yield return null;
			
		} while (Vector2.Distance (transform.position, targetPosition) > 0.2f);
	}
}
