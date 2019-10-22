namespace TwitterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwitterPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(nullable: false),
                        Text = c.String(nullable: false, maxLength: 250),
                        CreatedDate = c.DateTime(nullable: false),
                        RetweetCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TwitterTrends",
                c => new
                    {
                        TrendName = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TrendName);
            
            CreateTable(
                "dbo.TwitterUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        UserScreenName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 500),
                        FollowersCount = c.Int(nullable: false),
                        FavouritesCount = c.Int(nullable: false),
                        FriendsCount = c.Int(nullable: false),
                        ProfileImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TwitterUsers");
            DropTable("dbo.TwitterTrends");
            DropTable("dbo.TwitterPosts");
        }
    }
}
