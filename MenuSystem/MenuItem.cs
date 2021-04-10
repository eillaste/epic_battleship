using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public virtual string Label { get; set; }
        public virtual string UserChoice { get; set; }
        
        public virtual Func<string> MethodToExecute { get; set; }
        
        public MenuItem(string label, string userChoice, Func<string> methodToExecute)
        {
            if (label.Length < 1 || label.Length > 99)
            {
                throw new ArgumentException(
                    $"Please check that the label for {userChoice} is between one and 100 character in length");
            }
  
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodToExecute = methodToExecute;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }
    }
}