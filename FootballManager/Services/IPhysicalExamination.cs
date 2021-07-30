﻿namespace FootballManager.Services
{
    public interface IPhysicalExamination
    {
        bool IsHealthy(int age, int strength, int speed);
        void IsHealthy(int age, int strength, int speed, out bool isHealthy);
    }
}