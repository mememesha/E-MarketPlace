using EM.Contracts;

namespace EM.Dto
{
    public class UserWithRoleDto
    {
        public Guid Id { get; set; }
        public OrganizationShortDto? OrganizationShortDto { get; set; }
        public UserAbility Role { get; set; }
    }
}