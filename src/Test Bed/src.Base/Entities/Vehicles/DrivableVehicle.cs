using BEPUphysics;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Vehicle;
using BEPUphysicsDrawer.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{

    //[vxSandbox3DItem("Ford Fiesta", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/vehicles/ford/model")]
    [vxSandbox3DItem("Ford Fiesta", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/vehicles/lancer/lancer")]
    public class DrivableVehicle : TechDemoItem
    {
        /// <summary>
        /// Speed that the Vehicle tries towreach when moving backward.
        /// </summary>
        [vxShowInInspector("Vehicle Properties")]
        public float BackwardSpeed
        {
            get { return _backwardSpeed; }
            set { _backwardSpeed = value; }
        }
        float _backwardSpeed = -30;


        /// <summary>
        /// Current offset from the position of the Vehicle to the 'eyes.'
        /// </summary>
        public Vector3 CameraOffset;

        /// <summary>
        /// Speed that the Vehicle tries to reach when moving forward.
        /// </summary>
        [vxShowInInspector("Vehicle Properties")]
        public float ForwardSpeed
        {
            get { return _forwardSpeed; }
            set { _forwardSpeed = value; }
        }
        float _forwardSpeed = 100;


        /// <summary>
        /// Maximum turn angle of the wheels.
        /// </summary>
        public float MaximumTurnAngle = (float)Math.PI / 6;


        /// <summary>
        /// Turning speed of the wheels in radians per second.
        /// </summary>
        public float TurnSpeed = MathHelper.Pi;

        #region -- Wheel Properties --

        [vxShowInInspector("Vehicle Properties")]
        public float WheelStaticSlidingFriction
        {
            get
            {
                foreach (var wheel in Vehicle.Wheels)
                {
                    return wheel.SlidingFriction.StaticCoefficient;
                }
                return 0;
            }
            set
            {
                foreach(var wheel in Vehicle.Wheels)
                {
                    wheel.SlidingFriction.StaticCoefficient = value;
                }
            }
        }


        [vxShowInInspector("Vehicle Properties")]
        public float WheelKineticSlidingFriction
        {
            get
            {
                foreach (var wheel in Vehicle.Wheels)
                {
                    return wheel.SlidingFriction.KineticCoefficient;
                }
                return 0;
            }
            set
            {
                foreach (var wheel in Vehicle.Wheels)
                {
                    wheel.SlidingFriction.KineticCoefficient = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Physics representation of the Vehicle.
        /// </summary>
        public Vehicle Vehicle;

        bool isActive = false;

        //vxLightEntity coals;

        public DrivableVehicle(vxGameplayScene3D scene, Vector3 position) :
            base(scene, null, position)
        {
            ctorcount++;

            //coals = new vxLightEntity(scene, Position, LightType.Point, Color.OrangeRed, 5, 1);
        }
        public override void OnPhysicsColliderAddToSim()
        {
            //base.OnPhysicsColliderAddToSim();
        }

        public virtual void Activate()
        {
            isActive = true;
        }

        public virtual void Deactivate()
        {
            isActive = false;
        }

        static int ctorcount = 0;
        public override void OnPhysicsColliderSetup()
        {
            //var bodies = new List<CompoundShapeEntry>()
            //{
            //    new CompoundShapeEntry(new BoxShape(2.5f/2, .25f, 4.5f/2), new Vector3(0, 0, 0), 600),
            //    new CompoundShapeEntry(new BoxShape(3.5f, .3f, 4.5f/5), new Vector3(0, 0.5f,0), 600),
            //    //new CompoundShapeEntry(new SphereShape(0.5f),new Vector3(0,0,0), 1),
            //    //new CompoundShapeEntry(new CylinderShape(0.25f, 2.20f),new Vector3(0,0,0), 60),
            //    //new CompoundShapeEntry(new CylinderShape(0.25f, 1.5f),new Vector3(0,0.1f,0), 60),
            //    //new CompoundShapeEntry(new CylinderShape(0.25f, 1.5f),new Vector3(0,0.5f,0), 60),
            //};

            PhysicsCollider = new Box(Vector3.Zero, 3, 0.5f, 4, 250);
            PhysicsCollider.CollisionInformation.LocalPosition = new Vector3(0, 0.25f, 0);
            PhysicsCollider.Position = Position; //At first, just keep it out of the way.
            Vehicle = new Vehicle(PhysicsCollider);
            //PhysicsCollider.AngularDamping = 0.95f;
            PhysicsCollider.Material.KineticFriction = 0.51f;
            PhysicsCollider.Material.StaticFriction = 0.65f;
            
            PhysicsCollider.Material.Bounciness = 0;


            WorldTransform = Matrix.CreateTranslation(StartPosition);
            editorTransform = WorldTransform;

            if (ctorcount > 0)
            {
                #region RaycastWheelShapes

                float Wheel_Pos_W = 1.40f;
                float Wheel_Pos_L = 2.15f;


                //The wheel model used is not aligned initially with how a wheel would normally look, so rotate them.
                Matrix wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);

                var wh = new Wheel(new CylinderCastWheelShape(1, 1, Quaternion.Identity, Matrix.Identity, true));
                //var wh3 = new Wheel(new RaycastWheelShape(Wheel_RAD, wheelGraphicRotation));


                var wheel1 = new DrivableVehicleWheel(Scene, new Vector3(-Wheel_Pos_W, 0, Wheel_Pos_L));
                var wheel2 = new DrivableVehicleWheel(Scene, new Vector3(-Wheel_Pos_W, 0, -Wheel_Pos_L));
                var wheel3 = new DrivableVehicleWheel(Scene, new Vector3(Wheel_Pos_W, 0, Wheel_Pos_L));
                var wheel4 = new DrivableVehicleWheel(Scene, new Vector3(Wheel_Pos_W, 0, -Wheel_Pos_L));

                wheels.Add(wheel1);
                wheels.Add(wheel2);
                wheels.Add(wheel3);
                wheels.Add(wheel4);

                foreach (var wheel in wheels)
                    Vehicle.AddWheel(wheel.Wheel);


                Scene.PhyicsSimulation.Add(Vehicle);
                Scene.PhysicsDebugViewer.Add(PhysicsCollider);

                foreach (Wheel wheel in Vehicle.Wheels)
                {
                    //This is a cosmetic setting that makes it looks like the car doesn't have antilock brakes.
                    wheel.Shape.FreezeWheelsWhileBraking = true;

                    //By default, wheels use as many iterations as the space.  By lowering it,
                    //performance can be improved at the cost of a little accuracy.
                    //However, because the suspension and friction are not really rigid,
                    //the lowered accuracy is not so much of a problem.
                    wheel.Suspension.SolverSettings.MaximumIterationCount = 4;
                    wheel.Brake.SolverSettings.MaximumIterationCount = 1;
                    wheel.SlidingFriction.SolverSettings.MaximumIterationCount = 4;
                    wheel.DrivingMotor.SolverSettings.MaximumIterationCount = 1;

                    Scene.PhysicsDebugViewer.Add(wheel.Shape);
                    
                }
                Mass = 100;
                #endregion
            }

            IsMotionBlurEnabled = false;
            //DoSSR = true;
            DoReflections = true;
        }

        List<DrivableVehicleWheel> wheels = new List<DrivableVehicleWheel>();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Vehicle.Body.Mass = 100000;
            //coals.Position = this.Position;

            if (isActive)
            {
                //Reset properties to defaults.
                //foreach (Wheel wheel in Vehicle.Wheels)
                //{
                //    wheel.Brake.IsBraking = false;
                //    wheel.DrivingMotor.MaximumForwardForce = MaximumDriveForce;
                //    wheel.DrivingMotor.MaximumBackwardForce = MaximumDriveForce;
                //    wheel.SlidingFriction.KineticCoefficient = BaseSlidingFriction;
                //    wheel.SlidingFriction.StaticCoefficient = BaseSlidingFriction;
                //}
                if(vxInput.IsNewKeyPress(Keys.R))
                {
                    Vehicle.Body.WorldTransform = Matrix.CreateTranslation(Position + Vector3.Up * 5);
                }

                if (vxInput.IsKeyDown(Keys.W))
                {
                    //Drive
                    Vehicle.Wheels[1].DrivingMotor.TargetSpeed = _forwardSpeed;
                    Vehicle.Wheels[3].DrivingMotor.TargetSpeed = _forwardSpeed;
                }
                else if (vxInput.IsKeyDown(Keys.S))
                {
                    //Reverse
                    Vehicle.Wheels[1].DrivingMotor.TargetSpeed = BackwardSpeed;
                    Vehicle.Wheels[3].DrivingMotor.TargetSpeed = BackwardSpeed;
                }
                else
                {
                    //Idle
                    Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
                    Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
                }


                if (vxInput.IsKeyDown(Keys.Space))
                {
                    //Brake
                    foreach (Wheel wheel in Vehicle.Wheels)
                    {
                        wheel.Brake.IsBraking = true;
                    }
                    //Idle
                    Vehicle.Wheels[0].DrivingMotor.TargetSpeed = 0;
                    Vehicle.Wheels[2].DrivingMotor.TargetSpeed = 0;
                }
                else
                {
                    //Release brake
                    foreach (Wheel wheel in Vehicle.Wheels)
                    {
                        wheel.Brake.IsBraking = false;
                    }
                }

                //Use smooth steering; while held down, move towards maximum.
                //When not pressing any buttons, smoothly return to facing forward.
                float angle;
                bool steered = false;
                float dt = 0.0167f;
                if (vxInput.IsKeyDown(Keys.A))
                {
                    steered = true;
                    angle = Math.Max(Vehicle.Wheels[1].Shape.SteeringAngle - TurnSpeed * dt, -MaximumTurnAngle);
                    Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
                if (vxInput.IsKeyDown(Keys.D))
                {
                    steered = true;
                    angle = Math.Min(Vehicle.Wheels[1].Shape.SteeringAngle + TurnSpeed * dt, MaximumTurnAngle);
                    Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
                if (!steered)
                {
                    //Neither key was pressed, so de-steer.
                    if (Vehicle.Wheels[1].Shape.SteeringAngle > 0)
                    {
                        angle = Math.Max(Vehicle.Wheels[1].Shape.SteeringAngle - TurnSpeed * dt, 0);
                        Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                        Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                    }
                    else
                    {
                        angle = Math.Min(Vehicle.Wheels[1].Shape.SteeringAngle + TurnSpeed * dt, 0);
                        Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                        Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                    }
                }
            }
        }
    }
}
