﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("LineupPlayers")]
    public class LineupPlayer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }
        [Range(1, 16)]
        public int Week { get; set; }
        
        [Required]
        public Team Team { get; set; }
        [Required]
        public Player Player { get; set; }
        public Position Position { get; set; }

        public double Points { get; set; }
    }
}
