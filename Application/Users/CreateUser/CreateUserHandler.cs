namespace Sync.Application.Users.CreateUser;

class CreateUserHandler
{
    public IResult Handle(CreateUserCommand command)
    {
        return Results.BadRequest("No users can be created. Please login to an existing account.");
    }
}
