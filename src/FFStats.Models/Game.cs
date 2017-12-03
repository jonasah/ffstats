using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public List<GameScore> GameScores { get; set; }

        public bool HasValidResult { get => GameScores.All(gs => gs.Points != null); }

        public GameScore GetMyGameScore(int myTeamId)
        {
            return GameScores.First(gs => gs.TeamId == myTeamId);
        }

        public GameScore GetOpponentGameScore(int myTeamId)
        {
            return GameScores.First(gs => gs.TeamId != myTeamId);
        }
    }
}
