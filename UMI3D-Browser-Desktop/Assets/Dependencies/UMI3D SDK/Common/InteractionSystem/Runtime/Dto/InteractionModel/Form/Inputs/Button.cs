namespace umi3d.common.interaction.form
{
    public class Button : BaseInput
    {
        public ButtonType Type { get; set; }
    }
    public enum ButtonType
    {
        Submit,
        Reset,
        Cancel,
        Back
    }
}