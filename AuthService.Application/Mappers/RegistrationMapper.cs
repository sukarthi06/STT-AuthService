namespace AuthService.Application.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RegistrationMapper : MapperBase
{
    [MapValue(nameof(AppUser.UserId), Use = nameof(NewUserId))]
    public partial AppUser ToAppUser(RegistrationDto dto, string pwd);

    [MapValue(nameof(Organization.OrganizationId), Use = nameof(NewOrganizationId))]
    [MapProperty(nameof(RegistrationDto.OrganizationName), nameof(Organization.Name))]
    [MapProperty(nameof(RegistrationDto.OrganizationLicenseNo), nameof(Organization.LicenseNo))]
    public partial Organization ToOrganization(RegistrationDto dto);

    [MapperIgnoreTarget(nameof(UserDetails.UserId))]
    public partial UserDetails ToUserDetails(RegistrationDto dto);

    private UserId NewUserId() => UserId.Of(Guid.NewGuid());
    private OrganizationId NewOrganizationId() => OrganizationId.Of(Guid.NewGuid());
}
