using System;
using DataAccess;
using Domain;

namespace Core
{
    public class AssignmentCascadeNotifier : IAssignmentCascadeNotifier
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssignmentFactory _assignmentFactory;

        public AssignmentCascadeNotifier(IUnitOfWork unitOfWork, IAssignmentFactory assignmentFactory)
        {
            _unitOfWork = unitOfWork;
            _assignmentFactory = assignmentFactory;
        }

        public void Notify(string text, Workplace responsible)
        {
            var assignment = _assignmentFactory.Create("Просрочено поручение", text, DateTime.Now);
            assignment.SendToResponsibles(_unitOfWork.Workplaces.FindOne(x => x.Name == "System"), new[] {responsible});
            _unitOfWork.Assignments.AddOne(assignment);
            _unitOfWork.Commit();
        }
    }
}