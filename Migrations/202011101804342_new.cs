namespace LA_VENDUTA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Anuncios",
                c => new
                    {
                        AnuncioId = c.Int(nullable: false, identity: true),
                        ProductoId = c.Int(nullable: false),
                        VendedorId = c.Int(nullable: false),
                        Precio = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        Calificacion = c.Int(nullable: false),
                        Etiqueta = c.String(),
                        Descripcion = c.String(),
                        SoloParaSubasta = c.Boolean(nullable: false),
                        NuevoPrecio = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnuncioId)
                .ForeignKey("dbo.Productoes", t => t.ProductoId, cascadeDelete: true)
                .ForeignKey("dbo.Vendedors", t => t.VendedorId, cascadeDelete: true)
                .Index(t => t.ProductoId)
                .Index(t => t.VendedorId);
            
            CreateTable(
                "dbo.Carritoes",
                c => new
                    {
                        CarritoId = c.Int(nullable: false, identity: true),
                        CompradorId = c.Int(nullable: false),
                        AnuncioId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        FechaSeparado = c.DateTime(nullable: false),
                        TarjetaDeCreditoId = c.Double(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        NumDesuTarjeta_NumDeTarjeta = c.Double(),
                        Producto_ProductoId = c.Int(),
                    })
                .PrimaryKey(t => t.CarritoId)
                .ForeignKey("dbo.Anuncios", t => t.AnuncioId, cascadeDelete: true)
                .ForeignKey("dbo.Compradors", t => t.CompradorId, cascadeDelete: true)
                .ForeignKey("dbo.TarjetaDeCreditoes", t => t.NumDesuTarjeta_NumDeTarjeta)
                .ForeignKey("dbo.Productoes", t => t.Producto_ProductoId)
                .Index(t => t.CompradorId)
                .Index(t => t.AnuncioId)
                .Index(t => t.NumDesuTarjeta_NumDeTarjeta)
                .Index(t => t.Producto_ProductoId);
            
            CreateTable(
                "dbo.Compradors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Calificacion = c.Int(nullable: false),
                        User = c.String(nullable: false),
                        Mensaje = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subastas",
                c => new
                    {
                        SubastaId = c.Int(nullable: false, identity: true),
                        AnuncioId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        PujaInicial = c.Int(nullable: false),
                        Duracion = c.Int(nullable: false),
                        PujaActual = c.Int(nullable: false),
                        Comprador_Id = c.Int(),
                        UsuarioConTarjeta_id = c.Int(),
                    })
                .PrimaryKey(t => t.SubastaId)
                .ForeignKey("dbo.Anuncios", t => t.AnuncioId, cascadeDelete: true)
                .ForeignKey("dbo.Compradors", t => t.Comprador_Id)
                .ForeignKey("dbo.UsuarioConTarjetas", t => t.UsuarioConTarjeta_id)
                .Index(t => t.AnuncioId)
                .Index(t => t.Comprador_Id)
                .Index(t => t.UsuarioConTarjeta_id);
            
            CreateTable(
                "dbo.UsuarioConTarjetas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CompradorId = c.Int(nullable: false),
                        TarjetadeCredito_NumDeTarjeta = c.Double(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Compradors", t => t.CompradorId, cascadeDelete: true)
                .ForeignKey("dbo.TarjetaDeCreditoes", t => t.TarjetadeCredito_NumDeTarjeta)
                .Index(t => t.CompradorId)
                .Index(t => t.TarjetadeCredito_NumDeTarjeta);
            
            CreateTable(
                "dbo.TarjetaDeCreditoes",
                c => new
                    {
                        NumDeTarjeta = c.Double(nullable: false),
                        CreditoContable = c.Double(nullable: false),
                        CreditoDisponible = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.NumDeTarjeta);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                        AnuncioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileId)
                .ForeignKey("dbo.Anuncios", t => t.AnuncioId, cascadeDelete: true)
                .Index(t => t.AnuncioId);
            
            CreateTable(
                "dbo.Productoes",
                c => new
                    {
                        ProductoId = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Tipo = c.Int(nullable: false),
                        Especificacion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductoId);
            
            CreateTable(
                "dbo.Vendedors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TarjetadeCreditoId = c.Double(nullable: false),
                        Nombre = c.String(nullable: false),
                        Calificacion = c.Int(nullable: false),
                        User = c.String(nullable: false),
                        Mensaje = c.String(),
                        TarjetaDeCredito_NumDeTarjeta = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TarjetaDeCreditoes", t => t.TarjetaDeCredito_NumDeTarjeta)
                .Index(t => t.TarjetaDeCredito_NumDeTarjeta);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        roles = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Anuncios", "VendedorId", "dbo.Vendedors");
            DropForeignKey("dbo.Vendedors", "TarjetaDeCredito_NumDeTarjeta", "dbo.TarjetaDeCreditoes");
            DropForeignKey("dbo.Anuncios", "ProductoId", "dbo.Productoes");
            DropForeignKey("dbo.Carritoes", "Producto_ProductoId", "dbo.Productoes");
            DropForeignKey("dbo.Files", "AnuncioId", "dbo.Anuncios");
            DropForeignKey("dbo.Carritoes", "NumDesuTarjeta_NumDeTarjeta", "dbo.TarjetaDeCreditoes");
            DropForeignKey("dbo.Carritoes", "CompradorId", "dbo.Compradors");
            DropForeignKey("dbo.Subastas", "UsuarioConTarjeta_id", "dbo.UsuarioConTarjetas");
            DropForeignKey("dbo.UsuarioConTarjetas", "TarjetadeCredito_NumDeTarjeta", "dbo.TarjetaDeCreditoes");
            DropForeignKey("dbo.UsuarioConTarjetas", "CompradorId", "dbo.Compradors");
            DropForeignKey("dbo.Subastas", "Comprador_Id", "dbo.Compradors");
            DropForeignKey("dbo.Subastas", "AnuncioId", "dbo.Anuncios");
            DropForeignKey("dbo.Carritoes", "AnuncioId", "dbo.Anuncios");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Vendedors", new[] { "TarjetaDeCredito_NumDeTarjeta" });
            DropIndex("dbo.Files", new[] { "AnuncioId" });
            DropIndex("dbo.UsuarioConTarjetas", new[] { "TarjetadeCredito_NumDeTarjeta" });
            DropIndex("dbo.UsuarioConTarjetas", new[] { "CompradorId" });
            DropIndex("dbo.Subastas", new[] { "UsuarioConTarjeta_id" });
            DropIndex("dbo.Subastas", new[] { "Comprador_Id" });
            DropIndex("dbo.Subastas", new[] { "AnuncioId" });
            DropIndex("dbo.Carritoes", new[] { "Producto_ProductoId" });
            DropIndex("dbo.Carritoes", new[] { "NumDesuTarjeta_NumDeTarjeta" });
            DropIndex("dbo.Carritoes", new[] { "AnuncioId" });
            DropIndex("dbo.Carritoes", new[] { "CompradorId" });
            DropIndex("dbo.Anuncios", new[] { "VendedorId" });
            DropIndex("dbo.Anuncios", new[] { "ProductoId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Vendedors");
            DropTable("dbo.Productoes");
            DropTable("dbo.Files");
            DropTable("dbo.TarjetaDeCreditoes");
            DropTable("dbo.UsuarioConTarjetas");
            DropTable("dbo.Subastas");
            DropTable("dbo.Compradors");
            DropTable("dbo.Carritoes");
            DropTable("dbo.Anuncios");
        }
    }
}
