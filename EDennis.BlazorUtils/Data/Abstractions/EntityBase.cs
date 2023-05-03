﻿using System.ComponentModel.DataAnnotations;

namespace EDennis.BlazorUtils
{ 
    public class EntityBase : IHasIntegerId, IHasSysGuid, IHasSysUser
    {
        [Key]
        public int Id { get; set; }
        public string SysUser { get; set; } = "SYSTEM";
        public Guid SysGuid { get; set; } = Guid.NewGuid();
    }
}