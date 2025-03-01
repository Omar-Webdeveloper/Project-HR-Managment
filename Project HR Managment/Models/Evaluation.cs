using System;
using System.Collections.Generic;

namespace Project_HR_Managment.Models;

public partial class Evaluation
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? ManegerId { get; set; }

    public DateOnly? EvaluationDate { get; set; }

    public string? Status { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Manager? Maneger { get; set; }
}
