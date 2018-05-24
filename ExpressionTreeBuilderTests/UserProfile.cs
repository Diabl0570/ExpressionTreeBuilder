using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionTreeBuilderTests
{
    public class UserProfile
    {
        #region constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserProfile()
        {
        }

        #endregion

        /// <summary>
        ///Username of the user, this is a unique identifier
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Displayname of the user
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// The mailaddress of the user
        /// </summary>
        public string MailAddress { get; set; }

        /// <summary>
        /// the location of the user profile picture
        /// </summary>
        public string UserProfilePictureLocation { get; set; }

        /// <summary>
        /// The groupnames of the user
        /// </summary>
        public List<string> GroupNames { get; set; }

        /// <summary>
        /// The displaynames of the groups the user is a member of
        /// </summary>
        public List<string> GroupDisplayNames { get; set; }

        /// <summary>
        /// The theme of the user inside the RUP
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
		/// The birthday of the user inside the RUP
		/// </summary>
        public DateTime Birthday { get; set; }
    }
}
