using System;
using FootballManager.Physical;

namespace FootballManager.Services
{
    public class PhysicalExamination : IPhysicalExamination
    {
        public bool IsHealthy(int age, int strength, int speed)
        {
            throw new System.NotImplementedException();
        }

        public void IsHealthy(int age, int strength, int speed, out bool isHealthy)
        {
            throw new System.NotImplementedException();
        }

        public IMedicalRoom MedicalRoom
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public PhysicalGrade PhysicalGrade
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }
    }
}