namespace umi3d.common.interaction.form
{
    public class Range<T> : BaseInput
    {
        public T Min { get; set; }
        public T Max { get; set; }
        public T Value { get; set; }
        public T Step { get; set; }

        public override object GetValue() => Value;
    }
}