﻿/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2010 Ian Qvist
* 
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.TestBed.Tests
{
    public class PathTest : Test
    {
        private Body _movingBody;
        private Path _path;

        private float _time;

        private PathTest()
        {
            //Single body that moves around path
            _movingBody = BodyFactory.CreateBody(World);
            _movingBody.Position = new Vector2(-25, 25);
            _movingBody.BodyType = BodyType.Dynamic;
            _movingBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(0.5f, 0.5f), 1));

            //Static shape made up of bodies
            _path = new Path();
            _path.Add(new Vector2(0, 20));
            _path.Add(new Vector2(5, 15));
            _path.Add(new Vector2(20, 18));
            _path.Add(new Vector2(15, 1));
            _path.Add(new Vector2(-5, 14));
            _path.Closed = true;

            CircleShape shape = new CircleShape(0.25f, 1);

            PathManager.EvenlyDistributeShapesAlongPath(World, _path, shape, BodyType.Static, 100);

            ////Smaller shape that is movable. Created from small rectangles and circles.
            Vector2 xform = new Vector2(0.5f, 0.5f);
            _path.Scale(ref xform);
            xform = new Vector2(5, 5);
            _path.Translate(ref xform);

            List<Shape> shapes = new List<Shape>(2);
            shapes.Add(new PolygonShape(PolygonTools.CreateRectangle(0.5f, 0.5f, new Vector2(-0.1f, 0), 0), 1));
            shapes.Add(new CircleShape(0.5f, 1));

            List<Body> bodies = PathManager.EvenlyDistributeShapesAlongPath(World, _path, shapes, BodyType.Dynamic, 20);

            //Attach the bodies together with revolute joints
            PathManager.AttachBodiesWithRevoluteJoint(World, bodies, new Vector2(0, 0.5f), new Vector2(0, -0.5f), true,
                                                      true);

            xform = new Vector2(-25, 0);
            _path.Translate(ref xform);

            Body body = BodyFactory.CreateBody(World);
            body.BodyType = BodyType.Static;

            //Static shape made up of edges
            PathManager.ConvertPathToEdges(_path, body, 25);
            body.Position -= new Vector2(0, 10);

            xform = new Vector2(0, 15);
            _path.Translate(ref xform);

            PathManager.ConvertPathToPolygon(_path, body, 1, 50);
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            _time += 0.01f;
            if (_time > 1f)
                _time = 0;

            PathManager.MoveBodyOnPath(_path, _movingBody, _time, 1f, 1f / 60f);

            base.Update(settings, gameTime);
        }

        internal static Test Create()
        {
            return new PathTest();
        }
    }
}