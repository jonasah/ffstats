using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("GameScores")]
    public class GameScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(2014, int.MaxValue)]
        public int Year { get; set; }
        [Range(1, 16)]
        public int Week { get; set; }

        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; }

        public double? Points { get; set; }

        public int GameId { get; set; }
    }
}
