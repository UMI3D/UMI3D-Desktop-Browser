namespace umi3d.common.interaction.form
{
    public class Text : Input<string>
    {
        public TextType Type { get; set; }
        public string PlaceHolder { get; set; }
    }
    public enum TextType
    {
        Text,
        Mail,
        Password,
        Phone,
        URL,
        Number
    }
}