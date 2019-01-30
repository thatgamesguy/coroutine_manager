using UnityEngine;
using System.Collections;

/// <summary>
/// Includes a number of methods to test the functionality of the <see cref="CM_JobManager"/> class.
/// Each method showcases a particular functionality of the CM_JobManager that you can implement in your own projects.
/// </summary>
public class ExampleJobManagerTest : MonoBehaviour
{
	/// <summary>
	/// Runs all test methods.
	/// </summary>
	void Start ()
	{
		SetupGlobalJob ();
		GlobalJobManagerEventTest ();
		GlobalJobStartTest ();
		GlobalJobPauseTest ();
		GlobalJobResumeTest ();
		GlobalJobStopTest ();

		SetupGroupJobs ();
		GlobalStartAllTest ();
		DelayedPauseAllTest ();
		DelayedResumeAllTest ();
		DelayedKillAllTest ();

		LocalJobManagerTest ();

	}

	/// <summary>
	/// Subscribes to each of the global managers events.
	///  NotifyOnJobAdded: called everytime a job is added to the global manager, NotifyOnJobRemoved: called everytime
	/// a job is removed, NotifyOnAllJobsKilled: called when <see cref="CM_JobManager.KillAll"/> is invoked, 
	/// NotifyOnAllJobsResumed: called when <see cref="CM_JobManager.ResumeAll"/> is called, 
	/// NotifyOnAllJobsPaused: called when <see cref="CM_JobManager.PauseAll"/> is invoked, and
	/// NotifyOnAllJobsCleared: called when <see cref="CM_JobManager.ClearJobList"/> is invoked.
	/// </summary>
	public void GlobalJobManagerEventTest ()
	{
		CM_JobManager.Global
			.NotifyOnJobAdded ((object sender, CM_JobManagerJobEditedEventArgs e) => {
			Debug.Log ("Job added to global manager: " + e.jobEdited.id);
		}).NotifyOnJobRemoved ((object sender, CM_JobManagerJobEditedEventArgs e) => {
			Debug.Log ("Job removed from the global manager: " + e.jobEdited.id);
		}).NotifyOnAllJobsKilled ((object sender, CM_JobManagerEventArgs e) => {
			Debug.Log ("All jobs killed");
		}).NotifyOnAllJobsResumed ((object sender, CM_JobManagerEventArgs e) => {
			Debug.Log ("All jobs resumed");
		}).NotifyOnAllJobsPaused ((object sender, CM_JobManagerEventArgs e) => {
			Debug.Log ("All jobs paused");
		}).NotifyOnAllJobsCleared ((object sender, CM_JobManagerEventArgs e) => {
			Debug.Log ("All jobs cleared");
		});
	}

	/// <summary>
	/// Starts the test job.
	/// </summary>
	public void GlobalJobStartTest ()
	{
		CM_JobManager.Global.StartCoroutine ("infinite_job");
	}

	/// <summary>
	/// Pauses the test job.
	/// </summary>
	public void GlobalJobPauseTest ()
	{
		CM_JobManager.Global.PauseCoroutine ("infinite_job");
	}

	/// <summary>
	/// Resumes the test job.
	/// </summary>
	public void GlobalJobResumeTest ()
	{
		CM_JobManager.Global.ResumeCoroutine ("infinite_job");
	}

	/// <summary>
	/// Stops the test job. This also removes the reference from the JobManager.
	/// </summary>
	public void GlobalJobStopTest ()
	{
		CM_JobManager.Global.StopCoroutine ("infinite_job");
	}

	/// <summary>
	/// Starts all jobs owned by the global JobManager.
	/// </summary>
	public void GlobalStartAllTest ()
	{
		CM_JobManager.Global.StartAll ();
	}

	/// <summary>
	/// Pauses all jobs owned by the global JobManager after 1 second has passed.
	/// </summary>
	public void DelayedPauseAllTest ()
	{
		CM_JobManager.Global.PauseAll (1f);
	}

	/// <summary>
	/// Resumes all jobs owned by the global JobManager after 1.5 seconds have passed.
	/// </summary>
	public void DelayedResumeAllTest ()
	{
		CM_JobManager.Global.ResumeAll (1.5f);
	}

	/// <summary>
	/// Kills all jobs owned by the global JobManager after 2 seconds have passed. This also removes all references
	/// to those jobs from the JobManager.
	/// </summary>
	public void DelayedKillAllTest ()
	{
		CM_JobManager.Global.KillAll (2f);
	}

	/// <summary>
	/// Local job manager test. Creates a new local JobManager, subscribes to
	///  <see cref="CM_JobManager.jobAdded"/>  and  <see cref="CM_JobManager.jobRemoved"/>  events,
	/// adds a test job to the local JobManager, and finally starts, pauses, resumes, and stops this test job.
	/// This is used to show that anything you can do with the glocal JobManager you can also do with a local JobManager. This is useful
	/// if you want to create seperate JobManagers for seperate parts of your codebase.
	/// </summary>
	public void LocalJobManagerTest ()
	{
		var localJobManager = CM_JobManager.Make ();

		localJobManager.NotifyOnJobAdded ((object sender, CM_JobManagerJobEditedEventArgs e) => {
			Debug.Log ("Job added to local manager: " + e.jobEdited.id);
		}).NotifyOnJobRemoved ((object sender, CM_JobManagerJobEditedEventArgs e) => {
			Debug.Log ("Job removed from the local manager: " + e.jobEdited.id);
		});

		localJobManager.AddJob (GetSimpleInfiniteTestJob ("Local job one", "local_job_1"));

		localJobManager.StartCoroutine ("local_job_1");
		localJobManager.PauseCoroutine ("local_job_1");
		localJobManager.ResumeCoroutine ("local_job_1");
		localJobManager.StopCoroutine ("local_job_1");
	}

	#region HelperMethods
	private void SetupGlobalJob ()
	{
		var testJob = GetSimpleInfiniteTestJob ("Infinite Job", "global_job_1");
		
		CM_JobManager.Global.AddJob (testJob);
	}

	private void SetupGroupJobs ()
	{
		for (int i = 0; i < 4; i++) {
			CM_JobManager.Global.AddJob (
				GetSimpleInfiniteTestJob ("Infinite job: " + (i + 1), "global_job_" + (i + 2)));
		}
	}

	private CM_Job GetSimpleInfiniteTestJob (string output, string id)
	{
		return CM_Job.Make (InfiniteTest (output), id)
			.NotifyOnJobPaused ((object sender, CM_JobEventArgs e) => {
			Debug.Log (e.job.id + ": paused");
		})
			.NotifyOnJobResumed ((object sender, CM_JobEventArgs e) => {
			Debug.Log (e.job.id + ": resumed");
		})
			.NotifyOnJobComplete ((object sender, CM_JobEventArgs e) => {
			Debug.Log (e.job.id + ": complete, killed = " + e.job.jobKilled);
		});

	
	}

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
