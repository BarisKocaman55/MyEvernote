using MyEvernote.BusinessLayer.Abstracts;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Common.Helpers;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer.Concretes
{
    public class EverNoteUserManager : ManagerBase<EvernoteUser>
    {
        //private Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            // Check Username
            // Check Email
            // Register Process
            // Activation Code

            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();

            if (user != null)
            {
                if(user.Username == data.Username)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.UsernameAlreadyExists, "Username already exists !!!");
                }

                if(user.Email == data.Email)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.EmailAlreadyExists, "Email already exists !!!");
                }
                //throw new Exception("Username or Email is already used!!!");
            }

            else
            {
                int dbResult = base.Insert(new EvernoteUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFilename = "user_boy.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false,
                    ModifiedUsername = "system"
                });

                if(dbResult > 1)
                {
                    layerResult.Result =  Find(x => x.Email == data.Email || x.Username == data.Username);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{layerResult.Result.ActivateGuid}";
                    string body = $"Hello, {layerResult.Result.Username};<br><br>Please <a href='{activateUri}' target='_blank'>click</a> to activate your account...";
                    MailHelper.SendMail(body, layerResult.Result.Email, "MyEvernote Account Activation");
                }
            }

            return layerResult; 
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username && x.Password == data.Password);
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();

            layerResult.Result = user;

            if (user != null)
            {
                if (!user.IsActive)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.UserIsNotActivate, "User is not Activated !!!");
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.CheckYourEmail, "Please check your email...");
                }
            }

            else
            {
                layerResult.AddError(Entities.Messages.ErrorMessageCode.UsernameOrPassWrong, "Username or Password is not valid !!!");
            }

            return layerResult;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();
            result.Result = Find(x => x.Id == id);

            if(result.Result == null)
            {
                result.AddError(Entities.Messages.ErrorMessageCode.UserNotFound, "User Not Found");
            }

            return result;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();
            result.Result = Find(x => x.ActivateGuid == activateId);

            if(result.Result != null)
            {
                if (result.Result.IsActive)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.UserAlreadyActive, "User already activated !!!");
                    return result;
                }

                result.Result.IsActive = true;
                Update(result.Result);
            }

            else
            {
                result.AddError(Entities.Messages.ErrorMessageCode.ActivateIdDoesNotExists, "There is no account to be activated !!!");
            }

            return result;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();

            if(db_user != null && db_user.Id != data.Id)
            {
                if(db_user.Username == data.Username)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.UsernameAlreadyExists, "Username already exists !!!");
                }

                if(db_user.Email == data.Email)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.EmailAlreadyExists, "Email alredy exists !!!");
                }

                return result;
            }

            result.Result = Find(x => x.Id == data.Id);
            result.Result.Email = data.Email;
            result.Result.Name = data.Name;
            result.Result.Surname = data.Surname;
            result.Result.Password = data.Password;
            result.Result.Username = data.Username;

            if(string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                result.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if(base.Update(result.Result) == 0)
            {
                result.AddError(Entities.Messages.ErrorMessageCode.ProfileCouldNotUpdated, "Error while updating profile !!!");
            }

            return result;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
            EvernoteUser db_user = Find(x => x.Id == id);
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();

            if(db_user != null)
            {
                if(Delete(db_user) == 0)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.UserCouldNotRemove, "User can not deleted !!!");
                    return result;
                }
            }

            else
            {
                result.AddError(Entities.Messages.ErrorMessageCode.UserCouldNotFound, "User can not found !!!");
            }

            return result;
        }

        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data) // new kullanarak method hiding yapıldı. Miras alan sınıftaki aynı adda olan Insert yerine kullanılabilir.
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();

            layerResult.Result = data;

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.UsernameAlreadyExists, "Username already exists !!!");
                }

                if (user.Email == data.Email)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.EmailAlreadyExists, "Email already exists !!!");
                }
                //throw new Exception("Username or Email is already used!!!");
            }

            else
            {
                layerResult.Result.ProfileImageFilename = "user_boy.png";
                layerResult.Result.ActivateGuid = Guid.NewGuid();

                if(base.Insert(layerResult.Result) == 0)
                {
                    layerResult.AddError(Entities.Messages.ErrorMessageCode.UserCouldNotInserted, "User can not inserted...");
                }

            }

            return layerResult;
        }
    
        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();
            result.Result = data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.UsernameAlreadyExists, "Username already exists !!!");
                }

                if (db_user.Email == data.Email)
                {
                    result.AddError(Entities.Messages.ErrorMessageCode.EmailAlreadyExists, "Email alredy exists !!!");
                }

                return result;
            }

            result.Result = Find(x => x.Id == data.Id);
            result.Result.Email = data.Email;
            result.Result.Name = data.Name;
            result.Result.Surname = data.Surname;
            result.Result.Password = data.Password;
            result.Result.Username = data.Username;
            result.Result.IsActive = data.IsActive;
            result.Result.IsAdmin = data.IsAdmin;

            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                result.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if (base.Update(result.Result) == 0)
            {
                result.AddError(Entities.Messages.ErrorMessageCode.ProfileCouldNotUpdated, "Error while updating profile !!!");
            }

            return result;
        }
    }
}
