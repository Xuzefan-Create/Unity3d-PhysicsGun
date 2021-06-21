using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class RopeGun : MonoBehaviour
{
    
    public List<Vector3> RayPos;
    public List<Transform> ChooseObj;

    public Transform solverObject;
    public Material material;
    private Vector3 pos0;
    private Vector3 pos1;

    private Vector3 C_pos0;
    private Vector3 C_pos1;
    
    void Start()
    {
        ChooseObj = new List<Transform>();
        RayPos = new List<Vector3>();
    }
    
    private ObiRopeBlueprint blueprint;
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == 6)
                {
                    var ChoosedObj = hit.collider.gameObject;
                    ChoosedObj.transform.SetParent(solverObject);
                    
                    if (ChooseObj.Count<2)
                    {
                        RayPos.Add(hit.point);
                        ChooseObj.Add(hit.collider.transform);
                    }
                }
            }
            if (ChooseObj.Count==2)
            {
                StartCoroutine(GenerateBlue());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ChooseObj.Clear();
            RayPos.Clear();
        }
       
    }
    

    IEnumerator GenerateBlue()
    {
        // create the blueprint: (ltObiRopeBlueprint, ObiRodBlueprint)
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        pos0 = RayPos[0];
        pos1 = RayPos[1];
       
        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        
        blueprint.path.AddControlPoint(pos0, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "start");
        blueprint.path.AddControlPoint(pos1, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "end");
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
        startAt.target = ChooseObj[0];
        startAt.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
        startAt.particleGroup = blueprint.groups[0];
        endAt.target = ChooseObj[1];
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
        rope.transform.SetParent(solverObject);// = solverObject.transform;
        ChooseObj.Clear();
        RayPos.Clear();
    }
}
