namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProgrammingLanguage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgrammingLanguage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Friend", "FavoriteLangugeId", c => c.Int());
            AddColumn("dbo.Friend", "FavoriteLanguage_Id", c => c.Int());
            CreateIndex("dbo.Friend", "FavoriteLanguage_Id");
            AddForeignKey("dbo.Friend", "FavoriteLanguage_Id", "dbo.ProgrammingLanguage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friend", "FavoriteLanguage_Id", "dbo.ProgrammingLanguage");
            DropIndex("dbo.Friend", new[] { "FavoriteLanguage_Id" });
            DropColumn("dbo.Friend", "FavoriteLanguage_Id");
            DropColumn("dbo.Friend", "FavoriteLangugeId");
            DropTable("dbo.ProgrammingLanguage");
        }
    }
}
