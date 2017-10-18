using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FFStats.Models
{
    [Table("TeamRecords")]
    public class TeamRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(2014, int.MaxValue)]
        public int Year { get; set; }
        [Range(1, 14)]
        public int Week { get; set; }

        [Required]
        public Team Team { get; set; }

        [Range(0, 14)]
        public int Win { get; set; }
        [Range(0, 14)]
        public int Loss { get; set; }
        public double Pct { get => ((double)Win / (Win + Loss)); }

        public double PointsFor { get; set; }
        public double PointsAgainst { get; set; }
        public double PointsDiff { get => (PointsFor - PointsAgainst); }

        [Required]
        public List<Head2HeadRecord> Head2HeadRecords { get; set; }
    }
}
