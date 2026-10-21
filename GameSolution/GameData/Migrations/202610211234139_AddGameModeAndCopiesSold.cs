namespace GameData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGameModeAndCopiesSold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Mode", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "CopiesSold", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "CopiesSold");
            DropColumn("dbo.Games", "Mode");
        }
    }
}
