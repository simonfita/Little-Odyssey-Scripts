using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractFlagMesh : MonoBehaviour
{
    public Renderer mesh;

    public void SetFlagColor(ContractColor flag)
    {       
        mesh.gameObject.SetActive(true);
        mesh.material = Contracts.GetMaterialForContractColor(flag); ;
    }

    public void ClearFlag()
    {
        mesh.gameObject.SetActive(false);
    }
}
