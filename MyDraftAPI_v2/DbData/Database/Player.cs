using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class Player
    {
        public int ID { get; set; }
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(5)]
        public string? TeamAbbr { get; set; }
        public DateTime BirthDate { get; set; }
        public int? Experience { get; set; }
        [MaxLength(3)]
        public string? Position { get; set; }
        [MaxLength(50)]
        public string? PositionGroup { get; set; }
        public int Weight { get; set; }
        [MaxLength(6)]
        public string? Height { get; set; }
        [MaxLength(50)]
        public string? College { get; set; }
        public bool IsRookie { get; set; }
        [MaxLength(250)]
        public string? PhotoUrl { get; set; }
        [MaxLength(10)]
        public string? Status { get; set; }

    }
}
