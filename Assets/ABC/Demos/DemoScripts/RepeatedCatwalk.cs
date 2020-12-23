using UnityEngine;

/// <summary>
/// Will make the entity repeatedly walk forward, turn around then walk forward again. 
/// </summary>
public class RepeatedCatwalk : MonoBehaviour {

    public float moveSpeed = 20f;
    public float turnInterval = 8f;

    //Move the entity forward
    void MoveForward(){
        this.transform.Translate(Vector3.forward * this.moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Will rotate the entity 180, turning them around. 
    /// </summary>
	void TurnAround(){
        transform.Rotate(0, 180, 0);
    }



    void Start () {
		InvokeRepeating ("TurnAround", 0f, this.turnInterval);
	}

    
	void Update () {
        this.MoveForward();
	}



}
