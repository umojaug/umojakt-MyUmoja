﻿namespace GrapesTl.Models;

public class AcSubGroup
{
    public string SubGroupId { get; set; }
    public string GroupId { get; set; }
    public string SubGroupName { get; set; }

}

public class AcSubGroupView : AcSubGroup
{
    public string MainName { get; set; }
    public string GroupName { get; set; }
    public string SubGroupCode { get; set; }
}
