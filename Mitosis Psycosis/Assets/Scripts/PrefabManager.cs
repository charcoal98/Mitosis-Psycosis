using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public static PrefabManager instance;
	
	public GameObject discardedSlime;
	public GameObject newPlayerSlime;

	void Awake() {
		instance = this;
	}
}
