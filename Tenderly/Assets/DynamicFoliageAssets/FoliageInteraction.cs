using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageInteraction : MonoBehaviour {

    public Shader foliageShader;

    //MAXIUM OBJECTS = 512 
    public Transform[] foliageAffectorObjects;

    public float windSpeed = 1f;

    void Awake () {
        Shader.SetGlobalVectorArray("_FoliageAffectorObjects", new Vector4[1023]);
    }

	void Update () {

        Shader.SetGlobalFloat("_WindSpeed", windSpeed);

        if (foliageAffectorObjects == null || foliageAffectorObjects.Length <= 0)
            Debug.LogWarning("Assign at least one foliage affector object");
        else
        {
            Vector4[] values = new Vector4[foliageAffectorObjects.Length];
            for (int i = 0; i < foliageAffectorObjects.Length; i++)
            {
                values[i] = foliageAffectorObjects[i].position;
            }


            Shader.SetGlobalVectorArray("_FoliageAffectorObjects", values);
            Shader.SetGlobalInt("_AffectorObjectCount", foliageAffectorObjects.Length);
        }
	}
}
