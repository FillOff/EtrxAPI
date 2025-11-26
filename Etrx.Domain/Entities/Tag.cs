using System;
using System.Collections.Generic;

namespace Etrx.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Complexity { get; set; }

        public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    }
}