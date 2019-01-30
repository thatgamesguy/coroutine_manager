using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Includes a number of methods to test the functionality of the <see cref="CM_JobQueue"/> class.
/// Each method showcases a particular functionality of the CM_JobQueue that you can implement in your own projects.
/// Each method returns an ienumerator so that it can added to a seperate job queue to be run in sequence for test purposes.
/// </summary>
public class ExampleJobQueueTest : MonoBehaviour
{

	/// <summary>
	/// Adds each method to a queue to be run in sequene.
	/// </summary>
	void Start ()
	{
		CM_JobQueue.Make ()
			.Enqueue (SimpleGlobalQueueTest ())
			.Enqueue (SimpleLocalQueueTest ())
			.Enqueue (LocalQueueDelayedStartTest ())
			.Enqueue (LocalQueueDelayedPauseAndResumeTest ())
			.Enqueue (DelayedKillCurrentTest ())
			.Enqueue (DelayedKillAllTest ())
			.Enqueue (AddLocalQueueToGlobalQueueTest ())
			.Enqueue (AddRepeatingJobToQueueTest ())
			.Enqueue (SetNumberRepeatingQueueTest ())
			.Enqueue (TimedRepeatingQueueTest ())
			.Enqueue (ClonedRepeatingQueueTest ())
			.Enqueue (MultipleClonedQueueTest ())
			.Enqueue (QueueEventTest ())
			.Start ();
	}
	

	// test local and global queues

	/// <summary>
	/// Creates a list of jobs, adds them to the global queue and starts global queue. The global queue can be accessed
	/// from any script.
	/// </summary>
	public IEnumerator SimpleGlobalQueueTest ()
	{
		// Creates a list of jobs with three entries.
		var jobsToQueue = new List<CM_Job> () {CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job one"), "job_1"), 
												CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job two"), "job_2"),
												CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job three"), "job_3")}; 

		CM_JobQueue.Global.Enqueue (jobsToQueue).Start ();

		yield return null;
	}

	/// <summary>
	/// Creates a list of jobs, adds them to a newly created local queue and starts the queue. 
	/// </summary>
	public IEnumerator SimpleLocalQueueTest ()
	{
		// Creates a list of jobs with three entries.
		var jobsToQueue = new List<CM_Job> () {CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job one"), "job_1"), 
			CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job two"), "job_2"),
			CM_Job.Make (PrintStringAfterDelay ("Simple global queue test: job three"), "job_3")}; 

		CM_JobQueue.Make ().Enqueue (jobsToQueue).Start ();

		yield return null;
	}

	/// <summary>
	/// Creates and starts a queue after a delay of two seconds.
	/// </summary>
	public IEnumerator LocalQueueDelayedStartTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Delayed queue start, job 1"))
			.Enqueue (PrintStringAfterDelay ("Delayed queue start, job 2"))
			.Start (2f); 

		yield return null;
	}

	/// <summary>
	/// Creates a local queue, starts queue, pauses queue after 1 seconds, and lastly resumes queue after 3 seconds.
	/// </summary>
	public IEnumerator LocalQueueDelayedPauseAndResumeTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Delayed pause and resume test, job one"))
			.Enqueue (PrintStringAfterDelay ("Delayed pause and resume test, job two"))
			.Enqueue (PrintStringAfterDelay ("Delayed pause and resume test, job three"))
			.Enqueue (PrintStringAfterDelay ("Delayed pause and resume test, job four"))
			.Start ()
			.Pause (1f)
			.Resume (3f); 

		yield return null;
	}

	/// <summary>
	/// Subscribes to the global queue "queueStarted", "jobProcessed", and "queueComplete" events.
	/// </summary>
	public IEnumerator QueueEventTest ()
	{
		CM_JobQueue.Global
			.NotifyOnQueueStarted ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Global queue started processing. " +
				"Number in queue: " + (e.hasJobsInQueue ? e.queuedJobs.Length.ToString () : "0"));
		}) 
			.NotifyOnJobProcessed ((object sender, CM_QueueEventArgs e) => {
			if (e.hasCompletedJobs) {
				Debug.Log ("Finished processing global job queue: " 
					+ e.completedJobs [e.completedJobs.Length - 1].id);
			}
		})
			.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			if (e.hasCompletedJobs) {
				int numOfJobsKilled = e.completedJobs.Where (i => i.jobKilled == true).Count ();
						
				Debug.Log ("Global queue started processing. " +
					"Number of jobs completed successfully: " 
					+ (e.completedJobs.Length - numOfJobsKilled)
					+ ", number of jobs killed: " + numOfJobsKilled);
			}
		});

		yield return null;
	}

	/// <summary>
	/// Creates a local queue and sets the current job in the queue to be killed after three seconds.
	/// This kills the currently running job but the queue still executes.
	/// </summary>
	public IEnumerator DelayedKillCurrentTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Delayed kill current test, job one"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill current test, job two"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill current test, job three"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill current test, job four"))
				.Enqueue (InfiniteTest ("Delayed kill current test"))
				.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			if (e.hasCompletedJobs) {
				int numOfJobsKilled = e.completedJobs.Where (i => i.jobKilled == true).Count ();
						
				Debug.Log ("Number of jobs completed successfully: " + (e.completedJobs.Length - numOfJobsKilled)
					+ ", number of jobs killed: " + numOfJobsKilled);
			}
		})
				.Start ()
				.KillCurrent (3f);

		yield return null;
	}

	/// <summary>
	/// Creates a local queue and sets all jobs in the queue to be killed after one second.
	/// This kills all jobs and clears the queue.
	/// </summary>
	public IEnumerator DelayedKillAllTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job one", .5f))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job two", .5f))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job three"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job four"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job five"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job six"))
				.Enqueue (PrintStringAfterDelay ("Delayed kill all test, job seven"))
				.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			if (e.hasCompletedJobs) {

				int numOfJobsKilled = e.completedJobs.Where (i => i.jobKilled == true).Count ();

				Debug.Log ("Number of jobs completed successfully: " + (e.completedJobs.Length - numOfJobsKilled)
					+ ", number of jobs killed: " + numOfJobsKilled);
			}
		})
				.Start ()
				.KillAll (1f);
		
		yield return null;
	}

	/// <summary>
	/// Creates a local queue and then adds it to the glocal queue. 
	/// The event subscriptions are also added to the global queue.
	/// </summary>
	public IEnumerator AddLocalQueueToGlobalQueueTest ()
	{
		CM_JobQueue localQueue = CM_JobQueue.Make ()
			.NotifyOnQueueStarted ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Local queue (run in glocal queue) started");
		})
			.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Local queue (run in glocal queue) finished");
		}).Enqueue (PrintStringAfterDelay ("Local to global queue test: job one"))
			  	.Enqueue (PrintStringAfterDelay ("Local to global queue test: job two"))
				.Enqueue (PrintStringAfterDelay ("Local to global queue test: job three"));

		CM_JobQueue.Global.Enqueue (localQueue).Start ();

		yield return null;
	}

	/// <summary>
	/// Adds a repeating job to a queue. 
	/// Queue will not progress if a repeating job is added until that job is manually killed
	/// or has reached its set number of times to repeat.
	/// </summary>
	public IEnumerator AddRepeatingJobToQueueTest ()
	{
		var jobsToQueue = new List<CM_Job> () {
			CM_Job.Make (PrintStringAfterDelay ("Repeating job added to queue test, job one")), 
			 CM_Job.Make (PrintStringAfterDelay ("Repeating job added to queue test, job two")).Repeat (5),
			 CM_Job.Make (PrintStringAfterDelay ("Repeating job added to queue test, , job one"))};
		
		CM_JobQueue.Make ()
			.Enqueue (jobsToQueue).Start ();

		yield return null;
	}

	/// <summary>
	/// Creates a new local queue, adds a number of test jobs and sets the queue to repeat two times.
	/// </summary>
	public IEnumerator SetNumberRepeatingQueueTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Set number repeating test, job one"))
				.Enqueue (PrintStringAfterDelay ("Set number repeating test, job two"))
				.Enqueue (PrintStringAfterDelay ("Set number repeating test, job three"))
				.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Job repeating: " + e.jobQueue.repeating +
				" num of times repeated: " + e.jobQueue.numOfTimesExecuted);
		})
				.Repeat (2)
				.Start ();

		yield return null;
	}

	/// <summary>
	/// Creates a new local queue, sets it to repeat and then stop repeating after 1 seconds.
	/// </summary>
	public IEnumerator TimedRepeatingQueueTest ()
	{
		CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Time repeating test, job one"))
				.Enqueue (PrintStringAfterDelay ("Time repeating test, job two"))
				.Enqueue (PrintStringAfterDelay ("Time repeating test, job three"))
				.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Job repeating: " + e.jobQueue.repeating +
				" num of times repeated: " + e.jobQueue.numOfTimesExecuted);
		})
				.Repeat ()
				.StopRepeat (1f)
				.Start ();

		yield return null;
	}

	/// <summary>
	/// Creates a local queue and sets it to repeat twice. The queue is then cloned and
	/// started. The new cloned queue will contain the original queues repeat status and event subscriptions.
	/// </summary>
	public IEnumerator ClonedRepeatingQueueTest ()
	{
		var origQueue = CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Cloned repeating test, job one"))
				.Enqueue (PrintStringAfterDelay ("Cloned repeating test, job two"))
				.Enqueue (PrintStringAfterDelay ("Cloned repeating test, job three"))
				.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Job repeating: " + e.jobQueue.repeating +
				" num of times repeated: " + e.jobQueue.numOfTimesExecuted);
		})
				.Repeat (2);
		
		var clonedQueue = origQueue.Clone ();
		clonedQueue.Start ();

		yield return null;
	}

	/// <summary>
	/// Creates a local queue and clones the queue twice. 
	/// Both of the cloned queues are then started.
	/// </summary>
	public IEnumerator MultipleClonedQueueTest ()
	{
		var origQueue = CM_JobQueue.Make ()
			.Enqueue (PrintStringAfterDelay ("Multiple cloned test, job one"))
				.Enqueue (PrintStringAfterDelay ("Multiple cloned test, job two"))
			.NotifyOnQueueStarted ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Clone Started");
		})
			.NotifyOnQueueComplete ((object sender, CM_QueueEventArgs e) => {
			Debug.Log ("Clone Complete");
		});

		var queueClones = origQueue.Clone (2);

		foreach (var clone in queueClones) {
			clone.Start ();
		}

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
	#endregion
}