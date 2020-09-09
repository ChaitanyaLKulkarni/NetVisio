using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disturbance : BaseComp {


	
	void Start() {
        Para["DisplayName"] = "";

        gameObject.tag = "Noise";

        //adding rigidbody to detect the collision
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
	}

}
