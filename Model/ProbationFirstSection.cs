using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class ProbationFirstSection
    {
        public int Id { get; set; }
        public string Probationno { get; set; }
        public string Employeeno { get; set; }
        public string Employeename { get; set; }
        public string Creationdate { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }
        public string Managername { get; set; }
        public string Skill { get; set; }
         

        public bool Outstanding { get; set; }
        public bool AboveAverage { get; set; }
        public bool Satisfactory { get; set; }
        public bool Marginal { get; set; }
        public bool Unsatisfactory { get; set; }
        public string PerformanceComment { get; set; }

        public bool ExcellentAttendance { get; set; }
        public bool OccasionalAbsence { get; set; }
        public bool RepeatedAbsence { get; set; }
        public bool UnjustifiedAbsence { get; set; }
        public string AttendanceComment { get; set; }

        public bool AlwaysInterested { get; set; }
        public bool ReasonablyDevoted { get; set; }
        public bool PassiveAttitude { get; set; }
        public bool ActiveDislikeofWork { get; set; }
        public string AttitudeComment { get; set; }

        public bool AlwaysNeat { get; set; }
        public bool GenerallyNeat { get; set; }
        public bool SometimesCareles { get; set; }
        public bool AttirenotSuitable { get; set; }
        public string AppearanceComment { get; set; }


        public bool SelfStarter { get; set; }
        public bool NeedsStimilus { get; set; }
        public bool NeedsCSupervision { get; set; }
        public bool ShowNoInitiative { get; set; }
        public string InitiativeComment { get; set; }

        public bool AlwayOnTime { get; set; }
        public bool OccasionallyLate { get; set; }
        public bool RepeatedLate { get; set; }
        public bool RarelyOnTime { get; set; }
        public string DependabilityComment { get; set; }

        public bool DecisionLogical { get; set; }
        public bool GenSoundJudgment { get; set; }
        public bool ReqFreqCorrection { get; set; }
        public bool JudgmentOftenFaulty { get; set; }
        public string JudmentComment { get; set; }

        public bool RarelyMakesErrs { get; set; }
        public bool FewErrThanMost { get; set; }
        public bool AvgAccuracy { get; set; }
        public bool UnacceptablyErratic { get; set; }
        public string AttentionToDetailComment { get; set; }

        public bool FriendlyOutgoing { get; set; }
        public bool SomewhatBusinesslike { get; set; }
        public bool GregariousToPoint { get; set; }
        public bool SullenAndWithdrawn { get; set; }
        public string InterpersonalComment { get; set; }

        public bool AlwayscourteousTactful { get; set; }
        public bool GenCourteous { get; set; }
        public bool SometimesIncosiderate { get; set; }
        public bool ArouseAntagonism { get; set; }
        public string MannersComment { get; set; }

        public bool SeeksAddResponsibility { get; set; }
        public bool WillinglyAcceptResp { get; set; }
        public bool AssumesWhenUnavoidable { get; set; }
        public bool AlwaysAvoidResponsibility { get; set; }
        public string ResponsiblityComment { get; set; }

        public bool GraspImmediately { get; set; }
        public bool QuickerThanAvg { get; set; }
        public bool AvgLearning { get; set; }
        public bool SlowLearner { get; set; }
        public bool UnableToGraspNew { get; set; }
        public string LearningCampacityComment { get; set; }

        public bool ExcepHighProductivity { get; set; }
        public bool CompleteMoreThanAvg { get; set; }
        public bool AdequatePerHr { get; set; }
        public bool InadequateOutput { get; set; }
        public string OutputComment { get; set; }

        public bool AssumesLeadershipInit { get; set; }
        public bool WillLeadEncouraged { get; set; }
        public bool CanLeadifNecessary { get; set; }
        public bool RefusesLeadership { get; set; }
        public bool AttemptbutInefficient { get; set; }
        public string LeadershipComment { get; set; }

        public bool NeverFalter { get; set; }
        public bool MaintainPoise { get; set; }
        public bool DependableExcUnderPress { get; set; }
        public bool CantTakePressure { get; set; }
        public string PressureComment { get; set; }


        public string HRcomment { get; set; }
        public string MDcomment { get; set; }
        public string HODComment { get; set; }


        public string empStrongestpt { get; set; }
        public string empWeakestPt { get; set; }
        public string qualifiedPromo { get; set; }
        public string promoPstn { get; set; }
        public string promotable { get; set; }
        public string effectiveWithDifferent { get; set; }
        public string differentAssingment { get; set; }
        public string recommendationSectionComment { get; set; }
        public string empRecConfirm { get; set; }
        public string empRecExtProb { get; set; }
        public string empRecTerminate { get; set; }

        public string Jobtitle { get; set; }
        public string Branch { get; set; }
        public string Product { get; set; }
        public string Employmentyear { get; set; }
        public string Tenureofservice { get; set; }
        public string Contractstart { get; set; }
        public string Contractexpiry { get; set; }
        public string Probationstart { get; set; }
        public string Probationexpiry { get; set; }
        public string Supervisionduration { get; set; }

    }
}
