using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terrain : MonoBehaviour
{
    public Texture2D image;
    private GameObject pObject;
    private Vector3[] vertex;

    void Start()
    {
        pObject = new GameObject("terrain");
        vertex = new Vector3[image.width*image.height];
        Mesh m = new Mesh();
        pObject.AddComponent<MeshFilter>();
        pObject.AddComponent<MeshRenderer>();
        pObject.GetComponent<MeshFilter>().mesh = m;
        pObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        List<int> triangles = new List<int>();
        for(int i= 0; i<image.width; i++)
        {
            for(int j =0; j<image.height;j++)
            {
                // color  https://www.youtube.com/watch?v=lNyZ9K71Vhc
                Color c = image.GetPixel(i,j);
                vertex[i*image.width+j] = Vector3.Lerp(new Vector3(i,0,j),new Vector3(i,50,j),c.grayscale);
                if(i+1<image.width && j+1<image.height)
                {
                    triangles.Add((i)*image.width+j);
                    triangles.Add((i)*image.width+j+1);
                    triangles.Add((i+1)*image.width+j);
                    triangles.Add((i)*image.width+j+1);
                    triangles.Add(((i+1)*image.width)+(j+1));
                    triangles.Add((i+1)*image.width+j);
                }
                /*Color c = image.GetPixel(i,j);
                t[i,j] = c.grayscale;
                g[i,j] = GameObject.Instantiate(prefab);
                g[i,j].transform.parent = terrain.transform;
                r =  g[i,j].GetComponent<Renderer>();
                m = g[i,j].GetComponent<MeshFilter>();
                Vector3 min =  new Vector3(i,0,j);
                Vector3 max =  new Vector3(i,50,j);
                g[i,j].transform.position = Vector3.Lerp(min,max,t[i,j]);
                if(g[i,j].transform.position.y<1.0f)c= Color.blue;
                else if (g[i,j].transform.position.y<5.0f)c= Color.gray;
                else if (g[i,j].transform.position.y<40.0f)c = Color.green;
                else c = Color.white;
                r.material.color = c;
                Debug.Log(m.mesh.vertices.Length);*/
                
            }
        }

        int[] tList = new int[triangles.Count];
        for(int i =0; i <tList.Length; i++)
        {
            tList[i]=triangles[i];
        }

        m.Clear();
        m.vertices= vertex;
        m.triangles = tList;
        m.RecalculateNormals();
    }
}
