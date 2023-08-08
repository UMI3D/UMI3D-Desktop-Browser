namespace umi3d.common.interaction.form
{
    public class Input<T> : BaseInput
    {
        public T Value { get; set; }

        public override object GetValue() => Value;
    }
}