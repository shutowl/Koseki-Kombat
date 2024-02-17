using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{

	private PlayerControls player;

	void Start()
	{
		player = gameObject.GetComponentInParent<PlayerControls>();
	}

	//If ground collider touches the floor (tilemap)
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Ground"))
		{
			player.grounded = true;

			//AudioManager.Instance.Play("JumpLand");
		}
	}

	//If ground collider is still touching the floor 
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Ground"))
		{
			player.grounded = true;
		}
	}

	//When ground collider leaves the floor
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Ground"))
		{
			player.grounded = false;
		}
	}
}