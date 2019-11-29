using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxdraw : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int i1 = 0; i1 < 10; i1++)
		{
			for (int i2 = 0; i2 < 10; i2++)
			{
                for (int i3 = 0; i3 < 10; i2++)
                {
                    GameObject myObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    myObj.transform.position = new Vector3(i1, i2, i3);
                    //myObj.gameObject.tag = "surface";
                    Rigidbody gameObjectsRigidBody = myObj.AddComponent<Rigidbody>();
                }

			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
