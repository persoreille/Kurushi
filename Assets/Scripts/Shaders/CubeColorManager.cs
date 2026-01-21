using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CubeMaterialController))]
public class CubeColorManager : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 0.5f;

    private CubeMaterialController materialCtrl;
    private CubeShaderType current;
    private Coroutine transitionRoutine;

    void Awake()
    {
        materialCtrl = GetComponent<CubeMaterialController>();
        Debug.Assert(materialCtrl != null, "CubeMaterialController missing !");
    }

    public void Init(CubeShaderType initial)
    {
        current = initial;

        materialCtrl.SetEdges(
            enabled: true,
            thickness: 3f,
            softness: 5f,
            intensity: 50f
        );

        materialCtrl.ForceSetShader((int)initial);
    }

    public void ChangeTo(CubeShaderType target)
    {
        if(transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
        }

        CubeShaderType from = current;
        current = target;

        transitionRoutine = StartCoroutine(TransitionRoutine(from, target));
    }

    private IEnumerator TransitionRoutine(CubeShaderType from, CubeShaderType target)
    {
        materialCtrl.StartShaderTransition((int)from, (int)target, transitionDuration);

        yield return new WaitForSeconds(transitionDuration);

        transitionRoutine = null;
    }
}
