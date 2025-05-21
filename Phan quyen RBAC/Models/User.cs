namespace Phan_quyen_RBAC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Lưu trữ hash của mật khẩu
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
