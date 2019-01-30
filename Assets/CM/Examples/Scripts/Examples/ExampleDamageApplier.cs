using UnityEngine;
using System.Collections;

/// <summary>
/// Applies damage actions to the example character.
/// </summary>
public class ExampleDamageApplier : MonoBehaviour 
{
	/// <summary>
	/// The character to apply damage actions to.
	/// </summary>
	public ExampleCharacterDamage character;

	private CM_Job fireDamage, poisonDamage, heal;

	/// <summary>
	/// Instantiates three jobs to be sent to the character.
	/// </summary>
	void Start () 
	{
		fireDamage = CM_Job.Make (character.ApplyDamage ("fire damage", 1f));
		poisonDamage = CM_Job.Make (character.ApplyDamage ("poison damage", 1f));
		heal = CM_Job.Make (character.RestoreHealth (1f));
	}

	void Update () 
	{
		if (Input.GetKeyUp (KeyCode.Alpha1)) {
			character.AddActionToQueue (fireDamage.Clone ());
		} else if (Input.GetKeyUp (KeyCode.Alpha2)) {
			character.AddActionToQueue (poisonDamage.Clone ());
		} else if (Input.GetKeyUp (KeyCode.Alpha3)) {
			character.AddActionToQueue (heal.Clone ());
		}
	
	}
}
