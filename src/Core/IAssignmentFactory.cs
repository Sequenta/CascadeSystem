using System;
using Domain;

namespace Core
{
    public interface IAssignmentFactory
    {
        Assignment Create(string title, string text, DateTime deadline);
    }
}