﻿using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers.Buoyancy;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.TestBed.Tests
{
    public class BuoyancyTest : Test
    {
        private AABBFluidContainer _aabbContainer;
        private WaveContainer _waveContainer;

        private BuoyancyTest()
        {
            //Make a box
            //Bottom
            Body ground = BodyFactory.CreateBody(World);
            Vertices edge = PolygonTools.CreateEdge(new Vector2(0.0f, 0.0f), new Vector2(40.0f, 0.0f));
            PolygonShape shape = new PolygonShape(edge);
            ground.CreateFixture(shape);

            //Left side
            shape.Set(PolygonTools.CreateEdge(new Vector2(0.0f, 0.0f), new Vector2(00.0f, 15.0f)));
            ground.CreateFixture(shape);

            //Right side
            shape.Set(PolygonTools.CreateEdge(new Vector2(40.0f, 0.0f), new Vector2(40.0f, 15.0f)));
            ground.CreateFixture(shape);

            //Buoyancy controller
            _aabbContainer = new AABBFluidContainer(new Vector2(0, 0), 40, 10);
            _waveContainer = new WaveContainer(new Vector2(0, 0), 40, 10);
            _waveContainer.WaveGeneratorStep = 0;

            FluidDragController buoyancyController = new FluidDragController(_waveContainer, 4f, 0.98f, 0.2f,
                                                                             World.Gravity);
            buoyancyController.Entry += EntryEventHandler;

            Vector2 offset = new Vector2(5, 0);

            //Bunch of balls
            for (int i = 0; i < 4; i++)
            {
                Fixture fixture = FixtureFactory.CreateCircle(World, 1, 1, new Vector2(15, 1) + offset*i);
                fixture.Body.BodyType = BodyType.Dynamic;
                buoyancyController.AddGeom(fixture);
            }

            World.Add(buoyancyController);
        }

        public override void Initialize()
        {
            //move the field of view to the right - the wavecontroller only works in positive coordinates.
            GameInstance.ViewCenter = new Vector2(GameInstance.ViewCenter.X + 20);

            base.Initialize();
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            DebugView.DrawWaveContainer(_waveContainer);
            base.Update(settings, gameTime);
        }

        public void EntryEventHandler(Fixture geom, Vertices verts)
        {
            //for (int i = 0; i < verts.Count; i++)
            //{
            //    Vector2 point = verts[i];
            //    Vector2 vel = geom.Body.GetLinearVelocityFromWorldPoint(point);

            //    _waveContainer.Disturb(verts[i].X, (vel.Y * geom.Body.Mass) / (100.0f * geom.Body.Mass));
            //}
        }

        public static Test Create()
        {
            return new BuoyancyTest();
        }
    }
}