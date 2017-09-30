﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }
        [Range(1, 16)]
        public int Week { get; set; }

        [Required]
        public Team Team1 { get; set; }
        [Required]
        public Team Team2 { get; set; }
    }
}
