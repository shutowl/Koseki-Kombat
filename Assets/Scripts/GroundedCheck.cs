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

			if(PlayerPrefs.GetInt("option1") == 1)
            {
				player.stepTimer = player.stepRate;
				AudioManager.Instance.PlayOneShot("Step" + Random.Range(1, 11));
			}
			else if (PlayerPrefs.GetInt("option1") == 2)
			{
				player.stepTimer = player.stepRate;
				AudioManager.Instance.PlayOneShot("LoudStep" + Random.Range(1, 11));
			}
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