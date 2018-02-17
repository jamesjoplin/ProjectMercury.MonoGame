/*  
 Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/


namespace ProjectMercury
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Xna.Framework;

    [StructLayout(LayoutKind.Sequential)]
    public struct Particle
    {
        public Vector3 Position;
        public float Scale;
        public Vector3 Rotation;
        public Vector4 Colour;
        public Vector3 Momentum;
        public Vector3 Velocity;
        public float Inception;
        public float Age;
    }
}