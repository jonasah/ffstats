using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFStats.Models
{
    [Table("SeasonInfo")]
    public class SeasonInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(2014, int.MaxValue)]
        public int Year { get; set; }

        public int NumTeams { get; set; }
        public int NumPlayoffTeams { get; set; }
        public int RegularSeasonLength { get; set; }
        public int PlayoffLength { get; set; }
        public int SeasonLength => RegularSeasonLength + PlayoffLength;

        public int? ChampionId { get; set; }
        public Team Champion { get; set; }

        public int? SecondPlaceId { get; set; }
        public Team SecondPlace { get; set; }

        public int? ThirdPlaceId { get; set; }
        public Team ThirdPlace { get; set; }

        public int? SackoId { get; set; }
        public Team Sacko { get; set; }

        public int? RegularSeasonChampionId { get; set; }
        public Team RegularSeasonChampion { get; set; }

        public int? HighestPointsForTeamId { get; set; }
        public Team HighestPointsForTeam { get; set; }

        public double HighestPointsFor { get; set; }
    }
}
