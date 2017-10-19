using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("PlayoffProbabilities")]
    public class PlayoffProbability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Range(2014, int.MaxValue)]
        public int Year { get; set; }
        [Range(1, 14)]
        public int Week { get; set; }

        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; }

        [Range(0.0, 1.0)]
        public double IncludingTiebreaker { get; set; }
        [Range(0.0, 1.0)]
        public double ExcludingTiebreaker { get; set; }
    }
}
