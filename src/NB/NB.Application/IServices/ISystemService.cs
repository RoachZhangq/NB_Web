namespace NB.Application.IServices;

public interface ISystemService
{
    string GetDescription();

    (string userName, string password) GetLoginInfo();
}
