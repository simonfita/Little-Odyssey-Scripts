using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;


[ExecuteInEditMode]
public class TokenRenderer : MonoBehaviour
{
    private TokenOutfits outfits;

    public GameObject bodyRendMesh, backArmRendMesh, frontArmRendMesh, hairRendMesh, backHairRendMesh, facialHairRendMesh, hatRendMesh, faceRendMesh, headRendMesh;

    public float speechAnimationSpeed = 5;

    [Range(0,7)]
    public int BodyStyle;

    [Range(0, 10)]
    public int hairStyle;

    public bool backHair;

    [Range(0, 12)]
    public int facialHairStyle;

    [Range(0,7)]
    public int hairColor;

    [Range(0, 17)]
    public int hatStyle;

    public bool randomize;

    public string suitName;

    private void Awake()
    {
        if (outfits == null)
            outfits = Resources.Load<TokenOutfits>("TokenOutfits");


        UpdateVisuals();
    }

    private void Update()
    {


#if UNITY_EDITOR
        if (randomize)
            Randomize(); 
        
        if (!Application.isPlaying)
        {
            UpdateSuitPath();
            UpdateVisuals();
        }
#endif

    }

    public void UpdateVisuals()
    {      

        ApplyMeshToObject(bodyRendMesh, "Suit/" + suitName + "/" + outfits.bodies[BodyStyle].name, outfits.bodies[BodyStyle].bounds);

        ApplyMeshToObject(backArmRendMesh, "Suit/" + suitName + "/" + outfits.backArms[BodyStyle].name, outfits.backArms[BodyStyle].bounds);

        ApplyMeshToObject(frontArmRendMesh, "Suit/" + suitName + "/" + outfits.frontArms[BodyStyle].name, outfits.frontArms[BodyStyle].bounds);

        ApplyMeshToObject(hairRendMesh, "Hair/" + outfits.hairs[hairStyle].name, outfits.hairs[hairStyle].bounds);


        ApplyMeshToObject(facialHairRendMesh, "Face Decoration/" + outfits.facialHairs[facialHairStyle].name, outfits.facialHairs[facialHairStyle].bounds);

        hairRendMesh.GetComponent<MeshRenderer>().enabled = hairColor != 0;
        hairRendMesh.GetComponent<MeshRenderer>().material = outfits.hairColors[hairColor];
        backHairRendMesh.GetComponent<MeshRenderer>().enabled = backHair;
        backHairRendMesh.GetComponent<MeshRenderer>().material = outfits.hairColors[hairColor];
        facialHairRendMesh.GetComponent<MeshRenderer>().enabled = hairColor != 0;
        facialHairRendMesh.GetComponent<MeshRenderer>().material = outfits.hairColors[hairColor];

        ApplyMeshToObject(hatRendMesh, "Hat/" + outfits.hats[hatStyle].name, outfits.hats[hatStyle].bounds);

        ApplyMeshToObject(faceRendMesh, "Face/" + outfits.faces[0].name, outfits.faces[0].bounds);

    }

    public void SetHoldsArms(bool holding)
    {
        Sprite backArms = holding ? outfits.backArmsHolding[BodyStyle] : outfits.backArms[BodyStyle];

        ApplyMeshToObject(backArmRendMesh, "Suit/" + suitName + "/" + backArms.name, backArms.bounds);

        Sprite frontArms = holding ? outfits.frontArmsHolding[BodyStyle] : outfits.frontArms[BodyStyle];

        ApplyMeshToObject(frontArmRendMesh, "Suit/" + suitName + "/" + frontArms.name, frontArms.bounds);

    }

    public void SetHat(int hat)
    {
        hatStyle = hat;
        ApplyMeshToObject(hatRendMesh, "Hat/" + outfits.hats[hatStyle].name, outfits.hats[hatStyle].bounds);
    }

    public void SetFlip(bool flip)
    {
        Vector3 desiredScale = transform.localScale;

        desiredScale.x = Mathf.Abs(desiredScale.x) * (flip ? -1 : 1);

        transform.localScale = desiredScale;
    
    }

    private void ApplyMeshToObject(GameObject obj, string meshPath, Bounds bounds)
    {
        obj.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("TokenModels/"+meshPath);
        obj.transform.localPosition = bounds.center;
    }

    private void OnEnable()
    {
        bodyRendMesh.SetActive(true);
        backArmRendMesh.SetActive(true);
        frontArmRendMesh.SetActive(true);
        hairRendMesh.SetActive(true);
        backHairRendMesh.SetActive(backHair);
        facialHairRendMesh.SetActive(true);
        hatRendMesh.SetActive(true);
        faceRendMesh.SetActive(true);
        headRendMesh.SetActive(true);
    }

    private void OnDisable()
    {
        bodyRendMesh.SetActive(false);
        backArmRendMesh.SetActive(false);
        frontArmRendMesh.SetActive(false);
        hairRendMesh.SetActive(false);
        backHairRendMesh.SetActive(false);
        facialHairRendMesh.SetActive(false);
        hatRendMesh.SetActive(false);
        faceRendMesh.SetActive(false);
        headRendMesh.SetActive(false);
    }

    public IEnumerator SpeechAnimation(float duration)
    {
        WaitForSeconds delay = new WaitForSeconds(1 / speechAnimationSpeed);
        for (float i = 0; i < duration; i+= 1/speechAnimationSpeed)
        {
            Sprite face = outfits.faces[Random.Range(0, 3)];
            ApplyMeshToObject(faceRendMesh, "Face/" + face.name, face.bounds);
            yield return delay;
        }
        ApplyMeshToObject(faceRendMesh, "Face/" + outfits.faces[0].name, outfits.faces[0].bounds);

        yield break;
    }

#if UNITY_EDITOR
    private void Randomize()
    {
        Undo.RecordObject(this, "Randomized");
        randomize = false;


        BodyStyle = Random.Range(0, outfits.bodies.Count);

        hairStyle = Random.Range(0, outfits.hairs.Count);

        backHair = Random.Range(0, 2) == 2;

        facialHairStyle = Random.Range(0, outfits.facialHairs.Count);

        hairColor = Random.Range(0, outfits.hairColors.Count);

        hatStyle = Random.Range(0, outfits.hats.Count);

        UpdateVisuals();

        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }


    private void UpdateSuitPath()
    {
        Undo.RecordObject(this, "Generated suit name");
        int pathLength = "Assets/Art/Textures/Tokens/2D Middle Age/Sprite/Human/Suit/".Length;

        suitName = AssetDatabase.GetAssetPath(outfits.backArms[BodyStyle]).Substring(pathLength);
        suitName = suitName.Substring(0, suitName.Length - (outfits.backArms[BodyStyle].name.Length + "/.png".Length));
    }
#endif



}

