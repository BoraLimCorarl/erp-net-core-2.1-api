using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using CorarlERP.Authorization.Users;
using CorarlERP.MultiTenancy;

namespace CorarlERP.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}