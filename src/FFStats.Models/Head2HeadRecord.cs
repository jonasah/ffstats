using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("Head2HeadRecords")]
    public class Head2HeadRecord
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

        public int OpponentId { get; set; }
        [Required]
        public Team Opponent { get; set; }

        [Range(0, 14)]
        public int Win { get; set; }
        [Range(0, 14)]
        public int Loss { get; set; }
        public int GamesPlayed { get => (Win + Loss); }
        public double Pct { get => (GamesPlayed == 0 ? 0 : ((double)Win / GamesPlayed)); }
    }
}
