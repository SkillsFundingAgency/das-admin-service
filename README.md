# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  ESFA Admin Service
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-admin-service/blob/master/LICENSE)

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Admin Service Web|
| Info | A service which allows EFSA staff members to assess an End Point Assessment Organisation (EPAO) or Approved Training Provider (ATP)  and add then to the register (RoEPAO & RoATP). |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-admin-service?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1644&branchName=master) |
| Web  | https://localhost:44347/ |

See [Support Site](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1686241410/Admin+Service+-+Developer+Overview) for EFSA developer details.

### Developer Setup

#### Requirements

- Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
- Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
- Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on at least v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/) 
- Administrator Access

#### Setup

- Clone this repository
- Open Visual Studio as an administrator

##### Publish Database
The Admin service does not have a database it accesses both the RoEPAO and RoATP databases via their internal APIs.

##### Config

- Get the das-admin-service configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-admin-service/SFA.DAS.AdminService.json); which is a non-public repository.
- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.AdminService_1.0, Data: {{The contents of the local config json file}}.

##### To run a local copy you will also require 
To use EAPO or RoATP admin functionality; you will need to have the SFA.DAS.AssessorService.Application.Api and SFA.DAS.RoATPService.Application.Api projects running, from the das-assessor-service and das-roatp-service projects respectively.

- [Assessor Service](https://github.com/SkillsFundingAgency/das-assessor-service)
- [RoATP Service](https://github.com/SkillsFundingAgency/das-roatp-service)

To access the assessment functionality for training providers, organisations or standards:

- [QnA API](https://github.com/SkillsFundingAgency/das-qna-api)

#### Run the solution

- Set SFA.DAS.AdminService.Web as the startup project
- Running the solution will launch the site and API in your browser
- JSON configuration was created to work with dotnet run

-or-

- Navigate to src/SFA.DAS.AdminService.Web/
- run `dotnet restore`
- run `dotnet run`
- Open https://localhost:44347

### Getting Started

- Either follow the RoEPAO [Walkthrough](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1533345867/EPAO+-+Walkthrough); which is a non-public Wiki.
- Or follow the RoATP Walkthough (coming soon)

