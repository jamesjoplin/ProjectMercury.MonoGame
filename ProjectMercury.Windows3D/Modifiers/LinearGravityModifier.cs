/*  
 Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines a Modifier that applies a constant force vector to Particles over their lifetime.
    /// </summary>
    public sealed class LinearGravityModifier : Modifier
    {
        /// <summary>
        /// Gets or sets the gravity vector.
        /// </summary>
        public Vector3 Gravity { get; set; }

        public override Modifier DeepCopy()
        {
            return new LinearGravityModifier
            {
                Gravity = this.Gravity
            };
        }

        /// <summary>
        /// Processes the particles.
        /// </summary>
        /// <param name="dt">Elapsed time in whole and fractional seconds.</param>
        /// <param name="particleArray">A pointer to an array of particles.</param>
        /// <param name="count">The number of particles which need to be processed.</param>
        protected internal override unsafe void Process(float deltaSeconds, Particle* particle, int count)
        {
            float deltaGravityX = this.Gravity.X * deltaSeconds;
            float deltaGravityY = this.Gravity.Y * deltaSeconds;
            float deltaGravityZ = this.Gravity.Z * deltaSeconds;

            for (int i = 0; i < count; i++)
            {
                particle->Velocity.X += deltaGravityX;
                particle->Velocity.Y += deltaGravityY;
                particle->Velocity.Z += deltaGravityZ;

                particle++;
            }
        }
    }
}