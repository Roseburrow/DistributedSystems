using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SecuroteckWebApplication.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key
        [Key]
        public string m_ApiKey { get; set; }
        public string m_UserName { get; set; }
        public virtual ICollection<Log> Logs { get; set; }

        public User()
        {
        }
        #endregion
    }

    #region Task11?
    // TODO: You may find it useful to add code here for Log
    public class Log
    {
        [Key]
        public int m_LogID { get; set; }
        public string m_LogDescription { get; set; }
        public DateTime m_LogDateTime { get; set; }

        public Log(string description)
        {
            m_LogDescription = description;
            m_LogDateTime = DateTime.Now;
        }

        public Log() { }
    }

    public class ArchiveLog : Log
    {
        public ArchiveLog() { }

        public ArchiveLog(Log newLog)
        {
            m_LogDescription = newLog.m_LogDescription;
            m_LogDateTime = newLog.m_LogDateTime;
        }

    }
    #endregion

    public class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        public string CreateNewUser(string p_Username)
        {
            using (var context = new UserContext())
            {
                Guid newKey = Guid.NewGuid();
                User newUser = new User()
                {
                    m_ApiKey = newKey.ToString(),
                    m_UserName = p_Username
                };

                context.Users.Add(newUser);
                context.SaveChanges();
                return newKey.ToString();
            }
        }

        public bool CheckUserExists(string p_ApiKey)
        {
            using (var context = new UserContext())
            {
                var existingUser = context.Users.Find(p_ApiKey);
                if (existingUser != null)
                    return true;
            }
            return false;
        }

        public bool CheckUserExists(string p_ApiKey, string p_Username)
        {
            using (var context = new UserContext())
            {
                var existingUser = context.Users.Find(p_ApiKey);

                if (existingUser != null && existingUser.m_UserName == p_Username)
                {
                    return true;
                }
            }
            return false;
        }

        public User CheckandGetUserExists(string p_ApiKey)
        {
            using (var context = new UserContext())
            {
                var existingUser = context.Users.Find(p_ApiKey);
                if (existingUser != null)
                    return existingUser;
            }
            return null;
        }

        public bool DeleteUser(string p_ApiKey)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.Find(p_ApiKey);

                if (user != null)
                {
                    List<Log> logsToAdd = new List<Log>();
                    for (int i = 0; i < user.Logs.Count; i++)
                    {
                        var l = user.Logs.ElementAt(i);
                        context.LogArchive.Add(new ArchiveLog(l));
                        logsToAdd.Add(l);
                    }

                    foreach (Log log in logsToAdd)
                    {
                        context.Logs.Remove(log);
                    }
                    context.Users.Remove(user);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool CheckUsernameExists(string username)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.Where(u => u.m_UserName == username)
                                        .FirstOrDefault();

                if (user != null)
                    return true;
            }
            return false;
        }
        #endregion

        #region Task11 Loggin'
        public void CreateNewLogEntry(string apiKey, string logDescription)
        {
            using (var context = new UserContext())
            {
                User user = context.Users.Find(apiKey);
                if (user != null)
                {
                    Log newLog = new Log(logDescription);
                    context.Logs.Add(newLog);
                    user.Logs.Add(newLog);
                    context.SaveChanges();
                }
            }
        }
        #endregion
    }
}