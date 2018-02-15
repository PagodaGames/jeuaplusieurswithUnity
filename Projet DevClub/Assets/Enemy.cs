using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	private GameObject player;

	private bool playerSpotted= false; //Front montant de playerInSight
	private bool playerInSightInPreviousFrame = false;
	private bool playerInSight= false; //Sees the player
	private bool playerDetected = false; //Is aware of the player's presence (has seen him at least once or got hit)

	public float reactionTime = 2f;


	private Animator anim;
	public GameObject eyes;
	private AimAt aimingScr;

	public GameObject gun;
	private Gun gunScr;
	private LineRenderer laser;

	private Unit unit;

	private float nextTimeToFire = 0f;

	public LayerMask cantSeeThrough;

	public GameObject bulletStart;



	void Start () {
		player=GameObject.Find("Player");
		anim=GetComponent<Animator>();
		aimingScr=GetComponent<AimAt>();
		gunScr=gun.GetComponent<Gun>();
		if(aimingScr.laserSight != null)
			laser = aimingScr.laserSight.GetComponent<LineRenderer>();
		unit = GetComponent<Unit>();

		if( player != null) aimingScr.target = player;
	}

	void Update ()
	{	
		
		if (!unit.IsDead ()) {
			RaycastHit hit;
			playerInSight = !Physics.Linecast (eyes.transform.position, player.transform.position, out hit, cantSeeThrough);

			if (playerInSight || !unit.IsFullLife ())
				playerDetected = true;

			anim.SetBool ("aiming", playerInSight);

			aimingScr.looksAtTarget = !(playerDetected && !playerInSight); //Ne regarde pas le joueur si il n'est pas en vue



			playerSpotted = (playerInSight && !playerInSightInPreviousFrame);

			if (playerSpotted) {
				nextTimeToFire = Time.time + reactionTime;
			}
			if (playerDetected) {
				aimingScr.enabled = true;

			}


			if (playerInSight && Time.time >= nextTimeToFire) {
				gunScr.Shoot (bulletStart.transform.position, aimingScr.trueAimPos);
				nextTimeToFire = Time.time + 1 / gunScr.fireRate;
			}



		}

		if(laser !=null)
			laser.enabled = playerInSight && !unit.IsDead();

		playerInSightInPreviousFrame = playerInSight;
	}

}
