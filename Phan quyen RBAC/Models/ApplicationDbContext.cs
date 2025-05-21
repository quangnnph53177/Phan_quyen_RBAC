using Microsoft.EntityFrameworkCore;

namespace Phan_quyen_RBAC.Models
{
    public class ApplicationDbContext : DbContext
    {
        // Khai báo biến static readonly để lưu trữ hash mật khẩu
        private static readonly string AdminPasswordHash = BCrypt.Net.BCrypt.HashPassword("password");
        private static readonly string EditorPasswordHash = BCrypt.Net.BCrypt.HashPassword("password");
        private static readonly string ViewerPasswordHash = BCrypt.Net.BCrypt.HashPassword("password");


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính và mối quan hệ cho UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Cấu hình khóa chính và mối quan hệ cho RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // --- Seed Data (Dữ liệu mẫu) ---
            // Permissions
            var readProductsPerm = new Permission { Id = 1, Name = "Product.Read" };
            var writeProductsPerm = new Permission { Id = 2, Name = "Product.Write" };
            var manageUsersPerm = new Permission { Id = 3, Name = "User.Manage" };

            modelBuilder.Entity<Permission>().HasData(
                readProductsPerm,
                writeProductsPerm,
                manageUsersPerm
            );

            // Roles
            var adminRole = new Role { Id = 1, Name = "Admin" };
            var editorRole = new Role { Id = 2, Name = "Editor" };
            var viewerRole = new Role { Id = 3, Name = "Viewer" };

            modelBuilder.Entity<Role>().HasData(
                adminRole,
                editorRole,
                viewerRole
            );

            // RolePermissions (Gán quyền cho vai trò)
            modelBuilder.Entity<RolePermission>().HasData(
                // Admin có tất cả quyền
                new RolePermission { RoleId = adminRole.Id, PermissionId = readProductsPerm.Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = writeProductsPerm.Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = manageUsersPerm.Id },

                // Editor có quyền đọc và ghi sản phẩm
                new RolePermission { RoleId = editorRole.Id, PermissionId = readProductsPerm.Id },
                new RolePermission { RoleId = editorRole.Id, PermissionId = writeProductsPerm.Id },

                // Viewer chỉ có quyền đọc sản phẩm
                new RolePermission { RoleId = viewerRole.Id, PermissionId = readProductsPerm.Id }
            );

            // Users (Sử dụng các biến hash đã tạo sẵn)
            var adminUser = new User { Id = 1, Username = "admin", PasswordHash = AdminPasswordHash };
            var editorUser = new User { Id = 2, Username = "editor", PasswordHash = EditorPasswordHash };
            var viewerUser = new User { Id = 3, Username = "viewer", PasswordHash = ViewerPasswordHash };

            modelBuilder.Entity<User>().HasData(
                adminUser,
                editorUser,
                viewerUser
            );

            // UserRoles (Gán vai trò cho người dùng)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id },
                new UserRole { UserId = editorUser.Id, RoleId = editorRole.Id },
                new UserRole { UserId = viewerUser.Id, RoleId = viewerRole.Id }
            );
        }
    }
}
