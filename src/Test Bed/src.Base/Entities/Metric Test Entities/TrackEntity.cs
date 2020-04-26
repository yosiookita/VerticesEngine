

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class TrackEntity : vxEntity3D
    {
        public TrackEntity(vxGameplayScene3D scene, Vector3 StartPosition)
            : base(scene, StartPosition)
        {
            
        }

        // Now setup the Section sizes and pitch
        float sectionPitch = 10;
        float numOfSections = 5;

        public override vxModel OnLoadModel()
        {
            vxModel tempModel = Engine.ContentManager.LoadModel("Models/track/track");

            vxModelMesh newmesh = new vxModelMesh(Engine);

            // First get all of the Vertex Data and Indices for one stock section.
            List<vxMeshVertex> verts_orig = new List<vxMeshVertex>();
            List<ushort> indices_orig = new List<ushort>();

            int primCount = 0;

            foreach(var mesh in tempModel.Meshes)
            {
                foreach(var part in mesh.MeshParts)
                {
                    foreach (var v in part.MeshVertices)
                        verts_orig.Add(v);

                    foreach (var ind in part.Indices)
                        indices_orig.Add(ind);

                    primCount += part.PrimitiveCount;
                }
            }


            // The new Vertices and Indices Collections

            List<vxMeshVertex> verts = new List<vxMeshVertex>();
            List<ushort> indices = new List<ushort>();

            int indicesOffset = 0;
            for (int z = 0; z < numOfSections; z++)
            {
                // Loop through all of the original vertices and indices and add them
                // to the new collection
                for (int vi = 0; vi < verts_orig.Count(); vi++)
                {
                    var v = verts_orig[vi];

                    bool IsCurve = false;

                    if (IsCurve)
                    {
                        /* Method to generate extruded curve
                         1) Get 'point' about origin
                         2) Rotate 'point' z axis about origin
                         3) Calculate point along the circumference
                         4) Translate to that point along the circumference
                         */

                        float TotalSweep = MathHelper.ToRadians(90);

                        // Section Sweep
                        float SectionSweep = TotalSweep / numOfSections;

                        // Extra Sweep for this vertices 
                        float VertexSweep = (v.Position.Z / (sectionPitch * numOfSections)) * TotalSweep;

                        // Current Sweep
                        float CurrentSweep = SectionSweep * z + VertexSweep;

                        float ExtrudedPercentage = CurrentSweep / TotalSweep;

                        // 1) Get point about origin
                        Vector3 origPoint = new Vector3(v.Position.X, v.Position.Y, 0);


                        // 2) Perform Required Rotations
                        float angle = MathHelper.PiOver4 * (ExtrudedPercentage); // 45 degrees = Pi/4

                        Vector3 newPoint = Vector3.Transform(origPoint, Matrix.CreateRotationZ(angle));



                        // 3) Calculate point along circumference
                        float R = 50;

                        // Rotate it by the percentage it's complteted the total sweep.
                        newPoint = Vector3.Transform(newPoint, Matrix.CreateRotationY(-CurrentSweep));

                        float xT = R * (float)Math.Cos(CurrentSweep);

                        float zT = R * (float)Math.Sin(CurrentSweep);

                        newPoint += new Vector3(xT - R, 0, zT);

                        v.Position = newPoint;
                    }
                    else
                    {
                        // Extrude it along the z axis.
                        // x & y are the planar values.

                        // Rotation Offset
                        Vector3 newPoint = new Vector3(v.Position.X, v.Position.Y, 0);

                        // Extrude along the 'x' axis
                        float extZ = v.Position.Z + sectionPitch * z;

                        // Get Extruded Persentage
                        float ExtrudedPercentage = extZ / (sectionPitch * numOfSections);

                        // 45 degrees = Pi/4
                        float angle = MathHelper.PiOver4 * ExtrudedPercentage;

                        Vector3 vrot = Vector3.Transform(newPoint, Matrix.CreateRotationZ(angle));

                        // Set Position
                        v.Position = new Vector3(v.Position.X, v.Position.Y, extZ);


                        v.Position += (vrot - newPoint);
                    }

                    verts.Add(v);
                }

                // handle indices
                for (int indi = 0; indi < indices_orig.Count(); indi++)
                {
                    var i = indices_orig[indi];
                    // Extrude along the 'x' axis
                    ushort inf = (ushort)(i + indicesOffset);
                    //Console.WriteLine(string.Format("indices: {0}, i: {1}", i, inf));
                    indices.Add(inf);
                }
                indicesOffset += verts_orig.Count();

                var node = new TrackNode(Scene, this, StartPosition + new Vector3(0, 0, sectionPitch * z), sectionPitch);

            }
            vxModelMeshPart newpart = new vxModelMeshPart(Engine,
                                                      verts.ToArray(),
                                                      indices.ToArray(),
                                                       primCount * (int)numOfSections);

            tempModel.Meshes[0].MeshParts[0] = newpart;

            return tempModel;
        }


        //public override BoundingSphere GetBoundingShape()
        //{
        //    BoundingSphere boundingShape = vxGeometryHelper.GetModelBoundingSphere(Model.ModelMain);
        //    boundingShape.Radius = pitch * sections/2;
        //    boundingShape.Center = new Vector3(0, pitch * sections / 2, 0);
        //    return boundingShape;
        //}
    }
}
