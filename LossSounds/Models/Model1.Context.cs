﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LossSounds.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BD_LOSS_SOUNDSEntities : DbContext
    {
        public BD_LOSS_SOUNDSEntities()
            : base("name=BD_LOSS_SOUNDSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tb_Album> tb_Album { get; set; }
        public DbSet<tb_Artista> tb_Artista { get; set; }
        public DbSet<tb_Cancion> tb_Cancion { get; set; }
        public DbSet<tb_Comentarios> tb_Comentarios { get; set; }
        public DbSet<tb_DisLikeMusic> tb_DisLikeMusic { get; set; }
        public DbSet<tb_LikeMusic> tb_LikeMusic { get; set; }
        public DbSet<tb_Playlist> tb_Playlist { get; set; }
        public DbSet<tb_RolesPrivacidad> tb_RolesPrivacidad { get; set; }
        public DbSet<tb_Seguidos> tb_Seguidos { get; set; }
        public DbSet<tb_Usuario> tb_Usuario { get; set; }
    }
}
