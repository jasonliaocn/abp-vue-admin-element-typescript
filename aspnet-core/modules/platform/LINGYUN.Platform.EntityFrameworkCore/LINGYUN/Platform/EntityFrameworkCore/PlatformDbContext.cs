﻿using LINGYUN.Platform.Datas;
using LINGYUN.Platform.Layouts;
using LINGYUN.Platform.Menus;
using LINGYUN.Platform.Packages;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace LINGYUN.Platform.EntityFrameworkCore
{
    [ConnectionStringName(PlatformDbProperties.ConnectionStringName)]
    public class PlatformDbContext : AbpDbContext<PlatformDbContext>, IPlatformDbContext
    {
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Data> Datas { get; set; }
        public DbSet<Package> Packages { get; set; }

        public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigurePlatform();

            base.OnModelCreating(builder);
        }
    }
}
