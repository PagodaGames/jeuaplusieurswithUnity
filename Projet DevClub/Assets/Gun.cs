using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public GameObject muzzle;

	public float fireRate = 15f;
	public float damage = 10f;
	public float knockBack = 10f;
	public bool automatic = true;
	public int ammo = 30;

	private AudioSource audioSource;
	public ParticleSystem muzzleFlash;
	public GameObject bulletImpact;
	public GameObject bulletEffect;
	public AudioClip shotSound;


	public LayerMask hitsLayers;


	public bool bulletsAreRays = true;

	public GameObject bullet;
	public float bulletForce;

	[Range(0,4)] public int holdingPose;

	//Tracer Effect
	private LineRenderer bulletLine;
	private float timeToDisableTracerEffect;
	public float tracerEffectDuration = 0.1f;
	public float tracerLength = 0.2f;


	public void Start(){
		bulletLine = bulletEffect.GetComponent<LineRenderer>();
		audioSource = GetComponent<AudioSource>();

	}

	public void Update(){
		if(Time.time >= timeToDisableTracerEffect) bulletLine.enabled = false;
	}

	public void Shoot (Vector3 startPos, Vector3 aimPos)
	{
		if (ammo > 0) {
			ammo--;
			muzzleFlash.Play ();
			audioSource.PlayOneShot(shotSound);

			if (bulletsAreRays) {

				RaycastHit hit;
				if (Physics.Raycast (startPos, aimPos - startPos, out hit, 1000f, hitsLayers)) {
					Debug.Log (hit.transform.name+" was hit !");

					Unit u = hit.transform.GetComponentInParent<Unit> ();
					if (u != null)
						u.TakeDamage (damage);

					Rigidbody rb = hit.rigidbody;
					if (rb != null)
						rb.AddForce (-hit.normal * knockBack);
					
					GameObject b = Instantiate (bulletImpact, hit.point, Quaternion.LookRotation (hit.normal));
					Destroy (b, 3f);

					bulletLine.enabled = true;
					timeToDisableTracerEffect = Time.time + tracerEffectDuration;
					float r = Random.value;
					float lineLength = Vector3.Distance (bulletEffect.transform.position, hit.point);
					bulletLine.SetPosition (0, r * bulletEffect.transform.position + (1 - r) * hit.point);
					bulletLine.SetPosition (1, (r + tracerLength / lineLength) * bulletEffect.transform.position + (1 - r - tracerLength / lineLength) * hit.point);

				}
			} else {
				GameObject bul = Instantiate (bullet, muzzle.transform.position, muzzle.transform.rotation);
				bul.GetComponent<Rigidbody> ().AddForce ((aimPos - startPos).normalized * bulletForce);
				Bullet bulStats = bul.GetComponent<Bullet> ();
				bulStats.damage = damage;
				bulStats.knockBack = knockBack;
				bulStats.hitsLayers = hitsLayers;
			}
		}
	}

	public void Shoot (Vector3 aimPos)
	{
		Shoot(muzzle.transform.position,aimPos);

	}



}
