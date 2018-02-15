using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float damage;
	public float knockBack;
	public LayerMask hitsLayers;

	public GameObject impactEffect;
	private Vector3 previousPos;


	void Update ()
	{
		RaycastHit hit;
		if (Physics.Linecast (previousPos, transform.position, out hit, hitsLayers)) {
			Debug.Log (hit.transform.name);

			Unit u = hit.transform.GetComponentInParent<Unit> ();
			if (u != null)
				u.TakeDamage (damage);

			Rigidbody rb = hit.rigidbody;
			if (rb != null)
				rb.AddForce(-hit.normal*knockBack);
				

			GameObject b = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
			Destroy(b,3f);

			Destroy(gameObject);
		}
	}

	void OnCollisionEnter (Collision col)
	{
		Debug.Log (col.transform.name);

		Unit u = col.transform.GetComponentInParent<Unit> ();
		if (u != null)
			u.TakeDamage (damage);

		Rigidbody rb = col.rigidbody;
		if (rb != null)
			rb.AddForce(-col.contacts[0].normal*knockBack);
				

		GameObject b = Instantiate(impactEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
		Destroy(b,3f);

		Destroy(gameObject);
	}

	void LateUpdate()
	{
		previousPos = transform.position;
	}
}
