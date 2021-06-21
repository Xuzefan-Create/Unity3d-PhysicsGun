using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using Unity.Collections;

public class TestObi : MonoBehaviour
{
    public GameObject solverObject;
    public Transform startPos;
    public Transform endPos;
    public Material material;
    
    // public ObiParticleGroup startGroup;
    // public ObiParticleGroup endGroup;

    private ObiRopeBlueprint blueprint;
    
    private void Start()
    {
        // // create an object containing both the solver and the updater:
        // solverObject = new GameObject("solver", typeof(ObiSolver), typeof(ObiFixedUpdater));
        // ObiSolver solver = solverObject.GetComponent<ObiSolver>();
        // ObiFixedUpdater updater = solverObject.GetComponent<ObiFixedUpdater>();
        //
        // // add the solver to the updater:
        // updater.solvers.Add(solver);
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(GenerateBlue());
        }
    }
    

    IEnumerator GenerateBlue()
    {
        // create the blueprint: (ltObiRopeBlueprint, ObiRodBlueprint)
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();

        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(startPos.position, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "start");
        blueprint.path.AddControlPoint(endPos.position, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "end");
        blueprint.path.FlushEvents();

        // generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();
        
        CreateActor();
    }

    void CreateActor()
    {
        // create a rope:
        GameObject ropeObject = new GameObject("rope", typeof(ObiRope), typeof(ObiRopeExtrudedRenderer));

        ropeObject.GetComponent<MeshRenderer>().material = material;

        var startAt = ropeObject.AddComponent<ObiParticleAttachment>();
        var endAt = ropeObject.AddComponent<ObiParticleAttachment>();
        startAt.target = startPos;
        startAt.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        startAt.particleGroup = blueprint.groups[0];
        endAt.target = endPos;
        endAt.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        endAt.particleGroup = blueprint.groups[1];
        //endAt.particleGroup = endGroup;
        //ropeObject.AddComponent<ObiRopeLineRenderer>();

        // get component references:
        ObiRope rope = ropeObject.GetComponent<ObiRope>();
        ObiRopeExtrudedRenderer ropeRenderer = ropeObject.GetComponent<ObiRopeExtrudedRenderer>();

        // load the default rope section:
        ropeRenderer.section = Resources.Load<ObiRopeSection>("DefaultRopeSection");

       // instantiate and set the blueprint:
        rope.ropeBlueprint = ScriptableObject.Instantiate(blueprint);

        // parent the cloth under a solver to start simulation:
        rope.transform.SetParent(solverObject.transform);// = solverObject.transform;
    }
}