using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Services
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the base price.
        /// </summary>
        /// <value>
        /// The base price.
        /// </value>
        public decimal BasePrice { get; set; } = decimal.Min(500, 2100);

        // Navigation
        public ICollection<PriestService> PriestServices { get; set; } = new List<PriestService>();
    }

}
