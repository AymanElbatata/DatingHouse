﻿using AYMDatingCore.DAL.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYMDatingCore.DAL.Entities
{
    public class UserAddressListTBL : BaseEntity<int>
    {
        public string? AppUserId { get; set; }
        public string? Address { get; set; } = null!;
        public string? AddressFamily { get; set; } = null!;
        public virtual AppUser? AppUser { get; set; }
    }
}
