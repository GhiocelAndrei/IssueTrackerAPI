namespace IssueTracker.Abstractions.Mapping
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UserCreatingDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
    public class CreateUserCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UserUpdatingDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UpdateUserCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

}
