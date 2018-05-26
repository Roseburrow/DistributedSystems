namespace SecuroteckWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        m_LogID = c.Int(nullable: false, identity: true),
                        m_LogDescription = c.String(),
                        m_LogDateTime = c.DateTime(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        User_m_ApiKey = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.m_LogID)
                .ForeignKey("dbo.Users", t => t.User_m_ApiKey)
                .Index(t => t.User_m_ApiKey);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        m_ApiKey = c.String(nullable: false, maxLength: 128),
                        m_UserName = c.String(),
                    })
                .PrimaryKey(t => t.m_ApiKey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "User_m_ApiKey", "dbo.Users");
            DropIndex("dbo.Logs", new[] { "User_m_ApiKey" });
            DropTable("dbo.Users");
            DropTable("dbo.Logs");
        }
    }
}
