using System;
using FootballManager.Physical;
using FootballManager.Services;
using FootballManager.Transfer;
using Moq;
using Moq.Protected;
using Xunit;
using Range = Moq.Range;

namespace FootballManager.UnitTests
{
    public class TransferApprovalShould
    {
        [Fact]
        public void ApproveYoungCheapPlayerTransfer()
        {
            var mockExamination = new Mock<IPhysicalExamination>()
            {
                DefaultValue = DefaultValue.Mock
            };
            var isHealthy = true;
            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable);
            mockExamination.Setup(x => x.IsHealthy(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), out isHealthy));

            var approve = new TransferApproval(mockExamination.Object,null);

            var youngPlayer = new TransferApplication
            {
                PlayerName = "young player",
                PlayAge = 22,
                TransferFee = 0,
                AnnualSalary = 10m,
                ContractYears = 4,
                IsSupperStar = false,
                PlayerSpeed = 80,
                PlayerStrength = 80
            };

            var result = approve.Evaluate(youngPlayer);
            Assert.Equal(TransferResult.Approved, result);
        }

        [Fact]
        public void OldPlayerNonSuperStarTransfer()
        {
            var mockExamination = new Mock<IPhysicalExamination>()
            {
                DefaultValue = DefaultValue.Mock
            };
            var isHealthy = true;
            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable);
            mockExamination.Setup(x => x.IsHealthy(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), out isHealthy));

            var approve = new TransferApproval(mockExamination.Object, null);

            var oldPlayer = new TransferApplication
            {
                PlayerName = "player",
                PlayAge = 31,
                TransferFee = 30,
                AnnualSalary = 10m,
                ContractYears = 4,
                IsSupperStar = false,
                PlayerSpeed = 80,
                PlayerStrength = 80
            };

            var result = approve.Evaluate(oldPlayer);
            Assert.Equal(TransferResult.Rejected, result);
        }

        [Fact]
        public void PlayerNeedSpeedTransfer()
        {
            var mockExamination = new Mock<IPhysicalExamination>()
            {
                DefaultValue = DefaultValue.Mock
            };
            var isHealthy = true;
            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable);
            mockExamination.Setup(x => 
                x.IsHealthy(It.IsAny<int>(), It.IsAny<int>(), It.IsInRange(90,100,Range.Inclusive) ,out isHealthy));

            var approve = new TransferApproval(mockExamination.Object,null);

            var speedPlayer = new TransferApplication
            {
                PlayerName = "player",
                PlayAge = 25,
                TransferFee = 30,
                AnnualSalary = 10m,
                ContractYears = 4,
                IsSupperStar = false,
                PlayerSpeed = 90,
                PlayerStrength = 80
            };

            var result = approve.Evaluate(speedPlayer);
            Assert.Equal(TransferResult.Approved, result);
        }

        [Fact]
        public void PhysicalGradeShouldPassWhenTransferIsSuperStar()
        {
            var mockExamination = new Mock<IPhysicalExamination>
            {
                //为嵌套的属性设置默认值 DefaultValue.Mock只对接口, 抽象类和非sealed的class起作用
                DefaultValue = DefaultValue.Mock
            };
            var isHealthy = true;
            mockExamination.SetupProperty(x => x.PhysicalGrade);
            mockExamination.Setup(x => x.IsHealthy(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), out isHealthy));

            var approval = new TransferApproval(mockExamination.Object, null);

            var player = new TransferApplication()
            {
                PlayerName = "super Star",
                PlayAge = 34,
                TransferFee = 112m,
                AnnualSalary = 30m,
                ContractYears = 4,
                IsSupperStar = true,
                PlayerSpeed = 90,
                PlayerStrength = 90
            };

            var result = approval.Evaluate(player);

            Assert.Equal(PhysicalGrade.Passed,mockExamination.Object.PhysicalGrade);
        }

        [Fact]
        public void ShouldPhysicalExamineWhenTransferringSuperStar()
        {
            var mockExamination = new Mock<IPhysicalExamination>();
            
            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable).Returns("Available");
            mockExamination.SetupProperty(x => x.PhysicalGrade, PhysicalGrade.Passed);

            var isHealthy = true;
            mockExamination.Setup(x => x.IsHealthy(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), out isHealthy));

            var approval = new TransferApproval(mockExamination.Object, null);

            var player = new TransferApplication()
            {
                PlayerName = "super Star",
                PlayAge = 34,
                TransferFee = 112m,
                AnnualSalary = 30m,
                ContractYears = 4,
                IsSupperStar = true,
                PlayerSpeed = 90,
                PlayerStrength = 90
            };

            var result = approval.Evaluate(player);

            //mockExamination.Verify(x=>x.IsHealthy(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<int>(),out isHealthy));
            mockExamination.Verify(x => x.IsHealthy(34, 90, 90, out isHealthy), "Argument Wrong");
        }

        [Fact]
        public void PostponeWhenTransferringChildPlayer()
        {
            var mockExamination = new Mock<IPhysicalExamination>();

            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable).Returns("Available");
            mockExamination.SetupProperty(x => x.PhysicalGrade, PhysicalGrade.Passed);

            //mockExamination.Setup(x => x.IsHealthy(It.Is<int>(age => age < 16), It.IsAny<int>(), It.IsAny<int>()))
            //    .Throws<Exception>();
            mockExamination.Setup(x => x.IsHealthy(It.Is<int>(age => age < 16), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("The player is still a child"));

            var approval = new TransferApproval(mockExamination.Object, null);

            var childTransfer = new TransferApplication
            {
                PlayerName = "Some Child Player",
                PlayAge = 13,
                TransferFee = 0,
                AnnualSalary = 1,
                ContractYears = 3,
                IsSupperStar = false,
                PlayerSpeed = 40,
                PlayerStrength = 50
            };

            var result = approval.Evaluate(childTransfer);

            Assert.Equal(TransferResult.Postponed, result);

        }

        [Fact]
        public void ShouldSetPhysicalGradeWhenTransferringSuperStar_Sequence()
        {
            var mockExamination = new Mock<IPhysicalExamination>();

            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable).Returns("Available");
            mockExamination.SetupProperty(x => x.PhysicalGrade, PhysicalGrade.Passed);


            //Sequence Returns
            mockExamination
                .SetupSequence(x => x.IsHealthy(It.Is<int>(age => age < 16), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true)
                .Returns(false);
            
            var approval = new TransferApproval(mockExamination.Object, null);

            var transfer = new TransferApplication
            {
                PlayerName = "Some Child Player",
                PlayAge = 33,
                TransferFee = 122m,
                AnnualSalary = 30m,
                ContractYears = 3,
                IsSupperStar = true,
                PlayerSpeed = 90,
                PlayerStrength = 90
            };

            var result = approval.Evaluate(transfer);
            Assert.Equal(TransferResult.ReferredToBoss, result);

            var resultTow = approval.Evaluate(transfer);
            Assert.Equal(TransferResult.Rejected, resultTow);

        }

        [Fact]
        public void ShouldPostponeWhenNotInTransferPeriod()
        {
            var mockExamination = new Mock<IPhysicalExamination>();

            mockExamination.Setup(x => x.MedicalRoom.Status.IsAvailable).Returns("Available");
            mockExamination.SetupProperty(x => x.PhysicalGrade, PhysicalGrade.Passed);

            mockExamination
                .Setup(x => x.IsHealthy(It.Is<int>(age => age < 16), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);

            var mockTransferPolicy = new Mock<TransferPolicyEvaluator>();
            mockTransferPolicy.Setup(x => x.IsInTransferPeriod()).Returns(false);
            mockTransferPolicy.Protected().Setup<bool>("IsBannedFromTransferring").Returns(true);

            var approval = new TransferApproval(mockExamination.Object, mockTransferPolicy.Object);
            var transfer = new TransferApplication
            {
                PlayerName = "Some Child Player",
                PlayAge = 33,
                TransferFee = 122m,
                AnnualSalary = 30m,
                ContractYears = 3,
                IsSupperStar = true,
                PlayerSpeed = 90,
                PlayerStrength = 90
            };

            var result = approval.Evaluate(transfer);
            Assert.Equal(TransferResult.Postponed, result);
        }

    }
}