using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionTreeBuilderTests
{
    public class ReportingTimeActiveModel
    {
        /// <summary>
        /// The unique Id for this db record (pk)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The UserName that generated this log
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user object that holds the user details
        /// </summary>
        public UserProfile UserProfile { get; set; }
        /// <summary>
        /// On what DateTime did this log started
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// On what DateTime did this user logged off
        /// </summary>
        public DateTime DateLogoff { get; set; }
        /// <summary>
        /// For how many minutes was this user loggedin?
        /// </summary>
        public double MinutesActive { get; set; }
        /// <summary>
        /// This is used to display and group by 
        /// </summary>
        public string DisplayName { get; set; }
    }
}
