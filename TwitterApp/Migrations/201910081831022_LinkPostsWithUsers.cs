namespace TwitterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkPostsWithUsers : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TwitterPosts", "AuthorId");
            AddForeignKey("dbo.TwitterPosts", "AuthorId", "dbo.TwitterUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TwitterPosts", "AuthorId", "dbo.TwitterUsers");
            DropIndex("dbo.TwitterPosts", new[] { "AuthorId" });
        }
    }
}
