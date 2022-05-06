using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AuthManagement.DbUtil.Entity
{
    public partial class AuthDbContext : DbContext
    {
        public AuthDbContext()
        {
        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TAuth> TAuths { get; set; }
        public virtual DbSet<TDept> TDepts { get; set; }
        public virtual DbSet<TLog> TLogs { get; set; }
        public virtual DbSet<TUser> TUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { 
                optionsBuilder.UseMySql("server=127.0.0.1;database=auth_manage;port=3306;user=sims;password=sims@123;character set=utf8mb4", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.6.45-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<TAuth>(entity =>
            {
                entity.HasKey(e => e.AuthId)
                    .HasName("PRIMARY");

                entity.ToTable("t_auth");

                entity.Property(e => e.AuthId)
                    .HasColumnType("int(11)")
                    .HasColumnName("auth_id")
                    .HasComment("权限编号（主键，自增）");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.FuncCode)
                    .HasMaxLength(50)
                    .HasColumnName("func_code")
                    .HasComment("功能代码");

                entity.Property(e => e.ModifyTime)
                    .HasColumnType("datetime")
                    .HasColumnName("modify_time")
                    .HasComment("修改时间");

                entity.Property(e => e.TargetId)
                    .HasColumnType("int(11)")
                    .HasColumnName("target_id")
                    .HasComment("用户/部门编号");

                entity.Property(e => e.TargetType)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("target_type")
                    .HasComment("授权类型 1:部门，2:用户");
            });

            modelBuilder.Entity<TDept>(entity =>
            {
                entity.HasKey(e => e.DeptId)
                    .HasName("PRIMARY");

                entity.ToTable("t_dept");

                entity.Property(e => e.DeptId)
                    .HasColumnType("int(11)")
                    .HasColumnName("dept_id")
                    .HasComment("部门编号（主键，自增）");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(10)
                    .HasColumnName("dept_name")
                    .HasComment("部门名称");

                entity.Property(e => e.IsValid)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("is_valid")
                    .HasComment("1:有效(默认)，0:作废");

                entity.Property(e => e.ModifyTime)
                    .HasColumnType("datetime")
                    .HasColumnName("modify_time")
                    .HasComment("修改时间");
            });

            modelBuilder.Entity<TLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PRIMARY");

                entity.ToTable("t_log");

                entity.Property(e => e.LogId)
                    .HasColumnType("int(11)")
                    .HasColumnName("log_id")
                    .HasComment("日志编号（主键，自增）");

                entity.Property(e => e.BatchNo)
                    .HasMaxLength(45)
                    .HasColumnName("batch_no")
                    .HasComment("批次号（GUID字符串）");

                entity.Property(e => e.LogTime)
                    .HasColumnType("datetime")
                    .HasColumnName("log_time")
                    .HasComment("记录时间");

                entity.Property(e => e.TableData)
                    .HasMaxLength(450)
                    .HasColumnName("table_data")
                    .HasComment("表数据");

                entity.Property(e => e.TableName)
                    .HasMaxLength(45)
                    .HasColumnName("table_name")
                    .HasComment("表名称");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id")
                    .HasComment("操作人编号");

                entity.Property(e => e.UserName)
                    .HasMaxLength(10)
                    .HasColumnName("user_name")
                    .HasComment("操作人名字");
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("t_user");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id")
                    .HasComment("用户编号（主键，自增）");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DeptId)
                    .HasColumnType("int(11)")
                    .HasColumnName("dept_id")
                    .HasComment("所属部门编号");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(10)
                    .HasColumnName("dept_name")
                    .HasComment("属部门名称");

                entity.Property(e => e.IsValid)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("is_valid")
                    .HasComment("1:有效，0:锁定");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(15)
                    .HasColumnName("mobile")
                    .HasComment("联系电话");

                entity.Property(e => e.ModifyTime)
                    .HasColumnType("datetime")
                    .HasColumnName("modify_time")
                    .HasComment("修改时间");

                entity.Property(e => e.SigninAcc)
                    .HasMaxLength(15)
                    .HasColumnName("signin_acc")
                    .HasComment("登录帐号");

                entity.Property(e => e.SigninPwd)
                    .HasMaxLength(45)
                    .HasColumnName("signin_pwd")
                    .HasComment("登录密码");

                entity.Property(e => e.UserName)
                    .HasMaxLength(10)
                    .HasColumnName("user_name")
                    .HasComment("用户名称");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
