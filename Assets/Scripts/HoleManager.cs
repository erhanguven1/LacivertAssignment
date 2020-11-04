using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public MeshFilter holeMeshFilter;
    public float r;

    public List<Vector3> verts, chosenVerts;

    // Start is called before the first frame update
    void Start()
    {
        ChooseVerts();
    }

    public void ChooseVerts()
    {
        //Hole vertexlerini seçmek adına tüm vertexlerin arasından bize uzaklığı 
        //r kadar olanları seçip yeni listeye aktar

        verts.Clear();
        chosenVerts.Clear();

        Mesh holeMesh = holeMeshFilter.mesh;

        verts = holeMesh.vertices.ToList();


        chosenVerts = verts.Where(x => Vector3.Distance(x, transform.position) < r).ToList();
    }

    public void RepositionHole()
    {
        //Yeniden konumlandırmak için seçilen vertexleri random seçilen bir noktaya kadar ilerlet

        Vector3 newPoint = new Vector3(Random.Range(-3,3), Random.Range(-3, 3), 0);

        for (int i = 0; i < chosenVerts.Count; i++)
        {
            verts[verts.IndexOf(chosenVerts[i])] += (newPoint - new Vector3(transform.position.x,transform.position.z,0));
            chosenVerts[i] += (newPoint - new Vector3(transform.position.x, transform.position.z, 0));
        }

        holeMeshFilter.mesh.vertices = verts.ToArray();
        holeMeshFilter.mesh.RecalculateBounds();

        holeMeshFilter.GetComponent<MeshCollider>().sharedMesh = holeMeshFilter.mesh;

        transform.position = new Vector3(newPoint.x, 0, newPoint.y);

    }
}
