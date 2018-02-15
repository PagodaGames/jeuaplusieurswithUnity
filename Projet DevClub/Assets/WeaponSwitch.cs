using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour {

	public GameObject equippedWeapon;
	private Rigidbody equippedWeaponRb;
	public Gun equippedWeaponGunScr;
	public GameObject weaponHolder;
	public float pickUpRadius=1f;

	void Start ()
	{
		if (equippedWeapon != null) {

			equippedWeapon.transform.position = weaponHolder.transform.position;
			equippedWeapon.transform.rotation = weaponHolder.transform.rotation;
			equippedWeapon.transform.SetParent (weaponHolder.transform);
			equippedWeaponRb = equippedWeapon.GetComponent<Rigidbody> ();
			equippedWeaponGunScr = equippedWeapon.GetComponent<Gun>();
			equippedWeaponRb.isKinematic = true;
		}
	}

	public void DropWeapon ()
	{
		if (equippedWeapon != null) {
			equippedWeapon.transform.parent = null;
			equippedWeaponRb.isKinematic = false;
			equippedWeaponRb = null;
			equippedWeaponGunScr = null;
			equippedWeapon = null;
		}else
			Debug.Log("Error : Dropping weapon when none is equipped !");
	}
	//Pick up a weapon and returns true if a weapon was found, false if none was found
	public bool PickUpWeapon ()
	{
		Collider[] colliders;
		colliders = Physics.OverlapSphere (transform.position, pickUpRadius);
		foreach (Collider col in colliders) {
			if (col.tag == "Weapon") {
				equippedWeapon = col.gameObject;
				break;
			}
		}
		//Si on trouve une arme
		if (equippedWeapon != null) {
			equippedWeapon.transform.position = weaponHolder.transform.position;
			equippedWeapon.transform.rotation = weaponHolder.transform.rotation;
			equippedWeapon.transform.parent = weaponHolder.transform;
			equippedWeaponRb = equippedWeapon.GetComponent<Rigidbody> ();
			equippedWeaponGunScr = equippedWeapon.GetComponent<Gun> ();
			equippedWeaponRb.isKinematic = true;
			return true;
		}else return false;
	}

}
