using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum AnimationState
{
    PAUSED, FORWARD, BACKWARD
}

public class SierpinskiTetrahedron : MonoBehaviour
{
    /// <summary>
    /// Color of the bottom triangle of the tetrahedron
    /// </summary>
    public Color colorBottom    = new Color(242f / 255f, 255f / 255f, 90f / 255f);

    /// <summary>
    /// Color of the front triangle of the tetrahedron
    /// </summary>
    public Color colorFront     = new Color(103f / 255f, 255f / 255f, 93f / 255f);

    /// <summary>
    /// Color of the left triangle of the tetrahedron
    /// </summary>
    public Color colorLeft      = new Color(110f / 255f, 189f / 255f, 253f / 255f);

    /// <summary>
    /// Color of the right triangle of the tetrahedron
    /// </summary>   
    public Color colorRight     = new Color(246f / 255f, 73f / 255f, 138f / 255f);

    /// <summary>
    /// The base tetrahedron for the folding animation
    /// </summary>
    private Tetrahedron tetrahedron;

    /// <summary>
    /// A mesh containing the information of the tetrahedron for rendering
    /// </summary>
    private Mesh mesh;

    /// <summary>
    /// A mesh containing the information of a triangle for rendering
    /// </summary>
    private Mesh triangle;

    /// <summary>
    /// Determines if lighting is enabled
    /// </summary>
    private bool lightingEnabled = true;

    /// <summary>
    /// Determines if instanced drawing is enabled
    /// </summary>
    private bool drawInstanced = true;

    /// <summary>
    /// The Material used for rendering the folding animation of a tetrahedron
    /// </summary>
    [SerializeField]
    private Material tetrahedronFoldingMaterial;
    
    /// <summary>
    /// The material used for rendering the folding animation of a triangle
    /// </summary>
    [SerializeField]
    private Material triangleFoldingMaterial;

    
    [SerializeField]
    private float animationSpeed = 1.0f;
    
    /// <summary>
    /// The maximum number of iterations
    /// </summary>
    [SerializeField]
    private int maxIterations = 5;
    
    /// <summary>
    /// The current number of iterations
    /// </summary>
    private int numIterations = 0;
    
    /// <summary>
    /// The progress of the current animation in range of[0,1]
    /// </summary>
    private float animationProgress = 0.0f;
    
    /// <summary>
    /// The state of the animation
    /// </summary>
    private AnimationState state    = AnimationState.PAUSED;

    /// <summary>
    /// Collect all positions for the instances
    /// </summary>
    private List<Vector3> instances = new List<Vector3>();
    /// <summary>
    /// The scale of all instances of the tetrahedron
    /// </summary>
    private float scale = 1;

    /// <summary>
    /// A collection for localToWorld-Matrices for all instances of the tetrahedron.
    /// Used for rendering. 
    /// </summary>
    private List<Matrix4x4> instanceMatrices = new List<Matrix4x4>();



    /// <summary>
    /// Determines if instanced drawing should be used
    /// </summary>
    public bool DrawInstanced
    {
        get{return drawInstanced;}
        set{drawInstanced = value;}
    }

    /// <summary>
    /// Determines if lighting is enabled
    /// </summary>
    public bool LightingEnabled
    {
        get { return lightingEnabled; }
        set { lightingEnabled = value; }
    }

    /// <summary>
    /// Controls the speed of one folding iteration in range of [0,1]
    /// </summary>
    public float AnimationSpeed
    {
        get{ return animationSpeed;}
        set{ animationSpeed = value;}
    }


    /// <summary>
    /// Returns the current number of folding iterations
    /// </summary>
    public int NumIterations
    {
        get{return numIterations;}       
    }

    /// <summary>
    /// Callback function to listen for change in the Animation Toggle group
    /// </summary>
    public Toggle OnToggleChangedCallback
    {
        set
        {
            if (value.gameObject.name == "PauseButton")
            {
                this.state = AnimationState.PAUSED;
            }
            else if (value.gameObject.name == "PlayForwardButton")
            {
                this.state = AnimationState.FORWARD;
            }
            else if (value.gameObject.name == "PlayReversedButton")
            {
                this.state = AnimationState.BACKWARD;
            }
        }
    }


    /// <summary>
    /// Returns the current state of the animation
    /// </summary>
    public AnimationState State
    {
        get{return state;}

        set{state = value;}
    }


    /// <summary>
    /// Used for Initialization
    /// </summary>
    void Start()
    {
        // Create tetrahedron
        tetrahedron = Tetrahedron.Unit;
        tetrahedron.colorBottom = colorBottom;
        tetrahedron.colorFront  = colorFront;
        tetrahedron.colorLeft   = colorLeft;
        tetrahedron.colorRight  = colorRight;
        instances.Add(Vector3.zero);

        // Create mesh from tetrahedron
        mesh = new Mesh();
        mesh.vertices   = tetrahedron.Vertices;
        mesh.triangles  = tetrahedron.Indices;
        mesh.colors     = tetrahedron.Colors;

        // Create mesh for single triangle
        float h = Mathf.Sqrt(3f) * (1f / 2f);            
        var p0 = new Vector3( 0.5f, 0.0f,-h );
        var p1 = new Vector3(-0.5f, 0.0f, h);
        var p2 = new Vector3(1.5f, 0.0f, h);
        
        triangle = new Mesh();
        triangle.vertices   = new Vector3[] {p0, p1, p2 };
        triangle.triangles  = new int[] { 2, 1, 0 };

        // Let camera look at center
        Camera.main.transform.LookAt(Vector3.zero, Vector3.up);

        // Initialize the number of iterations
        instances.Clear();
        instances.Add(Vector3.zero);
        scale = 1.0f;
        instanceMatrices.Clear();
        instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.Center, Quaternion.identity, Vector3.one * scale));
        numIterations = 0;
    }


    /// <summary>
    /// Applies an iteration to the animation based on the current animation State
    /// </summary>
    public void ApplyIteration()
    {
        if(state == AnimationState.FORWARD)
        {
            ApplyForwardIteration();
        }
        if(state == AnimationState.BACKWARD)
        {
            ApplyReversedIteration();
        }
    }


    /// <summary>
    /// Applies an forward iteration.
    /// It creates 4 new instance for each instance and adapt the global scale accordingly.
    /// </summary>
    private void ApplyForwardIteration()
    {
        // Only apply new iteration if maxIterations has not exceeded
        // maxIterations is currently set to 9, due to memory limits
        if (numIterations < maxIterations)
        {
            // Don't iterate form 0 to 1, because animation 0 is triangle folding, animation 1 is tetrahedron folding
            if (numIterations == 0)
            {
                numIterations++;
                return;
            }

            scale *= 0.5f;
            Vector3 v01 = (tetrahedron.p1 - tetrahedron.p0) * scale;
            Vector3 v02 = (tetrahedron.p2 - tetrahedron.p0) * scale;
            Vector3 v03 = (tetrahedron.p3 - tetrahedron.p0) * scale;

            var s = tetrahedron.Center;

            var newInstances = new List<Vector3>();
            instances.ForEach(p =>
            {
                newInstances.Add(p);
                newInstances.Add(p + v01);
                newInstances.Add(p + v02);
                newInstances.Add(p + v03);
            });

            instances = newInstances;
            instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.Center, Quaternion.identity, Vector3.one * scale));
            numIterations++;
        }
    }

    /// <summary>
    /// Applies a reveresed iteration, by only taking every 4th instance and adapting the scale accordingly. 
    /// </summary>
    public void ApplyReversedIteration()
    {
        if (numIterations > 0)   
        {
            if(numIterations == 1)
            {
                numIterations--;
                return;
            }

            scale *= 2.0f;

            var newInstances = new List<Vector3>();
            for (int i = 0; i < instances.Count; i++)
            {
                if (i % 4 == 0) newInstances.Add(instances[i]);
            }

            instances = newInstances;
            instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.Center, Quaternion.identity, Vector3.one * scale));
            numIterations--;
        }
    }



    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
        Draw();
    }



    /// <summary>
    /// Updates the animatiaon
    /// </summary>
    private void UpdateAnimation()
    {

        switch(state)
        {
            case AnimationState.PAUSED:
                return;
            case AnimationState.FORWARD:
                animationProgress += Time.deltaTime * animationSpeed;
                
                if (animationProgress >= 1.0f)
                {
                    animationProgress = 0.0f;
                    if (numIterations < maxIterations)
                    {
                        ApplyForwardIteration();
                    }
                    else
                    {
                        animationProgress = 1.0f;
                    }
                }
                
                break;
            case AnimationState.BACKWARD:
                animationProgress -= Time.deltaTime * animationSpeed;
                if (animationProgress <= 0.0f)
                {
                    animationProgress = 1.0f;
                    if (numIterations > 0)
                    {
                        ApplyReversedIteration();
                    }
                    else
                    {
                        animationProgress = 0.0f;
                    }               
                }
                break;
        }               
    }


    /// <summary>
    /// Draws the current animation
    /// </summary>
    private void Draw()
    {

        // Draw Triangle Folding

        if (numIterations == 0)
        {
            // Material Uniforms
            var angle = animationProgress * (-109.5f) * Mathf.Deg2Rad;
            triangleFoldingMaterial.SetFloat("cosPhi", Mathf.Cos(angle));
            triangleFoldingMaterial.SetFloat("sinPhi", Mathf.Sin(angle));
            triangleFoldingMaterial.SetFloat("animationProgress", animationProgress);
            triangleFoldingMaterial.SetInt("lightingEnabled", lightingEnabled ? 1: 0);

            triangleFoldingMaterial.SetColor("colorBottom"  , tetrahedron.colorBottom);
            triangleFoldingMaterial.SetColor("colorFront"   , tetrahedron.colorFront);
            triangleFoldingMaterial.SetColor("colorLeft"    , tetrahedron.colorLeft);
            triangleFoldingMaterial.SetColor("colorRight"   , tetrahedron.colorRight);


            var matrix = Matrix4x4.TRS(Vector3.zero - tetrahedron.Center, Quaternion.identity, Vector3.one);
            Graphics.DrawMesh(triangle, this.transform.localToWorldMatrix * matrix, triangleFoldingMaterial, 0);
        }
        else
        {
            // Material Uniforms
            var angle = animationProgress * (-109.5f) * Mathf.Deg2Rad;
            tetrahedronFoldingMaterial.SetFloat("cosPhi", Mathf.Cos(angle));
            tetrahedronFoldingMaterial.SetFloat("sinPhi", Mathf.Sin(angle));
            tetrahedronFoldingMaterial.SetFloat("animationProgress", animationProgress);
            tetrahedronFoldingMaterial.SetInt("lightingEnabled", lightingEnabled ? 1 : 0);


            tetrahedronFoldingMaterial.SetColor("colorBottom", tetrahedron.colorBottom);
            tetrahedronFoldingMaterial.SetColor("colorFront", tetrahedron.colorFront);
            tetrahedronFoldingMaterial.SetColor("colorLeft", tetrahedron.colorLeft);
            tetrahedronFoldingMaterial.SetColor("colorRight", tetrahedron.colorRight);

            // Draw Tetrahedrons
            var matrices = instanceMatrices.ConvertAll(m => this.transform.localToWorldMatrix * m);
            if (drawInstanced)
            {
                // Split list into chunks, since Instanced drawing has a limited number of instances
                var chunks = matrices.Split(1023);
                chunks.ToList().ForEach(m => Graphics.DrawMeshInstanced(mesh, 0, tetrahedronFoldingMaterial, m.ToArray(), m.Count()));
            }
            else
            {
                matrices.ForEach(m => Graphics.DrawMesh(mesh, m, tetrahedronFoldingMaterial, 0));
            }
        }
    }
}