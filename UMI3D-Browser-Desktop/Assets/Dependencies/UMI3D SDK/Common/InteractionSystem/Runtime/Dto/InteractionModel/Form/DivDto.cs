using System.Collections.Generic;

namespace umi3d.common.interaction.form
{
    public abstract class DivDto : ItemDto
    {
        public List<Tooltip> Tooltips { get; set; }
        public List<StyleDto> Styles { get; set; }
    }
}