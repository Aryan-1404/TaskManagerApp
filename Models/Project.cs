using System;
using System.Collections.Generic;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Guid> AssignedUserIds { get; set; } = new List<Guid>(); 
}
