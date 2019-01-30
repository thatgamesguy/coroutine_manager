using UnityEngine;
using System.Collections;

/// <summary>
/// Example character with action queue.
/// </summary>
public class ExampleCharacterDamage : MonoBehaviour 
{
	public float health = 20f;

	private CM_JobQueue _actionQueue;

	void Start () 
	{
		_actionQueue = CM_JobQueue.Make ().ContinousRunning ().Start ();
	}

	/// <summary>
	/// Adds the action to the current queue. An action can be any coroutine you want.
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddActionToQueue (CM_Job action)
	{
		_actionQueue.Enqueue (action);
	}

	/// <summary>
	/// Simulates the application of damage over time.
	/// </summary>
	/// <returns>The damage.</returns>
	/// <param name="damageType">Damage type.</param>
	/// <param name="time">Time.</param>
	public IEnumerator ApplyDamage (string damageType, float time)
	{
		var waitTime = new WaitForSeconds (0.1f);

		while (time > 0f) {
			time -= 0.1f;

			health -= 1f;

			Debug.Log ("Ouch you got me with " + damageType + "! Health = " + health);



			if (health <= 0f) {
				OnDead ();
			}

			yield return waitTime;
		}
	}

	/// <summary>
	/// Simulates restoring of health over time.
	/// </summary>
	/// <returns>The health.</returns>
	/// <param name="time">Time.</param>
	public IEnumerator RestoreHealth (float time)
	{
		var waitTime = new WaitForSeconds (0.1f);

		bool dead = (health <= 0f);


		while (time > 0f) {

			time -= 0.1f;

			health += 1f;

			Debug.Log ("Healing! Health = " + health);

			if (dead && health > 0f) {
				dead = false;
				OnRevive ();
			}

			yield return waitTime;
		}
	}

	private void OnDead ()
	{
		GetComponent<Animator> ().SetBool ("dead", true);
	}

	private void OnRevive ()
	{
		GetComponent<Animator> ().SetBool ("dead", false);
	}
}
