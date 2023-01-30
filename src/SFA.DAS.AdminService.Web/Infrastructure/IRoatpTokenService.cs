using System.Threading.Tasks;
namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpTokenService
    {
        string GetToken();
    }
}