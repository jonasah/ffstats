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

        public int Year { get; set; }
        [Range(1, 16)]
        public int Week { get; set; }

        [Required]
        public Team Team { get; set; }
        [Required]
        public Team Opponent { get; set; }

        [Range(0, 14)]
        public int Win { get; set; }
        [Range(0, 14)]
        public int Loss { get; set; }
        public int Record { get => (Win - Loss); }
    }
}
