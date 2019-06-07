using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IAssessmentOrgsImporter
    {
       AssessmentOrgsImportResponse ImportAssessmentOrganisations();
    }
}
