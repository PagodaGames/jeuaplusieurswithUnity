using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour {

	public GameObject target;
	[Range(0f,10f)] public float range;
	public Vector3 offset;

	void Start(){
		offset = transform.position - target.transform.position;
	}

	void LateUpdate () {
		transform.position = target.transform.position + offset - range*transform.forward;
	}
}
