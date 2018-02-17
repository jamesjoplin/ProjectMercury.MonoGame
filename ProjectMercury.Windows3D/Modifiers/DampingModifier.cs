/*  
 Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using Microsoft.Xna.Framework;

    public sealed class DampingModifier : Modifier
    {
        /// <summary>
        /// The damping coefficient.
        /// </summary>
        public float DampingCoefficient { get; set; }

        public override Modifier DeepCopy()
        {
            return new DampingModifier
            {
                DampingCoefficient = this.DampingCoefficient
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
            float inverseCoefficientDelta = ((this.DampingCoefficient * deltaSeconds) * -1f);

            for (int i = 0; i < count; i++)
            {
                particle->Velocity.X += particle->Momentum.X * inverseCoefficientDelta;
                particle->Velocity.Y += particle->Momentum.Y * inverseCoefficientDelta;
                particle->Velocity.Z += particle->Momentum.Z * inverseCoefficientDelta;

                particle++;
            }
        }
    }
}
