namespace TwitterApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TwitterPostId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TwitterPosts", "TwitterId", c => c.Long(nullable: false));
            CreateIndex("dbo.TwitterPosts", "TwitterId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TwitterPosts", new[] { "TwitterId" });
            DropColumn("dbo.TwitterPosts", "TwitterId");
        }
    }
}
