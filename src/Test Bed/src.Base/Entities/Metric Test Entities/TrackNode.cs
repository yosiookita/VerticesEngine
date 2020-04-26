

using System;
using System.Linq;
using BEPUutilities;
using Microsoft.Xna.Framework;

using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using VerticesEngine.Utilities;
using VerticesEngine.Scenes;
using VerticesEngine.Diagnostics;

namespace Virtex.App.VerticesTechDemo
{
    public enum FalloffType
    {
        Linear,
        Parabolic,
        InverseParabolic,
        InvTangent
    }
    public class TrackNode : vxEntity3D
    {
        public FalloffType FalloffType;

        TrackEntity TrackEntity;
        float Radius = 10;
        public TrackNode(vxGameplayScene3D scene, TrackEntity TrackEntity, Vector3 StartPosition, float Radius,
                         FalloffType FalloffType = FalloffType.InvTangent)
            : base(scene, StartPosition)
        {
            this.TrackEntity = TrackEntity;
            DoShadowMapping = false;
            prevPos = StartPosition;

            this.Radius = Radius;
            this.FalloffType = FalloffType;


        }

        public override vxModel OnLoadModel()
        {
            return Engine.ContentManager.LoadModel("Models/track/node");
        }
        public override void RenderMesh(VerticesEngine.Cameras.vxCamera3D Camera)
        {
            base.RenderMesh(Camera);


            //Console.WriteLine(Yaw);
            //Console.WriteLine(Pitch);
            //Console.WriteLine(Roll);
        }

        Vector3 delta;
        Vector3 prevPos;

        public override void OnGimbalRotate(Vector3 axis, float delta)
        {
            base.OnGimbalRotate(axis, delta);

                   vxDebug.DrawLine(Position,
                                         Position + axis * 10,
                                         Color.DeepPink);

            //Vector3 d = Vector3.Transform(delta, Matrix.CreateRotationX(MathHelper.PiOver2));
            Vector3 dVec = new Vector3(0);

            //this.TrackEntity.Model.ModelMeshes[0].MeshParts[0].VerticesPoints
            foreach (var part in TrackEntity.Model.Meshes[0].MeshParts)
            {
                for (int i = 0; i < part.MeshVertices.Count(); i++)
                {
                    Vector3 v = Vector3.Transform(part.MeshVertices[i].Position, TrackEntity.WorldTransform);
                    float dist = MathHelper.Clamp(Math.Abs(v.Z - Position.Z) / Radius, 0, 1); ;
                    float factor = 1;
                    switch (FalloffType)
                    {
                        case FalloffType.Linear:
                            factor = 1 - dist;
                            break;
                        case FalloffType.Parabolic:
                            factor = (1 - dist) * (1 - dist);
                            break;
                        case FalloffType.InverseParabolic:
                            factor = 1 - dist * dist;
                            break;
                        case FalloffType.InvTangent:
                            // =(cos(x/10)+1)/2
                            factor = (float)(Math.Cos(dist * MathHelper.Pi) + 1) / 2;
                            break;
                    }

                    // Get the Flat Cross Section Point
                    Vector3 planePoint = new Vector3(v.X, v.Y, 0) - Position;

                    // Now Rotate The Point about the Axis
                    //Vector3 RotatedPoint = Vector3.Transform(planePoint, Matrix.CreateRotationZ(delta));
                    Vector3 RotatedPoint = Vector3.Transform(planePoint, Matrix.CreateFromAxisAngle(axis, delta));


                    // Now get the difference between these points.
                    dVec = RotatedPoint - planePoint;

                    //Console.WriteLine(v);
                    part.MeshVertices[i].Position += dVec * factor;

                }

                part.SetData();
            }
        }


        public override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();

            delta = Position - prevPos;
            //Console.WriteLine(delta);
            prevPos = Position;


            //Vector3 d = delta;

            //this.TrackEntity.Model.ModelMeshes[0].MeshParts[0].VerticesPoints
            if(TrackEntity != null)
            foreach(var part in TrackEntity.Model.Meshes[0].MeshParts)
            {
                for (int i = 0; i < part.MeshVertices.Count(); i++)
                {
                    Vector3 v = Vector3.Transform( part.MeshVertices[i].Position, TrackEntity.WorldTransform);
                    float dist = MathHelper.Clamp(Math.Abs(v.Z - Position.Z) / Radius, 0, 1);;
                    float factor = 1;
                    switch(FalloffType)
                    {
                        case FalloffType.Linear:
                            factor = 1- dist;
                            break;
                        case FalloffType.Parabolic:
                            factor = (1-dist) * (1-dist);
                            break;
                        case FalloffType.InverseParabolic:
                            factor = 1 - dist * dist;
                            break;
                        case FalloffType.InvTangent:
                            // =(cos(x/10)+1)/2
                            factor = (float)(Math.Cos(dist * MathHelper.Pi)+1) / 2;
                            break;
                    }

                    //Console.WriteLine(v);
                    part.MeshVertices[i].Position += delta * factor;

                }

                part.SetData();
            }
        }
    }
}
