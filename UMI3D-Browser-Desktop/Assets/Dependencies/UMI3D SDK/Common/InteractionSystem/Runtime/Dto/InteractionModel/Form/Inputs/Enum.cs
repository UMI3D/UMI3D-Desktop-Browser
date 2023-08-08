using System.Collections.Generic;
using System.Linq;
using umi3d.common.interaction.form;

public class Enum<T, G> : BaseInput
    where T : EnumValue<G> 
    where G : Div
{
    public List<T> Values { get; set; }
    public bool CanSelectMultiple { get; set; }

    public override object GetValue() => Values.Where(e => e.IsSelected);
}

public class EnumValue<G> where G : Div
{
    public G Item { get; set; }
    public bool IsSelected { get; set; }
}