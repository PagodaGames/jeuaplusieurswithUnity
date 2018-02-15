using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AimAt : MonoBehaviour {

	//private Animator animator;
	public GameObject spine;

	[HideInInspector] public Vector3 aimPos;
	public Camera cam;
	public Vector3 aoffset;

	public bool looksAtTarget = true;
	public bool aimsOnVerticalPlane = true;

	public GameObject target;
	public bool followMouse = false;
	public LayerMask mouseLaysOn;

	public bool laserActivated = true;
	public GameObject laserSight;
	private LineRenderer laser;

	public float lerpValue = 1f; // Entre 0 et 1
	[HideInInspector] public Vector3 trueAimPos;




	void Start ()
	{
		//animator = GetComponent<Animator> ();
		cam = Camera.main;

		if (laserSight != null) {
			laser = laserSight.GetComponent<LineRenderer> ();
			laser.useWorldSpace = true;
		}else
			laserActivated=false;

		trueAimPos=transform.position+transform.forward*5f;
	}
	

    void LateUpdate ()
	{
		if (followMouse) {
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);// Rayon caméra-position de la souris dans le monde
			Plane plane = new Plane (Vector3.forward, Vector3.zero); //Plan où on lira l'intersection avec le rayon

			float distance;
			plane.Raycast (ray, out distance);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 1000f, mouseLaysOn))
				aimPos = hit.point;
			else
				aimPos = ray.GetPoint (distance); // position de la souris dans le world
			Debug.DrawLine (ray.origin, aimPos);
		} else {
			if (target != null)
				if(aimsOnVerticalPlane) 
					aimPos = Vector3.ProjectOnPlane(target.transform.position,Vector3.forward);
				else
					aimPos = target.transform.position;

		}

		trueAimPos=Vector3.Lerp(trueAimPos,aimPos,lerpValue*Time.deltaTime);


		//Orientation du buste vers le point visé
		if (looksAtTarget) {
			spine.transform.LookAt (trueAimPos);
			if (aimsOnVerticalPlane) {
				Vector3 vrot = spine.transform.eulerAngles;

				spine.transform.eulerAngles=vrot;
			} 

			spine.transform.Rotate (aoffset);
			Debug.DrawLine(Vector3.ProjectOnPlane(transform.position,Vector3.forward), trueAimPos);
		}

		//Laser
		if (laserActivated) {
			laser.SetPosition (0, laserSight.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (laserSight.transform.position, trueAimPos - laserSight.transform.position, out hit))
				laser.SetPosition (1, hit.point);
			else
				laser.SetPosition (1, laserSight.transform.position + 100f * laserSight.transform.forward);
		}



	}

	public Vector3 AimDir ()
	{
		return (aimPos - (Vector3.ProjectOnPlane(transform.position,Vector3.forward))).normalized;
	}

	public Vector3 TrueAimDir ()
	{
		return (trueAimPos - (Vector3.ProjectOnPlane(transform.position,Vector3.forward))).normalized;
	}


}
