namespace ProjectMercury
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using ProjectMercury.Emitters;

    public class ParticleEffect : EmitterCollection
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public virtual ParticleEffect DeepCopy()
        {
            ParticleEffect effect = new ParticleEffect
            {
                Author = this.Author,
                Description = this.Description,
                Name = this.Name,
            };

            foreach (Emitter emitter in this)
                effect.Add(emitter.DeepCopy());

            return effect;
        }

        public virtual void Trigger(Vector3 position)
        {
            this.Trigger(ref position);
        }

        public virtual void Trigger(ref Vector3 position)
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Trigger(ref position);
        }

        public virtual void Initialise()
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Initialise();
        }

        public virtual void Terminate()
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Terminate();
        }

        public virtual void LoadContent(ContentManager content)
        {
            for (int i = 0; i < this.Count; i++)
                this[i].LoadContent(content);
        }

        public virtual void Update(float deltaSeconds)
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Update(deltaSeconds);
        }

        public int ActiveParticlesCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < this.Count; i++)
                    count += this[i].ActiveParticlesCount;

                return count;
            }
        }
    }
}