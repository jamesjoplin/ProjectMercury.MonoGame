namespace ProjectMercury.Emitters
{
    using Microsoft.Xna.Framework;

    public sealed class SphereEmitter : Emitter
    {
        public float Radius { get; set; }

        public bool Shell { get; set; }

        public bool Radiate { get; set; }

        public override Emitter DeepCopy()
        {
            SphereEmitter copy = new SphereEmitter
            {
                Radius = this.Radius,
                Shell = this.Shell,
                Radiate = this.Radiate
            };

            base.CopyBaseFields(copy);

            return copy;
        }

        protected override void GenerateOffsetAndForce(out Vector3 offset, out Vector3 force)
        {
            Vector3 angle = RandomHelper.NextUnitVector3();

            float radiusMultiplier = this.Shell ? this.Radius : this.Radius * RandomHelper.NextFloat();

            offset = new Vector3
            {
                X = angle.X * radiusMultiplier,
                Y = angle.Y * radiusMultiplier,
                Z = angle.Z * radiusMultiplier,
            };

            force = this.Radiate ? angle : RandomHelper.NextUnitVector3();
        }
    }
}