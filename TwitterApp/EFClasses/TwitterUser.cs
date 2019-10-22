using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.EFClasses
{
    public class TwitterUser
    {
        public TwitterUser()
        {
            Posts = new List<TwitterPost>();
        }

        [Required, Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserScreenName { get; set; }


        [MaxLength(500)]
        public string Description { get; set; }

        public int FollowersCount { get; set; }

        public int FavouritesCount { get; set; }

        public int FriendsCount { get; set; }

        public string ProfileImageUrl { get; set; }

        public virtual IList<TwitterPost> Posts { get; set; }

    }
}
