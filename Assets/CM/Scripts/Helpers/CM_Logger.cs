using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple logging class used by the Coroutine Manager.
/// </summary>
public class CM_Logger : CM_Singleton<CM_Logger>  
{
	/// <summary>
	/// The status level of the message.
	/// </summary>
	public enum Status
	{
		Log,
		Warning,
		Error
	}

	private Dictionary<Status, Action<string>> _statuslookup = new Dictionary<Status, Action<string>> ();

	private const string PRE_TEXT = "[CM]";

	void Awake ()
	{
		_statuslookup.Add (Status.Log, Debug.Log);
		_statuslookup.Add (Status.Warning, Debug.LogWarning);
		_statuslookup.Add (Status.Error, Debug.LogError);
	}

	/// <summary>
	/// Log the specified message with default log status.
	/// </summary>
	/// <param name="message">Message.</param>
	public void Log (object message)
	{
		Log (message, Status.Log);
	}

	/// <summary>
	/// Log the message with the specified context.
	/// </summary>
	/// <param name="context">Context i.e. the calling class.</param>
	/// <param name="message">Message.</param>
	public void Log (object context, object message)
	{
		Log (new Message (context, message), Status.Log);
	}

	/// <summary>
	/// Uses a lookup to either write a log, warning, or error based on the status.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="status">Status.</param>
	public void Log (object message, Status status)
	{
		_statuslookup[status] (string.Format ("{0} {1}", PRE_TEXT, message.ToString ()));
	}
	
	/// <summary>
	/// Encapsulates a message used by <see cref="CM_Logger"/>.
	/// </summary>
	public class Message 
	{
		/// <summary>
		/// Gets the content of the message.
		/// </summary>
		/// <value>The content.</value>
		public object content {get;private set;}

		/// <summary>
		/// Gets the invoker of the message.
		/// </summary>
		/// <value>The invoker.</value>
		public object invoker {get;private set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_Logger+Message"/> class.
		/// </summary>
		/// <param name="invoker">Invoker.</param>
		/// <param name="content">Content.</param>
		public Message (object invoker, object content)
		{
			this.content = content;
			this.invoker = invoker;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CM_Logger+Message"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CM_Logger+Message"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[{0}] [{1}]: {2}", System.DateTime.Now, invoker.GetType ().Name, content.ToString ());
		}
	}
}
