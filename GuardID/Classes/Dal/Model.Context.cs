﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Classes.Conexoes
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities2 : DbContext
    {
        public Entities2()
            : base("name=Entities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CIDADOES> CIDADOES { get; set; }
        public DbSet<LOG_ERROS> LOG_ERROS { get; set; }
        public DbSet<LOGS> LOGS { get; set; }
        public DbSet<PLATAFORMAS> PLATAFORMAS { get; set; }
        public DbSet<PROGRAMAS> PROGRAMAS { get; set; }
        public DbSet<SETORES> SETORES { get; set; }
        public DbSet<USUARIOS> USUARIOS { get; set; }
        public DbSet<USUARIOS_SETORES> USUARIOS_SETORES { get; set; }
    }
}
