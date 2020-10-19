using System.Collections.Generic;

namespace Playground.Mvc.Models
{
    public class DiffViewModel
    {
        public int ParentSetId { get; set; }
        public List<int> ChildrenSetIds { get; set; }
        public int SelectedSetId { get; set; }
        public Dictionary<string, DiffContent> DiffContent { get; set; }
    }

    public class DiffContent
    {
        public string Type { get; set; }
        public string ParentImpactStatus { get; set; }
        public string ChildImpactStatus { get; set; }
        public int SelectedChildSetId { get; set; }
        public string HtmlTableString { get; set; }
    }

    public class StudentSet
    {
        public string Type { get; set; }
        public string StudentName { get; set; }
        public string ImpactStatus { get; set; }
        public string Comments { get; set; }
        public bool ApplyDifferentStyles { get; set; }

#pragma warning disable 659
        public override bool Equals(object obj)
#pragma warning restore 659
        {
            StudentSet other = obj as StudentSet;
            if (other == null)
            {
                return false;
            }
            else
            {
                if (other.StudentName == StudentName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return StudentName;
        }
    }
}