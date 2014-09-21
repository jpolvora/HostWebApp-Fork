namespace Frankstein.DbFileSystem
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class teste : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.DbFiles",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ParentId = c.Int(),
            //            IsDirectory = c.Boolean(nullable: false),
            //            VirtualPath = c.String(nullable: false, maxLength: 255),
            //            Name = c.String(maxLength: 512),
            //            Extension = c.String(maxLength: 8),
            //            Bytes = c.Binary(),
            //            Texto = c.String(),
            //            IsHidden = c.Boolean(nullable: false),
            //            IsBinary = c.Boolean(nullable: false),
            //            Created = c.DateTime(nullable: false),
            //            Modified = c.DateTime(),
            //            Modifier = c.String(maxLength: 100),
            //            Status = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.DbFiles", t => t.ParentId)
            //    .Index(t => t.ParentId)
            //    .Index(t => t.VirtualPath, unique: true);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.DbFiles", "ParentId", "dbo.DbFiles");
            //DropIndex("dbo.DbFiles", new[] { "VirtualPath" });
            //DropIndex("dbo.DbFiles", new[] { "ParentId" });
            //DropTable("dbo.DbFiles");
        }
    }
}
