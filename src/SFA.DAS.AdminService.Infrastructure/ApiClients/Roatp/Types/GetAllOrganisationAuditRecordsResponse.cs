using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

public class GetAllOrganisationAuditRecordsResponse
{
    public IEnumerable<OrganisationAuditRecord> AuditRecords { get; set; } = [];
}

public class OrganisationAuditRecord
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string FieldChanged { get; set; }
    public string PreviousValue { get; set; }
    public string NewValue { get; set; }
    public DateTime? PreviousStatusDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}
