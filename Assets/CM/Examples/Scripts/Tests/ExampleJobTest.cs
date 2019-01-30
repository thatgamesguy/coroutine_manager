using UnityEngine;
using System.Collections;

/// <summary>
/// Includes a number of methods to test the functionality of the <see cref="CM_Job"/> class.
/// Each method showcases a particular functionality of the CM_Job class that you can implement in your own projects.
/// Each method returns an ienumerator so that it can added to a job queue to be run in sequence for test purposes.
/// </summary>
public class ExampleJobTest : MonoBehaviour
{
	/// <summary>
	/// Adds each method to a queue to be run in sequene.
	/// </summary>
	void Start ()
	{
		CM_JobQueue.Make ()
			.Enqueue (SimpleJobTest ())
			.Enqueue (JobTestWithDelayedStart ())
			.Enqueue (JobTestWithDelayedPause ())
			.Enqueue (JobTestWithDelayedResume ())
			.Enqueue (JobTestWithDelayedKill ())
			.Enqueue (JobTestWithStartAndEndEvents ())
			.Enqueue (ChildJobTest ())
			.Enqueue (SingleCloneJobTest ())
			.Enqueue (MultipleCloneJobTest ())
			.Enqueue (InfinitelyRepeatableJobTest ())
			.Enqueue (MultipleRepeatableJobTest ())
			.Enqueue (MutltipleRepeatableJobTestWithChild ())
			.Start ();
	}

	/// <summary>
	/// Creates and starts a job.
	/// </summary>
	public IEnumerator SimpleJobTest ()
	{
		CM_Job.Make (PrintStringAfterDelay ("Simple job test running")).Start ();

		yield return null;
	}

	/// <summary>
	/// Creates and starts a job after a delay.
	/// </summary>
	public IEnumerator JobTestWithDelayedStart ()
	{
		CM_Job.Make (PrintStringAfterDelay ("Job test with delayed start running")).Start (1.5f);
		
		yield return null;
	}

	/// <summary>
	/// Creates coroutine job, starts job immediately and then pauses coroutine after 4 seconds. This
	/// paused job is then stored and can be resumed/killed from any class that has a reference to that job.
	/// </summary>
	public IEnumerator JobTestWithDelayedPause ()
	{
		var job = CM_Job.Make (InfiniteTest ("Job test with delayed pause running")).Start ();

		job.Pause (4f);
		
		yield return null;
	}

	/// <summary>
	/// Creates new job, pauses after 1 second, and resumes after 3 seconds.
	/// </summary>
	public IEnumerator JobTestWithDelayedResume ()
	{
		CM_Job.Make (InfiniteTest ("Job test with delayed resume running")).Start ().Pause (1f).Resume (3f);
		
		yield return null;
	}

	/// <summary>
	/// Creates coroutine job, starts job immediately and then sets the job to be killed after 4 seconds.
	/// </summary>
	public IEnumerator JobTestWithDelayedKill ()
	{
		CM_Job.Make (InfiniteTest ("Job test with delayed kill running")).Start ().Kill (4f);
		
		yield return null;
	}

	/// <summary>
	/// Creates new job, subscribes to the job start and end events and then starts job to be 
	/// run immediately.
	/// </summary>
	public IEnumerator JobTestWithStartAndEndEvents ()
	{
		CM_Job.Make (PrintStringAfterDelay ("Job test with start and end event subscription"))
			.NotifyOnJobStarted ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Started: job test with event subscription");
		})
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Finished: job test with event subscription");
		}).Start ();

		yield return null;
	}
	

	/// <summary>
	/// Child job test. Creates new job, adds two children (parent will not complete until children have finished processing)
	/// and starts the job.
	/// </summary>
	public IEnumerator ChildJobTest ()
	{
		CM_Job.Make (PrintStringAfterDelay ("Parent job test")).AddChild (PrintStringAfterDelay ("Child1 job test")).AddChild (PrintStringAfterDelay ("Child2 job test"))
			.NotifyOnChildJobStarted ((object sender, CM_JobEventArgs e) => {
			if (e.hasChildJobs) {
				Debug.Log (e.childJobs.Length + " child jobs to process");
			}
		})
			.NotifyOnChildJobComplete ((object sender, CM_JobEventArgs e) => {
			if (e.hasChildJobs) {
				Debug.Log ("Finished processing " + e.childJobs.Length + " child jobs");
			}
		})
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Parent job completed");
		}).Start ();
		
		yield return null;
	}

	/// <summary>
	/// Creates a new job, clones job, and then runs clone.
	/// </summary>
	public IEnumerator SingleCloneJobTest ()
	{
		// Get original job.
		var origJob = GetJobToClone ();

		// Clone job.
		var cloneJobOne = origJob.Clone ();

		// Starts cloned job.
		cloneJobOne.Start ();
		
		yield return null;

	}

	/// <summary>
	/// Creates a new job, clones five copies of the original job, and then starts a random clone.
	/// </summary>
	public IEnumerator MultipleCloneJobTest ()
	{
		// Get original job.
		var origJob = GetJobToClone ();

		// Clone five copies of original job.
		CM_Job[] cloneJobArray = origJob.Clone (5);

		// Start random job.
		cloneJobArray [Random.Range (0, cloneJobArray.Length)].Start ();
		
		yield return null;
	}

	/// <summary>
	/// Creates and starts a job that will repeat for 3 seconds.
	/// The job complete event is subscribed to, this is used to display how many times the job will be repeated.
	/// </summary>
	public IEnumerator InfinitelyRepeatableJobTest ()
	{
		CM_Job.Make (PrintStringAfterDelay ("Infinitely repeatable Test"))
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Job repeated " + e.job.numOfTimesExecuted + " times");
		})
			.Repeat ().StopRepeat (3f).Start ();
		
		yield return null;

	}

	/// <summary>
	/// Creates and starts a job that will repeat three times.
	/// The job complete event is subscribed to, this is used to display how many times the job will be repeated.
	/// </summary>
	public IEnumerator MultipleRepeatableJobTest ()
	{
		int numOfTimesToRepeat = 3;

		CM_Job.Make (PrintStringAfterDelay ("Multiple repeatable test"))
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Job repeated " + e.job.numOfTimesExecuted + " of " + numOfTimesToRepeat + " times");
		})
				.Repeat (numOfTimesToRepeat).Start ();
		
		yield return null;
	}

	/// <summary>
	/// Creates and starts a job that will repeat three times. Adds a child job that will also repeat three times.
	/// The job complete event and child job complete events are subscribed to, this is used to display how many times the job will be repeated.
	/// </summary>
	public IEnumerator MutltipleRepeatableJobTestWithChild ()
	{
		int numOfTimesToRepeat = 3;
		
		CM_Job.Make (PrintStringAfterDelay ("Repeating test with child"))
			.AddChild (PrintStringAfterDelay ("Repeating test (child)"))
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Parent job repeated " + e.job.numOfTimesExecuted + " of " + numOfTimesToRepeat + " times");
		})
			.NotifyOnChildJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Child job repeated " + (e.job.numOfTimesExecuted + 1) + " of " + numOfTimesToRepeat + " times");
		})
			.Repeat (numOfTimesToRepeat).Start ();
		
		yield return null;
	}


	#region HelperMethods
	private IEnumerator PrintStringAfterDelay (string a, float delay = 0f)
	{
		yield return new WaitForSeconds (delay);
		Debug.Log (a);
	}
	
	private IEnumerator InfiniteTest (string a)
	{
		var wait = new WaitForSeconds (0.5f);
		
		int i = 0;
		while (true) {
			Debug.Log (a + ": " + ++i);
			yield return wait;
		}
	}

	/// <summary>
	/// Creates and returns a job to be cloned in tests.
	/// </summary>
	/// <returns>The job to clone.</returns>
	private CM_Job GetJobToClone ()
	{
		return CM_Job.Make (PrintStringAfterDelay ("Clone job test"), "cloned_id")
			.NotifyOnJobStarted ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned job started");
		})
				.NotifyOnJobPaused ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned job paused");
		})
				.NotifyOnJobResumed ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned job resumed");
		})
				.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned job complete");
		})
				.NotifyOnChildJobStarted ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned child jobs started");
		})
				.NotifyOnChildJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log ("Cloned child jobs completed");
		})
				.AddChild (PrintStringAfterDelay ("Clone Child Test"));
	}
	#endregion

}
