/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using System;
    using System.ComponentModel;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines a Modifier which applies a force to a Particle when it enters a circular area.
    /// </summary>
#if WINDOWS
    [TypeConverter("ProjectMercury.Design.Modifiers.RadialForceModifierTypeConverter, ProjectMercury.Design")]
#endif
    public class RadialForceModifier : Modifier
    {
        /// <summary>
        /// Gets or sets the position of the force.
        /// </summary>
        public Vector3 Position { get; set; }

        private float _radius;

        /// <summary>
        /// Gets or sets the radius of the force.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the specified value is negetive or zero</exception>
        public float Radius
        {
            get { return this._radius; }
            set
            {
                this._radius = value;

                this.SquareRadius = value * value;
            }
        }

        private float SquareRadius;

        /// <summary>
        /// Gets or sets the force vector.
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// Gets or sets the strength of the force.
        /// </summary>
        public float Strength { get; set; }

        public override Modifier DeepCopy()
        {
            return new RadialForceModifier
            {
                Force = this.Force,
                Position = this.Position,
                Radius = this.Radius,
                Strength = this.Strength
            };
        }

        /// <summary>
        /// Processes the particles.
        /// </summary>
        /// <param name="deltaSeconds">Elapsed time in whole and fractional seconds.</param>
        /// <param name="particle">A pointer to an array of particles.</param>
        /// <param name="count">The number of particles which need to be processed.</param>
        protected internal override unsafe void Process(float deltaSeconds, Particle* particle, int count)
        {
            Vector3 distance;

            float strengthDelta = this.Strength * deltaSeconds;

            float deltaForceX = this.Force.X * strengthDelta;
            float deltaForceY = this.Force.Y * strengthDelta;
            float deltaForceZ = this.Force.Z + strengthDelta;

            for (int i = 0; i < count; i++)
            {
                // Calculate the distance between the Particle and the center of the force...
                distance.X = this.Position.X - particle->Position.X;
                distance.Y = this.Position.Y - particle->Position.Y;
                distance.Z = this.Position.Z - particle->Position.Z;

                float squareDistance = ((distance.X * distance.X) +
                                        (distance.Y * distance.Y) + 
                                        (distance.Z * distance.Z));

                // Check to see if the Particle is within range of the force...
                if (squareDistance < this.SquareRadius)
                {
                    // Adjust the force vector based on the strength of the force and the time delta...
                    particle->Velocity.X += deltaForceX;
                    particle->Velocity.Y += deltaForceY;
                    particle->Velocity.Z += deltaForceZ;
                }

                particle++;
            }
        }
    }
}