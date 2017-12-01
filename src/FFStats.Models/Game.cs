using System;
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

        [Range(2014, int.MaxValue)]
        public int Year { get; set; }
        [Range(1, 16)]
        public int Week { get; set; }

        public int Team1Id { get; set; }
        [Required]
        public Team Team1 { get; set; }

        public int Team2Id { get; set; }
        [Required]
        public Team Team2 { get; set; }

        public double? Points1 { get; set; }
        public double? Points2 { get; set; }

        public bool HasValidResult { get => (Points1 != null && Points2 != null); }

        public bool HasTeam(int id)
        {
            return Team1Id == id || Team2Id == id;
        }

        public Tuple<Team, double?> GetMyTeam(int myTeamId)
        {
            return (Team1Id == myTeamId ?
                Tuple.Create(Team1, Points1) :
                Tuple.Create(Team2, Points2));
        }

        public Tuple<Team, double?> GetOpponent(int myTeamId)
        {
            return (Team1Id == myTeamId ?
                Tuple.Create(Team2, Points2) :
                Tuple.Create(Team1, Points1));
        }
    }
}
