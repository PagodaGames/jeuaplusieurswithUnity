using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// Physics
	public Vector3 speed;
	public float maxSpeed;
	public float jumpSpeed;
	public float gravity;


	private bool isGrounded;
	private bool wasGroundedLastFrame;

	public Transform groundCheck;
	private float groundRadius = .2f;

	private CharacterController controller;
	private Animator anim;
	private AimAt aiming;

	public GameObject gun;
	private float nextTimeToFire = 0f;
	public float aimingTime;
	private float nextTimeToLowerGun = 0f;
	public GameObject shootingPivot;
	private GameObject bulletStart;

	private WeaponSwitch weaponSwitch;
	private int layerAmount = 2;


	void Start ()
	{
		speed = Vector3.zero;
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		aiming = GetComponent<AimAt> ();
		weaponSwitch = GetComponent<WeaponSwitch> ();


		for (int i = 1; i <= layerAmount; i++) {
				anim.SetLayerWeight (i, 0);
			}
		if (weaponSwitch.equippedWeapon != null) {
			aiming.looksAtTarget=true;
			anim.SetLayerWeight (weaponSwitch.equippedWeaponGunScr.holdingPose, 100);
		}
		else{
			aiming.looksAtTarget=false;
		}

		bulletStart=shootingPivot.transform.FindChild("BulletStart").gameObject;
		if(bulletStart == null)	Debug.Log("Error : BulletStart not found !");

	}

	void Update ()
	{
		isGrounded = false;

		//Si il y a du sol dans le groudCheck, ie sous le joueur
		Collider[] colliders = Physics.OverlapSphere (groundCheck.position, groundRadius);
		foreach (Collider col in colliders) {
			if (col.gameObject.layer == 8)
				isGrounded = true;
		}

		//Mouvement
		speed.x = Input.GetAxis ("Horizontal") * maxSpeed;

		//Saut
		if (isGrounded) {
			if (Input.GetKeyDown ("up")) {
				speed.y = jumpSpeed;
				anim.SetTrigger ("jump");
			}
			if (speed.y < 0)
				speed.y = 0;
		}

		//Gravité
		if (!isGrounded)
			speed.y -= gravity * Time.deltaTime;

		//Si vitesse non nulle, orienter le joueur dans la direction de la vitesse projetée sur le plan horizontal, et dans la direction opposée si il recule en regardant devant lui
		if (Vector3.ProjectOnPlane (speed, Vector3.up).magnitude> 0.1f) {
			int flip;
			if(aiming.looksAtTarget) flip=(Vector3.Dot (speed, aiming.AimDir()) >= 0) ? 1 : -1;
			else flip=1;
			transform.rotation = Quaternion.LookRotation (Vector3.ProjectOnPlane (flip * speed, Vector3.up));
		}
			

		//Update Animation
		controller.Move (speed * Time.deltaTime);
		anim.SetFloat ("speed", Vector3.ProjectOnPlane (speed, Vector3.up).magnitude);
		anim.SetBool ("runsBackward", !(Vector3.Dot (speed, aiming.AimDir()) >= 0));
		anim.SetBool ("grounded", isGrounded);


		//Position du BulletStart
		shootingPivot.transform.rotation=Quaternion.LookRotation(aiming.TrueAimDir());
		Debug.DrawRay(shootingPivot.transform.position,aiming.TrueAimDir());

		//Feu
		if (weaponSwitch.equippedWeapon != null && Time.time >= nextTimeToFire) {
			if ((weaponSwitch.equippedWeaponGunScr.automatic && Input.GetButton ("Fire1")) || Input.GetButtonDown ("Fire1")) {
				weaponSwitch.equippedWeaponGunScr.Shoot (bulletStart.transform.position,aiming.aimPos);
				anim.SetTrigger ("shoots");
				nextTimeToFire = Time.time + 1 / weaponSwitch.equippedWeaponGunScr.fireRate;
				nextTimeToLowerGun = Time.time + aimingTime;
			}
		}

		if (Time.time >= nextTimeToLowerGun) {
			//Ajouter une animation de position de combat universelle
		} else {
			
		}

		//WeaponSwitch
		if (Input.GetButtonDown ("Fire2")) {
			
			for (int i = 1; i <= layerAmount; i++) {
				anim.SetLayerWeight (i, 0);
			}
			if (weaponSwitch.equippedWeapon == null) {
				aiming.looksAtTarget=true;
				if(weaponSwitch.PickUpWeapon())
					anim.SetLayerWeight (weaponSwitch.equippedWeaponGunScr.holdingPose, 100);
			} else {
				weaponSwitch.DropWeapon ();
				aiming.looksAtTarget=false;
			}
		}

		if(!isGrounded && wasGroundedLastFrame) anim.SetTrigger("falls"); //if start falling

		wasGroundedLastFrame = isGrounded;
	}


}
