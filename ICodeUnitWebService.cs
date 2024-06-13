using AdminAccount;
using ApprovalMGT;
using DOCMgt;
using HRActivity;
using JobRequisition;
using Mailer;
using PayrollMGT;

namespace RPFBE
{
    public interface ICodeUnitWebService
    {
        JWS_PortClient Client();
        EmployeeAccountWebService_PortClient EmployeeAccount();
        Notifications_PortClient WSMailer();
        HRManagementWS_PortClient HRWS();
        DocumentMgmt_PortClient DOCMGT();
        PortalApprovalManager_PortClient ApprovalMGT();
        PayrollManagementWebService_PortClient PayrollMGT();
    }
}