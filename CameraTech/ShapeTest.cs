using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTech
{
    internal class ShapeTest
    {
        public int raycast { get; private set; }
        public int hitEntity { get; private set; }
        public bool hit { get; private set; }
        public Vector3 endCoords { get; private set; }
        public Vector3 surfaceNormal { get; private set; }
        public Vector3 from { get; private set; }
        public Vector3 to { get; private set; }

        public ShapeTest(int raycast, bool hit, Vector3 endCoords, Vector3 surfaceNormal, int hitEntity, Vector3 from, Vector3 to)
        {
            this.raycast = raycast;
            this.hitEntity = hitEntity;
            this.hit = hit;
            this.endCoords = endCoords;
            this.surfaceNormal = surfaceNormal;
            this.from = from;
            this.to = to;
        }

        public static ShapeTest StartShapeTest(Vector3 From, Vector3 To, float Radius, Entity Ignored, int flag = 10)
        {
            int raycast = API.StartShapeTestCapsule(From.X, From.Y, From.Z, To.X, To.Y, To.Z, Radius, 10, Ignored.Handle, 7);
            int hitEntity = 0;
            bool hit = false;
            Vector3 endCoords = Vector3.Zero;
            Vector3 surfaceNormal = Vector3.Zero;
            API.GetShapeTestResult(raycast, ref hit, ref endCoords, ref surfaceNormal, ref hitEntity);
            return new ShapeTest(raycast, hit, endCoords, surfaceNormal, hitEntity, From, To);
        }
    }
}
