using UnityEngine;
using System.Collections;

/// <summary>
/// Example Script. Used to spawn a number of objects using <see cref="CM_Job"/>.
/// </summary>
public class ExampleContinousSpawner : MonoBehaviour 
{
	/// <summary>
	/// The prefab to spawn.
	/// </summary>
	public GameObject prefab;

	/// <summary>
	/// The number of objects to spawn.
	/// </summary>
	public int numToSpawn = 200;

	/// <summary>
	/// The time between spawns.
	/// </summary>
	public float timeBetweenSpawns = 0.1f;

	/// <summary>
	/// Creates a new job to spawn prefabs, subscribes to JobComplete event (to log a message informing user that an object has spawned),
	/// sets the job to repeat, and finally starts the job. 
	/// </summary>
	void Start () 
	{
		CM_Job.Make (SpawnPrefab ())
			.Repeat (numToSpawn)
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
				Debug.Log ("Object Spawned");
			}).Start ();
	}
	
	private IEnumerator SpawnPrefab ()
	{
		Instantiate (prefab, transform.position + new Vector3 (Random.Range (-1f, 1f), 0f, 0f), Quaternion.identity);

		yield return new WaitForSeconds (timeBetweenSpawns);
	}
}
