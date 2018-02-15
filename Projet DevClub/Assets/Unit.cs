using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour {

	public float maxHp = 100;
	[Range(0,100)] public float hp = 100;
	public bool isPlayer = false;
	public bool ragdollUponDeath = false;
	public MonoBehaviour[] behaviourToDisableUponDeath;

	private Rigidbody[] bodies;

	void Start()
	{
		maxHp = hp;
		bodies = GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody rb in bodies) {
			rb.isKinematic = true;
		}

	}


	public void TakeDamage (float damage)
	{
		if (!IsDead ()) {
			hp -= damage;
			if (hp <= 0) {
				hp = 0;
				Die ();
			}
		}
	}


	public void Die ()
	{	
		if (ragdollUponDeath) {
			GetComponent<Animator> ().enabled = false;
			foreach (MonoBehaviour b in behaviourToDisableUponDeath) {
				b.enabled = false;
			}
			foreach (Rigidbody rb in bodies) {
				rb.isKinematic = false;
			}
			changeLayer(transform, 9); //Corpse

		}
		else Destroy(gameObject);

	}

	private void changeLayer (Transform root, int layer)
	{
		root.gameObject.layer = layer;
		foreach (Transform child in root) {
			changeLayer(child,layer);
		}
	}

	public bool IsFullLife()
	{
		return (hp >= maxHp);
	}

	public bool IsDead ()
	{	
		return (hp <= 0);
	}

}
