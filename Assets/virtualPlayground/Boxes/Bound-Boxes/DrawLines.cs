using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DimBoxes
{
    [ExecuteInEditMode]
    public class DrawLines : MonoBehaviour
    {
	    public Material lineMaterial;
	    public Color lColor = Color.green;
	    List<Vector3[,]> outlines;
        List<Vector3[][]> triangles;
	    public List<Color> colors;
        List<Vector3[,]> screenOutlines;
        public List<Color> screenColors;

        void OnEnable ()
        {
		    outlines = new List<Vector3[,]>();
		    colors = new List<Color>();
            triangles = new List<Vector3[][]>();
	    }
	
	    void Start () {
    //		outlines = new List<Vector3[,]>();
    //		colors = new List<Color>();
    //		CreateLineMaterial();
	    }

	    void OnRenderObject()
        {
		    if(outlines==null) return;
	        lineMaterial.SetPass( 0 );

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(Matrix4x4.identity);

            GL.Begin( GL.LINES );
		    for (int j=0; j<outlines.Count; j++) {
			    GL.Color(colors[j]);
			    for (int i=0; i<outlines[j].GetLength(0); i++) {
				    GL.Vertex(outlines[j][i,0]);
				    GL.Vertex(outlines[j][i,1]);
			    }
		    }
		    GL.End();

            GL.Begin(GL.TRIANGLES);
            //Debug.Log(triangles.Count.ToString());
            for (int j = 0; j <triangles.Count; j++)
            {
                GL.Color(colors[j]);
                for (int i = 0; i < triangles[j].GetLength(0); i++)
                {
                    //Debug.Log(j.ToString()+ " | " + i.ToString());
                    GL.Vertex(triangles[j][i][0]);
                    GL.Vertex(triangles[j][i][1]);
                    GL.Vertex(triangles[j][i][2]);
                }
            }

            GL.End();

            if (screenOutlines != null)
            {
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                /*GL.Color(Color.red);
                GL.Vertex(new Vector3(0.5f, 0.5f, 0));
                GL.Vertex(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0));*/
                for (int j = 0; j < screenOutlines.Count; j++)
                {
                    GL.Color(screenColors[j]);
                    for (int i = 0; i < screenOutlines[j].GetLength(0); i++)
                    {
                        GL.Vertex(screenOutlines[j][i, 0]);
                        GL.Vertex(screenOutlines[j][i, 1]);
                    }
                }
                GL.End();
            }
            GL.PopMatrix();
        }
		
	    public void setOutlines(Vector3[,] newOutlines, Color newcolor) {
            if (newOutlines == null) return;
            if (outlines == null) return;
		    if(newOutlines.GetLength(0)>0)	{
			    outlines.Add(newOutlines);
			    colors.Add(newcolor);
		    }
	    }


            public void setScreenOutlines(Vector3[,] newOutlines, Color newcolor)
            {
                if (newOutlines == null) return;
                if (outlines == null) return;
                if (newOutlines.GetLength(0) > 0)
                {
                    screenOutlines.Add(newOutlines);
                    screenColors.Add(newcolor);
                }
            }

            public void setOutlines(Vector3[,] newOutlines, Color newcolor, Vector3[][] newTriangles)
        {
            if (newOutlines == null) return;
            if (outlines == null) return;
            if (newOutlines.GetLength(0) > 0)
            {
                outlines.Add(newOutlines);
                colors.Add(newcolor);
                triangles.Add(newTriangles);
            }
        }	
	
	    void Update () {
		    outlines = new List<Vector3[,]>();
		    colors = new List<Color>();
            triangles = new List<Vector3[][]>();
            screenOutlines = new List<Vector3[,]>();
            screenColors = new List<Color>();
        }
    }
}
