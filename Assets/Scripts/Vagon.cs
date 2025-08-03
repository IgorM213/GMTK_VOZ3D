using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Vagon : MonoBehaviour
{

    [SerializeField] public SplineContainer splineContainer;
    [SerializeField] public float OffsetDistance = 2f;
    [SerializeField] public Transform pos1;
    [SerializeField] public Transform pos2;


    private float currentSplinePosition = 0f;
    private float velocity = 0f;
    private Rigidbody rb;

    [SerializeField] public GameObject glavaVozaObj;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        Sine glavaVoza = glavaVozaObj.GetComponent<Sine>();

        float baseT = glavaVoza.vratiTrenutniT(); // mora� dodati ovu metodu u Sine.cs

        float offsetT = GetOffsetSplinePosition(baseT, OffsetDistance);
        velocity = glavaVoza.vratiVelocity();
        
        // Pomeri voz po spline-u
        currentSplinePosition += glavaVoza.vratiVelocity() * Time.deltaTime / splineContainer.Splines[0].GetLength();
        currentSplinePosition = (currentSplinePosition + 1f) % 1f;



        // Get position and tangent (direction) from the spline
        //Vector3 position = splineContainer.EvaluatePosition(currentSplinePosition);
        ////Debug.Log(position);

        //Vector3 tangent = splineContainer.EvaluateTangent(currentSplinePosition);
        Vector3 position = splineContainer.EvaluatePosition(offsetT);
        Vector3 tangent = splineContainer.EvaluateTangent(offsetT);

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent);

        // Update player's position and rotation
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent);
    }
    private float GetOffsetSplinePosition(float baseT, float offsetDistance)
    {
        var native = new NativeSpline(splineContainer.Splines[0]);

        // Izra?unaj ukupnu du�inu spline-a
        float totalLength = SplineUtility.CalculateLength(native, float4x4.identity);


        // Izra?unaj trenutnu udaljenost du� spline-a
        float baseDistance = baseT * totalLength;

        // Nova meta udaljenost (unazad)
        float offsetTarget = baseDistance - offsetDistance;

        // Ako je negativno, loopuj
        if (offsetTarget < 0f)
            offsetTarget += totalLength;

        // Pretvori nazad u T vrednost (0�1)
        float offsetT = offsetTarget / totalLength;

        return offsetT;
    }
    


}
