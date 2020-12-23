using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABC_AssetID : MonoBehaviour{

    public string abcAssetID = "";


   public string GenerateAssetID() {

        if (String.IsNullOrEmpty(this.abcAssetID) == false)
            return this.abcAssetID;


        this.abcAssetID = System.Guid.NewGuid().ToString();

        return this.abcAssetID; 

    }


    public string GetABCAssetID() {
        return this.abcAssetID;
    }


}
