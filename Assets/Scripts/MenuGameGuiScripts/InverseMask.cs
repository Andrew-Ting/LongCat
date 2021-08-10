using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class InverseMask : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material mat = new Material(base.materialForRendering);
            mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            Debug.Log("some stuff");
            return mat;
        }
    }
}
