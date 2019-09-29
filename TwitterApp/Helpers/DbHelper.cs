using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.Helpers
{
    static class DbHelper
    {
        const string connectionString = @"Data Source=DELL-VIC\SQL2017EXP;Initial Catalog=TwitterApp;Integrated Security=True";

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static void CleanupContent(this SqlConnection connection)
        {
            SqlCommand command = CreateCommand(connection);
            command.CommandText = "DELETE FROM[dbo].[Users]";
            command.ExecuteNonQuery();
        }

        public static int InsertHashTag(this SqlConnection connection, string tag)
        {
            SqlCommand command = CreateCommand(connection);
            command.CommandText =
                "INSERT INTO[dbo].[HashTags] ([Tag]) VALUES (@tagValue)" +
                "SELECT CONVERT(Int,SCOPE_IDENTITY()) AS [value]";
            command.Parameters.AddWithValue("@tagValue", tag);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static int? FindUser(this SqlConnection connection, string userName)
        {
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = @"SELECT [Id] FROM [Users] WHERE [UserName]=@p0";
            command.Parameters.AddWithValue("@p0", userName);
            var result = command.ExecuteScalar();
            if (result == null)
                return null;
            else
                return Convert.ToInt32(result);
        }

        public static int InsertUser(this SqlConnection connection,
            string userName,
            string userScreenName,
            int favouritesCount,
            int followersCount,
            int friendsCount,
            string description,
            string profileImageUrl)
        {
            SqlCommand command = CreateCommand(connection);
            command.CommandText =
                "INSERT INTO[dbo].[Users] ([UserName],[UserScreenName],[FavouritesCount],[FollowersCount],[FriendsCount],[Description],[ProfileImageUrl]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)" +
                "SELECT CONVERT(Int,SCOPE_IDENTITY()) AS [value]";
            command.Parameters.AddWithValue("@p0", userName);
            command.Parameters.AddWithValue("@p1", userScreenName);
            command.Parameters.AddWithValue("@p2", favouritesCount);
            command.Parameters.AddWithValue("@p3", followersCount);
            command.Parameters.AddWithValue("@p4", friendsCount);
            command.Parameters.AddWithValue("@p5", description);
            command.Parameters.AddWithValue("@p6", profileImageUrl);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static int InsertPost(this SqlConnection connection,
            int authorId,
            string text,
            DateTime createdDate,
            int retweetCount)
        {
            SqlCommand command = CreateCommand(connection);
            command.CommandText =
                "INSERT INTO [dbo].[Posts] ([AuthorId],[Text],[CreatedDate],[RetweetCount]) VALUES (@p0,@p1,@p2,@p3)" +
                "SELECT CONVERT(Int,SCOPE_IDENTITY()) AS [value]";
            command.Parameters.AddWithValue("@p0", authorId);
            command.Parameters.AddWithValue("@p1", text);
            command.Parameters.AddWithValue("@p2", createdDate);
            command.Parameters.AddWithValue("@p3", retweetCount);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private static SqlCommand CreateCommand(SqlConnection connection)
        {
            var command = new SqlCommand();
            command.Connection = connection;
            return command;
        }
    }
}
