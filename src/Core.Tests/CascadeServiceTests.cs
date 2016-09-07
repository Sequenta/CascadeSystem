using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Tests.Infrastructure;
using DataAccess;
using Domain;
using FluentAssertions;
using Moq;
using Ploeh.AutoFixture;
using Xunit;

namespace Core.Tests
{
    public class CascadeServiceTests
    {
        [Theory, MoqAutoData]
        public void CheckCascadeConditionSendsAssignmentNotification(Assignment assignment, Mock<IUnitOfWork> unitOfWork, Mock<IAssignmentCascadeNotifier> assignmentNotifier, CascadeService cascadeService)
        {
            SetOrderForCascadeLevels(assignment);
            assignment.Cascade.SetCurrentLevel(0);
            assignment.Cascade.Levels.First().NotificationTypes = NotificationTypes.Assignment;
            assignment.Cascade.Levels.First().HeadLevel = 0;
            var cascadeResponsible = assignment.Cascade.Levels.First().Responsible;
            unitOfWork.Setup(x => x.Assignments.FindMany(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(new[] {assignment}).Verifiable();
            assignmentNotifier.Setup(x => x.Notify(It.IsAny<string>(), cascadeResponsible)).Verifiable();

            cascadeService.CheckCascadeCondition();

            unitOfWork.Verify();
            assignmentNotifier.Verify(x => x.Notify(It.IsAny<string>(), cascadeResponsible), Times.Once());
            assignment.Cascade.CurrentLevelNumber.Should().Be(1);
        }

        [Theory, MoqAutoData]
        public void CheckCascadeConditionSendsAssignmentNotificationToHeadOfStructuralUnit(Assignment assignment, Workplace head, StructuralUnit structuralUnit, Mock<IUnitOfWork> unitOfWork, 
                                                                                           Mock<IAssignmentCascadeNotifier> assignmentNotifier, 
                                                                                           Mock<IStructuralUnitService> structuralUnitService, CascadeService cascadeService)
        {
            SetOrderForCascadeLevels(assignment);
            SetOverdueResponsible(assignment);
            structuralUnit.HeadWorkplace = head;
            assignment.Cascade.SetCurrentLevel(0);
            assignment.Cascade.Levels.First().NotificationTypes = NotificationTypes.Assignment;
            assignment.Cascade.Levels.First().HeadLevel = 1;
            unitOfWork.Setup(x => x.Assignments.FindMany(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(new[] { assignment }).Verifiable();
            structuralUnitService.Setup(x => x.GetForWorkplace(It.IsAny<Workplace>())).Returns(structuralUnit).Verifiable();
            structuralUnitService.Setup(x => x.GetHeadOfLevel(structuralUnit, It.IsAny<int>())).Returns(structuralUnit.HeadWorkplace).Verifiable();
            assignmentNotifier.Setup(x => x.Notify(It.IsAny<string>(), head)).Verifiable();

            cascadeService.CheckCascadeCondition();

            unitOfWork.Verify();
            assignmentNotifier.Verify(x => x.Notify(It.IsAny<string>(), head), Times.Once());
        }

        [Theory, MoqAutoData]
        public void CheckCascadeConditionSkipSendingWithNoOverdueAssignments(Mock<IUnitOfWork> unitOfWork, Mock<IAssignmentCascadeNotifier> assignmentNotifier, Mock<IMailCascadeNotifier> mailNotifier, Mock<ISmsCascadeNotifier> smsNotifier, CascadeService cascadeService)
        {
            unitOfWork.Setup(x => x.Assignments.FindMany(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(new List<Assignment>()).Verifiable();

            cascadeService.CheckCascadeCondition();

            unitOfWork.Verify();
            assignmentNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<Workplace>()), Times.Never());
            mailNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            smsNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Theory, MoqAutoData]
        public void CheckCascadeConditionSkipSendingNotReadyForNextLevel(Assignment assignment, Mock<IUnitOfWork> unitOfWork, Mock<IAssignmentCascadeNotifier> assignmentNotifier, Mock<IMailCascadeNotifier> mailNotifier, Mock<ISmsCascadeNotifier> smsNotifier, CascadeService cascadeService)
        {
            SetOrderForCascadeLevels(assignment);
            assignment.Cascade.SetCurrentLevel(1);
            assignment.Cascade.Levels.First(x => x.OrderNumber == 1).TriggeredAt = null;
            unitOfWork.Setup(x => x.Assignments.FindMany(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(new [] {assignment}).Verifiable();

            cascadeService.CheckCascadeCondition();

            unitOfWork.Verify();
            assignmentNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<Workplace>()), Times.Never());
            mailNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            smsNotifier.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        private static void SetOrderForCascadeLevels(Assignment assignment)
        {
            
            for (var i = 0; i < assignment.Cascade.Levels.Count; i++)
            {
                assignment.Cascade.Levels[i].OrderNumber = i + 1;
            }
        }

        private void SetOverdueResponsible(Assignment assignment)
        {
            var fixture = new Fixture();
            var workplaces = fixture.CreateMany<Workplace>(2).ToList();
            assignment.WayPoints.ForEach(x => x.Close());
            assignment.WayPoints.Add(new WayPoint(workplaces[0], workplaces[1]));
        }
    }
}