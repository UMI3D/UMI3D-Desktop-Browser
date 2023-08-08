using System.Collections.Generic;

namespace umi3d.common.interaction.form
{
    public class Group : BaseInput
    {
        public List<Div> Children { get; set; }

        public bool CanRemember { get; set; }
        public bool SelectFirstInput { get; set; }
    }
}