namespace Phan_quyen_RBAC.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ví dụ: "Product.Read", "Product.Write", "User.Manage"
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
