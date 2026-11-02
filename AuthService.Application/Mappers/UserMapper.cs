namespace AuthService.Application.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMapper : MapperBase
{
    public partial UserOrganizationDto ToUserOrganizationDto(Organization organization);

    public partial UserAddressDto ToUserAddressDto(UserAddress address);

    private Guid ToGuid(OrganizationId id) => id.Value;
}