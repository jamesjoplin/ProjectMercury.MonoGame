/*  
 Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Emitters
{
    using System;
    using System.ComponentModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Modifiers;

    public abstract class Emitter
    {
        // todo: static name index, name changed event etc...
        public string Name { get; set; }

        private float TotalSeconds;

        public bool Initialised { get; private set; }

        public bool Enabled { get; set; }

        private int _budget;

        public int Budget
        {
            get { return this._budget; }
            set
            {
                // todo: argument validation...
                this._budget = value;
            }
        }

        private float _term;

        public float Term
        {
            get { return this._term; }
            set
            {
                // todo: argument validation...
                this._term = value;
            }
        }

        internal Particle[] Particles;

        private int IdleIndex;

        private int _releaseQuantity;

        public int ReleaseQuantity
        {
            get { return this._releaseQuantity; }
            set
            {
                // todo: argument validation...
                this._releaseQuantity = value;
            }
        }

        public VariableFloat ReleaseSpeed { get; set; }

        public Vector3 ReleaseColour { get; set; }

        public VariableFloat ReleaseOpacity { get; set; }

        public VariableFloat ReleaseScale { get; set; }

        public VariableFloat3 ReleaseRotation { get; set; }

        public Vector3 ReleaseImpulse { get; set; }

        public Texture2D ParticleTexture { get; set; }

        public Vector3 TriggerOffset { get; set; }

        public ModifierCollection Modifiers { get; set; }

        public EmitterBlendMode BlendMode { get; set; }

        // todo: ViewMode enum (fixed, free, upright [facing], upright [fixed])

        public float MinimumTriggerPeriod { get; set; }

        private float MostRecentTrigger;

        public int ActiveParticlesCount
        {
            get { return this.IdleIndex; }
        }

        protected Emitter()
        {
            this.Enabled = true;

            this.Modifiers = new ModifierCollection();
        }

        public abstract Emitter DeepCopy();

        protected void CopyBaseFields(Emitter emitter)
        {
            emitter.BlendMode = this.BlendMode;
            emitter.Budget = this.Budget;
            emitter.Enabled = this.Enabled;
            emitter.MinimumTriggerPeriod = this.MinimumTriggerPeriod;
            emitter.Modifiers = this.Modifiers.DeepCopy();
            emitter.Name = String.Format("Copy of {0}", this.Name);
            emitter.ParticleTexture = this.ParticleTexture;
            emitter.ReleaseColour = this.ReleaseColour;
            emitter.ReleaseOpacity = this.ReleaseOpacity;
            emitter.ReleaseQuantity = this.ReleaseQuantity;
            emitter.ReleaseRotation = this.ReleaseRotation;
            emitter.ReleaseScale = this.ReleaseScale;
            emitter.Term = this.Term;
            emitter.TriggerOffset = this.TriggerOffset;
        }

        public virtual void Initialise()
        {
            this.Particles = new Particle[this.Budget];

            this.IdleIndex = 0;

            this.TotalSeconds = 0f;

            this.Initialised = true;
        }

        public virtual void Initialise(int budget, float term)
        {
            this.Initialised = false;

            this.Budget = budget;
            this.Term = term;

            this.Initialise();
        }

        public virtual void LoadContent(ContentManager content) { }

        public void Terminate()
        {
            this.IdleIndex = 0;
        }

        private unsafe void RetireParticles(Particle* particleArray, int count)
        {
            Particle* source = (particleArray + count);
            Particle* destination = particleArray;

            int num = this.IdleIndex - count;

            for (int i = 0; i < num; i++)
            {
                *destination = *source;

                source++;
                destination++;
            }

            this.IdleIndex -= count;
        }

        public virtual void Update(float deltaSeconds)
        {
            this.TotalSeconds += deltaSeconds;

            unsafe
            {
                fixed (Particle* particleArray = this.Particles)
                {
                    int i = this.IdleIndex;

                    Particle* particle = particleArray + (i - 1);

                    while (--i >= 0)
                    {
                        float actualAge = this.TotalSeconds - particle->Inception;

                        if (actualAge > this.Term)
                            break;

                        particle->Age = actualAge / this.Term;

                        particle->Momentum.X += particle->Velocity.X;
                        particle->Momentum.Y += particle->Velocity.Y;
                        particle->Momentum.Z += particle->Velocity.Z;

                        particle->Velocity = Vector3.Zero;

                        particle->Position.X += particle->Momentum.X;
                        particle->Position.Y += particle->Momentum.Y;
                        particle->Position.Z += particle->Momentum.Z;

                        particle--;
                    }

                    if (i >= 0)
                        this.RetireParticles(particleArray, i + 1);

                    this.Modifiers.RunProcessors(deltaSeconds, particleArray, this.ActiveParticlesCount);
                }
            }
        }

        public virtual void Trigger(ref Vector3 triggerPosition)
        {
            if (this.Enabled == false)
                return;

            if ((this.TotalSeconds - this.MostRecentTrigger) < this.MinimumTriggerPeriod)
                return;

            Vector3 position = new Vector3
            {
                X = triggerPosition.X + this.TriggerOffset.X,
                Y = triggerPosition.Y + this.TriggerOffset.Y,
                Z = triggerPosition.Z + this.TriggerOffset.Z
            };

            int oldIdleIndex = this.IdleIndex;

            unsafe
            {
                fixed (Particle* particleArray = this.Particles)
                {
                    Particle* particle = particleArray + oldIdleIndex;

                    for (int i = oldIdleIndex; i < oldIdleIndex + this.ReleaseQuantity; i++)
                    {
                        if (i < this.Budget)
                        {
                            Vector3 offset, force;

                            this.GenerateOffsetAndForce(out offset, out force);

                            float speed = this.ReleaseSpeed;

                            particle->Inception = this.TotalSeconds;
                            particle->Position.X = position.X + offset.X;
                            particle->Position.Y = position.Y + offset.Y;
                            particle->Position.Z = position.Z + offset.Z;
                            particle->Velocity.X = force.X * speed;
                            particle->Velocity.Y = force.Y * speed;
                            particle->Velocity.Z = force.Z * speed;
                            particle->Momentum = this.ReleaseImpulse;
                            particle->Age = 0f;
                            particle->Colour = new Vector4(this.ReleaseColour, this.ReleaseOpacity);
                            particle->Scale = this.ReleaseScale;
                            particle->Rotation = this.ReleaseRotation;

                            particle++;
                            this.IdleIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    this.MostRecentTrigger = this.TotalSeconds;
                }
            }
        }

        public void Trigger(Vector3 position)
        {
            this.Trigger(ref position);
        }

        protected virtual void GenerateOffsetAndForce(out Vector3 offset, out Vector3 force)
        {
            offset = Vector3.Zero;

            force = Vector3.Forward;
        }
    }
}