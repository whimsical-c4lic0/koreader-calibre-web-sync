namespace Sync.Application.Users.CreateUser;

record CreateUserCommand(
    string Username,
    string Password
);
