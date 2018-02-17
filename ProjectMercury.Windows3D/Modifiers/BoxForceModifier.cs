/*  
 Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using System.ComponentModel;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines a Modifier which applies a force to a Particle when it enters a rectangular area.
    /// </summary>
    public sealed class BoxForceModifier : Modifier
    {
        /// <summary>
        /// Gets or sets the position of the centre of the rectangular force area.
        /// </summary>
        public Vector3 Position { get; set; }

        private float HalfWidth;

        /// <summary>
        /// Gets or sets the width of the rectangular force area.
        /// </summary>
        public float Width
        {
            get { return this.HalfWidth + this.HalfWidth; }
            set
            {
                Guard.ArgumentNotFinite("Width", value);
                Guard.ArgumentLessThan("Width", value, 0f);

                this.HalfWidth = value * 0.5f;
            }
        }

        private float HalfHeight;

        /// <summary>
        /// Gets or sets the height of the rectangular force area.
        /// </summary>
        public float Height
        {
            get { return this.HalfHeight + this.HalfHeight; }
            set
            {
                Guard.ArgumentNotFinite("Height", value);
                Guard.ArgumentLessThan("Height", value, 0f);

                this.HalfHeight = value * 0.5f;
            }
        }

        private float HalfDepth;

        public float Depth
        {
            get { return this.HalfDepth + this.HalfDepth; }
            set
            {
                Guard.ArgumentNotFinite("Depth", value);
                Guard.ArgumentLessThan("Depth", value, 0f);

                this.HalfDepth = value * 0.5f;
            }
        }

        /// <summary>
        /// Gets or sets the force vector.
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// Gets or sets the strength of the force.
        /// </summary>
        public float Strength;

        /// <summary>
        /// Returns a deep copy of the Modifier implementation.
        /// </summary>
        /// <returns></returns>
        public override Modifier DeepCopy()
        {
            return new BoxForceModifier
            {
                Position = this.Position,
                Width = this.Width,
                Height = this.Height,
                Depth = this.Depth,
                Force = this.Force,
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
            float deltaStrength = this.Strength * deltaSeconds;

            float deltaForceX = this.Force.X * deltaStrength;
            float deltaForceY = this.Force.Y * deltaStrength;
            float deltaForceZ = this.Force.Z * deltaStrength;

            for (int i = 0; i < count; i++)
            {
                if (particle->Position.X > (this.Position.X - this.HalfWidth))
                    if (particle->Position.X < (this.Position.X + this.HalfWidth))
                        if (particle->Position.Y > (this.Position.Y - this.HalfHeight))
                            if (particle->Position.Y < (this.Position.Y + this.HalfHeight))
                                if (particle->Position.Z > (this.Position.Z - this.HalfDepth))
                                    if (particle->Position.Z < (this.Position.Z + this.HalfDepth))
                                    {
                                        particle->Velocity.X += deltaForceX;
                                        particle->Velocity.Y += deltaForceY;
                                        particle->Velocity.Z += deltaForceZ;
                                    }

                particle++;
            }
        }
    }
}