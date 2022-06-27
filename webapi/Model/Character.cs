using System;
using System.Collections.Generic;

namespace webapi.Model
{
    public class Character
    {

        public int Id { get; set; }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the status of the character ('Alive', 'Dead' or 'unknown').
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets the species of the character.
        /// </summary>
        public string Species { get; set; }

        /// <summary>
        /// Gets the type or subspecies of the character.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the gender of the character ('Female', 'Male', 'Genderless' or 'unknown').
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets name and link to the character's last known location endpoint.
        /// </summary>
        public CharacterLocation Location { get; set; }

        /// <summary>
        /// Gets name and link to the character's origin location.
        /// </summary>
        public CharacterOrigin Origin { get; set; }

        /// <summary>
        /// Gets link to the character's image. All images are 300x300px and most are medium shots or portraits since they are intended to be used as avatars.
        /// </summary>
        public Uri Image { get; set; }

        /// <summary>
        /// Gets list of episodes in which this character appeared.
        /// </summary>
        public IEnumerable<Uri> Episode { get; set; }

        /// <summary>
        /// Gets link to the character's own URL endpoint.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets time at which the character was created in the database.
        /// </summary>
        public DateTime? Created { get; set; }
    }
}
