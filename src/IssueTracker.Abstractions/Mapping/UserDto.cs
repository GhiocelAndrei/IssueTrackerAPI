namespace IssueTracker.Abstractions.Mapping
{
    public class UserDto
    {
        public long Id { get; set; }
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
    public class UserLoginDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class LoginUserCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

}
