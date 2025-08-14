using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class PriestService
    {
        /// <summary>
        /// Gets or sets the priest profile identifier.
        /// </summary>
        /// <value>
        /// The priest profile identifier.
        /// </value>
        public int PriestProfileId { get; set; }
        /// <summary>
        /// Gets or sets the priest profile.
        /// </summary>
        /// <value>
        /// The priest profile.
        /// </value>
        public PriestProfile? PriestProfile { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public int ServiceId { get; set; }
        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public Services? Service { get; set; }
    }

}
