﻿using System;
using System.Collections.Generic;

namespace Project_HR_Managment.Models;

public partial class Hr
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ProfileImage { get; set; }
}
