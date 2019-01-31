using UnityEngine;
using System.Collections;

/// <summary>
/// Performance testing for large queues.
/// </summary>
public class ExampleLargeJobQueueTest : MonoBehaviour {

	public int queueSize = 1000;

	private CM_Job[] _jobsToQueue;

	void Start ()
	{
		_jobsToQueue = new CM_Job[queueSize];

		for (int i = 0; i < queueSize; i++) 
        {
			_jobsToQueue [i] = CM_Job.Make (SmallJobForLargeQueue(i + 1));
		}

		CM_JobQueue.Make ().Enqueue(_jobsToQueue).Start ();
	}

	private IEnumerator SmallJobForLargeQueue (int jobNum)
	{
		Debug.Log (string.Format ("Large queue test, job {0} of {1}", jobNum, queueSize));
        yield return null;
	}
	
}
