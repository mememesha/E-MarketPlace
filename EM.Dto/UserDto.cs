namespace EM.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public List<UserWithRoleDto>? UserWithRoleDtos { get; set; }
    }
}