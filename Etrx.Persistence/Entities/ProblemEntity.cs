using System.ComponentModel.DataAnnotations;

namespace Etrx.Persistence.Entities
{
    public class ProblemEntity
    {
        [Key]
        public int ProblemId { get; set; }

        public int ContestId { get; set; }

        public string Index { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; }

        public double? Points { get; set; }

        public int? Rating { get; set; }

        public string[] Tags { get; set; } = null!;
    }
}