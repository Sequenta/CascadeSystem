using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Domain;

namespace Core
{
    public class CascadeService : ICascadeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStructuralUnitService _structuralUnitService;
        private readonly IAssignmentCascadeNotifier _assignmentCascadeNotifier;
        private readonly IMailCascadeNotifier _emailCascadeNotifier;
        private readonly ISmsCascadeNotifier _smsCascadeNotifier;

        public CascadeService(IUnitOfWork unitOfWork, IStructuralUnitService structuralUnitService, IAssignmentCascadeNotifier assignmentCascadeNotifier, 
                                                                                                    IMailCascadeNotifier emailCascadeNotifier,
                                                                                                    ISmsCascadeNotifier smsCascadeNotifier)
        {
            _unitOfWork = unitOfWork;
            _structuralUnitService = structuralUnitService;
            _assignmentCascadeNotifier = assignmentCascadeNotifier;
            _emailCascadeNotifier = emailCascadeNotifier;
            _smsCascadeNotifier = smsCascadeNotifier;
        }

        public void CheckCascadeCondition()
        {
            var overdueAssignments = _unitOfWork.Assignments.FindMany(x => DateTime.Now > x.Deadline && x.Cascade != null && !x.Cascade.Completed).ToList();
            if (overdueAssignments.Any())
            {
                foreach (var assignment in overdueAssignments)
                {
                    var nextLevel = GetNextCascadeLevel(assignment);
                    if (nextLevel != null)
                    {
                        if (IsReadyForLevel(nextLevel, assignment.Cascade.CurrentLevel))
                        {
                            SendNotification(nextLevel, assignment);
                            nextLevel.Start();
                            assignment.Cascade.SetCurrentLevel(nextLevel.OrderNumber);
                            _unitOfWork.Assignments.UpdateOne(assignment);
                        }
                    }
                }
                _unitOfWork.Commit();
            }
        }

        private void SendNotification(CascadeLevel nextLevel, Assignment overdueAssignment)
        {
            if (nextLevel.NotificationTypes.HasFlag(NotificationTypes.None))
            {
                return;
            }
            var responsibles = GetResponsibles(nextLevel, overdueAssignment);
            foreach (NotificationTypes value in Enum.GetValues(nextLevel.NotificationTypes.GetType()))
            {
                if (nextLevel.NotificationTypes.HasFlag(value))
                {
                    var text = $"Просрочено поручение { GetAssignmentUri(overdueAssignment) }";
                    foreach (var responsible in responsibles)
                    {
                        switch (value)
                        {
                            case NotificationTypes.Assignment:
                            {
                                _assignmentCascadeNotifier.Notify(text, responsible);
                                break;
                            }
                            case NotificationTypes.Email:
                            {
                                _emailCascadeNotifier.Notify(text, responsible.Owner.Email);
                                break;
                            }
                            case NotificationTypes.Sms:
                            {
                                _smsCascadeNotifier.Notify(text, responsible.Owner.Phone);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private List<Workplace> GetResponsibles(CascadeLevel nextLevel, Assignment overdueAssignment)
        {
            var responsibles = new List<Workplace>();
            if (nextLevel.HeadLevel == 0)
            {
                responsibles.Add(nextLevel.Responsible);
            }
            else
            {
                var overdueResponsibles = overdueAssignment.WayPoints.Where(x => !x.IsClosed).Select(x => x.ToWorkplace);
                foreach (var overdueResponsible in overdueResponsibles)
                {
                    var head = _structuralUnitService.GetHeadOfLevel(_structuralUnitService.GetForWorkplace(overdueResponsible), nextLevel.HeadLevel);
                    responsibles.Add(head);
                }
            }
            return responsibles;
        }

        private CascadeLevel GetNextCascadeLevel(Assignment assignment)
        {
            var currentLevelNumber = assignment.Cascade.CurrentLevelNumber;
            var nextLevel = assignment.Cascade.Levels.FirstOrDefault(x => x.OrderNumber == currentLevelNumber + 1);
            return nextLevel;
        }

        private bool IsReadyForLevel(CascadeLevel nextLevel, CascadeLevel curentLevel)
        {
            if (curentLevel == null)
            {
                return true;
            }
            if (curentLevel.TriggeredAt != null)
            {
                return curentLevel.TriggeredAt.Value.Add(nextLevel.Delay) <= DateTime.Now;
            }
            return false;
        }

        private string GetAssignmentUri(Assignment assignment)
        {
            return $"<a href='#/assignment/{assignment.Id}'>{assignment.Title}</a>";
        }   
    }
}