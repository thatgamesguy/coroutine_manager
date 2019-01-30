using UnityEngine;
using System.Collections;

/// <summary>
/// A base class for any Singleton. Provides global singular access to a MonoBehaviour.
/// </summary>
public abstract class CM_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	/// <summary>
	/// Gets a value indicating whether this instance is destroyed.
	/// </summary>
	/// <value><c>true</c> if is destroyed; otherwise, <c>false</c>.</value>
	public static bool IsDestroyed { get { return _instance == null;	}}

	private static bool _applicationIsQuitting = false;

	/// <summary>
	/// Sets whether this instance will be lazily initialised i.e. only initialised when needed.
	/// </summary>
	private static readonly bool LAZY_INIT = false;
	
	private static T _instance = null;

	/// <summary>
	/// Gets the instance. The instance is created if not currently past of the scene.
	/// </summary>
	/// <value>The instance.</value>
	public static T instance {
		get {

			if (_applicationIsQuitting) {
				return null;
			}

			if (!_instance) {
				_instance = GameObject.FindObjectOfType<T> ();
				
				if (!_instance) {
					_instance = new GameObject().AddComponent<T> ();
					_instance.gameObject.name = _instance.GetType ().Name;
				}
			}

			return _instance;
		}
	}

	void Awake ()
	{
		if (!LAZY_INIT) {
			_instance = GameObject.FindObjectOfType<T> ();
		}
	}
	
	protected virtual void OnDestroy () 
	{
		_instance = null;
		_applicationIsQuitting = true;
	}
	
	protected virtual void OnApplicationQuit () 
	{
		_instance = null;
		_applicationIsQuitting = true;
	}


}
