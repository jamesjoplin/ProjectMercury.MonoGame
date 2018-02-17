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
    /// Defines a modifier which changes the Force of particles based on a linear interpolation over three values.
    /// </summary>
    public sealed class ForceInterpolatorModifier : Modifier
    {
        /// <summary>
        /// Gets or sets the initial force vector.
        /// </summary>
        /// <value>The initial force vector.</value>
        public Vector3 InitialForce { get; set; }

        /// <summary>
        /// Gets or sets the middle force vector.
        /// </summary>
        /// <value>The middle force vector.</value>
        public Vector3 MiddleForce { get; set; }

        /// <summary>
        /// Gets or sets the middle force position.
        /// </summary>
        /// <value>The middle position.</value>
        public float MiddlePosition { get; set; }

        /// <summary>
        /// Gets or sets the final force vector.
        /// </summary>
        /// <value>The final force vector.</value>
        public Vector3 FinalForce { get; set; }

        public override Modifier DeepCopy()
        {
            return new ForceInterpolatorModifier
            {
                FinalForce = this.FinalForce,
                InitialForce = this.InitialForce,
                MiddleForce = this.MiddleForce,
                MiddlePosition = this.MiddlePosition
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
            for (int i = 0; i < count; i++)
            {
                if (particle->Age < this.MiddlePosition)
                {
                    float positionDelta = (particle->Age / this.MiddlePosition);
                    
                    particle->Velocity.X += this.InitialForce.X + ((this.MiddleForce.X - this.InitialForce.X) * positionDelta);
                    particle->Velocity.Y += this.InitialForce.Y + ((this.MiddleForce.Y - this.InitialForce.Y) * positionDelta);
                    particle->Velocity.Z += this.InitialForce.Z + ((this.MiddleForce.Z - this.InitialForce.Z) * positionDelta);
                }
                else
                {
                    float positionDelta = ((particle->Age - this.MiddlePosition) / (1f - this.MiddlePosition));
                    
                    particle->Velocity.X += this.MiddleForce.X + ((this.FinalForce.X - this.MiddleForce.X) * positionDelta);
                    particle->Velocity.Y += this.MiddleForce.Y + ((this.FinalForce.Y - this.MiddleForce.Y) * positionDelta);
                    particle->Velocity.Z += this.MiddleForce.Z + ((this.FinalForce.Z - this.MiddleForce.Z) * positionDelta);
                }

                particle++;
            }
        }
    }
}
