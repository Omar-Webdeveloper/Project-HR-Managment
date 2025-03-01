using System;
using System.Collections.Generic;

namespace Project_HR_Managment.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ContactInfo { get; set; }

    public int? ManagerId { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Manager? Manager { get; set; }
}
